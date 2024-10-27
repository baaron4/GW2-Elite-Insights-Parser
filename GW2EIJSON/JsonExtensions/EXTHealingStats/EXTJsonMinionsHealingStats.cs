using System.Collections.Generic;


namespace GW2EIJSON;

/// <summary>
/// Class regrouping minion related healing statistics
/// </summary>
public class EXTJsonMinionsHealingStats
{
    /// <summary>
    /// Total Healing done by minions \n
    /// Length == # of phases
    /// </summary>
    public IReadOnlyList<int>? TotalHealing;
    /// <summary>
    /// Total Allied Healing done by minions \n
    /// Length == # of players and the length of each sub array is equal to # of phases
    /// </summary>
    public IReadOnlyList<IReadOnlyList<int>>? TotalAlliedHealing;
    /// <summary>
    /// Total Outgoing Healing distribution array \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="EXTJsonHealingDist"/>
    public IReadOnlyList<IReadOnlyList<EXTJsonHealingDist>>? TotalHealingDist;

    /// <summary>
    /// Total Outgoing Allied Healing distribution array \n
    /// Length == # of players and the length of each sub array is equal to # of phases
    /// </summary>
    /// <seealso cref="EXTJsonHealingDist"/>
    public IReadOnlyList<IReadOnlyList<IReadOnlyList<EXTJsonHealingDist>>>? AlliedHealingDist;
}
