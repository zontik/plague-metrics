using Newtonsoft.Json;

namespace PM.AppServer.Models
{

public class AppSettings
{
    [JsonIgnore]
    public string DataFetchUrl { get; set; }

    [JsonIgnore] 
    public string DataApiKey { get; set; }

    public long CacheTtlMs { get; set; }
}

}