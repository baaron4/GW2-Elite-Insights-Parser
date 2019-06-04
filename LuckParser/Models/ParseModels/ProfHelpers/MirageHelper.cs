using LuckParser.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class MirageHelper : MesmerHelper
    {
        public static List<AnimatedCastEvent> TranslateMirageCloak(List<AbstractBuffEvent> buffs)
        {
            List<AnimatedCastEvent> res = new List<AnimatedCastEvent>();
            long cloakStart = 0;
            foreach (AbstractBuffEvent ba in buffs.Where(x => x is BuffApplyEvent))
            {
                if (ba.Time - cloakStart > 10)
                {
                    AnimatedCastEvent dodgeLog = new AnimatedCastEvent(ba.Time, SkillItem.DodgeId, 50, ba.To);
                    res.Add(dodgeLog);
                    cloakStart = ba.Time;
                }
            }
            return res;
        }
    }
}
