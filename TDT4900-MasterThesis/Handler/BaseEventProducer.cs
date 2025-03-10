using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Handler;

public class BaseEventProducer : IEventProducer
{
    public EventHandler? EventHandler { get; set; }

    public void PostEvent(NodeEvent nodeEvent)
    {
        EventHandler?.PostEvent(nodeEvent);
    }
}
