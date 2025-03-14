using TDT4900_MasterThesis.Algorithm.Dijkstras.Component;
using TDT4900_MasterThesis.Model.Graph;

namespace TDT4900_MasterThesis.Algorithm.Dijkstras.Engine;

public class DijkstrasBacktrackEngine : IUpdatable
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
        _toBeTagged = _toBeTagged.Prior;
    }

    public void Initialize()
    {
        _toBeTagged = TargetNode;
    }
}
