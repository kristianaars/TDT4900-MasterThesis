namespace TDT4900_MasterThesis.Model.Db;

public class AlphaAlgorithmSpec : AlgorithmSpec
{
    public int DeltaTExcitatory { get; set; }
    public int DeltaTInhibitory { get; set; }
    public int RefractoryPeriod { get; set; }
    public int TauPlus { get; set; }
    public int TauZero { get; set; }
}
