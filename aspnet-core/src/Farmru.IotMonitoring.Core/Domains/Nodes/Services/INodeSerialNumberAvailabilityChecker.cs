using Abp.Domain.Services;
using System;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Domains.Nodes.Services
{
    public interface INodeSerialNumberAvailabilityChecker : IDomainService
    {
        Task EnsureAvailableAsync(string serialNumber, Guid? excludeNodeId = null);
    }
}
