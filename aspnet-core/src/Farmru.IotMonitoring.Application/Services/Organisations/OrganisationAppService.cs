using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Services.Organisations.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Organisations
{
    [AbpAuthorize()]
    public class OrganisationAppService : AsyncCrudAppService<Organisation, OrganisationDto, Guid, PagedResultRequestDto, OrganisationDto, OrganisationDto>
    {
        public OrganisationAppService(IRepository<Organisation, Guid> repository) : base(repository)
        {
        }

        public async Task<List<OrganisationsDto>> GetListOfOrganisations()
        {
            var organisations = await Repository.GetAllListAsync();
            return ObjectMapper.Map<List<OrganisationsDto>>(organisations);
        }

        public override async Task<OrganisationDto> CreateAsync(OrganisationDto input)
        {
            try
            {
                var organisation = Organisation.Create(
                    AbpSession.GetTenantId(),
                    input.Name,
                    input.ShortAlias,
                    input.Description);

                organisation.UpdateProfile(
                    input.ShortAlias,
                    input.Description,
                    input.FreeTextAddress,
                    input.OrganisationType,
                    input.CompanyRegistrationNo,
                    input.VatRegistrationNo,
                    input.ContactEmail,
                    input.ContactMobileNo);

                await Repository.InsertAsync(organisation);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(organisation);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public override async Task<OrganisationDto> UpdateAsync(OrganisationDto input)
        {
            var organisation = await Repository.GetAsync(input.Id);
            try
            {
                if (!string.IsNullOrWhiteSpace(input.Name))
                {
                    organisation.SetName(input.Name);
                }

                organisation.UpdateProfile(
                    input.ShortAlias,
                    input.Description,
                    input.FreeTextAddress,
                    input.OrganisationType,
                    input.CompanyRegistrationNo,
                    input.VatRegistrationNo,
                    input.ContactEmail,
                    input.ContactMobileNo);

                await Repository.UpdateAsync(organisation);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToEntityDto(organisation);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
