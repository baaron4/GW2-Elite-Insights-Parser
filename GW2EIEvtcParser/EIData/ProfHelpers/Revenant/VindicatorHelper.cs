using System.Collections.Generic;
using GW2EIEvtcParser.Extensions;
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
            new BuffGainCastFinder(62749, -1, EIData.InstantCastFinder.DefaultICD), // Legendary Alliance Stance
            new BuffGainCastFinder(62891, -1, EIData.InstantCastFinder.DefaultICD), // Legendary Alliance Stance (UW)
            new DamageCastFinder(62705, 62705, EIData.InstantCastFinder.DefaultICD), // Call of the Alliance
            new BuffGainCastFinder(62687, -1, EIData.InstantCastFinder.DefaultICD), // Urn of Saint Viktor
            //new EXTHealingCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD), // Redemptor's Sermon
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(-1, "Forerunner of Death", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Vindicator, ByPresence, "https://wiki.guildwars2.com/images/9/95/Forerunner_of_Death.png", 119939, ulong.MaxValue, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Legendary Alliance Stance",-1, Source.Revenant, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d6/Legendary_Alliance_Stance.png", 119939, ulong.MaxValue),
            new Buff("Urn of Saint Viktor", -1, Source.Vindicator, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/ff/Urn_of_Saint_Viktor.png", 119939, ulong.MaxValue),
            new Buff("Saint of zu Heltzer", -1, Source.Vindicator, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/3/36/Saint_of_zu_Heltzer.png", 119939, ulong.MaxValue),
            new Buff("Forerunner of Death", -1, Source.Vindicator, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/9/95/Forerunner_of_Death.png", 119939, ulong.MaxValue),
            new Buff("Imperial Guard", -1, Source.Vindicator, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7f/Imperial_Guard.png", 119939, ulong.MaxValue),
        };

        internal static HashSet<long> VindicatorDodges = new HashSet<long>()
        {
            62859, // Imperial Impact
            62693, // Death Drop
            62689, // Saint's Shield
        };
    }
}
