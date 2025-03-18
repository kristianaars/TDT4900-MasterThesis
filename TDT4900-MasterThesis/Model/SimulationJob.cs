using TDT4900_MasterThesis.Algorithm;
using TDT4900_MasterThesis.Algorithm.Component;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Model;

public class SimulationJob
{
    public IAlgorithm Algorithm { get; set; }
    public Simulation Simulation { get; set; }
}
