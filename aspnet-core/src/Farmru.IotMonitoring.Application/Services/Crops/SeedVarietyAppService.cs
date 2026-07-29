using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Crops;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Crops.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Crops
{
    [AbpAuthorize(PermissionNames.Pages_Crops)]
    public class SeedVarietyAppService : AsyncCrudAppService<SeedVariety, SeedVarietyDto, Guid, PagedResultRequestDto, CreateSeedVarietyInput, SeedVarietyDto>
    {
        private readonly IRepository<CropType, Guid> _cropTypeRepository;
        private readonly IRepository<SeedSupplier, Guid> _supplierRepository;

        public SeedVarietyAppService(
            IRepository<SeedVariety, Guid> repository,
            IRepository<CropType, Guid> cropTypeRepository,
            IRepository<SeedSupplier, Guid> supplierRepository) : base(repository)
        {
            _cropTypeRepository = cropTypeRepository;
            _supplierRepository = supplierRepository;
        }

        public override async Task<SeedVarietyDto> CreateAsync(CreateSeedVarietyInput input)
        {
            var cropType = await _cropTypeRepository.FirstOrDefaultAsync(input.CropTypeId)
                ?? throw new UserFriendlyException(L("CropTypeNotFound"));

            SeedSupplier supplier = null;
            if (input.SupplierId.HasValue)
            {
                supplier = await _supplierRepository.FirstOrDefaultAsync(input.SupplierId.Value)
                    ?? throw new UserFriendlyException(L("SeedSupplierNotFound"));
            }

            try
            {
                var variety = SeedVariety.Create(AbpSession.GetTenantId(), cropType, input.Name, supplier, input.DaysToMaturity);
                await Repository.InsertAsync(variety);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(variety);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        // SeedVariety exposes no domain update behavior (Technical Design Section 2.2 defines
        // only the Create factory) — overridden here to fail cleanly rather than let the base
        // AsyncCrudAppService fall through to an unconfigured AutoMapper Dto->entity map.
        public override Task<SeedVarietyDto> UpdateAsync(SeedVarietyDto input)
        {
            throw new UserFriendlyException(L("SeedVarietiesCannotBeEdited"));
        }

        protected override IQueryable<SeedVariety> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return Repository.GetAll().Include(v => v.CropType).Include(v => v.Supplier);
        }

        protected override SeedVarietyDto MapToEntityDto(SeedVariety entity)
        {
            return new SeedVarietyDto
            {
                Id = entity.Id,
                CropType = entity.CropType != null
                    ? new EntityWithDisplayNameDto<Guid?> { Id = entity.CropType.Id, DisplayText = entity.CropType.Name }
                    : null,
                Supplier = entity.Supplier != null
                    ? new EntityWithDisplayNameDto<Guid?> { Id = entity.Supplier.Id, DisplayText = entity.Supplier.Name }
                    : null,
                Name = entity.Name,
                DaysToMaturity = entity.DaysToMaturity
            };
        }
    }
}
