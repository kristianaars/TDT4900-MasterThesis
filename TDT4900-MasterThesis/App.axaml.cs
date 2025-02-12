using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using TDT4900_MasterThesis.engine;
using TDT4900_MasterThesis.factory;
using TDT4900_MasterThesis.host;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.view;
using TDT4900_MasterThesis.view.plot;

namespace TDT4900_MasterThesis;

public class App : Application
{
    public IServiceProvider Services => Host!.Services;
    public static IHost Host { get; set; }

    public override void OnFrameworkInitializationCompleted()
    {
        var mainWindow = Services!.GetRequiredService<MainWindow>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow;

            desktop.Exit += (sender, args) =>
            {
                Host!.StopAsync();
                Host.WaitForShutdown();
            };
        }

        // Start the application host for background services

        Task.Run(async () => await Host.StartAsync());

        base.OnFrameworkInitializationCompleted();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
