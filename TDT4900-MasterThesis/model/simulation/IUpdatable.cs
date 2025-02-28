namespace TDT4900_MasterThesis.model.simulation;

public interface IUpdatable
{
    public void Update(long currentTick);
    public void ResetComponent();
}
