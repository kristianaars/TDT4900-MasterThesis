using System.ComponentModel.DataAnnotations.Schema;

namespace TDT4900_MasterThesis.Model.Db;

public class SimulationBatch : BaseModel
{
    public List<Simulation> Simulations { get; set; }

    [NotMapped]
    public bool PersistSimulations { get; set; }
}
