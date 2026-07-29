using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Services.Weather.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Weather
{
    public interface IWeatherAlertRuleAppService : IApplicationService
    {
        Task<WeatherAlertRuleDto> Create(CreateWeatherAlertRuleInput input);
        Task<List<WeatherAlertRuleDto>> GetForFacility(EntityDto<Guid> facilityId);
        Task Deactivate(EntityDto<Guid> input);
    }
}
