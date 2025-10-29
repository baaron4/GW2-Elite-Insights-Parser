using System.Text.Json.Serialization;

namespace GW2EIJSON;

/// <summary>
/// Class corresponding a damage distribution
/// </summary>
public class JsonDamageDist
{
    /// <summary>
    /// Total damage done
    /// </summary>
    public int TotalDamage;

    /// <summary>
    /// Total breakbar damage done
    /// </summary>
    public double TotalBreakbarDamage;

    /// <summary>
    /// Minimum damage done
    /// </summary>
    public int Min;

    /// <summary>
    /// Maximum damage done
    /// </summary>
    public int Max;

    /// <summary>
    /// Number of hits
    /// </summary>
    public int Hits;

    /// <summary>
    /// Number of connected hits
    /// </summary>
    public int ConnectedHits;

    /// <summary>
    /// Number of crits
    /// </summary>
    public int Crit;

    /// <summary>
    /// Number of glances
    /// </summary>
    public int Glance;

    /// <summary>
    /// Number of flanks
    /// </summary>
    public int Flank;

    /// <summary>
    /// Number of time hits while target was moving
    /// </summary>
    public int AgainstMoving;

    /// <summary>
    /// Number of times the hit missed due to blindness
    /// </summary>
    public int Missed;

    /// <summary>
    /// Number of times the hit was invulned
    /// </summary>
    public int Invulned;

    /// <summary>
    /// Number of times the hit nterrupted
    /// </summary>
    public int Interrupted;

    /// <summary>
    /// Number of times the hit was evaded
    /// </summary>
    public int Evaded;

    /// <summary>
    /// Number of times the hit was blocked
    /// </summary>
    public int Blocked;

    /// <summary>
    /// Damage done against barrier, not necessarily included in total damage
    /// </summary>
    public int ShieldDamage;

    /// <summary>
    /// Critical damage
    /// </summary>
    public int CritDamage;

    /// <summary>
    /// Relevant for WvW, defined as the sum of damage done from 90% to down that led to a death. \n
    /// Only relevant for outgoing damage distribution, not incoming and for non minion actors.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? DownContribution;

    /// <summary>
    /// ID of the damaging skill
    /// </summary>
    /// <seealso cref="JsonLog.SkillMap"/>
    /// <seealso cref="JsonLog.BuffMap"/>
    public long Id;

    /// <summary>
    /// True if indirect damage \n
    /// If true, the id is a buff
    /// </summary>
    public bool IndirectDamage;
}
