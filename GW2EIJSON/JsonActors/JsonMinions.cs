namespace GW2EIJSON;

/// <summary>
/// Class corresponding to the regrouping of the same type of minions
/// </summary>
public class JsonMinions
{
    /// <summary>
    /// Name of the minion
    /// </summary>
    public string? Name;

    /// <summary>
    /// Game ID of the minion
    /// </summary>
    public int Id;

    /// <summary>
    /// Total Damage done by minions \n
    /// Length == # of phases
    /// </summary>
    public IReadOnlyList<int>? TotalDamage;

    /// <summary>
    /// Damage done by minions against targets \n
    /// Length == # of targets and the length of each sub array is equal to # of phases
    /// </summary>
    public IReadOnlyList<IReadOnlyList<int>>? TotalTargetDamage;
    /// <summary>
    /// Total Damage done to minions \n
    /// Length == # of phases
    /// </summary>
    public IReadOnlyList<int>? TotalDamageTaken;

    /// <summary>
    /// Total Breakbar Damage done by minions \n
    /// Length == # of phases
    /// </summary>
    public IReadOnlyList<double>? TotalBreakbarDamage;

    /// <summary>
    /// Breakbar Damage done by minions against targets \n
    /// Length == # of targets and the length of each sub array is equal to # of phases
    /// </summary>
    public IReadOnlyList<IReadOnlyList<double>>? TotalTargetBreakbarDamage;
    /// <summary>
    /// Total Breakbar Damage done to minions \n
    /// Length == # of phases
    /// </summary>
    public IReadOnlyList<double>? TotalBreakbarDamageTaken;

    /// <summary>
    /// Total Shield Damage done by minions \n
    /// Length == # of phases
    /// </summary>
    public IReadOnlyList<int>? TotalShieldDamage;

    /// <summary>
    /// Shield Damage done by minions against targets \n
    /// Length == # of targets and the length of each sub array is equal to # of phases
    /// </summary>
    public IReadOnlyList<IReadOnlyList<int>>? TotalTargetShieldDamage;
    /// <summary>
    /// Total Shield Damage done to minions \n
    /// Length == # of phases
    /// </summary>
    public IReadOnlyList<int>? TotalShieldDamageTaken;

    /// <summary>
    /// Total Damage distribution array \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="JsonDamageDist"/>
    public IReadOnlyList<IReadOnlyList<JsonDamageDist>>? TotalDamageDist;

    /// <summary>
    /// Per Target Damage distribution array \n
    /// Length == # of targets and the length of each sub array is equal to # of phases
    /// </summary>
    /// <seealso cref="JsonDamageDist"/>
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<JsonDamageDist>>>? TargetDamageDist;

    /// <summary>
    /// Total Damage Taken distribution array \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="JsonDamageDist"/>
    public IReadOnlyList<IReadOnlyList<JsonDamageDist>>? TotalDamageTakenDist;

    /// <summary>
    /// Rotation data
    /// </summary>
    /// <seealso cref="JsonRotation"/>
    public IReadOnlyList<JsonRotation>? Rotation;

    /// <summary>
    /// Healing stats data
    /// </summary>
    public EXTJsonMinionsHealingStats? EXTHealingStats;

    /// <summary>
    /// Barrier stats data
    /// </summary>
    public EXTJsonMinionsBarrierStats? EXTBarrierStats;
    /// <summary>
    /// Contains combat replay related data for each individual minion instance
    /// </summary>
    /// <seealso cref="JsonActorCombatReplayData"/>
    public IReadOnlyList<JsonActorCombatReplayData>? CombatReplayData;
}
