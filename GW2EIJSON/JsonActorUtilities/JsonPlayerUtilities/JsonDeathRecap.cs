using System.Collections.Generic;

namespace GW2EIJSON
{
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
            public long Id { get; set; }

            /// <summary>
            /// True if the damage was indirect
            /// </summary>
            public bool IndirectDamage { get; set; }

            /// <summary>
            /// Source of the damage
            /// </summary>
            public string Src { get; set; }

            /// <summary>
            /// Damage done
            /// </summary>
            public int Damage { get; set; }

            /// <summary>
            /// Time value
            /// </summary>
            public int Time { get; set; }


            public JsonDeathRecapDamageItem()
            {

            }
        }


        /// <summary>
        /// Time of death
        /// </summary>
        public long DeathTime { get; set; }

        /// <summary>
        /// List of damaging events to put into downstate
        /// </summary>
        public IReadOnlyList<JsonDeathRecapDamageItem> ToDown { get; set; }

        /// <summary>
        /// List of damaging events to put into deadstate
        /// </summary>
        public IReadOnlyList<JsonDeathRecapDamageItem> ToKill { get; set; }


        public JsonDeathRecap()
        {

        }

    }
}
