using TDT4900_MasterThesis.model.graph;

namespace TDT4900_MasterThesis.model;

public class Message(long executeAt, Node sender, Node receiver)
{
    public long ExecuteAt { get; } = executeAt;
    public Node Sender { get; } = sender;
    public Node Receiver { get; } = receiver;

    public override string ToString()
    {
        return $"{nameof(ExecuteAt)}: {ExecuteAt}, {nameof(Sender)}: {Sender}, {nameof(Receiver)}: {Receiver}";
    }
}
