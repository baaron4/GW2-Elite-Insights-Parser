namespace GW2EIJSON;

/// <summary>
/// Class representing damage modifier data
/// </summary>
public class JsonDamageModifierData
{
    /// <summary>
    /// Class corresponding to a buff based damage modifier
    /// </summary>
    public class JsonDamageModifierItem
    {
        /// <summary>
        /// Hits done under the buff
        /// </summary>
        public int HitCount;

        /// <summary>
        /// Total hits
        /// </summary>
        public int TotalHitCount;

        /// <summary>
        /// Gained damage \n
        /// If the corresponding <see cref="JsonLog.DamageModDesc.NonMultiplier"/> is true then this value correspond to the damage done while under the effect. One will have to deduce the gain manualy depending on your gear.
        /// </summary>
        public double DamageGain;

        /// <summary>
        /// Total damage done
        /// </summary>
        public int TotalDamage;
    }

    /// <summary>
    /// ID of the damage modifier \
    /// </summary>
    /// <seealso cref="JsonLog.DamageModMap"/>
    public int Id;

    /// <summary>
    /// Array of damage modifier data \n
    /// Length == # of phases
    /// </summary>
    /// <seealso cref="JsonDamageModifierItem"/>
    public IReadOnlyList<JsonDamageModifierItem>? DamageModifiers;
}
