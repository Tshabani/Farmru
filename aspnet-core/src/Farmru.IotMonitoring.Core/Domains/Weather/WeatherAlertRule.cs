using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains;
using Farmru.IotMonitoring.Domains.Alerts;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Organisations;
using System;

namespace Farmru.IotMonitoring.Domains.Weather
{
    /// <summary>
    /// A configured threshold that WeatherAlertEvaluationHostedService checks against the
    /// latest WeatherObservation/WeatherForecastDaily for a Facility, raising an Alert on
    /// breach (Phase 1 Technical Design Section 5.1/5.2).
    /// </summary>
    public class WeatherAlertRule : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        protected WeatherAlertRule()
        {
        }

        public int TenantId { get; set; }
        public virtual Guid? FacilityId { get; private set; }
        public virtual Facility Facility { get; private set; }
        public virtual Guid? OrganisationId { get; private set; }
        public virtual Organisation Organisation { get; private set; }
        public virtual WeatherAlertType AlertType { get; private set; }
        public virtual decimal ThresholdValue { get; private set; }
        public virtual AlertSeverity Severity { get; private set; }
        public virtual bool IsActive { get; private set; }

        public static WeatherAlertRule Create(
            int tenantId,
            Facility facility,
            Organisation organisation,
            WeatherAlertType alertType,
            decimal thresholdValue,
            AlertSeverity severity)
        {
            if (facility == null && organisation == null)
            {
                throw new DomainRuleException("A weather alert rule must be scoped to a Facility or an Organisation.");
            }

            if (facility != null && organisation != null)
            {
                throw new DomainRuleException("A weather alert rule must be scoped to exactly one of Facility or Organisation, not both.");
            }

            return new WeatherAlertRule
            {
                TenantId = tenantId,
                Facility = facility,
                FacilityId = facility?.Id,
                Organisation = organisation,
                OrganisationId = organisation?.Id,
                AlertType = alertType,
                ThresholdValue = thresholdValue,
                Severity = severity,
                IsActive = true
            };
        }

        public virtual void Deactivate()
        {
            IsActive = false;
        }
    }
}
