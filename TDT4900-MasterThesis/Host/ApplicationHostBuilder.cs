using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using TDT4900_MasterThesis.Context;
using TDT4900_MasterThesis.Engine;
using TDT4900_MasterThesis.Factory;
using TDT4900_MasterThesis.Factory.GraphFactory;
using TDT4900_MasterThesis.Factory.SimulationJob;
using TDT4900_MasterThesis.Model.Graph;
using TDT4900_MasterThesis.Repository;
using TDT4900_MasterThesis.Service;
using TDT4900_MasterThesis.View;
using TDT4900_MasterThesis.View.Plot;
using TDT4900_MasterThesis.ViewModel;
using TDT4900_MasterThesis.ViewModel.Component;
using TDT4900_MasterThesis.ViewModel.Configuration;
using AlphaAlgorithmConfigurationViewModel = TDT4900_MasterThesis.ViewModel.Configuration.AlphaAlgorithmConfigurationViewModel;
using RadiusNeighbourGraphConfigurationViewModel = TDT4900_MasterThesis.ViewModel.Configuration.RadiusNeighbourGraphConfigurationViewModel;
using SquareGridHierarchicalGraphConfigurationViewModel = TDT4900_MasterThesis.ViewModel.Configuration.SquareGridHierarchicalGraphConfigurationViewModel;

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
        _services.AddSingleton<SimulationEngine>();

        _services.AddSingleton<SimulationJobFactory>();
        _services.AddSingleton<GraphFactory>();

        _services.AddSingleton<SimulationBatchEngine>();
        _services.AddSingleton<SimulationEngine>();
        _services.AddSingleton<SimulationService>();
        _services.AddSingleton<SimulationBatchService>();

        //_services.AddHostedService<SimulationHost>();
        return this;
    }

    public ApplicationHostBuilder UseDbPersistence()
    {
        _services.AddSingleton<SimulationPersistenceService>();

        _services.AddSingleton<SimulationBatchRepository>();
        _services.AddSingleton<SimulationRepository>();
        _services.AddSingleton<GraphRepository>();

        _services.AddSingleton<SimulationDbContext>();

        return this;
    }

    public ApplicationHostBuilder UseGui()
    {
        // View Models
        _services.AddSingleton<GraphPlotViewModel>();
        _services.AddSingleton<SequencePlotViewModel>();
        _services.AddSingleton<MainWindowViewModel>();
        _services.AddSingleton<SimulationStatsViewModel>();

        // Configuration View Models
        _services.AddSingleton<AlphaAlgorithmConfigurationViewModel>();
        _services.AddSingleton<RadiusNeighbourGraphConfigurationViewModel>();
        _services.AddSingleton<SquareGridHierarchicalGraphConfigurationViewModel>();
        _services.AddSingleton<StratiumAlgorithmConfigurationViewModel>();

        // Views
        _services.AddSingleton<MainWindow>();
        _services.AddSingleton<GraphPlotView>();
        _services.AddSingleton<SequencePlotView>();
        _services.AddSingleton<GraphPlotView>();

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
