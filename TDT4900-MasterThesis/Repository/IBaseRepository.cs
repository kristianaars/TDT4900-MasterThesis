using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Repository;

public interface IBaseRepository<T>
    where T : BaseModel
{
    IEnumerable<T> List();

    T GetById(Guid id);

    void Insert(T e);

    void Delete(T id);

    void UpdateStudent(T e);

    void Save();
}
