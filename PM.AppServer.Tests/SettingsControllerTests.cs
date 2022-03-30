using System.Threading.Tasks;
using Newtonsoft.Json;
using PM.AppServer.Models;
using Xunit;

namespace PM.AppServer.Tests
{

public class SettingsControllerTests : ControllerTestsBase
{
    [Fact]
    public async Task Get_ReturnsSettings_WithoutApiKeyAndFetchUrl()
    {
        //Arrange
        //Arrange

        //Act
        var response = await HttpClient.GetAsync("settings");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var settings = JsonConvert.DeserializeObject<AppSettings>(content);
        //Act

        //Assert
        Assert.NotNull(settings);
        Assert.True(settings.CacheTtlMs > 0);
        Assert.True(string.IsNullOrEmpty(settings.DataApiKey));
        Assert.True(string.IsNullOrEmpty(settings.DataFetchUrl));
        //Assert
    }
}

}