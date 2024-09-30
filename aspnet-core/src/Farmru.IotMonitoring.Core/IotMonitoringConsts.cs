using Farmru.IotMonitoring.Debugging;

namespace Farmru.IotMonitoring
{
    public class IotMonitoringConsts
    {
        public const string LocalizationSourceName = "IotMonitoring";

        public const string ConnectionStringName = "Default";

        public const bool MultiTenancyEnabled = true;


        /// <summary>
        /// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
        /// </summary>
        public static readonly string DefaultPassPhrase =
            DebugHelper.IsDebug ? "gsKxGZ012HLL3MI5" : "ee70fb415f3d4406a75028ba0a894ef4";
    }
}
