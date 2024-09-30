using System.Collections.Generic;

namespace Farmru.IotMonitoring.Authentication.External
{
    public interface IExternalAuthConfiguration
    {
        List<ExternalLoginProviderInfo> Providers { get; }
    }
}
