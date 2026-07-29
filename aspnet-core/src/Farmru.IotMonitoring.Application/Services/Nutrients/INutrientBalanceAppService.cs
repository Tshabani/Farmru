using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Services.Nutrients.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Nutrients
{
    public interface INutrientBalanceAppService : IApplicationService
    {
        Task<NutrientBalanceSnapshotDto> GetLatest(EntityDto<Guid> fieldId);
        Task<List<NutrientBalanceSnapshotDto>> GetHistory(GetNutrientHistoryInput input);
    }
}
