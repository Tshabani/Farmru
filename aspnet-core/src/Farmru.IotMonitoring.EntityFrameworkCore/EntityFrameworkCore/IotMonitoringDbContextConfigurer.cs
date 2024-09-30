using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Farmru.IotMonitoring.EntityFrameworkCore
{
    public static class IotMonitoringDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<IotMonitoringDbContext> builder, string connectionString)
        {
            builder.UseSqlServer(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<IotMonitoringDbContext> builder, DbConnection connection)
        {
            builder.UseSqlServer(connection);
        }
    }
}
