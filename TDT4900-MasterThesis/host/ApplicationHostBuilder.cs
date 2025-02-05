using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using TDT4900_MasterThesis.engine;
using TDT4900_MasterThesis.factory;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;
using TDT4900_MasterThesis.view;
using TDT4900_MasterThesis.view.plot;

namespace TDT4900_MasterThesis.host;

public class ApplicationHostBuilder
{
    private IServiceCollection _services => _baseBuilder.Services;

    private HostApplicationBuilder _baseBuilder { get; } = Host.CreateApplicationBuilder();

    public ApplicationHostBuilder()
    {
        _baseBuilder.Logging.ClearProviders();
        _baseBuilder.Logging.AddSerilog();
        _baseBuilder.Logging.SetMinimumLevel(LogLevel.Debug);

        // Configure services
        _services.AddSingleton<AppSettings>();
    }

    public ApplicationHostBuilder AddSimulationHost()
    {
        _services.AddSingleton<MessageWaveEngine>();
        _services.AddSingleton<NodeMessageEngine>();
        _services.AddSingleton<SimulationEngine>();

        _services.AddTransient<IUpdatable>(p => p.GetRequiredService<MessageWaveEngine>());
        _services.AddTransient<IUpdatable>(p => p.GetRequiredService<NodeMessageEngine>());

        _services.AddHostedService<SimulationHost>();
        return this;
    }

    public ApplicationHostBuilder UseGui()
    {
        _services.AddSingleton<MainWindow>();
        _services.AddSingleton<GraphPlotView>();
        _services.AddSingleton<SequencePlotView>();

        _services.AddTransient<IUpdatable>(p => p.GetRequiredService<GraphPlotView>());
        _services.AddTransient<IDrawable>(p => p.GetRequiredService<GraphPlotView>());

        _services.AddTransient<IUpdatable>(p => p.GetRequiredService<SequencePlotView>());
        _services.AddTransient<IDrawable>(p => p.GetRequiredService<SequencePlotView>());

        return this;
    }

    public ApplicationHostBuilder UseRandomGraph(int vertexCount, int edgeCount)
    {
        RandomGraphFactory f = new RandomGraphFactory(vertexCount, edgeCount);
        Graph graph = f.GetGraph();
        graph.ConvertToBidirectional();

        _services.AddSingleton(graph);
        return this;
    }

    public IHost Build()
    {
        return _baseBuilder.Build();
    }
}
