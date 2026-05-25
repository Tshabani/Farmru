using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.UI;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Domains.Nodes.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Nodes
{
    public class NodeSerialNumberAvailabilityChecker : DomainService, INodeSerialNumberAvailabilityChecker
    {
        private readonly IRepository<Node, Guid> _nodeRepository;

        public NodeSerialNumberAvailabilityChecker(IRepository<Node, Guid> nodeRepository)
        {
            _nodeRepository = nodeRepository;
        }

        public async Task EnsureAvailableAsync(string serialNumber, Guid? excludeNodeId = null)
        {
            if (string.IsNullOrWhiteSpace(serialNumber))
            {
                return;
            }

            var trimmed = serialNumber.Trim();
            var exists = await _nodeRepository.GetAll()
                .AnyAsync(n => n.SerialNumber == trimmed && (!excludeNodeId.HasValue || n.Id != excludeNodeId.Value));

            if (exists)
            {
                throw new UserFriendlyException(L("DeviceSerialAlreadyExists"));
            }
        }
    }
}
