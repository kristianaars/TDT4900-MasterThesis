namespace TDT4900_MasterThesis.model;

public interface IMessage
{
    public long SentAt { get; }
    public long ReceiveAt { get; }
}
