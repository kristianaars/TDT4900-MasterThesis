// See https://aka.ms/new-console-template for more information

using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.simulation;


var n = new Node[]
{
    new Node(0),
    new Node(1),
    new Node(2),
    new Node(3),
    new Node(4)
};

var edges = new Edge[]
{
    new Edge(n[0], n[4], 3),
    new Edge(n[1], n[0], 2),
    new Edge(n[1], n[3], 4),
    new Edge(n[2], n[1], 0),
    new Edge(n[3], n[1], 1),

};

var g = new Graph(n, edges);

var o = g.GetOutEdges(n[1]);

var simLoop = new SimulationLoop();
simLoop.RunSimulation();

Console.WriteLine("Hello, World!");