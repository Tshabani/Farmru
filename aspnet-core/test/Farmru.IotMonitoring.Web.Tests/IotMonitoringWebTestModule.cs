using Abp.AspNetCore;
using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Farmru.IotMonitoring.EntityFrameworkCore;
using Farmru.IotMonitoring.Web.Startup;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Farmru.IotMonitoring.Web.Tests
{
    [DependsOn(
        typeof(IotMonitoringWebMvcModule),
        typeof(AbpAspNetCoreTestBaseModule)
    )]
    public class IotMonitoringWebTestModule : AbpModule
    {
        public IotMonitoringWebTestModule(IotMonitoringEntityFrameworkModule abpProjectNameEntityFrameworkModule)
        {
            abpProjectNameEntityFrameworkModule.SkipDbContextRegistration = true;
        } 
        
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(IotMonitoringWebTestModule).GetAssembly());
        }
        
        public override void PostInitialize()
        {
            IocManager.Resolve<ApplicationPartManager>()
                .AddApplicationPartsIfNotAddedBefore(typeof(IotMonitoringWebMvcModule).Assembly);
        }
    }
}