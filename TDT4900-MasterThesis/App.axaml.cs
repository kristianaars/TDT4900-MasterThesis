using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Factory;
using TDT4900_MasterThesis.Host;
using TDT4900_MasterThesis.Model.Graph;
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
                //Host!.StopAsync();
                Environment.Exit(0);
            };
        }

        // Global UI Exception Handling
        Dispatcher.UIThread.UnhandledException += (sender, e) =>
        {
            Log.Fatal(e.Exception, "Unhandled exception in UI thread");
            e.Handled = true;
        };

        // Handle Ctrl+C (SIGINT) to ensure cleanup
        Console.CancelKeyPress += async (sender, e) =>
        {
            e.Cancel = true; // Prevent immediate shutdown
            //await Host!.StopAsync();
            Environment.Exit(0);
        };

        // Start the application host for background services

        //Task.Run(() => Host.Start());

        base.OnFrameworkInitializationCompleted();
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
