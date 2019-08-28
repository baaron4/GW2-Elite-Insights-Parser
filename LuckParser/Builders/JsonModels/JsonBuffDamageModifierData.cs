using System.Collections.Generic;
using LuckParser.Models;

namespace LuckParser.Builders.JsonModels
{
    public class JsonBuffDamageModifierData
    {
        /// <summary>
        /// Class corresponding to a buff based damage modifier
        /// </summary>
        public class JsonBuffDamageModifierItem
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

            public JsonBuffDamageModifierItem(Statistics.DamageModifierData extraData)
            {
                HitCount = extraData.HitCount;
                TotalHitCount = extraData.TotalHitCount;
                DamageGain = extraData.DamageGain;
                TotalDamage = extraData.TotalDamage;
            }
        }

        public int Id;
        public List<JsonBuffDamageModifierItem> DamageModifiers;
    }
}
