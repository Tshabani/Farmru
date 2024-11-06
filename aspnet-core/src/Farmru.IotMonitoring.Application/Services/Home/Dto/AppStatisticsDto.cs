using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Home.Dto
{
    public class AppStatisticsDto
    {
        public int TotalNumberOfUsers { get; set; }
        public int TotalNumberOfRoles { get; set; }
        public int TotalNumberOfNodes { get; set; }
        public int TotalNumberOfActiveNodes { get; set; }
    }
}
