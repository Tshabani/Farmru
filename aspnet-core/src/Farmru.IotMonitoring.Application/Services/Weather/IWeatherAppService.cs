using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Services.Weather.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Weather
{
    public interface IWeatherAppService : IApplicationService
    {
        Task<WeatherObservationDto> GetCurrent(EntityDto<Guid> facilityId);
        Task<List<WeatherForecastDto>> GetForecast(EntityDto<Guid> facilityId);
        Task<PagedResultDto<WeatherObservationDto>> GetHistory(GetWeatherHistoryInput input);
        Task<List<EvapotranspirationDto>> GetEvapotranspiration(GetEvapotranspirationInput input);
    }
}
