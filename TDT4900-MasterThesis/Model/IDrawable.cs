namespace TDT4900_MasterThesis.Model.Graph;

public interface IDrawable
{
    public void Draw();
    public bool IsReadyToDraw { get; }
}
