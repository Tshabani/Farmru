using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Services.GeoSpatial.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.GeoSpatial
{
    public interface IGeoSpatialAppService : IApplicationService
    {
        Task<OperationalMapDto> GetOperationalMap();
        Task<ExecutiveGisSummaryDto> GetExecutiveSummary();
        Task<List<AlertHeatmapPointDto>> GetAlertHeatmap();
        Task<List<DeviceMapMarkerDto>> GetNearbyDevices(NearbyQueryInput input);
        Task<List<IncidentMapMarkerDto>> GetNearbyIncidents(NearbyQueryInput input);
        Task<List<GeoFenceDto>> GetGeoFences();
        Task<GeoFenceDto> GetGeoFence(EntityDto<Guid> input);
        Task<GeoFenceDto> CreateGeoFence(CreateGeoFenceInput input);
        Task<GeoFenceDto> UpdateGeoFence(UpdateGeoFenceInput input);
        Task DeleteGeoFence(EntityDto<Guid> input);
    }
}
