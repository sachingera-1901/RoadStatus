using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoadStatus.Contracts;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
Startup.ConfigureServices(builder);

using IHost host = builder.Build();

if(args.Length == 0){
    Console.WriteLine("Road name is not specified");
    return;
}

var getRoadStatus = host.Services.GetRequiredService<IGetRoadStatus>();
var displayText = await getRoadStatus.GetLiveRoadStatus(args[0]);
Console.WriteLine(displayText);
Console.WriteLine("Press Ctrl+c to exit");
if(!getRoadStatus.IsValid)
{
    Environment.ExitCode = 1;
}

await host.RunAsync();
