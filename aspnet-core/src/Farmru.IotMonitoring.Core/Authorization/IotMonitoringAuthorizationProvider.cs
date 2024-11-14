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
            context.CreatePermission(PermissionNames.Pages_Nodes, L("Nodes"));
            context.CreatePermission(PermissionNames.Pages_People, L("People"));
            context.CreatePermission(PermissionNames.Pages_Organisations, L("Organisations"));
            context.CreatePermission(PermissionNames.Pages_Facilities, L("Facilities"));
            context.CreatePermission(PermissionNames.Pages_Facility_Appointments, L("FacilityAppointments"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, IotMonitoringConsts.LocalizationSourceName);
        }
    }
}
