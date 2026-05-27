using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Farmru.IotMonitoring.Alerts;
using Farmru.IotMonitoring.Authorization;
using Farmru.IotMonitoring.GeoSpatial;
using Farmru.IotMonitoring.Incidents;
using Farmru.IotMonitoring.Monitoring;

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
            IocManager.Register<IAlertRealtimeNotifier, NullAlertRealtimeNotifier>(Abp.Dependency.DependencyLifeStyle.Transient);
            IocManager.Register<IOperationalRealtimeNotifier, NullOperationalRealtimeNotifier>(Abp.Dependency.DependencyLifeStyle.Transient);
            IocManager.Register<IGeoSpatialRealtimeNotifier, NullGeoSpatialRealtimeNotifier>(Abp.Dependency.DependencyLifeStyle.Transient);
            IocManager.Register<IIncidentRealtimeNotifier, NullIncidentRealtimeNotifier>(Abp.Dependency.DependencyLifeStyle.Transient);
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
