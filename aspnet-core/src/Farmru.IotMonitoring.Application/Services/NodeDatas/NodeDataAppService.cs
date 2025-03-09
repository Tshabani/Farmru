using Abp.Application.Services;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Threading.Tasks;
using Abp.UI;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;
using Farmru.IotMonitoring.Services.NodeDatas.Dto;
using Abp.Runtime.Session;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Organisations;
using Node = Farmru.IotMonitoring.Domains.Nodes.Node;
using Abp.Authorization;

namespace Farmru.IotMonitoring.Services.NodeDatas
{
    /// <summary>
    /// 
    /// </summary>
    [AbpAuthorize()]
    public class NodeDataAppService : AsyncCrudAppService<Domains.Nodes.NodeData, NodeDataDto, Guid, PagedUserResultRequestDto, CreateNodeData, NodeDataDto>, INodeDataAppService
    {
        private readonly IRepository<Node, Guid> _nodeRepository;
        private readonly IAbpSession _session;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="nodeRepository"></param>
        /// <param name="session"></param>
        public NodeDataAppService(IRepository<Domains.Nodes.NodeData, Guid> repository, IRepository<Node, Guid> nodeRepository, IAbpSession session) : base(repository)
        {
            _nodeRepository = nodeRepository;
            _session = session;
        }

        public override async Task<NodeDataDto> CreateAsync(CreateNodeData input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (string.IsNullOrWhiteSpace(input.SerialNumber)) throw new ArgumentNullException("Serial Number is required");

            var node = await _nodeRepository.FirstOrDefaultAsync(x => x.SerialNumber == input.SerialNumber) ?? throw new UserFriendlyException("Node not found");

            var nodeData = new Domains.Nodes.NodeData();
            ObjectMapper.Map(input, nodeData);

            nodeData.Node = node;
            nodeData.TenantId = node.TenantId;

            nodeData = await Repository.InsertAsync(nodeData);
            return ObjectMapper.Map<NodeDataDto>(nodeData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<HistoricalDataResponse> GetReadings()
        {
            try
            {
                var tenantId = _session.GetTenantId();

                var facilityRepo = Abp.Dependency.IocManager.Instance.Resolve<IRepository<Facility, Guid>>();
                var organisationRepo = Abp.Dependency.IocManager.Instance.Resolve<IRepository<Organisation, Guid>>();

                var organisation = await organisationRepo.FirstOrDefaultAsync(r => r.TenantId == tenantId);
                if (organisation == null)
                    return new HistoricalDataResponse(); // No organization found

                var facility = await facilityRepo.FirstOrDefaultAsync(r => r.IsDefault == true && r.OwnerOrganisation == organisation);
                if (facility == null)
                    return new HistoricalDataResponse(); // No default facility found

                var nodes = await _nodeRepository.GetAllListAsync(r => r.Facility == facility);
                if (!nodes.Any())
                    return new HistoricalDataResponse(); // No nodes found

                var sevenDaysAgo = DateTime.UtcNow.AddDays(-6); // Include today (7 days total)

                // Fetch all readings for the past 7 days
                var pastWeekReadings = await Repository.GetAll()
                    .Where(r => r.Node != null && nodes.Contains(r.Node) && r.CreationTime >= sevenDaysAgo)
                    .OrderByDescending(r => r.CreationTime)
                    .ToListAsync();

                if (!pastWeekReadings.Any())
                    return new HistoricalDataResponse(); // No readings available

                // Calculate average values rounded to 2 decimal places
                var averageReadings = new CurrentReadings
                {
                    SoilTemp = RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.SoilTemperature))),
                    SoilPH = RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.SoilPH))),
                    Moisture = RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.Moisture))),
                    Phosphorus = RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.Phosphorus))),
                    Potassium = RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.Potassium))),
                    Nitrogen = RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.Nitrogen))),
                    SolarVoltage = RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => r.SolarPanelVoltage.HasValue ? r.SolarPanelVoltage.Value / 1000.0 : 0)),
                    BatteryVoltage = RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => r.BatteryVoltage.HasValue ? r.BatteryVoltage.Value / 1000.0 : 0))
                };

                // Group historical data by day
                var historicalData = pastWeekReadings
                    .GroupBy(r => r.CreationTime.Date)
                    .Select(g => new HistoricalData
                    {
                        Day = g.Key.ToString("ddd"),
                        SoilTemp = RoundToTwoDecimalPlaces(g.Average(r => ConvertToDouble(r.SoilTemperature))),
                        Moisture = RoundToTwoDecimalPlaces(g.Average(r => ConvertToDouble(r.Moisture))),
                        PH = RoundToTwoDecimalPlaces(g.Average(r => ConvertToDouble(r.SoilPH)))
                    })
                    .OrderBy(h => h.Day) // Ensure correct order
                    .ToList();

                return new HistoricalDataResponse
                {
                    CurrentReadings = averageReadings, // Return the calculated averages
                    historicalData = historicalData
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Helper function to safely convert string values to double
        private double ConvertToDouble(string value)
        {
            return double.TryParse(value, out double result) ? result : 0;
        }

        // Helper function to round values to two decimal places
        private double RoundToTwoDecimalPlaces(double value)
        {
            return Math.Round(value, 2);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<NodeDataDto>> GetNodeDataByNodeId(Guid nodeId, PagedNodeDataResultRequestDto input)
        {
            // Ensure the node exists
            var nodeExists = await _nodeRepository.FirstOrDefaultAsync(x => x.Id == nodeId);
            if (nodeExists == null)
            {
                throw new UserFriendlyException("Node not found");
            }

            // Determine the date range based on predefined period or custom range
            DateTime? startDate = input.StartDate;
            DateTime? endDate = input.EndDate;

            if (!string.IsNullOrEmpty(input.PredefinedPeriod))
            {
                switch (input.PredefinedPeriod.ToLower())
                {
                    case "today":
                        startDate = DateTime.Today;
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                    case "week":
                        startDate = DateTime.Today.AddDays(-7);
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                    case "month":
                        startDate = DateTime.Today.AddMonths(-1);
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                    case "year":
                        startDate = DateTime.Today.AddYears(-1);
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                }
            }

            // Query filtered data
            var query = Repository.GetAll()
                .Where(r => r.Node != null && r.Node.Id == nodeId); // Filter by navigation property

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(r => r.CreationTime >= startDate && r.CreationTime <= endDate);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination and fetch results
            var nodeData = await query
                .OrderByDescending(c => c.CreationTime)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            // Map and return paginated results
            return new PagedResultDto<NodeDataDto>(
                totalCount,
                ObjectMapper.Map<List<NodeDataDto>>(nodeData)
            );
        }

    }
}
