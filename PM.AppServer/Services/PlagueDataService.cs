using System.Collections.Generic;
using Microsoft.Extensions.Options;
using PM.AppServer.Models;
using PM.AppServer.Models.Data;

namespace PM.AppServer.Services
{

public interface IPlagueDataService
{
    IEnumerable<PlagueData> List(string typeKey);
}

public class PlagueDataService : IPlagueDataService
{
    private readonly AppSettings _appSettings;

    private readonly SimpleCache<string, IEnumerable<PlagueData>> _dataCache;

    public PlagueDataService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _dataCache = new SimpleCache<string, IEnumerable<PlagueData>>(_appSettings.CacheTtl);
    }

    public IEnumerable<PlagueData> List(string typeKey)
    {
        if (_dataCache.TryGetValue(typeKey, out var data))
        {
            return data;
        }

        return new List<PlagueData>();
    }
}

}