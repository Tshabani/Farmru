using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Services.Crops.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Crops
{
    public interface IFieldAppService : IApplicationService
    {
        Task<List<FieldDto>> GetByFacility(EntityDto<Guid> facilityId);
    }
}
