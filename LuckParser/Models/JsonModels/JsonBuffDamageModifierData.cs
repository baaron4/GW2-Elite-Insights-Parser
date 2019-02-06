using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
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
            /// Gained damage
            /// </summary>
            public int DamageGain;
            /// <summary>
            /// Total damage done
            /// </summary>
            public int TotalDamage;
            /// <summary>
            /// True if the buff is a multiplier type, in which case a % gain makes sense
            /// </summary>
            public bool Multiplier;

            public JsonBuffDamageModifierItem(ParseModels.AbstractMasterActor.ExtraBoonData extraData)
            {
                HitCount = extraData.HitCount;
                TotalHitCount = extraData.TotalHitCount;
                DamageGain = extraData.DamageGain;
                TotalDamage = extraData.TotalDamage;
                Multiplier = extraData.Multiplier;
            }
        }

        public long Id;
        public List<JsonBuffDamageModifierItem> DamageModifiers;
    }
}
