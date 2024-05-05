using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoadStatus;
using RoadStatus.Contracts;

internal static class Startup{
    internal static void ConfigureServices(HostApplicationBuilder builder){
        builder.Environment.ContentRootPath = Directory.GetCurrentDirectory();
        builder.Configuration.AddJsonFile("appSettings.json");
        builder.Services.AddHttpClient<IGetRoadStatus, GetUKRoadStatus>();
    }
}