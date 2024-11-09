using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Farmru.IotMonitoring.Domains.Organisations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Organisations.Dto
{
    /// <summary>
    /// 
    /// </summary>
    [AutoMap(typeof(Organisation))]
    public class OrganisationDto : EntityDto<Guid>
    {
        public string Name { get; set; }
        public string ShortAlias { get; set; }
        public string Description { get; set; }
        public string FreeTextAddress { get; set; }
        public int? OrganisationType { get; set; }
        public string CompanyRegistrationNo { get; set; }
        public string VatRegistrationNo { get; set; }
        public string ContactEmail { get; set; }
        public string ContactMobileNo { get; set; }
        public int TenantId { get; set; }
    }
}
