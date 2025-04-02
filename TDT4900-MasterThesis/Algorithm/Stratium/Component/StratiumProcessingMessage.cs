namespace TDT4900_MasterThesis.Algorithm.Stratium.Component;

/// <summary>
/// Class to represent a message that is to be sent at a later time (processing time).
/// </summary>
public class StratiumProcessingMessage
{
    /// <summary>
    /// When the processing message was initially sent
    /// </summary>
    public required long SentAt { get; init; }

    /// <summary>
    /// When the <see cref="SendMessage"/> message is to be executed
    /// </summary>
    public required long ReceiveAt { get; init; }

    /// <summary>
    /// Message to execute after the given processing time
    /// </summary>
    public required StratiumNodeMessage SendMessage { get; init; }
}
