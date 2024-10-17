namespace GW2EIJSON;

/// <summary>
/// Class corresponding a barrier distribution
/// </summary>
public class EXTJsonBarrierDist
{

    /// <summary>
    /// Total barrier done
    /// </summary>
    public int TotalBarrier;

    /// <summary>
    /// Minimum barrier done
    /// </summary>
    public int Min;

    /// <summary>
    /// Maximum barrier done
    /// </summary>
    public int Max;

    /// <summary>
    /// Number of hits
    /// </summary>
    public int Hits;

    /// <summary>
    /// ID of the barrier skill
    /// </summary>
    /// <seealso cref="JsonLog.SkillMap"/>
    /// <seealso cref="JsonLog.BuffMap"/>
    public long Id;

    /// <summary>
    /// True if indirect barrier \n
    /// If true, the id is a buff
    /// </summary>
    public bool IndirectBarrier;
}
