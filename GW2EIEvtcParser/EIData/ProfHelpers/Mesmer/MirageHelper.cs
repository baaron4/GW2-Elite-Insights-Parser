using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;
using System.Linq;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData
{
    internal class MirageHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(45449, 45449, EIData.InstantCastFinder.DefaultICD), // Jaunt
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Mirage Cloak",40408, ParserHelper.Source.Mirage, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a5/Mirage_Cloak_%28effect%29.png"),
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
