using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Domains.Tasks;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Tasks.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Tasks
{
    [AbpAuthorize()]
    public class TaskManagementAppService : AsyncCrudAppService<TaskManagement, TaskManagementDto, Guid, PagedUserResultRequestDto, TaskManagementDto, TaskManagementDto>
    {
        private readonly IRepository<Person, Guid> _personRepository;

        public TaskManagementAppService(
            IRepository<TaskManagement, Guid> repository,
            IRepository<Person, Guid> personRepository) : base(repository)
        {
            _personRepository = personRepository;
        }

        public override async Task<TaskManagementDto> CreateAsync(TaskManagementDto input)
        {
            try
            {
                var task = TaskManagement.Create(
                    input.Title,
                    input.Description,
                    await ResolvePersonAsync(input.AssignedTo),
                    await ResolvePersonAsync(input.AssignedBy));

                await Repository.InsertAsync(task);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(task);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public override async Task<TaskManagementDto> UpdateAsync(TaskManagementDto input)
        {
            var task = await Repository.GetAsync(input.Id);
            try
            {
                task.UpdateDetails(
                    input.Title,
                    input.Description,
                    await ResolvePersonAsync(input.AssignedTo),
                    input.Status);

                await Repository.UpdateAsync(task);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(task);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        private async Task<Person> ResolvePersonAsync(EntityWithDisplayNameDto<Guid?> dto) =>
            dto?.Id == null ? null : await _personRepository.FirstOrDefaultAsync(dto.Id.Value);
    }
}
