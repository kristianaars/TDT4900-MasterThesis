namespace TDT4900_MasterThesis.Model.Graph;

public interface IMessage
{
    public long SentAt { get; }
    public long ReceiveAt { get; }
}
