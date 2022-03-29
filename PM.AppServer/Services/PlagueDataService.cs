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
    private readonly SimpleCache<string, IEnumerable<PlagueData>> _dataCache;

    public PlagueDataService(IOptions<AppSettings> appSettings)
    {
        _appSettings = appSettings.Value;
        _dataCache = new SimpleCache<string, IEnumerable<PlagueData>>(_appSettings.CacheTtlMs);

        _httpClient = new HttpClient();
        _httpClient.Timeout = TimeSpan.FromSeconds(3);
        _httpClient.BaseAddress = new Uri(_appSettings.DataFetchUrl);
    }

    public async Task<IEnumerable<PlagueData>> List(string typeKey)
    {
        if (_dataCache.TryGetValue(typeKey, out var data))
        {
            return data;
        }

        var list = await ListPlagueData(typeKey);
        _dataCache.Put(typeKey, list);

        return list;
    }

    private async Task<IEnumerable<PlagueData>> ListPlagueData(string typeKey)
    {
        var response = await _httpClient.GetAsync($"states.json?apiKey={_appSettings.DataApiKey}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var document = JArray.Parse(content);
        var elements = document.Children();
        return elements.Select(element => PlagueData.CreatePlagueData(typeKey, element)).ToList();
    }
}

}