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
using System.Collections.Generic;
using System.Linq;

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
            CreateMap<NodeData, NodeDataDto>()
                .ForMember(u => u.Node, opt => opt.MapFrom(r => r.Node != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.Node.Id, DisplayText = r.Node.SerialNumber } : null));

            CreateMap<Node, NodeDto>()
                .ForMember(u => u.Facility, opt => opt.MapFrom(r => r.Facility != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.Facility.Id, DisplayText = r.Facility.Name } : null))
                .ForMember(u => u.IsOnline, opt => opt.Ignore());
            CreateMap<Node, NodeDetailDto>()
                .ForMember(u => u.Facility, opt => opt.MapFrom(r => r.Facility != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.Facility.Id, DisplayText = r.Facility.Name } : null))
                .ForMember(u => u.IsOnline, opt => opt.Ignore())
                .ForMember(u => u.TelemetrySummary, opt => opt.Ignore())
                .ForMember(u => u.ReplacementHistory, opt => opt.Ignore());
            CreateMap<NodeReplacementHistory, NodeReplacementHistoryDto>()
                .ForMember(d => d.NodeId, opt => opt.MapFrom(s => s.Node.Id));

            CreateMap<Facility, FacilityDto>()
                .ForMember(r => r.OwnerOrganisation, opt => opt.MapFrom(r => r.OwnerOrganisation != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.OwnerOrganisation.Id, DisplayText = r.OwnerOrganisation.Name } : null))
                .ForMember(r => r.PrimaryContact, opt => opt.MapFrom(r => r.PrimaryContact != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.PrimaryContact.Id, DisplayText = r.PrimaryContact.FullName } : null))
                ;
            CreateMap<TaskManagement, TaskManagementDto>()
                .ForMember(r => r.AssignedTo, opt => opt.MapFrom(r => r.AssignedTo != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.AssignedTo.Id, DisplayText = r.AssignedTo.FullName } : null))
                .ForMember(r => r.AssignedBy, opt => opt.MapFrom(r => r.AssignedBy != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.AssignedBy.Id, DisplayText = r.AssignedBy.FullName } : null))
                ;
            CreateMap<FacilityAppointment, FacilityAppointmentDto>()
                .ForMember(r => r.AppointedUser, opt => opt.MapFrom(r => r.AppointedUser != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.AppointedUser.Id, DisplayText = r.AppointedUser.FullName } : null))
                .ForMember(r => r.Facility, opt => opt.MapFrom(r => r.Facility != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.Facility.Id, DisplayText = r.Facility.Name } : null))
                ;
            CreateMap<Organisation, OrganisationDto>();

            CreateMap<Person, PersonDto>();
            CreateMap<Person, CreatePersonDto>();

            CreateMap<Person, EntityWithDisplayNameDto<Guid?>>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.DisplayText, opt => opt.MapFrom(src => src.FullName != null ? src.FullName : string.Empty))
               ;

            CreateMap<Person, PeopleDto>()
                .ForMember(r => r.FullName, opt => opt.MapFrom(e => e.FullName))
                .ForMember(r => r.Id, opt => opt.MapFrom(e => e.Id))
                ;

            CreateMap<Facility, FacilitiesDto>()
                .ForMember(r => r.Name, opt => opt.MapFrom(e => e.Name))
                .ForMember(r => r.Id, opt => opt.MapFrom(e => e.Id))
                ;

            CreateMap<Organisation, OrganisationsDto>()
                .ForMember(r => r.Name, opt => opt.MapFrom(e => e.Name))
                .ForMember(r => r.Id, opt => opt.MapFrom(e => e.Id))
                ;

        }
    }
}
