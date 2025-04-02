using System.ComponentModel.DataAnnotations;

namespace TDT4900_MasterThesis.Model.Db;

public class AlgorithmEvent : BaseModel
{
    public long Tick { get; set; }
}
