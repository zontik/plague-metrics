using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using PM.AppServer.Models.Data;
using Xunit;

namespace PM.AppServer.Tests
{

public class DataControllerTests
{
    private readonly HttpClient _httpClient;

    public DataControllerTests()
    {
        var application = new WebApplicationFactory<Program>();

        var httpClientFactory = new TestsHttpClientFactory(application.Server.CreateHandler());
        _httpClient = httpClientFactory.CreateHttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(3);
        _httpClient.BaseAddress = new Uri("http://localhost/api/");
    }

    [Fact]
    public async Task ListDataTypes_ReturnsList()
    {
        //Arrange
        //Arrange

        //Act
        var response = await _httpClient.GetAsync("data/types");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<IEnumerable<PlagueDataType>>(content);
        //Act

        //Assert
        Assert.NotNull(list);
        Assert.NotEmpty(list);
        Assert.Equal(4, list.Count());
        //Assert
    }

    [Fact]
    public async Task ListData_ReturnsList()
    {
        //Arrange
        //Arrange

        //Act
        var response = await _httpClient.GetAsync("data");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var list = JsonSerializer.Deserialize<IEnumerable<PlagueData>>(content);
        //Act

        //Assert
        Assert.NotNull(list);
        Assert.Empty(list);
        //Assert
    }
}

}