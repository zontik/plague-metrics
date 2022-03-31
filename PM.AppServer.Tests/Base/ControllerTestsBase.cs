using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using PM.AppServer.Tests.Services;

namespace PM.AppServer.Tests.Base
{

public class ControllerTestsBase
{
    protected readonly HttpClient HttpClient;

    protected ControllerTestsBase()
    {
        var application = new WebApplicationFactory<Program>();

        var httpClientFactory = new TestsHttpClientFactory(application.Server.CreateHandler());
        HttpClient = httpClientFactory.CreateHttpClient();
        HttpClient.BaseAddress = new Uri("http://localhost/api/");
    }
}

}