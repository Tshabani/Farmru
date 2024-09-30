using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Farmru.IotMonitoring.Authorization;

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
