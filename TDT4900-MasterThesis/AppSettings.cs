namespace TDT4900_MasterThesis;

public class AppSettings
{
    public readonly string WindowTitle = "TDT4900 Master Thesis - Simulation";
    public readonly int DefaultWindowWidth = 1450;
    public readonly int DefaultWindowHeight = 800;

    /// <summary>
    /// Name/path of the local database file
    /// </summary>
    public readonly string DbPath = "./data.sqlite";

    public readonly SimulationSettings Simulation = new();

    public class SimulationSettings
    {
        public readonly int GraphNodeCount = 20;
        public readonly int GraphEdgeCount = 25;

        /// <summary>
        /// Target Frames per second for simulation
        /// </summary>
        public readonly int TargetFps = 60;

        /// <summary>
        /// Target ticks per second for simulation
        /// </summary>
        public readonly int TargetTps = 40;

        /// <summary>
        /// Target cooldown for nodes after activation (in ticks)
        /// </summary>
        public readonly int RefractoryPeriod = 22;

        /// <summary>
        /// The time it takes for an excitatory message to reach a node  (in ticks)
        /// </summary>
        public readonly int DeltaTExcitatory = 7;

        /// <summary>
        /// The time it takes for an inhibitory message to reach a node (in ticks)
        /// </summary>
        public readonly int DeltaTInhibitory = 5;

        /// <summary>
        /// Forward latency for tagged nodes (in ticks)
        /// </summary>
        public readonly int TauPlus = 2;

        /// <summary>
        /// Forward latency for non-tagged nodes (in ticks)
        /// </summary>
        public readonly int TauZero = 4;
    }
}
