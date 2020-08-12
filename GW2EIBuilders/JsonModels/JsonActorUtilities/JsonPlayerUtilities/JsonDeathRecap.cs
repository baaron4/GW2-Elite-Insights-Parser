using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using Newtonsoft.Json;

namespace GW2EIBuilders.JsonModels
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
            [JsonProperty]
            /// <summary>
            /// Id of the skill
            /// </summary>
            /// <seealso cref="JsonLog.SkillMap"/>
            /// <seealso cref="JsonLog.BuffMap"/>
            public long Id { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// True if the damage was indirect
            /// </summary>
            public bool IndirectDamage { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Source of the damage
            /// </summary>
            public string Src { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Damage done
            /// </summary>
            public int Damage { get; internal set; }
            [JsonProperty]
            /// <summary>
            /// Time value
            /// </summary>
            public int Time { get; internal set; }

            [JsonConstructor]
            internal JsonDeathRecapDamageItem()
            {

            }
            internal JsonDeathRecapDamageItem(DeathRecap.DeathRecapDamageItem item)
            {
                Id = item.ID;
                IndirectDamage = item.IndirectDamage;
                Src = item.Src;
                Damage = item.Damage;
                Time = item.Time;
            }
        }

        [JsonProperty]
        /// <summary>
        /// Time of death
        /// </summary>
        public long DeathTime { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// List of damaging events to put into downstate
        /// </summary>
        public List<JsonDeathRecapDamageItem> ToDown { get; internal set; }
        [JsonProperty]
        /// <summary>
        /// List of damaging events to put into deadstate
        /// </summary>
        public List<JsonDeathRecapDamageItem> ToKill { get; internal set; }

        [JsonConstructor]
        internal JsonDeathRecap()
        {

        }

        internal JsonDeathRecap(DeathRecap recap)
        {
            DeathTime = recap.DeathTime;
            ToDown = recap.ToDown?.Select(x => new JsonDeathRecapDamageItem(x)).ToList();
            ToKill = recap.ToKill?.Select(x => new JsonDeathRecapDamageItem(x)).ToList();
        }

    }
}
