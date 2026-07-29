using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains.Nutrients;
using Farmru.IotMonitoring.Services.Nutrients.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Nutrients
{
    [AbpAuthorize(PermissionNames.Pages_Nutrients)]
    public class NutrientBalanceAppService : IotMonitoringAppServiceBase, INutrientBalanceAppService
    {
        private readonly IRepository<NutrientBalanceSnapshot, Guid> _snapshotRepository;

        public NutrientBalanceAppService(IRepository<NutrientBalanceSnapshot, Guid> snapshotRepository)
        {
            _snapshotRepository = snapshotRepository;
        }

        public async Task<NutrientBalanceSnapshotDto> GetLatest(EntityDto<Guid> fieldId)
        {
            var latest = await _snapshotRepository.GetAll()
                .Where(s => s.FieldId == fieldId.Id)
                .OrderByDescending(s => s.SnapshotDate)
                .FirstOrDefaultAsync();

            if (latest == null)
            {
                throw new UserFriendlyException(L("NoNutrientBalanceDataAvailable"));
            }

            return MapToDto(latest);
        }

        public async Task<List<NutrientBalanceSnapshotDto>> GetHistory(GetNutrientHistoryInput input)
        {
            var query = _snapshotRepository.GetAll().Where(s => s.FieldId == input.FieldId);

            if (input.FromDate.HasValue)
            {
                query = query.Where(s => s.SnapshotDate >= input.FromDate.Value);
            }

            if (input.ToDate.HasValue)
            {
                query = query.Where(s => s.SnapshotDate <= input.ToDate.Value);
            }

            var items = await query.OrderByDescending(s => s.SnapshotDate).Take(90).ToListAsync();
            return items.Select(MapToDto).ToList();
        }

        private static NutrientBalanceSnapshotDto MapToDto(NutrientBalanceSnapshot snapshot) => new NutrientBalanceSnapshotDto
        {
            Id = snapshot.Id,
            FieldId = snapshot.FieldId,
            SnapshotDate = snapshot.SnapshotDate,
            SensedNitrogen = snapshot.SensedNitrogen,
            SensedPhosphorus = snapshot.SensedPhosphorus,
            SensedPotassium = snapshot.SensedPotassium,
            AppliedNitrogenTrailing30d = snapshot.AppliedNitrogenTrailing30d,
            AppliedPhosphorusTrailing30d = snapshot.AppliedPhosphorusTrailing30d,
            AppliedPotassiumTrailing30d = snapshot.AppliedPotassiumTrailing30d,
            NitrogenStatus = snapshot.NitrogenStatus,
            PhosphorusStatus = snapshot.PhosphorusStatus,
            PotassiumStatus = snapshot.PotassiumStatus
        };
    }
}
