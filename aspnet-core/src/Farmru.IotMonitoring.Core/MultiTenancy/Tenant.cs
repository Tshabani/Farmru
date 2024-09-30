using Abp.MultiTenancy;
using Farmru.IotMonitoring.Authorization.Users;

namespace Farmru.IotMonitoring.MultiTenancy
{
    public class Tenant : AbpTenant<User>
    {
        public Tenant()
        {            
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
