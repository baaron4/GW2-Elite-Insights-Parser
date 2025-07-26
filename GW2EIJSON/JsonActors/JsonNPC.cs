namespace GW2EIJSON;

/// <summary>
/// Class representing an NPC
/// </summary>
public class JsonNPC : JsonActor
{
    /// <summary>
    /// Game ID of the target
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
    /// If i corresponds to the last element that means the breakbar did not change for the remainder of the fight \n
    /// </summary>
    public IReadOnlyList<IReadOnlyList<double>>? BreakbarPercents;
}
