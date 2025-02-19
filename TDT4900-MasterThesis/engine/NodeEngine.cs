using CommunityToolkit.Mvvm.Messaging;
using TDT4900_MasterThesis.message;
using TDT4900_MasterThesis.model.graph;
using TDT4900_MasterThesis.model.simulation;

namespace TDT4900_MasterThesis.engine;

/// <summary>
/// Engine responsible for updating nodes of the graph
/// </summary>
public class NodeEngine : IUpdatable
{
    private Graph? _graph;

    public NodeEngine()
    {
        WeakReferenceMessenger.Default.Register<NewGraphMessage>(
            this,
            (o, m) => ReceiveNewGraphMessage(m)
        );
    }

    public void Update(long currentTick)
    {
        _graph?.Nodes.ForEach(n => n.Update(currentTick));
    }

    private void ReceiveNewGraphMessage(NewGraphMessage message)
    {
        _graph = message.Value;
    }
}
