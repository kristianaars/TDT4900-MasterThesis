using TDT4900_MasterThesis.Handler;
using TDT4900_MasterThesis.Model.Db;
using TDT4900_MasterThesis.Model.Graph;

namespace TDT4900_MasterThesis.Algorithm;

public interface IAlgorithm : IUpdatable, IEventProducer
{
    public bool IsFinished { get; }
    public void Initialize();
}
