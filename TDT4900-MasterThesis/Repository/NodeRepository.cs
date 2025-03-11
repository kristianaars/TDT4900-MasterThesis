using TDT4900_MasterThesis.Context;
using TDT4900_MasterThesis.Model;
using TDT4900_MasterThesis.Model.Db;

namespace TDT4900_MasterThesis.Repository;

public class NodeRepository(SimulationDbContext dbContext) : IBaseRepository<Node>
{
    public IEnumerable<Node> List() => dbContext.Nodes;

    public Node GetById(Guid id) =>
        dbContext.Nodes.Find(id) ?? throw new KeyNotFoundException($"Node with id {id} not found");

    public void Insert(Node node) => dbContext.Nodes.Add(node);

    public void Delete(Node node) => dbContext.Nodes.Remove(node);

    public void UpdateStudent(Node node) => dbContext.Nodes.Update(node);

    public void Save() => dbContext.SaveChanges();
}
