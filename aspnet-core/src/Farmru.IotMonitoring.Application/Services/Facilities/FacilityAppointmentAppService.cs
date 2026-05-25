using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Facilities.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Facilities
{
    [AbpAuthorize()]
    public class FacilityAppointmentAppService : AsyncCrudAppService<FacilityAppointment, FacilityAppointmentDto, Guid, PagedUserResultRequestDto, CreateFacilityAppointmentDto, FacilityAppointmentDto>
    {
        private readonly IRepository<Person, Guid> _personRepository;
        private readonly IRepository<Facility, Guid> _facilityRepository;

        public FacilityAppointmentAppService(
            IRepository<FacilityAppointment, Guid> repository,
            IRepository<Person, Guid> personRepository,
            IRepository<Facility, Guid> facilityRepository) : base(repository)
        {
            _personRepository = personRepository;
            _facilityRepository = facilityRepository;
        }

        public override async Task<FacilityAppointmentDto> CreateAsync(CreateFacilityAppointmentDto input)
        {
            try
            {
                var appointment = FacilityAppointment.Schedule(
                    await ResolvePersonAsync(input.AppointedUser),
                    await ResolveFacilityAsync(input.Facility));

                await Repository.InsertAsync(appointment);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(appointment);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public override async Task<FacilityAppointmentDto> UpdateAsync(FacilityAppointmentDto input)
        {
            var appointment = await Repository.GetAsync(input.Id);
            try
            {
                appointment.Reschedule(
                    await ResolvePersonAsync(input.AppointedUser),
                    await ResolveFacilityAsync(input.Facility));

                await Repository.UpdateAsync(appointment);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(appointment);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task<Person> ResolvePersonAsync(EntityWithDisplayNameDto<Guid?> dto)
        {
            if (dto?.Id == null)
            {
                throw new UserFriendlyException("Appointed user is required.");
            }

            return await _personRepository.FirstOrDefaultAsync(dto.Id.Value);
        }

        private async Task<Facility> ResolveFacilityAsync(EntityWithDisplayNameDto<Guid?> dto)
        {
            if (dto?.Id == null)
            {
                throw new UserFriendlyException("Facility is required.");
            }

            return await _facilityRepository.FirstOrDefaultAsync(dto.Id.Value);
        }
    }
}
