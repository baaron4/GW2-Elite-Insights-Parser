using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;

namespace GW2EIEvtcParser.EIData
{
    internal class ChronomancerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(26766, "Danger Time", "10% crit damage on slowed target", DamageSource.All, 10.0, DamageType.Power, DamageType.All, ParserHelper.Source.Chronomancer, ByPresence, "https://wiki.guildwars2.com/images/3/33/Fragility.png", DamageModifierMode.All, (x => x.HasCrit)),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Time Echo",29582, ParserHelper.Source.Chronomancer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/8d/Deja_Vu.png"),
                new Buff("Time Anchored",30136, ParserHelper.Source.Chronomancer, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/db/Continuum_Split.png"),
        };

    }
}
