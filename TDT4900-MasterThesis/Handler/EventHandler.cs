using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Handler;

/// <summary>
/// Handler for Node Events, which provides a blueprint for which compontents that should consume the events.
/// </summary>
public class EventHandler
{
    public required List<IEventConsumer?> Consumers { get; init; }

    public void PostEvent(NodeEvent nodeEvent) =>
        Consumers.ForEach(c => c?.ConsumeEvent(nodeEvent));
}
