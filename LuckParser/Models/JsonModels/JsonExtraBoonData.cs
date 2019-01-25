using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.JsonModels
{
    public class JsonExtraBoonData
    {
        public int HitCount;
        public int TotalHitCount;
        public int DamageGain;
        public int TotalDamage;

        public JsonExtraBoonData(ParseModels.AbstractMasterActor.ExtraBoonData extraData)
        {
            HitCount = extraData.HitCount;
            TotalHitCount = extraData.TotalHitCount;
            DamageGain = extraData.DamageGain;
            TotalDamage = extraData.TotalDamage;
        }
    }
}
