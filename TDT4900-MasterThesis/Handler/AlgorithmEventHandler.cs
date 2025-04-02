using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Handler;

/// <summary>
/// Handler for Node Events, which provides a blueprint for which compontents that should consume the events.
/// </summary>
public class AlgorithmEventHandler
{
    public required List<IAlgorithmEventConsumer?> Consumers { get; init; }

    public void PostEvent(AlgorithmEvent algEvent) =>
        Consumers.ForEach(c => c?.ConsumeEvent(algEvent));
}
