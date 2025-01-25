namespace TDT4900_MasterThesis;

public class AppSettings
{
    /// <summary>
    /// Target Frames per second for simulation
    /// </summary>
    public int TargetFPS { get; } = 60;

    /// <summary>
    /// Target ticks per second for simulation
    /// </summary>
    public int TargetTPS { get; } = 20;

    public string WindowTitle = "TDT4900 Master Thesis";
    public int DefaultWindowWidth = 800;
    public int DefaultWindowHeight = 600;
}
