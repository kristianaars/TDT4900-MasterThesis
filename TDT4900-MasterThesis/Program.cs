// See https://aka.ms/new-console-template for more information

using System.Net.Mime;
using Gtk;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TDT4900_MasterThesis;
using TDT4900_MasterThesis.engine;
using TDT4900_MasterThesis.factory;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.simulation;
using TDT4900_MasterThesis.view;
using Node = TDT4900_MasterThesis.model.graph.Node;

using var log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
Log.Logger = log;

var m = 5;
var n = new Node[]
{
    new(0) { X = 20 * m, Y = 10 * m },
    new(1) { X = 30 * m, Y = 40 * m },
    new(2) { X = 60 * m, Y = 20 * m },
    new(3) { X = 60 * m, Y = 50 * m },
    new(4) { X = 10 * m, Y = 50 * m },
};

var edges = new Edge[]
{
    new(n[0], n[4], 30),
    new(n[1], n[0], 20),
    new(n[1], n[3], 40),
    new(n[2], n[1], 10),
    new(n[3], n[1], 25),
};

RandomGraphFactory factory = new RandomGraphFactory(100, 200);
var g = factory.GetGraph();
g.ConvertToBidirectional();

//var g = new Graph(n, edges);

Application.Init();
ServiceCollection serviceCollection = new ServiceCollection();

serviceCollection.AddSingleton(g);
serviceCollection.AddSingleton<NodeMessageEngine>();
serviceCollection.AddSingleton<SimulationEngine>();
serviceCollection.AddSingleton<MessageWaveEngine>();
serviceCollection.AddSingleton<AppSettings>();

serviceCollection.AddSingleton<MainCanvas>();
serviceCollection.AddSingleton<GraphView>();

serviceCollection.AddSingleton<MainWindow>();

var services = serviceCollection.BuildServiceProvider();

var nodeMessageEngine = services.GetRequiredService<NodeMessageEngine>();
var simEngine = services.GetRequiredService<SimulationEngine>();
var messageWaveEngine = services.GetRequiredService<MessageWaveEngine>();
var graphView = services.GetRequiredService<GraphView>();
var window = services.GetRequiredService<MainWindow>();

simEngine.UpdatableComponents.AddRange([nodeMessageEngine, messageWaveEngine, graphView]);
simEngine.DrawableComponents.AddRange([graphView]);

Task.Run(() =>
{
    simEngine.RunSimulation();
});

window.ShowAll();
Application.Run();
