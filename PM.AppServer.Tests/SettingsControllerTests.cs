using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using PM.AppServer.Models;
using Xunit;

namespace PM.AppServer.Tests
{

public class SettingsControllerTests
{
    private readonly HttpClient _httpClient;

    public SettingsControllerTests()
    {
        var application = new WebApplicationFactory<Program>();

        var httpClientFactory = new TestsHttpClientFactory(application.Server.CreateHandler());
        _httpClient = httpClientFactory.CreateHttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost/api/");
    }

    [Fact]
    public async Task Get_ReturnsSettings()
    {
        //Arrange
        //Arrange

        //Act
        var response = await _httpClient.GetAsync("settings");
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