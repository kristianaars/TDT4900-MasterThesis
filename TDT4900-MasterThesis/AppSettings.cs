namespace TDT4900_MasterThesis;

public class AppSettings
{
    public readonly string WindowTitle = "TDT4900 Master Thesis - Simulation";
    public readonly int DefaultWindowWidth = 1000;
    public readonly int DefaultWindowHeight = 1000;

    public readonly SimulationSettings Simulation = new();

    public class SimulationSettings
    {
        /// <summary>
        /// Target Frames per second for simulation
        /// </summary>
        public readonly int TargetFps = 60;

        /// <summary>
        /// Target ticks per second for simulation
        /// </summary>
        public readonly int TargetTps = 150;

        /// <summary>
        /// Target cooldown for nodes after activation (in ticks)
        /// </summary>
        public readonly int NodeCooldownPeriod = 100;

        /// <summary>
        /// The interval in which a new wave of messages is sent (in ticks)
        /// </summary>
        public readonly int WaveInterval = 250;

        /// <summary>
        /// The time it takes for an excitatory message to reach a node  (in ticks)
        /// </summary>
        public readonly int DeltaTExcitatory = 25;

        /// <summary>
        /// The time it takes for an inhibitory message to reach a node (in ticks)
        /// </summary>
        public readonly int DeltaTInhibitory = 10;

        /// <summary>
        /// Forward latency for tagged nodes (in ticks)
        /// </summary>
        public readonly int TauPlus = 5;

        /// <summary>
        /// Forward latency for non-tagged nodes (in ticks)
        /// </summary>
        public readonly int TauZero = 10;
    }
}
