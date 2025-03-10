using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;
using EventHandler = TDT4900_MasterThesis.Handler.EventHandler;

namespace TDT4900_MasterThesis.Algorithm;

public abstract class BaseAlgorithm : BaseEventProducer, IAlgorithm
{
    public List<NodeEvent>? EventHistory { get; set; } = new();

    public void ConsumeEvent(NodeEvent nodeEvent)
    {
        EventHistory!.Add(nodeEvent);
    }

    public bool IsFinished { get; protected set; }

    public abstract void Initialize();
    public abstract void Update(long currentTick);
    public abstract void ResetComponent();
}
