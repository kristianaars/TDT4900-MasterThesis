using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Repository;

public interface IBaseRepository<T>
    where T : BaseModel { }
