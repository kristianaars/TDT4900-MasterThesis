using System.ComponentModel.DataAnnotations;

namespace TDT4900_MasterThesis.Model.Db;

public class BaseModel
{
    [Key]
    public Guid Id { get; set; }
}
