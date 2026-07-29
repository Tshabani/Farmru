using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Farmru.IotMonitoring.Authorization
{
    public class IotMonitoringAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Home, L("Home"));
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Users_Activation, L("UsersActivation"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            var nodes = context.CreatePermission(PermissionNames.Pages_Nodes, L("Devices"));
            nodes.CreateChildPermission(PermissionNames.Pages_Nodes_Manage, L("DeviceManagement"));
            context.CreatePermission(PermissionNames.Pages_People, L("People"));
            context.CreatePermission(PermissionNames.Pages_Organisations, L("Organisations"));
            context.CreatePermission(PermissionNames.Pages_Facilities, L("Facilities"));
            context.CreatePermission(PermissionNames.Pages_Facility_Appointments, L("FacilityAppointments"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

            var alerts = context.CreatePermission(PermissionNames.Pages_Alerts, L("Alerts"));
            alerts.CreateChildPermission(PermissionNames.Pages_Alerts_Manage, L("AlertManagement"));

            var monitoring = context.CreatePermission(PermissionNames.Pages_Monitoring, L("OperationalMonitoring"));
            monitoring.CreateChildPermission(PermissionNames.Pages_Monitoring_Manage, L("MonitoringConfiguration"));

            var gis = context.CreatePermission(PermissionNames.Pages_Gis, L("OperationalGis"));
            gis.CreateChildPermission(PermissionNames.Pages_Gis_Manage, L("GisManagement"));

            var incidents = context.CreatePermission(PermissionNames.Pages_Incidents, L("Incidents"));
            incidents.CreateChildPermission(PermissionNames.Pages_Incidents_Manage, L("IncidentManagement"));

            var weather = context.CreatePermission(PermissionNames.Pages_Weather, L("Weather"));
            weather.CreateChildPermission(PermissionNames.Pages_Weather_Configure, L("WeatherConfiguration"));

            var fields = context.CreatePermission(PermissionNames.Pages_Fields, L("Fields"));
            fields.CreateChildPermission(PermissionNames.Pages_Fields_Manage, L("FieldManagement"));

            var crops = context.CreatePermission(PermissionNames.Pages_Crops, L("Crops"));
            crops.CreateChildPermission(PermissionNames.Pages_Crops_Manage, L("CropManagement"));
            crops.CreateChildPermission(PermissionNames.Pages_Crops_Harvest, L("CropHarvest"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, IotMonitoringConsts.LocalizationSourceName);
        }
    }
}
