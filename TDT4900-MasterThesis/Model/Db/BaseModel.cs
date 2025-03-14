using System.ComponentModel.DataAnnotations;

namespace TDT4900_MasterThesis.Model.Db;

public class BaseModel
{
    [Key]
    public int Id { get; set; }
}
