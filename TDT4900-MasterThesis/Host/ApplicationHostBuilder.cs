using System.Reflection;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Factory;
using TDT4900_MasterThesis.Model.Graph;
using TDT4900_MasterThesis.view;
using TDT4900_MasterThesis.view.plot;
using TDT4900_MasterThesis.viewmodel;

namespace TDT4900_MasterThesis.Host;

public class ApplicationHostBuilder
{
    private IServiceCollection _services => _baseBuilder.Services;

    private HostApplicationBuilder _baseBuilder { get; } =
        Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder();

    private AppSettings _appSettings;

    public ApplicationHostBuilder()
    {
        _baseBuilder.Logging.ClearProviders();
        _baseBuilder.Logging.AddSerilog();
        _baseBuilder.Logging.SetMinimumLevel(LogLevel.Debug);

        // Configure services
        _appSettings = new AppSettings();
        _services.AddSingleton(_appSettings);

        // Automapper Configuration
        _services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(Assembly.GetExecutingAssembly());
        });
    }

    public ApplicationHostBuilder AddSimulationHost()
    {
        _services.AddSingleton<NodeMessageEngine>();
        _services.AddSingleton<SimulationEngine>();
        _services.AddSingleton<NodeEngine>();

        _services.AddSingleton<SimulationJobFactory>();

        _services.AddSingleton<SimulationBatchEngine>();
        _services.AddSingleton<SimulationEngine>();

        //_services.AddHostedService<SimulationHost>();
        return this;
    }

    public ApplicationHostBuilder UseGui()
    {
        // View Models
        _services.AddSingleton<GraphPlotViewModel>();
        _services.AddSingleton<SequencePlotViewModel>();
        _services.AddSingleton<MainWindowViewModel>();

        // Views
        _services.AddSingleton<MainWindow>();
        _services.AddSingleton<GraphPlotView>();
        _services.AddSingleton<SequencePlotView>();

        _services.AddTransient<IDrawable>(p => p.GetRequiredService<GraphPlotView>());

        _services.AddTransient<IUpdatable>(p => p.GetRequiredService<SequencePlotView>());
        _services.AddTransient<IDrawable>(p => p.GetRequiredService<SequencePlotView>());

        return this;
    }

    public IHost Build()
    {
        return _baseBuilder.Build();
    }
}
