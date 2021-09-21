using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class VindicatorHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD), // Legendary Alliance Stance
            new DamageCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD), // Call of the Alliance
            new BuffGainCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD), // Urn of Saint Viktor
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Legendary Alliance Stance",-1, Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d6/Legendary_Alliance_Stance.png", 119939, ulong.MaxValue),
            new Buff("Urn of Saint Viktor", -1, Source.Vindicator, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/ff/Urn_of_Saint_Viktor.png", 119939, ulong.MaxValue),
            new Buff("Imperial Guard", -1, Source.Vindicator, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7f/Imperial_Guard.png", 119939, ulong.MaxValue),
        };
    }
}
