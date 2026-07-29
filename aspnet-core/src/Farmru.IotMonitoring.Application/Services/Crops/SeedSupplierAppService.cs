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
    // Not enumerated as its own AppService in the Product Plan's abbreviated API surface list
    // for 5.2, but SeedVariety.Create requires a persisted SeedSupplier to reference — a
    // minimal CRUD surface is required for the Crop Management feature to be usable at all
    // (ADR-008-adjacent judgment call, not a scope expansion beyond what Section 5.2 implies).
    [AbpAuthorize(PermissionNames.Pages_Crops)]
    public class SeedSupplierAppService : AsyncCrudAppService<SeedSupplier, SeedSupplierDto, Guid, PagedResultRequestDto, CreateSeedSupplierInput, SeedSupplierDto>
    {
        public SeedSupplierAppService(IRepository<SeedSupplier, Guid> repository) : base(repository)
        {
        }

        public override async Task<SeedSupplierDto> CreateAsync(CreateSeedSupplierInput input)
        {
            try
            {
                var supplier = SeedSupplier.Create(AbpSession.GetTenantId(), input.Name, input.ContactInfo);
                await Repository.InsertAsync(supplier);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(supplier);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public override async Task<SeedSupplierDto> UpdateAsync(SeedSupplierDto input)
        {
            var supplier = await Repository.GetAsync(input.Id);
            try
            {
                supplier.UpdateDetails(input.Name, input.ContactInfo);
                await Repository.UpdateAsync(supplier);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(supplier);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
