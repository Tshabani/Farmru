using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Domains.Nodes.Services;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Nodes.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using NodeDataEntity = Farmru.IotMonitoring.Domains.Nodes.NodeData;

namespace Farmru.IotMonitoring.Services.Nodes
{
    [AbpAuthorize(PermissionNames.Pages_Nodes)]
    public class NodeAppService : AsyncCrudAppService<Node, NodeDto, Guid, PagedNodeResultRequestDto, CreateNode, NodeDto>, INodeAppService
    {
        private readonly IRepository<Person, Guid> _personRepository;
        private readonly IRepository<FacilityAppointment, Guid> _facilityAppointmentRepository;
        private readonly IRepository<NodeDataEntity, Guid> _nodeDataRepository;
        private readonly IRepository<NodeReplacementHistory, Guid> _replacementHistoryRepository;
        private readonly IRepository<Facility, Guid> _facilityRepository;
        private readonly INodeSerialNumberAvailabilityChecker _serialNumberChecker;

        public NodeAppService(
            IRepository<Node, Guid> repository,
            IRepository<Person, Guid> personRepository,
            IRepository<FacilityAppointment, Guid> facilityAppointmentRepository,
            IRepository<NodeDataEntity, Guid> nodeDataRepository,
            IRepository<NodeReplacementHistory, Guid> replacementHistoryRepository,
            IRepository<Facility, Guid> facilityRepository,
            INodeSerialNumberAvailabilityChecker serialNumberChecker) : base(repository)
        {
            _personRepository = personRepository;
            _facilityAppointmentRepository = facilityAppointmentRepository;
            _nodeDataRepository = nodeDataRepository;
            _replacementHistoryRepository = replacementHistoryRepository;
            _facilityRepository = facilityRepository;
            _serialNumberChecker = serialNumberChecker;
        }

        public async Task<List<NodeDto>> GetMyNodes()
        {
            var person = await _personRepository.GetAll()
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.User.Id == AbpSession.GetUserId());

            if (person == null)
            {
                return new List<NodeDto>();
            }

            var facilityIds = await _facilityAppointmentRepository.GetAll()
                .Include(fa => fa.AppointedUser)
                .Include(fa => fa.Facility)
                .Where(fa => fa.AppointedUser != null && fa.AppointedUser.Id == person.Id && fa.Facility != null)
                .Select(fa => fa.Facility.Id)
                .ToListAsync();

            if (!facilityIds.Any())
            {
                return new List<NodeDto>();
            }

            var nodes = await Repository.GetAll()
                .Include(n => n.Facility)
                .Where(n => n.Facility != null && facilityIds.Contains(n.Facility.Id))
                .ToListAsync();

            return nodes.Select(MapToNodeDto).ToList();
        }

        public async Task<NodeDetailDto> GetDetail(EntityDto<Guid> input)
        {
            var node = await GetNodeOrThrowAsync(input.Id);
            var detail = ObjectMapper.Map<NodeDetailDto>(node);
            detail.IsOnline = node.IsOnline();
            detail.TelemetrySummary = await BuildTelemetrySummaryAsync(node.Id);
            detail.ReplacementHistory = await GetReplacementHistoryInternalAsync(node.Id);
            return detail;
        }

        [AbpAuthorize(PermissionNames.Pages_Nodes_Manage)]
        public async Task<NodeDto> Activate(EntityDto<Guid> input)
        {
            var node = await GetNodeOrThrowAsync(input.Id);
            node.Activate();
            await Repository.UpdateAsync(node);
            await CurrentUnitOfWork.SaveChangesAsync();
            return MapToNodeDto(node);
        }

        [AbpAuthorize(PermissionNames.Pages_Nodes_Manage)]
        public async Task<NodeDto> Deactivate(EntityDto<Guid> input)
        {
            var node = await GetNodeOrThrowAsync(input.Id);
            node.Deactivate();
            await Repository.UpdateAsync(node);
            await CurrentUnitOfWork.SaveChangesAsync();
            return MapToNodeDto(node);
        }

        [AbpAuthorize(PermissionNames.Pages_Nodes_Manage)]
        public async Task<NodeDto> AssignToFacility(AssignNodeToFacilityInput input)
        {
            var node = await GetNodeOrThrowAsync(input.NodeId);
            var facility = await ResolveFacilityAsync(input.Facility);
            node.AssignToFacility(facility);
            await Repository.UpdateAsync(node);
            await CurrentUnitOfWork.SaveChangesAsync();
            return MapToNodeDto(node);
        }

