using Abp.Application.Services;
using Abp.Domain.Repositories;
using Farmru.IotMonitoring.Services.NodeData.Dto;
using Farmru.IotMonitoring.Users.Dto;
using System;
using System.Threading.Tasks;
using Abp.UI;
using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services.Dto;
using Microsoft.EntityFrameworkCore;
using Farmru.IotMonitoring.Services.NodeDatas.Dto;
using Abp.Runtime.Session;
using Farmru.IotMonitoring.Domains.Facilities;
using Farmru.IotMonitoring.Domains.Organisations;
using Node = Farmru.IotMonitoring.Domains.Nodes.Node;
using Farmru.IotMonitoring.Domains.Alerts.Services;
using Farmru.IotMonitoring.Domains.Geo.Services;
using Abp.Authorization;
using System.IO;
using ClosedXML.Excel;

namespace Farmru.IotMonitoring.Services.NodeDatas
{
    /// <summary>
    /// 
    /// </summary>
    //[AbpAuthorize()]
    public class NodeDataAppService : AsyncCrudAppService<Domains.Nodes.NodeData, NodeDataDto, Guid, PagedUserResultRequestDto, CreateNodeData, NodeDataDto>, INodeDataAppService
    {
        private readonly IRepository<Node, Guid> _nodeRepository;
        private readonly IAbpSession _session;
        private readonly ITelemetryAlertEvaluationService _alertEvaluationService;
        private readonly IGeoFenceEvaluationService _geoFenceEvaluationService;

        public NodeDataAppService(
            IRepository<Domains.Nodes.NodeData, Guid> repository,
            IRepository<Node, Guid> nodeRepository,
            IAbpSession session,
            ITelemetryAlertEvaluationService alertEvaluationService,
            IGeoFenceEvaluationService geoFenceEvaluationService) : base(repository)
        {
            _nodeRepository = nodeRepository;
            _session = session;
            _alertEvaluationService = alertEvaluationService;
            _geoFenceEvaluationService = geoFenceEvaluationService;
        }

