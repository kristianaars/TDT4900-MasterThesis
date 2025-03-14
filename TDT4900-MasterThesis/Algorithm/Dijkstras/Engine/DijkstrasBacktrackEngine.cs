using TDT4900_MasterThesis.Algorithm.Dijkstras.Component;
using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;

namespace TDT4900_MasterThesis.Algorithm.Dijkstras.Engine;

public class DijkstrasBacktrackEngine : BaseEventProducer, IUpdatable
{
    public bool IsFinished { get; set; }
    public required DijkstraNode TargetNode { get; set; }

    private DijkstraNode? _toBeTagged;

    public void Update(long currentTick)
    {
        IsFinished = _toBeTagged == null;
        if (IsFinished)
            return;

        _toBeTagged!.IsTagged = true;

        PostEvent(
            new NodeEvent()
            {
                EventType = EventType.Tagged,
                NodeId = _toBeTagged!.NodeId,
                Tick = currentTick,
            }
        );

        _toBeTagged = _toBeTagged.Prior;
    }

    public void Initialize()
    {
        _toBeTagged = TargetNode;
    }
}
