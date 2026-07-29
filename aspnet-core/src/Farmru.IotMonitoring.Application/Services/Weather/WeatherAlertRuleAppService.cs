using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Domains.Weather;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Weather.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Weather
{
    [AbpAuthorize(PermissionNames.Pages_Weather)]
    public class WeatherAlertRuleAppService : IotMonitoringAppServiceBase, IWeatherAlertRuleAppService
    {
        private readonly IRepository<WeatherAlertRule, Guid> _ruleRepository;
        private readonly IRepository<Facility, Guid> _facilityRepository;
        private readonly IRepository<Organisation, Guid> _organisationRepository;

        public WeatherAlertRuleAppService(
            IRepository<WeatherAlertRule, Guid> ruleRepository,
            IRepository<Facility, Guid> facilityRepository,
            IRepository<Organisation, Guid> organisationRepository)
        {
            _ruleRepository = ruleRepository;
            _facilityRepository = facilityRepository;
            _organisationRepository = organisationRepository;
        }

        [AbpAuthorize(PermissionNames.Pages_Weather_Configure)]
        public async Task<WeatherAlertRuleDto> Create(CreateWeatherAlertRuleInput input)
        {
            Facility facility = null;
            if (input.FacilityId.HasValue)
            {
                facility = await _facilityRepository.FirstOrDefaultAsync(input.FacilityId.Value)
                    ?? throw new UserFriendlyException(L("FacilityNotFound"));
            }

            Organisation organisation = null;
            if (input.OrganisationId.HasValue)
            {
                organisation = await _organisationRepository.FirstOrDefaultAsync(input.OrganisationId.Value)
                    ?? throw new UserFriendlyException(L("OrganisationNotFound"));
            }

            try
            {
                var rule = WeatherAlertRule.Create(
                    AbpSession.GetTenantId(),
                    facility,
                    organisation,
                    input.AlertType,
                    input.ThresholdValue,
                    input.Severity);

                await _ruleRepository.InsertAsync(rule);
                await CurrentUnitOfWork.SaveChangesAsync();
                return MapToDto(rule);
            }
            catch (DomainRuleException ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

        public async Task<List<WeatherAlertRuleDto>> GetForFacility(EntityDto<Guid> facilityId)
        {
            var rules = await _ruleRepository.GetAll()
                .Include(r => r.Facility)
                .Include(r => r.Organisation)
                .Where(r => r.FacilityId == facilityId.Id)
                .ToListAsync();

            return rules.Select(MapToDto).ToList();
        }

        [AbpAuthorize(PermissionNames.Pages_Weather_Configure)]
        public async Task Deactivate(EntityDto<Guid> input)
        {
            var rule = await _ruleRepository.FirstOrDefaultAsync(input.Id)
                ?? throw new UserFriendlyException(L("WeatherAlertRuleNotFound"));

            rule.Deactivate();
            await _ruleRepository.UpdateAsync(rule);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private static WeatherAlertRuleDto MapToDto(WeatherAlertRule rule) => new WeatherAlertRuleDto
        {
            Id = rule.Id,
            Facility = rule.Facility != null
                ? new EntityWithDisplayNameDto<Guid?> { Id = rule.Facility.Id, DisplayText = rule.Facility.Name }
                : null,
            Organisation = rule.Organisation != null
                ? new EntityWithDisplayNameDto<Guid?> { Id = rule.Organisation.Id, DisplayText = rule.Organisation.Name }
                : null,
            AlertType = rule.AlertType,
            ThresholdValue = rule.ThresholdValue,
            Severity = rule.Severity,
            IsActive = rule.IsActive
        };
    }
}
