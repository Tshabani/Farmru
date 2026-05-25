using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Facilities.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Facilities
{
    [AbpAuthorize()]
    public class FacilityAppService : AsyncCrudAppService<Facility, FacilityDto, Guid, PagedUserResultRequestDto, CreateFacilityDto, FacilityDto>
    {
        private readonly IRepository<Person, Guid> _personRepository;
        private readonly IRepository<Organisation, Guid> _organisationRepository;

        public FacilityAppService(
            IRepository<Facility, Guid> repository,
            IRepository<Person, Guid> personRepository,
            IRepository<Organisation, Guid> organisationRepository) : base(repository)
        {
            _personRepository = personRepository;
            _organisationRepository = organisationRepository;
        }

        public async Task<List<FacilitiesDto>> GetListOfFacilities()
        {
            var facilities = await Repository.GetAllListAsync();
            return ObjectMapper.Map<List<FacilitiesDto>>(facilities);
        }

        public override async Task<FacilityDto> CreateAsync(CreateFacilityDto input)
        {
            try
            {
                var facility = Facility.Create(
                    input.Name,
                    input.Description,
                    input.Address,
                    await ResolvePersonAsync(input.PrimaryContact),
                    await ResolveOrganisationAsync(input.OwnerOrganisation),
                    input.Latitude,
                    input.Longitude,
                    input.Altitude,
                    input.IsDefault);

                await Repository.InsertAsync(facility);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(facility);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public override async Task<FacilityDto> UpdateAsync(FacilityDto input)
        {
            var facility = await Repository.GetAsync(input.Id);
            try
            {
                if (!string.IsNullOrWhiteSpace(input.Name))
                {
                    facility.SetName(input.Name);
                }

                facility.UpdateDetails(
                    input.Description,
                    input.Address,
                    await ResolvePersonAsync(input.PrimaryContact),
                    await ResolveOrganisationAsync(input.OwnerOrganisation),
                    input.Latitude,
                    input.Longitude,
                    input.Altitude,
                    input.IsDefault);

                await Repository.UpdateAsync(facility);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(facility);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task<Person> ResolvePersonAsync(EntityWithDisplayNameDto<Guid?> dto) =>
            dto?.Id == null ? null : await _personRepository.FirstOrDefaultAsync(dto.Id.Value);

        private async Task<Organisation> ResolveOrganisationAsync(EntityWithDisplayNameDto<Guid?> dto) =>
            dto?.Id == null ? null : await _organisationRepository.FirstOrDefaultAsync(dto.Id.Value);
    }
}
