using System.Text.Json;
using TDT4900_MasterThesis.model.graph;

namespace TDT4900_MasterThesis.service;

public class PersistenceService(GraphSerializerService graphSerializerService)
{
    public void SaveGraph(Graph graph, string path)
    {
        var json = graphSerializerService.SerializeGraph(graph);

        var g = graphSerializerService.DeserializeGraph(json);
    }

    public void LoadGraph(Graph graph) { }
}
