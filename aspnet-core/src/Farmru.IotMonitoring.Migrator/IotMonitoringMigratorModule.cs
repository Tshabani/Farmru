using Microsoft.Extensions.Configuration;
using Castle.MicroKernel.Registration;
using Abp.Events.Bus;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Farmru.IotMonitoring.Configuration;
using Farmru.IotMonitoring.EntityFrameworkCore;
using Farmru.IotMonitoring.Migrator.DependencyInjection;

namespace Farmru.IotMonitoring.Migrator
{
    [DependsOn(typeof(IotMonitoringEntityFrameworkModule))]
    public class IotMonitoringMigratorModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public IotMonitoringMigratorModule(IotMonitoringEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbSeed = true;

            _appConfiguration = AppConfigurations.Get(
                typeof(IotMonitoringMigratorModule).GetAssembly().GetDirectoryPathOrNull()
            );
        }

        public override void PreInitialize()
        {
            Configuration.DefaultNameOrConnectionString = _appConfiguration.GetConnectionString(
                IotMonitoringConsts.ConnectionStringName
            );

            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;
            Configuration.ReplaceService(
                typeof(IEventBus), 
                () => IocManager.IocContainer.Register(
                    Component.For<IEventBus>().Instance(NullEventBus.Instance)
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(IotMonitoringMigratorModule).GetAssembly());
            ServiceCollectionRegistrar.Register(IocManager);
        }
    }
}
