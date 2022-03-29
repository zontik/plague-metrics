using System;

namespace PM.AppServer.Models
{

public class AppSettings
{
    public string DataFetchUrl { get; set; } = string.Empty;

    public TimeSpan CacheTtl { get; set; } = TimeSpan.FromSeconds(30);
}

}