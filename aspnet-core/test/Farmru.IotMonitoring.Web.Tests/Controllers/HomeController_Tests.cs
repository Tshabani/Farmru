using System.Threading.Tasks;
using Farmru.IotMonitoring.Models.TokenAuth;
using Farmru.IotMonitoring.Web.Controllers;
using Shouldly;
using Xunit;

namespace Farmru.IotMonitoring.Web.Tests.Controllers
{
    public class HomeController_Tests: IotMonitoringWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}