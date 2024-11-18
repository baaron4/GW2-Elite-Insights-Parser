namespace GW2EIJSON;

/// <summary>
/// Class corresponding a healing distribution
/// </summary>
public class EXTJsonHealingDist
{

    /// <summary>
    /// Total healing done
    /// </summary>
    public int TotalHealing;

    /// <summary>
    /// Total healing done against downed
    /// </summary>
    public int TotalDownedHealing;

    /// <summary>
    /// Minimum healing done
    /// </summary>
    public int Min;

    /// <summary>
    /// Maximum healing done
    /// </summary>
    public int Max;

    /// <summary>
    /// Number of hits
    /// </summary>
    public int Hits;

    /// <summary>
    /// ID of the healing skill
    /// </summary>
    /// <seealso cref="JsonLog.SkillMap"/>
    /// <seealso cref="JsonLog.BuffMap"/>
    public long Id;

    /// <summary>
    /// True if indirect healing \n
    /// If true, the id is a buff
    /// </summary>
    public bool IndirectHealing;
}
