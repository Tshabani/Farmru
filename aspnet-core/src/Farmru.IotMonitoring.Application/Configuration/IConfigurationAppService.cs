using System.Threading.Tasks;
using Farmru.IotMonitoring.Configuration.Dto;

namespace Farmru.IotMonitoring.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
