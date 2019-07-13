using LuckParser.Parser.ParsedData;
using LuckParser.Parser.ParsedData.CombatEvents;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.EIData
{
    public class MirageHelper : MesmerHelper
    {
        public static List<AnimatedCastEvent> TranslateMirageCloak(List<AbstractBuffEvent> buffs, SkillData skillData)
        {
            List<AnimatedCastEvent> res = new List<AnimatedCastEvent>();
            long cloakStart = 0;
            foreach (AbstractBuffEvent ba in buffs.Where(x => x is BuffApplyEvent))
            {
                if (ba.Time - cloakStart > 10)
                {
                    AnimatedCastEvent dodgeLog = new AnimatedCastEvent(ba.Time, skillData.Get(SkillItem.DodgeId), 50, ba.To);
                    res.Add(dodgeLog);
                    cloakStart = ba.Time;
                }
            }
            return res;
        }
    }
}
