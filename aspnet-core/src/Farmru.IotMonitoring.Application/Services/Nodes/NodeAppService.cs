using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Runtime.Session;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using Farmru.IotMonitoring.Services.Nodes.Dto;
using Farmru.IotMonitoring.Users.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Nodes
{
    /// <summary>
    /// 
    /// </summary>
    [AbpAuthorize]
    public class NodeAppService : AsyncCrudAppService<Node, NodeDto, Guid, PagedResultRequestDto, CreateNode, NodeDto>, INodeAppService
    {
        private readonly IRepository<Person, Guid> _personRepository;
        private readonly IRepository<FacilityAppointment, Guid> _facilityAppointmentRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        public NodeAppService(IRepository<Node, Guid> repository, IRepository<Person, Guid> personRepository, IRepository<FacilityAppointment, Guid> facilityAppointmentRepository) : base(repository)
        {
            _personRepository = personRepository;
            _facilityAppointmentRepository = facilityAppointmentRepository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<NodeDto>> GetMyNodes()
        { 
            var person = await _personRepository.GetAll().FirstOrDefaultAsync(p => p.User.Id == AbpSession.GetUserId());
            var facilityAppointments = await _facilityAppointmentRepository.GetAllListAsync(r => r.AppointedUser == person);
            var facility = facilityAppointments.Select(fa => fa.Facility).ToList();

            var nodes = await Repository.GetAllListAsync(r => facility.Contains(r.Facility));

            return ObjectMapper.Map<List<NodeDto>>(nodes);
        }
        
    }
}
