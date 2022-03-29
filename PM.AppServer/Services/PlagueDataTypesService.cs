using System.Collections.Generic;
using PM.AppServer.Models.Data;

namespace PM.AppServer.Services
{

public interface IPlagueDataTypesService
{
    IEnumerable<PlagueDataType> List();
}

public class PlagueDataTypesService : IPlagueDataTypesService
{
    private readonly IEnumerable<PlagueDataType> _dataTypes = new List<PlagueDataType>
    {
        new() { Key = "overall", Name = "Overall Risk Level" },
        new() { Key = "cases", Name = "Cases per 100k level" },
        new() { Key = "test", Name = "Test positivity ration level" },
        new() { Key = "infection", Name = "Infection rate level" }
    };

    public IEnumerable<PlagueDataType> List() => _dataTypes;
}

}