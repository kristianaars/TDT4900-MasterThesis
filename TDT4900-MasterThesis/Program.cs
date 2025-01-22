// See https://aka.ms/new-console-template for more information

using TDT4900_MasterThesis.model.graph;

Console.WriteLine("Hello, World!");

var n = new Node[]
{
    new Node(1),
    new Node(2),
    new Node(3)
};

var edges = new Edge[]
{
    new Edge(n[0], n[1], 1),
    new Edge(n[1], n[2], 1),
    new Edge(n[2], n[0], 1)
};

var g = new Graph(n, edges);