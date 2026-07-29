using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Farmru.IotMonitoring.Services.Nutrients.Dto;
using System.Threading.Tasks;

namespace Farmru.IotMonitoring.Services.Nutrients
{
    public interface IFertilizerAppService : IApplicationService
    {
        Task<FertilizerProductDto> CreateProduct(CreateFertilizerProductInput input);
        Task<PagedResultDto<FertilizerProductDto>> GetProducts(PagedResultRequestDto input);
        Task<FertilizerApplicationDto> RecordApplication(RecordFertilizerApplicationInput input);
        Task<PagedResultDto<FertilizerApplicationDto>> GetApplicationsByField(GetApplicationsByFieldInput input);
    }
}
