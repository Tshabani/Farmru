using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using Farmru.IotMonitoring.Configuration.Dto;

namespace Farmru.IotMonitoring.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : IotMonitoringAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
