using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace Farmru.IotMonitoring.Controllers
{
    public abstract class IotMonitoringControllerBase: AbpController
    {
        protected IotMonitoringControllerBase()
        {
            LocalizationSourceName = IotMonitoringConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
