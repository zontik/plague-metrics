using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
    private readonly ICacheService<IEnumerable<PlagueData>> _cacheService;

    public PlagueDataService(IOptions<AppSettings> appSettings, IOptions<List<PlagueDataType>> dataTypes)
    {
        _appSettings = appSettings.Value;
        _dataTypes = dataTypes.Value;

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_appSettings.DataFetchUrl);

        _cacheService = new SimpleCacheService<IEnumerable<PlagueData>>(_appSettings.CacheTtlMs);
    }

    public async Task<IEnumerable<PlagueData>> ListData(string tokenPath)
    {
        if (!_dataTypes.Exists(dt => dt.TokenPath == tokenPath))
        {
            throw new ArgumentOutOfRangeException(nameof(tokenPath), "Wrong tokenPath.");
        }

        if (_cacheService.TryGetValue(tokenPath, out var cache))
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
            _cacheService.Put(type.TokenPath, dataList);

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