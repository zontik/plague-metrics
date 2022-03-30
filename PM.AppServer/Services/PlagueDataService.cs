using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PM.AppServer.Models;
using PM.AppServer.Models.Data;

namespace PM.AppServer.Services
{

public interface IPlagueDataService
{
    Task<IEnumerable<PlagueData>> List(string tokenPath);
}

public class PlagueDataService : IPlagueDataService
{
    private readonly AppSettings _appSettings;
    private readonly List<PlagueDataType> _dataTypes;

    private readonly HttpClient _httpClient;
    private readonly SimpleCache<IEnumerable<PlagueData>> _dataCache;

    public PlagueDataService(IOptions<AppSettings> appSettings, IOptions<List<PlagueDataType>> dataTypes)
    {
        _appSettings = appSettings.Value;
        _dataTypes = dataTypes.Value;

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_appSettings.DataFetchUrl);

        _dataCache = new SimpleCache<IEnumerable<PlagueData>>(_appSettings.CacheTtlMs);
    }

    public async Task<IEnumerable<PlagueData>> List(string tokenPath)
    {
        if (!_dataTypes.Exists(dt => dt.TokenPath == tokenPath))
        {
            throw new ArgumentOutOfRangeException(nameof(tokenPath), "Wrong tokenPath.");
        }

        if (_dataCache.TryGetValue(tokenPath, out var cache))
        {
            return cache;
        }

        return await FetchPlagueData(tokenPath);
    }

    private async Task<IEnumerable<PlagueData>> FetchPlagueData(string tokenPath)
    {
        var response = await _httpClient.GetAsync($"states.json?apiKey={_appSettings.DataApiKey}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var jTokens = JArray.Parse(content).Children();

        var returnList = Enumerable.Empty<PlagueData>();
        foreach (var type in _dataTypes)
        {
            var dataList = ListPlagueData(jTokens, type.TokenPath);
            _dataCache.Put(type.TokenPath, dataList);

            if (type.TokenPath == tokenPath)
            {
                returnList = dataList;
            }
        }

        return returnList;
    }

    private static List<PlagueData> ListPlagueData(JEnumerable<JToken> jTokens, string tokenPath)
    {
        return jTokens.Select(jToken => new PlagueData(jToken, tokenPath)).ToList();
    }
}

}