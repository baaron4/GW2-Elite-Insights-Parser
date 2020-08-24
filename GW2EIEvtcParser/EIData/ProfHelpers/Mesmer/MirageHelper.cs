using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    internal class MirageHelper : MesmerHelper
    {

        internal static readonly List<InstantCastFinder> MirageInstantCastFinders = new List<InstantCastFinder>()
        {
            new DamageCastFinder(45449, 45449, InstantCastFinder.DefaultICD), // Jaunt
        };


        public static List<InstantCastEvent> TranslateMirageCloak(List<AbstractBuffEvent> buffs, SkillData skillData)
        {
            var res = new List<InstantCastEvent>();
            long cloakStart = 0;
            foreach (AbstractBuffEvent ba in buffs.Where(x => x is BuffApplyEvent))
            {
                if (ba.Time - cloakStart > 10)
                {
                    var dodgeLog = new InstantCastEvent(ba.Time, skillData.Get(SkillItem.MirageCloakDodgeId), ba.To.GetFinalMaster());
                    res.Add(dodgeLog);
                    cloakStart = ba.Time;
                }
            }
            return res;
        }
    }
}
