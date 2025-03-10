using System.Text.Json;
using TDT4900_MasterThesis.Model.Graph;

namespace TDT4900_MasterThesis.service;

public class GraphSerializerService
{
    public string SerializeGraph(SimulationGraph simulationGraph)
    {
        var graphDto = new GraphDto()
        {
            Nodes = simulationGraph.Nodes.ToArray(),
            Edges = simulationGraph
                .Edges.Select(e => new EdgeDto
                {
                    Target = e.Target.Id,
                    Source = e.Source.Id,
                    Weight = e.Weight,
                })
                .ToArray(),
        };

        return JsonSerializer.Serialize(graphDto);
    }

    public SimulationGraph DeserializeGraph(string json)
    {
        var graphDto = JsonSerializer.Deserialize<GraphDto>(json);
        var nodes = graphDto!.Nodes;
        var edges = graphDto
            .Edges.Select(e => new SimulationEdge(nodes[e.Source], nodes[e.Target], e.Weight))
            .ToArray();
        return new SimulationGraph(nodes, edges);
    }

    class GraphDto
    {
        public SimulationNode[] Nodes { get; set; }
        public EdgeDto[] Edges { get; set; }
    }

    class EdgeDto
    {
        public int Source { get; set; }
        public int Target { get; set; }
        public int Weight { get; set; }
    }
}
