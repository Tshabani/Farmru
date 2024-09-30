using Abp.Application.Services.Dto;

namespace Farmru.IotMonitoring.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

