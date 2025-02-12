using Tmds.DBus.Protocol;

namespace TDT4900_MasterThesis.model;

/// <summary>
/// Class to represent a message that is to be sent at a later time (processing time).
/// </summary>
public class ProcessMessage(long currentTick, long sendAt, NodeMessage message) : IMessage
{
    public long SentAt { get; } = currentTick;
    public long ReceiveAt { get; } = sendAt;
    public NodeMessage SendMessage { get; } = message;

    public override string ToString()
    {
        return $"{nameof(ReceiveAt)}: {ReceiveAt}, {nameof(SendMessage)}: {SendMessage}";
    }
}
