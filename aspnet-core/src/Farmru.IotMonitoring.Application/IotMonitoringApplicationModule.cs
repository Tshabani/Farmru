using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Farmru.IotMonitoring.Alerts;
using Farmru.IotMonitoring.Incidents;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.Domains.Alerts.Services;
using Farmru.IotMonitoring.GeoSpatial;
using Farmru.IotMonitoring.Monitoring;
using Farmru.IotMonitoring.Services.Alerts;

namespace Farmru.IotMonitoring
{
    [DependsOn(
        typeof(IotMonitoringCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class IotMonitoringApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<IotMonitoringAuthorizationProvider>();
            Configuration.ReplaceService(typeof(IAlertRealtimeNotifier), () => IocManager.Resolve<NullAlertRealtimeNotifier>());
            Configuration.ReplaceService(typeof(IOperationalRealtimeNotifier), () => IocManager.Resolve<NullOperationalRealtimeNotifier>());
            Configuration.ReplaceService(typeof(IGeoSpatialRealtimeNotifier), () => IocManager.Resolve<NullGeoSpatialRealtimeNotifier>());
            Configuration.ReplaceService(typeof(IIncidentRealtimeNotifier), () => IocManager.Resolve<NullIncidentRealtimeNotifier>());
        }

        public override void Initialize()
        {
            var thisAssembly = typeof(IotMonitoringApplicationModule).GetAssembly();

            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                // Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg => cfg.AddMaps(thisAssembly)
            );
        }
    }
}
