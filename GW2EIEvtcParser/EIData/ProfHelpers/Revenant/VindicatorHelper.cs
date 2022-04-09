using System.Collections.Generic;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class VindicatorHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(62749, LegendaryAllianceStance, EIData.InstantCastFinder.DefaultICD), // Legendary Alliance Stance
            new BuffGainCastFinder(62891, LegendaryAllianceStance, EIData.InstantCastFinder.DefaultICD), // Legendary Alliance Stance (UW)
            new DamageCastFinder(62705, 62705, EIData.InstantCastFinder.DefaultICD), // Call of the Alliance
            new BuffGainCastFinder(62687, UrnOfSaintViktor, EIData.InstantCastFinder.DefaultICD), // Urn of Saint Viktor
            new BuffGainCastFinder(62693, ForerunnerOfDeath, EIData.InstantCastFinder.DefaultICD), // Forerunner of Death (Death Drop) 
            new BuffGainCastFinder(62689, SaintOfzuHeltzer, EIData.InstantCastFinder.DefaultICD), // Saint of zu Heltzer (Saint's Shield)
            new DamageCastFinder(62859, 62859, EIData.InstantCastFinder.DefaultICD), // Vassals of the Empire (Imperial Impact)
            //new EXTHealingCastFinder(-1, -1, EIData.InstantCastFinder.DefaultICD), // Redemptor's Sermon
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(ForerunnerOfDeath, "Forerunner of Death", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Vindicator, ByPresence, "https://wiki.guildwars2.com/images/9/95/Forerunner_of_Death.png", GW2Builds.EODBeta2, GW2Builds.EndOfLife, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Legendary Alliance Stance",LegendaryAllianceStance, Source.Revenant, BuffClassification.Other, "https://wiki.guildwars2.com/images/d/d6/Legendary_Alliance_Stance.png"),
            new Buff("Urn of Saint Viktor", UrnOfSaintViktor, Source.Vindicator, BuffClassification.Other,"https://wiki.guildwars2.com/images/f/ff/Urn_of_Saint_Viktor.png"),
            new Buff("Saint of zu Heltzer", SaintOfzuHeltzer, Source.Vindicator, BuffClassification.Other,"https://wiki.guildwars2.com/images/3/36/Saint_of_zu_Heltzer.png"),
            new Buff("Forerunner of Death", ForerunnerOfDeath, Source.Vindicator, BuffClassification.Other,"https://wiki.guildwars2.com/images/9/95/Forerunner_of_Death.png"),
            new Buff("Imperial Guard", ImperialGuard, Source.Vindicator, BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7f/Imperial_Guard.png"),
        };
    }
}
