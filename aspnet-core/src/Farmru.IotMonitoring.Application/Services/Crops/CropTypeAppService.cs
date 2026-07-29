using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Crops;
using Farmru.IotMonitoring.Services.Crops.Dto;
using System;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Crops
{
    [AbpAuthorize(PermissionNames.Pages_Crops)]
    public class CropTypeAppService : AsyncCrudAppService<CropType, CropTypeDto, Guid, PagedResultRequestDto, CreateCropTypeInput, CropTypeDto>
    {
        public CropTypeAppService(IRepository<CropType, Guid> repository) : base(repository)
        {
        }

        public override async Task<CropTypeDto> CreateAsync(CreateCropTypeInput input)
        {
            try
            {
                var cropType = CropType.Create(AbpSession.GetTenantId(), input.Name, input.ScientificName, input.TypicalGrowthDurationDays);
                await Repository.InsertAsync(cropType);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(cropType);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public override async Task<CropTypeDto> UpdateAsync(CropTypeDto input)
        {
            var cropType = await Repository.GetAsync(input.Id);
            try
            {
                cropType.UpdateDetails(input.Name, input.ScientificName, input.TypicalGrowthDurationDays);
                cropType.SetActive(input.IsActive);
                await Repository.UpdateAsync(cropType);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(cropType);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
