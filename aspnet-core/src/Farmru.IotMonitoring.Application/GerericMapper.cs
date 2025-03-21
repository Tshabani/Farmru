﻿using Farmru.IotMonitoring.Domains.Facilities;
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
            CreateMap<CreateNodeData, NodeData>();
            CreateMap<NodeData, NodeDataDto>()
                .ForMember(u => u.Node, opt => opt.MapFrom(r => r.Node != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.Node.Id, DisplayText = r.Node.SerialNumber } : null));
            CreateMap<NodeDataDto, NodeData>();
            
            CreateMap<CreateNode, Node>()
                .ForMember(u => u.Facility, options => options.MapFrom(e => GetEntity<Facility>(e.Facility)))
                ;

            CreateMap<Node, NodeDto>()
                .ForMember(u => u.Facility, opt => opt.MapFrom(r => r.Facility != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.Facility.Id, DisplayText = r.Facility.Name } : null));
            CreateMap<NodeDto, Node>()
                .ForMember(u => u.Facility, options => options.MapFrom(e => GetEntity<Facility>(e.Facility)));

            CreateMap<Facility, FacilityDto>()
                .ForMember(r => r.OwnerOrganisation, opt => opt.MapFrom(r => r.OwnerOrganisation != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.OwnerOrganisation.Id, DisplayText = r.OwnerOrganisation.Name } : null))
                .ForMember(r => r.PrimaryContact, opt => opt.MapFrom(r => r.PrimaryContact != null ? new EntityWithDisplayNameDto<Guid?> { Id = r.PrimaryContact.Id, DisplayText = r.PrimaryContact.FullName } : null))
                ;
            CreateMap<FacilityDto, Facility>()
                .ForMember(e => e.OwnerOrganisation, options => options.MapFrom(e => GetEntity<Organisation>(e.OwnerOrganisation)))
                .ForMember(e => e.PrimaryContact, options => options.MapFrom(e => GetEntity<Person>(e.PrimaryContact)))
                ; 
            
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
            CreateMap<FacilityAppointmentDto, FacilityAppointment>()
                .ForMember(u => u.AppointedUser, options => options.MapFrom(e => GetEntity<Person>(e.AppointedUser))) 
                .ForMember(u => u.Facility, options => options.MapFrom(e => GetEntity<Facility>(e.Facility))) 
                ;

            CreateMap<Organisation, OrganisationDto>();
            CreateMap<OrganisationDto, Organisation>();

            CreateMap<Person, PersonDto>();
            CreateMap<Person, CreatePersonDto>();
            CreateMap<PersonDto, Person>();
            CreateMap<CreatePersonDto, Person>();

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

            CreateMap<CreateFacilityDto, Facility>()
                .ForMember(u => u.PrimaryContact, options => options.MapFrom(e => GetEntity<Person>(e.PrimaryContact)))
                .ForMember(u => u.OwnerOrganisation, options => options.MapFrom(e => GetEntity<Organisation>(e.OwnerOrganisation)))                
                ;

            CreateMap<CreateFacilityAppointmentDto, FacilityAppointment>()
                .ForMember(u => u.AppointedUser, options => options.MapFrom(e => GetEntity<Person>(e.AppointedUser)))
                .ForMember(u => u.Facility, options => options.MapFrom(e => GetEntity<Facility>(e.Facility)))
                ;
        }
    }
}
