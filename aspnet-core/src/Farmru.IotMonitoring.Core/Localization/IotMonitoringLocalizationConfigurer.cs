using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace Farmru.IotMonitoring.Localization
{
    public static class IotMonitoringLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(IotMonitoringConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(IotMonitoringLocalizationConfigurer).GetAssembly(),
                        "Farmru.IotMonitoring.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