        [AbpAuthorize(PermissionNames.Pages_Nodes_Manage)]
        public async Task<NodeDto> ReplaceDevice(ReplaceNodeInput input)
        {
            var node = await GetNodeOrThrowAsync(input.NodeId);
            await _serialNumberChecker.EnsureAvailableAsync(input.NewSerialNumber, node.Id);

            try
            {
                var history = node.ReplaceSerialNumber(input.NewSerialNumber, input.Reason, input.Notes);
                await _replacementHistoryRepository.InsertAsync(history);
                await Repository.UpdateAsync(node);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToNodeDto(node);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<List<NodeReplacementHistoryDto>> GetReplacementHistory(EntityDto<Guid> input)
        {
            return await GetReplacementHistoryInternalAsync(input.Id);
        }

        protected override IQueryable<Node> CreateFilteredQuery(PagedNodeResultRequestDto input)
        {
            IQueryable<Node> query = Repository.GetAll().Include(n => n.Facility);

            if (!string.IsNullOrWhiteSpace(input.Keyword))
            {
                var keyword = input.Keyword.Trim().ToLower();
                query = query.Where(n =>
                    (n.SerialNumber != null && n.SerialNumber.ToLower().Contains(keyword)) ||
                    (n.DisplayName != null && n.DisplayName.ToLower().Contains(keyword)) ||
                    (n.FirmwareVersion != null && n.FirmwareVersion.ToLower().Contains(keyword)));
            }

            if (input.FacilityId.HasValue)
            {
                query = query.Where(n => n.Facility != null && n.Facility.Id == input.FacilityId.Value);
            }

            if (input.DeviceStatus.HasValue)
            {
                query = query.Where(n => n.DeviceStatus == input.DeviceStatus.Value);
            }

            if (input.HealthStatus.HasValue)
            {
                query = query.Where(n => n.HealthStatus == input.HealthStatus.Value);
            }

            return query;
        }

        protected override IQueryable<Node> ApplySorting(IQueryable<Node> query, PagedNodeResultRequestDto input)
        {
            if (!string.IsNullOrWhiteSpace(input.Sorting))
            {
                return query.OrderBy(input.Sorting);
            }

            return query.OrderByDescending(n => n.LastSeenAt).ThenBy(n => n.SerialNumber);
        }

        public override async Task<PagedResultDto<NodeDto>> GetAllAsync(PagedNodeResultRequestDto input)
        {
            var query = CreateFilteredQuery(input);
            var totalCount = await query.CountAsync();

            var entities = await query
                .PageBy(input)
                .ToListAsync();

            var items = entities.Select(MapToNodeDto).ToList();

            if (input.OnlineOnly == true)
            {
                items = items.Where(i => i.IsOnline).ToList();
            }
            else if (input.OfflineOnly == true)
            {
                items = items.Where(i => !i.IsOnline).ToList();
            }

            return new PagedResultDto<NodeDto>(totalCount, items);
        }

        protected override NodeDto MapToEntityDto(Node entity)
        {
            return MapToNodeDto(entity);
        }

        public override async Task<NodeDto> CreateAsync(CreateNode input)
        {
            await _serialNumberChecker.EnsureAvailableAsync(input.SerialNumber);

            var facility = await ResolveFacilityAsync(input.Facility);
            var node = Node.Register(
                AbpSession.GetTenantId(),
                input.SerialNumber,
                facility,
                input.DisplayName,
                input.FirmwareVersion,
                input.Notes);

            await Repository.InsertAsync(node);
            await CurrentUnitOfWork.SaveChangesAsync();
            return MapToNodeDto(node);
        }

        public override async Task<NodeDto> UpdateAsync(NodeDto input)
        {
            var node = await GetNodeOrThrowAsync(input.Id);

            try
            {
                if (!string.Equals(node.SerialNumber, input.SerialNumber, StringComparison.OrdinalIgnoreCase))
                {
                    await _serialNumberChecker.EnsureAvailableAsync(input.SerialNumber, node.Id);
                    node.UpdateSerialNumber(input.SerialNumber);
                }

                node.UpdateProfile(input.DisplayName, input.FirmwareVersion, input.Notes);
                node.AssignToFacility(await ResolveFacilityAsync(input.Facility));
                node.ApplyOperationalStatus(input.DeviceStatus);

                await Repository.UpdateAsync(node);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToNodeDto(node);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task<Facility> ResolveFacilityAsync(EntityWithDisplayNameDto<Guid?> facilityRef)
        {
            if (facilityRef?.Id == null)
            {
                return null;
            }

            return await _facilityRepository.FirstOrDefaultAsync(facilityRef.Id.Value);
        }

        private async Task<Node> GetNodeOrThrowAsync(Guid id)
        {
            var node = await Repository.GetAll()
                .Include(n => n.Facility)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (node == null)
            {
                throw new UserFriendlyException(L("NodeNotFound"));
            }

            return node;
        }

        private NodeDto MapToNodeDto(Node node)
        {
            var dto = ObjectMapper.Map<NodeDto>(node);
            dto.IsOnline = node.IsOnline();
            return dto;
        }

        private async Task<NodeTelemetrySummaryDto> BuildTelemetrySummaryAsync(Guid nodeId)
        {
            var readingsQuery = _nodeDataRepository.GetAll()
                .Where(nd => nd.Node != null && nd.Node.Id == nodeId);

            var total = await readingsQuery.CountAsync();
            var latest = await readingsQuery
                .OrderByDescending(nd => nd.LoggingTime ?? nd.CreationTime)
                .FirstOrDefaultAsync();

            if (latest == null)
            {
                return new NodeTelemetrySummaryDto { TotalReadings = total };
            }

            return new NodeTelemetrySummaryDto
            {
                TotalReadings = total,
                LastReadingTime = latest.LoggingTime ?? latest.CreationTime,
                Moisture = latest.Moisture,
                SoilTemperature = latest.SoilTemperature,
                SoilPH = latest.SoilPH,
                Nitrogen = latest.Nitrogen,
                Phosphorus = latest.Phosphorus,
                Potassium = latest.Potassium,
                Latitude = latest.Latitude,
                Longitude = latest.Longitude
            };
        }

        private async Task<List<NodeReplacementHistoryDto>> GetReplacementHistoryInternalAsync(Guid nodeId)
        {
            var history = await _replacementHistoryRepository.GetAll()
                .Include(h => h.Node)
                .Where(h => h.Node.Id == nodeId)
                .OrderByDescending(h => h.ReplacedAt)
                .ToListAsync();

            return ObjectMapper.Map<List<NodeReplacementHistoryDto>>(history);
        }
    }
}