        public override async Task<NodeDataDto> CreateAsync(CreateNodeData input)
        {
            ArgumentNullException.ThrowIfNull(input);
            if (string.IsNullOrWhiteSpace(input.SerialNumber)) throw new ArgumentNullException("Serial Number is required");

            var node = await _nodeRepository.FirstOrDefaultAsync(x => x.SerialNumber == input.SerialNumber) ?? throw new UserFriendlyException("Node not found");

            var nodeData = Domains.Nodes.NodeData.RecordFromDevice(
                node,
                input.SoilTemperature,
                input.SoilPH,
                input.Moisture,
                input.Phosphorus,
                input.Potassium,
                input.Nitrogen,
                input.Latitude,
                input.Longitude,
                input.SolarPanelVoltage,
                input.BatteryVoltage,
                input.Conductivity,
                input.LoggingTime);

            nodeData = await Repository.InsertAsync(nodeData);

            var seenAt = nodeData.LoggingTime ?? DateTime.UtcNow;
            var batteryLevel = Node.BatteryLevelFromVoltage(input.BatteryVoltage);
            node.RecordTelemetry(seenAt, batteryLevel, node.SignalStrength);
            node.UpdateKnownLocation(input.Latitude, input.Longitude);
            await _nodeRepository.UpdateAsync(node);

            await CurrentUnitOfWork.SaveChangesAsync();

            await _alertEvaluationService.EvaluateAfterTelemetryAsync(node, nodeData);

            var coords = node.ResolveMapCoordinates();
            if (coords.HasValue)
            {
                await _geoFenceEvaluationService.EvaluateNodeLocationAsync(
                    node,
                    coords.Value.Latitude,
                    coords.Value.Longitude);
            }

            return ObjectMapper.Map<NodeDataDto>(nodeData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<HistoricalDataResponse> GetReadings()
        {
            try
            {
                var tenantId = _session.GetTenantId();

                var facilityRepo = Abp.Dependency.IocManager.Instance.Resolve<IRepository<Facility, Guid>>();
                var organisationRepo = Abp.Dependency.IocManager.Instance.Resolve<IRepository<Organisation, Guid>>();

                var organisation = await organisationRepo.FirstOrDefaultAsync(r => r.TenantId == tenantId);
                if (organisation == null)
                    return new HistoricalDataResponse(); // No organization found

                var facility = await facilityRepo.FirstOrDefaultAsync(r => r.IsDefault == true && r.OwnerOrganisation == organisation);
                if (facility == null)
                    return new HistoricalDataResponse(); // No default facility found

                var nodes = await _nodeRepository.GetAllListAsync(r => r.Facility == facility);
                if (!nodes.Any())
                    return new HistoricalDataResponse(); // No nodes found

                var sevenDaysAgo = DateTime.UtcNow.AddDays(-800); // Include today (7 days total)

                // Fetch all readings for the past 7 days
                var pastWeekReadings = await Repository.GetAll()
                    .Where(r => r.Node != null && nodes.Contains(r.Node) && r.CreationTime >= sevenDaysAgo)
                    .OrderByDescending(r => r.CreationTime)
                    .ToListAsync();

                if (!pastWeekReadings.Any())
                    return new HistoricalDataResponse(); // No readings available

                // Define max allowed values for each parameter
                const double MaxSoilTemp = 50.0;
                const double MaxSoilPH = 14.0;
                const double MaxMoisture = 100.0;
                const double MaxPhosphorus = 150.0;
                const double MaxPotassium = 150.0;
                const double MaxNitrogen = 100.0;
                const double MaxConductivity = 2500.0;
                const double MaxSolarVoltage = 12.0;
                const double MaxBatteryVoltage = 12.0;

                // Calculate average values rounded to 2 decimal places with upper limits applied
                var averageReadings = new CurrentReadings
                {
                    SoilTemp = Math.Min(RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.SoilTemperature))), MaxSoilTemp),
                    SoilPH = Math.Min(RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.SoilPH))), MaxSoilPH),
                    Moisture = Math.Min(RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.Moisture))), MaxMoisture),
                    Phosphorus = Math.Min(RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.Phosphorus))), MaxPhosphorus),
                    Potassium = Math.Min(RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.Potassium))), MaxPotassium),
                    Nitrogen = Math.Min(RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => ConvertToDouble(r.Nitrogen))), MaxNitrogen),
                    Conductivity = pastWeekReadings.Any(r => r.Conductivity.HasValue) 
                                        ? Math.Min(RoundToTwoDecimalPlaces(pastWeekReadings.Where(r => r.Conductivity.HasValue).Average(r => r.Conductivity.Value)), MaxConductivity) : 0,
                    SolarVoltage = Math.Min(RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => r.SolarPanelVoltage.HasValue ? r.SolarPanelVoltage.Value / 1000.0 : 0)), MaxSolarVoltage),
                    BatteryVoltage = Math.Min(RoundToTwoDecimalPlaces(pastWeekReadings.Average(r => r.BatteryVoltage.HasValue ? r.BatteryVoltage.Value / 1000.0 : 0)), MaxBatteryVoltage)
                };

                // Group historical data by day
                var historicalData = pastWeekReadings
                    .GroupBy(r => r.CreationTime.Date)
                    .Select(g => new HistoricalData
                    {
                        Day = g.Key.ToString("ddd"),
                        SoilTemp = RoundToTwoDecimalPlaces(g.Average(r => ConvertToDouble(r.SoilTemperature))),
                        Moisture = RoundToTwoDecimalPlaces(g.Average(r => ConvertToDouble(r.Moisture))),
                        PH = RoundToTwoDecimalPlaces(g.Average(r => ConvertToDouble(r.SoilPH)))
                    })
                    .OrderBy(h => h.Day) // Ensure correct order
                    .ToList();

                return new HistoricalDataResponse
                {
                    CurrentReadings = averageReadings, // Return the calculated averages
                    historicalData = historicalData
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Helper function to safely convert string values to double
        private double ConvertToDouble(string value)
        {
            return double.TryParse(value, out double result) ? result : 0;
        }

        // Helper function to round values to two decimal places
        private double RoundToTwoDecimalPlaces(double value)
        {
            return Math.Round(value, 2);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<NodeDataDto>> GetNodeDataByNodeId(Guid nodeId, PagedNodeDataResultRequestDto input)
        {
            await EnsureNodeExistsAsync(nodeId);

            var query = GetNodeDataQuery(nodeId, input);

            var totalCount = await query.CountAsync();

            var nodeData = await query
                .OrderByDescending(c => c.CreationTime)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .ToListAsync();

            return new PagedResultDto<NodeDataDto>(
                totalCount,
                ObjectMapper.Map<List<NodeDataDto>>(nodeData)
            );
        }

        /// <summary>
        /// Builds an Excel workbook for all node readings matching the same filters as the grid (not paginated).
        /// </summary>
        public async Task<NodeDataExcelExportDto> ExportNodeDataToExcel(Guid nodeId, PagedNodeDataResultRequestDto input)
        {
            await EnsureNodeExistsAsync(nodeId);

            var rows = await GetNodeDataQuery(nodeId, input)
                .OrderByDescending(c => c.CreationTime)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Node Data");

            ws.Cell(1, 1).Value = "Latitude";
            ws.Cell(1, 2).Value = "Longitude";
            ws.Cell(1, 3).Value = "Creation Time";
            ws.Cell(1, 4).Value = "Moisture";
            ws.Cell(1, 5).Value = "Nitrogen";
            ws.Cell(1, 6).Value = "Phosphorus";
            ws.Cell(1, 7).Value = "Potassium";
            ws.Cell(1, 8).Value = "Soil PH";
            ws.Cell(1, 9).Value = "Soil Temperature";
            ws.Cell(1, 10).Value = "Solar Panel Voltage";
            ws.Cell(1, 11).Value = "Battery Voltage";
            ws.Cell(1, 12).Value = "Conductivity";

            for (var i = 0; i < rows.Count; i++)
            {
                var r = rows[i];
                var excelRow = i + 2;
                ws.Cell(excelRow, 1).Value = r.Latitude;
                ws.Cell(excelRow, 2).Value = r.Longitude;
                ws.Cell(excelRow, 3).Value = r.CreationTime;
                ws.Cell(excelRow, 3).Style.DateFormat.Format = "dd MMMM yyyy HH:mm:ss";
                ws.Cell(excelRow, 4).Value = r.Moisture;
                ws.Cell(excelRow, 5).Value = r.Nitrogen;
                ws.Cell(excelRow, 6).Value = r.Phosphorus;
                ws.Cell(excelRow, 7).Value = r.Potassium;
                ws.Cell(excelRow, 8).Value = r.SoilPH;
                ws.Cell(excelRow, 9).Value = r.SoilTemperature;
                ws.Cell(excelRow, 10).Value = r.SolarPanelVoltage;
                ws.Cell(excelRow, 11).Value = r.BatteryVoltage;
                ws.Cell(excelRow, 12).Value = r.Conductivity;
            }

            ws.SheetView.FreezeRows(1);
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return new NodeDataExcelExportDto
            {
                FileName = $"node-data-{nodeId:N}.xlsx",
                ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                FileBytes = stream.ToArray()
            };
        }

        private async Task EnsureNodeExistsAsync(Guid nodeId)
        {
            var nodeExists = await _nodeRepository.FirstOrDefaultAsync(x => x.Id == nodeId);
            if (nodeExists == null)
            {
                throw new UserFriendlyException("Node not found");
            }
        }

        private static (DateTime? Start, DateTime? End) ResolveDateRange(PagedNodeDataResultRequestDto input)
        {
            DateTime? startDate = input.StartDate;
            DateTime? endDate = input.EndDate;

            if (!string.IsNullOrEmpty(input.PredefinedPeriod))
            {
                switch (input.PredefinedPeriod.ToLowerInvariant())
                {
                    case "today":
                        startDate = DateTime.Today;
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                    case "week":
                        startDate = DateTime.Today.AddDays(-7);
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                    case "month":
                        startDate = DateTime.Today.AddMonths(-1);
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                    case "year":
                        startDate = DateTime.Today.AddYears(-1);
                        endDate = DateTime.Today.AddDays(1).AddTicks(-1);
                        break;
                }
            }

            return (startDate, endDate);
        }

        private IQueryable<Domains.Nodes.NodeData> GetNodeDataQuery(Guid nodeId, PagedNodeDataResultRequestDto input)
        {
            var (startDate, endDate) = ResolveDateRange(input);

            var query = Repository.GetAll()
                .Where(r => r.Node != null && r.Node.Id == nodeId);

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(r => r.CreationTime >= startDate && r.CreationTime <= endDate);
            }

            return query;
        }

    }
}
