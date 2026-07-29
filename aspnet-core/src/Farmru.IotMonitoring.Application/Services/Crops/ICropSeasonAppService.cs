using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Services.Crops.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Crops
{
    public interface ICropSeasonAppService : IApplicationService
    {
        Task<CropSeasonDto> Plant(PlantCropSeasonInput input);
        Task<CropSeasonDetailDto> GetDetail(EntityDto<Guid> input);
        Task<PagedResultDto<CropSeasonDto>> GetByField(GetCropSeasonsByFieldInput input);
        Task<CropSeasonDetailDto> LogGrowthStage(LogGrowthStageInput input);
        Task<CropSeasonDetailDto> Harvest(HarvestCropSeasonInput input);
        Task<CropSeasonDto> Close(EntityDto<Guid> input);
        Task<List<CropRotationEntryDto>> GetRotationHistory(EntityDto<Guid> fieldId);
    }
}
