using Abp.Domain.Entities;
using Farmru.IotMonitoring.Domains;
using Abp.Domain.Entities.Auditing;
using Farmru.IotMonitoring.Domains.Facilities;
using System;
using System.Collections.Generic;

namespace Farmru.IotMonitoring.Domains.Nodes
{
    /// <summary>
    /// IoT device aggregate root (node).
    /// </summary>
    public partial class Node : FullAuditedAggregateRoot<Guid>, IMustHaveTenant
    {
        private readonly List<NodeReplacementHistory> _replacementHistories = new();

        protected Node()
        {
        }

        /// <summary>ABP multi-tenancy; set at registration.</summary>
        public virtual int TenantId { get; set; }
        public virtual string SerialNumber { get; private set; }
        public virtual string DisplayName { get; private set; }
        public virtual Facility Facility { get; private set; }
        public virtual bool IsActive { get; private set; }
        public virtual DeviceOperationalStatus DeviceStatus { get; private set; }
        public virtual DeviceHealthStatus HealthStatus { get; private set; }
        public virtual DateTime? LastSeenAt { get; private set; }
        public virtual decimal? BatteryLevel { get; private set; }
        public virtual int? SignalStrength { get; private set; }
        public virtual string FirmwareVersion { get; private set; }
        public virtual string Notes { get; private set; }

        public virtual IReadOnlyCollection<NodeReplacementHistory> ReplacementHistories => _replacementHistories.AsReadOnly();

        public static Node Register(
            int tenantId,
            string serialNumber,
            Facility facility = null,
            string displayName = null,
            string firmwareVersion = null,
            string notes = null)
        {
            var node = new Node();
            node.SetTenant(tenantId);
            node.SetSerialNumber(serialNumber);
            node.AssignToFacility(facility);
            node.UpdateProfile(displayName, firmwareVersion, notes);
            node.Activate();
            node.EvaluateHealth();
            return node;
        }

        public virtual void SetTenant(int tenantId)
        {
            if (tenantId <= 0)
            {
                throw new DomainRuleException("Tenant is required.");
            }

            TenantId = tenantId;
        }

        public virtual void UpdateProfile(string displayName, string firmwareVersion, string notes)
        {
            DisplayName = NormalizeOptional(displayName);
            FirmwareVersion = NormalizeOptional(firmwareVersion);
            Notes = NormalizeOptional(notes);
        }

        public virtual void UpdateSerialNumber(string serialNumber)
        {
            SetSerialNumber(serialNumber);
            EvaluateHealth();
        }

        public virtual NodeReplacementHistory ReplaceSerialNumber(string newSerialNumber, string reason, string notes = null)
        {
            if (DeviceStatus == DeviceOperationalStatus.Decommissioned)
            {
                throw new DomainRuleException("Cannot replace serial number on a decommissioned device.");
            }

            var trimmed = RequireSerialNumber(newSerialNumber);
            var history = NodeReplacementHistory.Record(
                TenantId,
                this,
                SerialNumber,
                trimmed,
                RequireReason(reason),
                NormalizeOptional(notes));

            SerialNumber = trimmed;
            _replacementHistories.Add(history);
            EvaluateHealth();
            return history;
        }

        public virtual void AssignToFacility(Facility facility)
        {
            Facility = facility;
        }

        public virtual void ApplyOperationalStatus(DeviceOperationalStatus status)
        {
            switch (status)
            {
                case DeviceOperationalStatus.Active:
                    Activate();
                    break;
                case DeviceOperationalStatus.Inactive:
                    Deactivate();
                    break;
                case DeviceOperationalStatus.Maintenance:
                    SetMaintenance();
                    break;
                case DeviceOperationalStatus.Decommissioned:
                    Decommission();
                    break;
                default:
                    throw new DomainRuleException("Unknown device status.");
            }
        }

        private void SetSerialNumber(string serialNumber)
        {
            SerialNumber = RequireSerialNumber(serialNumber);
        }

        private static string RequireSerialNumber(string serialNumber)
        {
            var trimmed = serialNumber?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                throw new DomainRuleException("Serial number is required.");
            }

            if (trimmed.Length > 128)
            {
                throw new DomainRuleException("Serial number cannot exceed 128 characters.");
            }

            return trimmed;
        }

        private static string RequireReason(string reason)
        {
            var trimmed = reason?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                throw new DomainRuleException("Replacement reason is required.");
            }

            return trimmed;
        }

        private static string NormalizeOptional(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
