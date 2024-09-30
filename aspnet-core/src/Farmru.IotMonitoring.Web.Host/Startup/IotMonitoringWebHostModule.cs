using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Farmru.IotMonitoring.Configuration;

namespace Farmru.IotMonitoring.Web.Host.Startup
{
    [DependsOn(
       typeof(IotMonitoringWebCoreModule))]
    public class IotMonitoringWebHostModule: AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public IotMonitoringWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(IotMonitoringWebHostModule).GetAssembly());
        }
    }
}
