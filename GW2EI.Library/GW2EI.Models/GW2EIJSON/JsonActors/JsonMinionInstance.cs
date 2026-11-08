namespace GW2EIJSON;

/// <summary>
/// Class corresponding to a specific instance of minion in <see cref="JsonMinions"/>
/// </summary>
public class JsonMinionInstance
{
    /// <summary>
    /// Time at which actor started being tracked. \n
    /// For players, this and <see cref="JsonMinionInstance.LastAware"/> should be used to determine in which phases given player was active.
    /// </summary>
    public int FirstAware;
    /// <summary>
    /// Time at which target ended being tracked. \n
    /// For players, this and <see cref="JsonMinionInstance.FirstAware"/> should be used to determine in which phases given player was active.
    /// </summary>
    public int LastAware;
    /// <summary>
    /// Total health of the actor. -1 if information is missing (ex: players)
    /// </summary>
    public int TotalHealth;
    /// <summary>
    /// ID of the actor in the instance
    /// </summary>
    public ushort InstanceID;
    /// <summary>
    /// Unique EI ID of the actor
    /// </summary>
    public int UniqueID;

    /// <summary>
    /// Rotation data
    /// </summary>
    /// <seealso cref="JsonRotation"/>
    public IReadOnlyList<JsonRotation>? Rotation;

    /// <summary>
    /// Array of int[2] that represents the number of conditions \n
    /// Array[i][0] will be the time, Array[i][1] will be the number of conditions present from Array[i][0] to Array[i+1][0] \n
    /// If i corresponds to the last element that means the status did not change for the remainder of the log \n
    /// </summary>
    public IReadOnlyList<IReadOnlyList<long>>? ConditionsStates;

    /// <summary>
    /// Array of int[2] that represents the number of boons \n
    /// Array[i][0] will be the time, Array[i][1] will be the number of boons present from Array[i][0] to Array[i+1][0] \n
    /// If i corresponds to the last element that means the status did not change for the remainder of the log
    /// </summary>
    public IReadOnlyList<IReadOnlyList<long>>? BoonsStates;

    /// <summary>
    /// Array of double[2] that represents the health status of the actor \n
    /// Array[i][0] will be the time, Array[i][1] will be health % \n
    /// If i corresponds to the last element that means the health did not change for the remainder of the log \n
    /// </summary>
    public IReadOnlyList<IReadOnlyList<double>>? HealthPercents;
    /// <summary>
    /// Array of double[2] that represents the barrier status of the actor \n
    /// Array[i][0] will be the time, Array[i][1] will be barrier % \n
    /// If i corresponds to the last element that means the health did not change for the remainder of the log \n
    /// </summary>
    public IReadOnlyList<IReadOnlyList<double>>? BarrierPercents;

    /// <summary>
    /// List of buff status
    /// </summary>
    /// <seealso cref="JsonBuffsUptime"/>
    public IReadOnlyList<JsonBuffsUptime>? Buffs;

    /// <summary>
    /// List of buff status on active time \n
    /// Only relevant for player minions
    /// </summary>
    /// <seealso cref="JsonBuffsUptime"/>
    public IReadOnlyList<JsonBuffsUptime>? BuffsActive;

    /// <summary>
    /// List of volumes status
    /// </summary>
    /// <seealso cref="JsonBuffVolumes"/>
    public IReadOnlyList<JsonBuffVolumes>? BuffVolumes;
    /// <summary>
    /// List of buff volumes on active time\n
    /// Only relevant for player minions
    /// </summary>
    /// <seealso cref="JsonBuffVolumes"/>
    public IReadOnlyList<JsonBuffVolumes>? BuffVolumesActive;

    /// <summary>
    /// List of time during which the minion was active (not dead and not dc) \n
    /// Only relevant for player minions \n
    /// Length == number of phases
    /// </summary>
    public IReadOnlyList<long>? ActiveTimes;
}
