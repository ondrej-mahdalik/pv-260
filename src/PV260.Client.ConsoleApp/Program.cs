using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using PV260.Client.BL;
using System;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var baseAddress = context.Configuration["ApiSettings:BaseAddress"];
        if (string.IsNullOrEmpty(baseAddress))
        {
            throw new ArgumentNullException(nameof(baseAddress), "Base address cannot be null or empty.");
        }
        services.AddHttpClient<IApiClient, ApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseAddress);
        });
    })
    .Build();

var apiClient = host.Services.GetRequiredService<IApiClient>();

Console.WriteLine("Hello World!");

await host.RunAsync();
