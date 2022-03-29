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
    Task<IEnumerable<PlagueData>> List(string typeKey);
}

public class PlagueDataService : IPlagueDataService
{
    private readonly AppSettings _appSettings;

    private readonly HttpClient _httpClient;
    private readonly SimpleCache<JArray> _dataCache;

    public PlagueDataService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _dataCache = new SimpleCache<JArray>(_appSettings.CacheTtlMs);

        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(3);
        _httpClient.BaseAddress = new Uri(_appSettings.DataFetchUrl);
    }

    public async Task<IEnumerable<PlagueData>> List(string typeKey)
    {
        if (_dataCache.TryGetValue(nameof(PlagueDataService), out var cache))
        {
            return ListPlagueData(cache, typeKey);
        }

        return await FetchPlagueData(typeKey);
    }

    private async Task<IEnumerable<PlagueData>> FetchPlagueData(string typeKey)
    {
        var response = await _httpClient.GetAsync($"states.json?apiKey={_appSettings.DataApiKey}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var jArray = JArray.Parse(content);

        _dataCache.Put(nameof(PlagueDataService), jArray);

        return ListPlagueData(jArray, typeKey);
    }

    private static IEnumerable<PlagueData> ListPlagueData(JToken jArray, string typeKey)
    {
        return jArray.Children().Select(element => PlagueData.CreatePlagueData(typeKey, element)).ToList();
    }
}

}