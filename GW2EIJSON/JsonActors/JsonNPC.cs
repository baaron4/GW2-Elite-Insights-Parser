namespace GW2EIJSON;

/// <summary>
/// Class representing an NPC
/// </summary>
public class JsonNPC : JsonActor
{
    /// <summary>
    /// Class representing an NPC health bar
    /// </summary>
    public class JsonNPCHealthBar
    {
        /// <summary>
        /// Minimum health percent value allowed on the health bar
        /// </summary>
        public double MinPercent;
        /// <summary>
        /// Maximum health percent value allowed on the health bar
        /// </summary>
        public double MaxPercent;
        /// <summary>
        /// Health value for when the bar is at 100%. \n
        /// <see cref="JsonNPCHealthBar.MaxPercent"/> does not affect this value.
        /// </summary>
        public int Health;
        /// <summary>
        /// Indicates that, at the end of the encounter, this health bar was the active one. \n
        /// <see cref="JsonNPC.HealthPercentBurned"/> and <see cref="JsonNPC.BarrierPercent"/> operates on the active health bar. \n
        /// Health bars before active health bar have been fully consumed, health damage done computed as <see cref="JsonNPCHealthBar.Health"/> * (<see cref="JsonNPCHealthBar.MaxPercent"/> - <see cref="JsonNPCHealthBar.MinPercent"/>) / 100.0. \n
        /// Health bars after active health bar are untouched, remaining health computed as <see cref="JsonNPCHealthBar.Health"/> * (<see cref="JsonNPCHealthBar.MaxPercent"/> - <see cref="JsonNPCHealthBar.MinPercent"/>) / 100.0. \n
        /// Remaining health on active health bar is computed as <see cref="JsonNPCHealthBar.Health"/> * (100.0 - <see cref="JsonNPC.HealthPercentBurned"/> - <see cref="JsonNPCHealthBar.MinPercent"/>) / 100.0. \n
        /// </summary>
        public bool Active;
    }

    /// <summary>
    /// Species ID of the target
    /// </summary>
    public int Id;

    /// <summary>
    /// Final health of the target
    /// </summary>
    public int FinalHealth;

    /// <summary>
    /// Final barrier on the target
    /// </summary>
    public int FinalBarrier;

    /// <summary>
    /// % of barrier remaining on the target
    /// </summary>
    public double BarrierPercent;

    /// <summary>
    /// % of health burned
    /// </summary>
    public double HealthPercentBurned;

    /// <summary>
    /// Healths bars for the NPC. Used when an NPC restore its health or change max health during an encounter. \n
    /// <see cref="JsonNPCHealthBar"/> \n
    /// If missing, the NPC had a single health bar, active, going from 100% to 0%.
    /// </summary>
    public IReadOnlyList<JsonNPCHealthBar>? HealthBars;

    /// <summary>
    /// List of buff status
    /// </summary>
    /// <seealso cref="JsonBuffsUptime"/>
    public IReadOnlyList<JsonBuffsUptime>? Buffs;
    /// <summary>
    /// List of buff volumes
    /// </summary>
    /// <seealso cref="JsonBuffVolumes"/>
    public IReadOnlyList<JsonBuffVolumes>? BuffVolumes;

    /// <summary>
    /// Indicates that the JsonNPC is actually an enemy player
    /// </summary>
    public bool EnemyPlayer;

    /// <summary>
    /// Array of double[2] that represents the breakbar percent of the actor \n
    /// Value[i][0] will be the time, value[i][1] will be breakbar % \n
    /// If i corresponds to the last element that means the breakbar did not change for the remainder of the log \n
    /// </summary>
    public IReadOnlyList<IReadOnlyList<double>>? BreakbarPercents;
}
