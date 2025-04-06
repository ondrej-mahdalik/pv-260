using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PV260.Client.BL;
using PV260.Client.ConsoleApp;
using PV260.Client.ConsoleApp.Components;
using PV260.Client.ConsoleApp.Components.Interfaces;
using PV260.Client.Mock;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) => { config.AddJsonFile("appsettings.json", true, true); })
    .ConfigureLogging(logging => logging.ClearProviders())
    .ConfigureServices((context, services) =>
    {
        var baseAddress = context.Configuration["ApiSettings:BaseAddress"];
        if (string.IsNullOrEmpty(baseAddress))
        {
            throw new ArgumentNullException(nameof(baseAddress), "Base address cannot be null or empty.");
        }

        services.AddHttpClient<IApiClient, ApiClientMock>(client => { client.BaseAddress = new Uri(baseAddress); });

        services.AddSingleton<IHeaderComponent, HeaderComponent>();
        services.AddSingleton<INavbarComponent, NavbarComponent>();
        services.AddSingleton<IFooterComponent, FooterComponent>();
        services.AddSingleton<IContentRouter, DefaultContentRouter>();
        services.AddSingleton<ILayoutBuilder, LayoutBuilder>();
        services.AddSingleton<ConsoleApplication>();
    })
    .Build();

var app = host.Services.GetRequiredService<ConsoleApplication>();

_ = host.RunAsync();

app.Run();