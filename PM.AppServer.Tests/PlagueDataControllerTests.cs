using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PM.AppServer.Tests.Base;
using PM.Model.Data;
using Xunit;

namespace PM.AppServer.Tests
{

public class PlagueDataControllerTests : ControllerTestsBase
{
    [Fact]
    public async Task ListDataTypes_ReturnsList()
    {
        //Arrange
        //Arrange

        //Act
        var response = await HttpClient.GetAsync("plague_data/types");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var list = JsonConvert.DeserializeObject<IEnumerable<PlagueDataType>>(content).ToList();
        //Act

        //Assert
        Assert.NotNull(list);
        Assert.NotEmpty(list);
        Assert.Equal(4, list.Count);
        //Assert
    }

    [Fact]
    public async Task ListData_ReturnsList()
    {
        //Arrange
        //Arrange

        //Act
        var response = await HttpClient.GetAsync("plague_data?tokenPath=riskLevels.overall");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var list = JsonConvert.DeserializeObject<IEnumerable<PlagueData>>(content);
        //Act

        //Assert
        Assert.NotNull(list);
        Assert.NotEmpty(list);
        //Assert
    }

    [Fact]
    public async Task ListData_Throws_IfTokenIsInvalid()
    {
        //Arrange
        //Arrange

        //Act
        var response = await HttpClient.GetAsync("plague_data?tokenPath=invalidToken");
        //Act

        //Assert
        Assert.ThrowsAny<Exception>(() => response.EnsureSuccessStatusCode());
        //Assert
    }
}

}