using TDT4900_MasterThesis.Algorithm;

namespace TDT4900_MasterThesis.Model.Db;

public class StratiumAlgorithmSpec : AlgorithmSpec
{
    public StratiumAlgorithmSpec()
    {
        AlgorithmType = AlgorithmType.Stratium;
    }

    public int DeltaTExcitatory { get; set; }
    public int DeltaTInhibitory { get; set; }
    public int RefractoryPeriod { get; set; }
    public int TauPlus { get; set; }
    public int TauZero { get; set; }
}
