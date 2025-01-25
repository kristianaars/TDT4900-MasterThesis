// See https://aka.ms/new-console-template for more information

using Serilog;
using Serilog.Enrichers.CallerInfo;
using Serilog.Formatting.Display;
using TDT4900_MasterThesis.engine;
using TDT4900_MasterThesis.model;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.simulation;

using var log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
Log.Logger = log;

var n = new Node[] { new(0), new(1), new(2), new(3), new(4) };

var edges = new Edge[]
{
    new(n[0], n[4], 30),
    new(n[1], n[0], 20),
    new(n[1], n[3], 40),
    new(n[2], n[1], 10),
    new(n[3], n[1], 25),
};

var g = new Graph(n, edges);

var nodeMessageEngine = new NodeMessageEngine(graph: g);
nodeMessageEngine.SendMessage(new Message(0, n[1], n[1]));

var o = g.GetOutEdges(n[1]);

var simLoop = new SimulationEngine()
{
    UpdatableComponents = [nodeMessageEngine],
    DrawableComponents = [],
};

simLoop.RunSimulation();
