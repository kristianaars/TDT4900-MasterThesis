using Avalonia;
using Serilog;
using TDT4900_MasterThesis;
using TDT4900_MasterThesis.host;

// Configure logging
using var log = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .CreateLogger();
Log.Logger = log;

bool useGui = true;

var hostBuilder = new ApplicationHostBuilder().AddSimulationHost().UseRandomGraph(10, 20);

if (useGui)
{
    hostBuilder.UseGui();

    App.Host = hostBuilder.Build();

    var appBuilder = AppBuilder.Configure<App>().UsePlatformDetect();
    appBuilder.StartWithClassicDesktopLifetime([]);
}
else
{
    App.Host = hostBuilder.Build();

    await App.Host.StartAsync();
}
