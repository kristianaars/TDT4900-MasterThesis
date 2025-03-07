using SkiaSharp;

namespace TDT4900_MasterThesis.model.simulation;

public interface IDrawable
{
    public void Draw();
    public bool IsReadyToDraw { get; }
}
