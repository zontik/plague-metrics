using System;
using System.Net.Http;

namespace PM.AppServer.Tests
{

public interface IHttpClientFactory
{
    HttpClient CreateHttpClient();
}

public class StaticHttpClientFactory : IHttpClientFactory, IDisposable
{
    private readonly HttpMessageHandler _httpMessageHandler;

    public StaticHttpClientFactory(HttpMessageHandler httpMessageHandler = default)
    {
        _httpMessageHandler = httpMessageHandler ?? new HttpClientHandler();
    }

    public void Dispose()
    {
        _httpMessageHandler.Dispose();
    }

    HttpClient IHttpClientFactory.CreateHttpClient()
    {
        return new HttpClient(_httpMessageHandler, false);
    }
}

}