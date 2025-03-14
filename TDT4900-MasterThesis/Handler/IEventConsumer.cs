using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Handler;

public interface IEventConsumer
{
    public void ConsumeEvent(NodeEvent nodeEvent);
}
