using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using PM.AppServer.Services.Base;
using PM.Model;
using PM.Model.Data;

namespace PM.AppServer.Services
{

public class PlagueDataService : IPlagueDataService
{
    private readonly AppSettings _appSettings;
    private readonly List<PlagueDataType> _dataTypes;

    private readonly HttpClient _httpClient;

    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _cacheTtl;

    public PlagueDataService(IOptions<AppSettings> appSettings, IOptions<List<PlagueDataType>> dataTypes, IMemoryCache memoryCache)
    {
        _appSettings = appSettings.Value;

        _dataTypes = dataTypes.Value;
        _memoryCache = memoryCache;
        _cacheTtl = TimeSpan.FromMilliseconds(_appSettings.CacheTtlMs);

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_appSettings.DataFetchUrl);
    }

    public async Task<IEnumerable<PlagueData>> ListData(string tokenPath)
    {
        if (!_dataTypes.Exists(dt => dt.TokenPath == tokenPath))
        {
            throw new ArgumentOutOfRangeException(nameof(tokenPath), "Wrong tokenPath.");
        }

        if (_memoryCache.TryGetValue(tokenPath, out IEnumerable<PlagueData> cache))
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
            _memoryCache.Set(type.TokenPath, dataList, _cacheTtl);

            if (type.TokenPath == tokenPath)
            {
                returnList = dataList;
            }
        }

        return returnList;
    }

    private static List<PlagueData> ListPlagueData(JEnumerable<JToken> jTokens, string tokenPath)
    {
        return jTokens.Select(jToken =>
            {
                var stateId = jToken.Value<string>("state");
                var level = (int)jToken.SelectToken(tokenPath, true);
                return new PlagueData(stateId, level);
            }
        ).ToList();
    }
}

}