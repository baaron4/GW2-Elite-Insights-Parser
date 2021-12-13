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
            new BuffGainCastFinder(62749, 62919, EIData.InstantCastFinder.DefaultICD), // Legendary Alliance Stance
            new BuffGainCastFinder(62891, 62919, EIData.InstantCastFinder.DefaultICD), // Legendary Alliance Stance (UW)
            new DamageCastFinder(62705, 62705, EIData.InstantCastFinder.DefaultICD), // Call of the Alliance
            new BuffGainCastFinder(62687, 62864, EIData.InstantCastFinder.DefaultICD), // Urn of Saint Viktor
            new BuffGainCastFinder(62693, 62811, EIData.InstantCastFinder.DefaultICD), // Forerunner of Death (Death Drop) 
            new BuffGainCastFinder(62689, 62994, EIData.InstantCastFinder.DefaultICD), // Saint of zu Heltzer (Saint's Shield)
            new DamageCastFinder(62859, 62859, EIData.InstantCastFinder.DefaultICD), // Vassals of the Empire (Imperial Impact)
            //new EXTHealingCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD), // Redemptor's Sermon
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(62811, "Forerunner of Death", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Vindicator, ByPresence, "https://wiki.guildwars2.com/images/9/95/Forerunner_of_Death.png", GW2Builds.EODBeta2, GW2Builds.EndOfLife, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Legendary Alliance Stance",62919, Source.Revenant, BuffNature.GraphOnly, "https://wiki.guildwars2.com/images/d/d6/Legendary_Alliance_Stance.png"),
            new Buff("Urn of Saint Viktor", 62864, Source.Vindicator, BuffNature.GraphOnly,"https://wiki.guildwars2.com/images/f/ff/Urn_of_Saint_Viktor.png"),
            new Buff("Saint of zu Heltzer", 62994, Source.Vindicator, BuffNature.GraphOnly,"https://wiki.guildwars2.com/images/3/36/Saint_of_zu_Heltzer.png"),
            new Buff("Forerunner of Death", 62811, Source.Vindicator, BuffNature.GraphOnly,"https://wiki.guildwars2.com/images/9/95/Forerunner_of_Death.png"),
            new Buff("Imperial Guard", 62819, Source.Vindicator, BuffStackType.Stacking, 5, BuffNature.GraphOnly, "https://wiki.guildwars2.com/images/7/7f/Imperial_Guard.png"),
        };
    }
}
