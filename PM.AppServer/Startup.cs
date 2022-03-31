using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PM.AppServer.Services;
using PM.AppServer.Services.Base;
using PM.Model;
using PM.Model.Data;

namespace PM.AppServer
{

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
        services.Configure<List<PlagueDataType>>(Configuration.GetSection(nameof(PlagueDataType)));

        services.AddSingleton<IPlagueDataService, PlagueDataService>();

        services.AddControllers().AddNewtonsoftJson();

        services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseDeveloperExceptionPage();

        app.UseStaticFiles();
        if (!env.IsDevelopment())
        {
            app.UseSpaStaticFiles();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapControllerRoute("default", "{controller}/{action=Index}/{id?}"); });

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "ClientApp";

            if (env.IsDevelopment())
            {
                spa.UseAngularCliServer("start");
            }
        });
    }
}

}