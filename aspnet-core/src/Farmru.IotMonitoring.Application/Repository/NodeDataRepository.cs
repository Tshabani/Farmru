using Abp.EntityFrameworkCore;
using Farmru.IotMonitoring.Domains.Stats;
using Farmru.IotMonitoring.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Abp.Domain.Uow.AbpDataFilters;

namespace Farmru.IotMonitoring.Repository
{
    public class NodeDataRepository : INodeDataRepository
    {
        private readonly IDbContextProvider<IotMonitoringDbContext> _dbContextProvider;
        public NodeDataRepository(IDbContextProvider<IotMonitoringDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }
        
        public async Task<AverageNodeData> GetAverageNodeDataAsync()
        {
            var dbContext = await _dbContextProvider.GetDbContextAsync();
            var result = await dbContext.AverageNodeData.FromSqlRaw("EXEC [zarleaks].[sp_GetAverageNodeData]").ToListAsync();

            var averageNodeData = result.FirstOrDefault();

            return averageNodeData;
        }
    }
}
