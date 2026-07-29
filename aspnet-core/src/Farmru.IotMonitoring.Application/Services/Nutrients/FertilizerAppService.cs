using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using Abp.Runtime.Session;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Crops;
using Farmru.IotMonitoring.Domains.Nutrients;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Nutrients.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Nutrients
{
    [AbpAuthorize(PermissionNames.Pages_Nutrients)]
    public class FertilizerAppService : IotMonitoringAppServiceBase, IFertilizerAppService
    {
        private readonly IRepository<FertilizerProduct, Guid> _productRepository;
        private readonly IRepository<FertilizerApplication, Guid> _applicationRepository;
        private readonly IRepository<Field, Guid> _fieldRepository;
        private readonly IRepository<CropSeason, Guid> _cropSeasonRepository;
        private readonly IRepository<Person, Guid> _personRepository;

        public FertilizerAppService(
            IRepository<FertilizerProduct, Guid> productRepository,
            IRepository<FertilizerApplication, Guid> applicationRepository,
            IRepository<Field, Guid> fieldRepository,
            IRepository<CropSeason, Guid> cropSeasonRepository,
            IRepository<Person, Guid> personRepository)
        {
            _productRepository = productRepository;
            _applicationRepository = applicationRepository;
            _fieldRepository = fieldRepository;
            _cropSeasonRepository = cropSeasonRepository;
            _personRepository = personRepository;
        }

        [AbpAuthorize(PermissionNames.Pages_Nutrients_Apply)]
        public async Task<FertilizerProductDto> CreateProduct(CreateFertilizerProductInput input)
        {
            try
            {
                var product = FertilizerProduct.Create(
                    AbpSession.GetTenantId(),
                    input.Name,
                    input.NitrogenPercent,
                    input.PhosphorusPercent,
                    input.PotassiumPercent,
                    input.UnitCostPerKg);

                await _productRepository.InsertAsync(product);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToDto(product);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<FertilizerProductDto>> GetProducts(PagedResultRequestDto input)
        {
            var query = _productRepository.GetAll();
            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(p => p.Name).PageBy(input).ToListAsync();
            return new PagedResultDto<FertilizerProductDto>(totalCount, items.Select(MapToDto).ToList());
        }

        [AbpAuthorize(PermissionNames.Pages_Nutrients_Apply)]
        public async Task<FertilizerApplicationDto> RecordApplication(RecordFertilizerApplicationInput input)
        {
            var field = await _fieldRepository.FirstOrDefaultAsync(input.FieldId)
                ?? throw new UserFriendlyException(L("FieldNotFound"));

            var product = await _productRepository.FirstOrDefaultAsync(input.ProductId)
                ?? throw new UserFriendlyException(L("FertilizerProductNotFound"));

            CropSeason cropSeason = null;
            if (input.CropSeasonId.HasValue)
            {
                cropSeason = await _cropSeasonRepository.FirstOrDefaultAsync(input.CropSeasonId.Value)
                    ?? throw new UserFriendlyException(L("CropSeasonNotFound"));
            }

            Person operatorPerson = null;
            if (input.OperatorPersonId.HasValue)
            {
                operatorPerson = await _personRepository.FirstOrDefaultAsync(input.OperatorPersonId.Value)
                    ?? throw new UserFriendlyException(L("PersonNotFoundForOperator"));
            }

            try
            {
                var application = FertilizerApplication.Apply(
                    AbpSession.GetTenantId(),
                    field,
                    cropSeason,
                    product,
                    input.RateKgPerHectare,
                    input.ApplicationDate,
                    input.Cost,
                    operatorPerson);

                await _applicationRepository.InsertAsync(application);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToDto(application);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<PagedResultDto<FertilizerApplicationDto>> GetApplicationsByField(GetApplicationsByFieldInput input)
        {
            var query = _applicationRepository.GetAll()
                .Include(a => a.Field)
                .Include(a => a.CropSeason)
                .Include(a => a.Product)
                .Include(a => a.Operator)
                .Where(a => a.FieldId == input.FieldId);

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(a => a.ApplicationDate)
                .PageBy(input)
                .ToListAsync();

            return new PagedResultDto<FertilizerApplicationDto>(totalCount, items.Select(MapToDto).ToList());
        }

        private static FertilizerProductDto MapToDto(FertilizerProduct product) => new FertilizerProductDto
        {
            Id = product.Id,
            Name = product.Name,
            NitrogenPercent = product.NitrogenPercent,
            PhosphorusPercent = product.PhosphorusPercent,
            PotassiumPercent = product.PotassiumPercent,
            UnitCostPerKg = product.UnitCostPerKg
        };

        private static FertilizerApplicationDto MapToDto(FertilizerApplication application) => new FertilizerApplicationDto
        {
            Id = application.Id,
            Field = application.Field != null
                ? new EntityWithDisplayNameDto<Guid?> { Id = application.Field.Id, DisplayText = application.Field.Name }
                : null,
            CropSeason = application.CropSeason != null
                ? new EntityWithDisplayNameDto<Guid?> { Id = application.CropSeason.Id, DisplayText = application.CropSeason.PlantingDate.ToShortDateString() }
                : null,
            Product = application.Product != null
                ? new EntityWithDisplayNameDto<Guid?> { Id = application.Product.Id, DisplayText = application.Product.Name }
                : null,
            RateKgPerHectare = application.RateKgPerHectare,
            ApplicationDate = application.ApplicationDate,
            Cost = application.Cost,
            Operator = application.Operator != null
                ? new EntityWithDisplayNameDto<Guid?> { Id = application.Operator.Id, DisplayText = $"{application.Operator.FirstName} {application.Operator.LastName}".Trim() }
                : null
        };
    }
}
