using System.Text.Json.Serialization;

namespace GW2EIJSON;

/// <summary>
/// Class corresponding to a rotation
/// </summary>
public class JsonRotation
{
    /// <summary>
    /// Class corresponding to a skill
    /// </summary>
    public class JsonSkill
    {
        /// <summary>
        /// Time at which the skill was cast
        /// </summary>
        public int CastTime;

        /// <summary>
        /// Duration of the animation, instant cast if 0
        /// </summary>
        public int Duration;

        /// <summary>
        /// Gained time from the animation, could be negative \n
        /// Strictly Negative indicates that the cast was interrupted, considered as time wasted \n
        /// Strictly Positive indicates that the after cast was interrupted, considered as time gained \n
        /// Zero indicates that the animation was fully executed, after cast included
        /// </summary>
        public int TimeGained;

        /// <summary>	
        /// Value between -1 (100% slow) and 1 (100% quickness) \n
        /// Prior arcdps activation update (nov 07 2019) this value can only be 0 or 1
        /// </summary>	
        public double Quickness;

        /// <summary>	
        /// If true, the EI html will ignore this event when displaying rotations. The cast itself remains relevant for statistical purposes. This is only for visual convenience on rotation display. \n
        /// </summary>	
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? IgnoreOnRotationRender = null;
    }


    /// <summary>
    /// ID of the skill
    /// </summary>
    /// <seealso cref="JsonLog.SkillMap"/>
    public long Id;

    /// <summary>
    /// List of casted skills
    /// </summary>
    /// <seealso cref="JsonSkill"/>
    public IReadOnlyList<JsonSkill>? Skills;
}
