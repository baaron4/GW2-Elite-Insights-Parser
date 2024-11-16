namespace GW2EIJSON;

/// <summary>
/// Class corresponding to a death recap
/// </summary>
public class JsonDeathRecap
{
    /// <summary>
    /// Elementary death recap item
    /// </summary>
    public class JsonDeathRecapDamageItem
    {
        /// <summary>
        /// Id of the skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id;

        /// <summary>
        /// True if the damage was indirect
        /// </summary>
        public bool IndirectDamage;

        /// <summary>
        /// Source of the damage
        /// </summary>
        public string? Src;

        /// <summary>
        /// Damage done
        /// </summary>
        public int Damage;

        /// <summary>
        /// Time value
        /// </summary>
        public int Time;
    }


    /// <summary>
    /// Time of death
    /// </summary>
    public long DeathTime;

    /// <summary>
    /// List of damaging events to put into downstate
    /// </summary>
    public IReadOnlyList<JsonDeathRecapDamageItem>? ToDown;

    /// <summary>
    /// List of damaging events to put into deadstate
    /// </summary>
    public IReadOnlyList<JsonDeathRecapDamageItem>? ToKill;
}
