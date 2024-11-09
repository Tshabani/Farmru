using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Incidents;
using Farmru.IotMonitoring.Domains.Nodes;
using Farmru.IotMonitoring.Domains.Organisations;
using Farmru.IotMonitoring.Domains.Persons;
using Farmru.IotMonitoring.Domains.Tasks;
using Farmru.IotMonitoring.Helpers;
using Farmru.IotMonitoring.Services.Facilities.Dto;
using Farmru.IotMonitoring.Services.Incidents.Dto;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using Farmru.IotMonitoring.Services.Nodes.Dto;
using Farmru.IotMonitoring.Services.Organisations.Dto;
using Farmru.IotMonitoring.Services.Persons.Dtos;
using Farmru.IotMonitoring.Services.Tasks.Dto;
using System;

namespace Farmru.IotMonitoring
{
    /// <summary>
    /// 
    /// </summary>
    public class GerericMapper : ProfileHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public GerericMapper()
        {
            CreateMap<CreateNodeData, NodeData>();
            CreateMap<NodeData, NodeDataDto>()
                .ForMember(u => u.Node, opt => opt.MapFrom(r => r.Node != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.Node.Id, DisplayText = r.Node.SerialNumber } : null));
            CreateMap<NodeDataDto, NodeData>();
            
            CreateMap<CreateNode, Node>();
            CreateMap<Node, NodeDto>();
            CreateMap<NodeDto, Node>();

            CreateMap<Facility, FacilityDto>()
                .ForMember(r => r.OwnerOrganisation, opt => opt.MapFrom(r => r.OwnerOrganisation != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.OwnerOrganisation.Id, DisplayText = r.OwnerOrganisation.Name } : null))
                .ForMember(r => r.PrimaryContact, opt => opt.MapFrom(r => r.PrimaryContact != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.PrimaryContact.Id, DisplayText = r.PrimaryContact.FullName } : null))
                ;
            CreateMap<FacilityDto, Facility>(); 
            
            CreateMap<TaskManagement, TaskManagementDto>()
                .ForMember(r => r.AssignedTo, opt => opt.MapFrom(r => r.AssignedTo != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.AssignedTo.Id, DisplayText = r.AssignedTo.FullName } : null))
                .ForMember(r => r.AssignedBy, opt => opt.MapFrom(r => r.AssignedBy != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.AssignedBy.Id, DisplayText = r.AssignedBy.FullName } : null))
                ;
            CreateMap<TaskManagementDto, TaskManagement>(); 
            
            
            CreateMap<Incident, IncidentDto>()
                .ForMember(r => r.CreatedBy, opt => opt.MapFrom(r => r.CreatedBy != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.CreatedBy.Id, DisplayText = r.CreatedBy.FullName } : null))
                .ForMember(r => r.AssignedTo, opt => opt.MapFrom(r => r.AssignedTo != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.AssignedTo.Id, DisplayText = r.AssignedTo.FullName } : null))
                ;
            CreateMap<IncidentDto, Incident>();
            
            CreateMap<FacilityAppointment, FacilityAppointmentDto>()
                .ForMember(r => r.AppointedUser, opt => opt.MapFrom(r => r.AppointedUser != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.AppointedUser.Id, DisplayText = r.AppointedUser.FullName } : null))
                .ForMember(r => r.Facility, opt => opt.MapFrom(r => r.Facility != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.Facility.Id, DisplayText = r.Facility.Name } : null))
                ;
            CreateMap<FacilityAppointmentDto, FacilityAppointment>();

            CreateMap<Organisation, OrganisationDto>();
            CreateMap<OrganisationDto, Organisation>();

            CreateMap<Person, PersonDto>()
                .ForMember(u => u.IsActive, opt => opt.MapFrom(r => r.User.IsActive))
               ;

        }
    }
}
