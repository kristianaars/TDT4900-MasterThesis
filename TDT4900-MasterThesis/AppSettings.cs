namespace TDT4900_MasterThesis;

public class AppSettings
{
    public string WindowTitle = "TDT4900 Master Thesis - Simulation";
    public int DefaultWindowWidth = 1000;
    public int DefaultWindowHeight = 1000;

    public SimulationSettings Simulation { get; } = new();

    public class SimulationSettings
    {
        /// <summary>
        /// Target Frames per second for simulation
        /// </summary>
        public int TargetFps => 60;

        /// <summary>
        /// Target ticks per second for simulation
        /// </summary>
        public int TargetTps => 500;

        /// <summary>
        /// Target cooldown for nodes after activation (in ticks)
        /// </summary>
        public int NodeCooldownPeriod = 300;

        /// <summary>
        /// The interval in which a new wave of messages is sent (in ticks)
        /// </summary>
        public int WaveInterval = 500;
    }
}
