using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Crops;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Geo;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Crops.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Crops
{
    [AbpAuthorize(PermissionNames.Pages_Fields)]
    public class FieldAppService : AsyncCrudAppService<Field, FieldDto, Guid, PagedFieldResultRequestDto, CreateFieldInput, FieldDto>, IFieldAppService
    {
        private readonly IRepository<Facility, Guid> _facilityRepository;
        private readonly IRepository<GeoFence, Guid> _geoFenceRepository;

        public FieldAppService(
            IRepository<Field, Guid> repository,
            IRepository<Facility, Guid> facilityRepository,
            IRepository<GeoFence, Guid> geoFenceRepository) : base(repository)
        {
            _facilityRepository = facilityRepository;
            _geoFenceRepository = geoFenceRepository;
        }

        public async Task<List<FieldDto>> GetByFacility(EntityDto<Guid> facilityId)
        {
            var fields = await Repository.GetAll()
                .Include(f => f.Facility)
                .Where(f => f.FacilityId == facilityId.Id)
                .ToListAsync();

            return fields.Select(MapToEntityDto).ToList();
        }

        [AbpAuthorize(PermissionNames.Pages_Fields_Manage)]
        public override async Task<FieldDto> CreateAsync(CreateFieldInput input)
        {
            var facility = await _facilityRepository.FirstOrDefaultAsync(input.FacilityId)
                ?? throw new UserFriendlyException(L("FacilityNotFound"));

            GeoFence boundary = null;
            if (input.BoundaryGeoFenceId.HasValue)
            {
                boundary = await _geoFenceRepository.FirstOrDefaultAsync(input.BoundaryGeoFenceId.Value);
            }

            try
            {
                var field = Field.Create(AbpSession.GetTenantId(), facility, input.Name, input.AreaHectares, input.SoilType, boundary);
                await Repository.InsertAsync(field);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(field);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        protected override IQueryable<Field> CreateFilteredQuery(PagedFieldResultRequestDto input)
        {
            var query = Repository.GetAll().Include(f => f.Facility).AsQueryable();
            return query.WhereIf(input.FacilityId.HasValue, f => f.FacilityId == input.FacilityId.Value);
        }

        protected override FieldDto MapToEntityDto(Field entity)
        {
            return new FieldDto
            {
                Id = entity.Id,
                Facility = entity.Facility != null
                    ? new EntityWithDisplayNameDto<Guid?> { Id = entity.Facility.Id, DisplayText = entity.Facility.Name }
                    : null,
                Name = entity.Name,
                AreaHectares = entity.AreaHectares,
                SoilType = entity.SoilType,
                BoundaryGeoFenceId = entity.BoundaryGeoFenceId
            };
        }
    }
}
