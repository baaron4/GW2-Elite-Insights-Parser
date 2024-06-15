using System.Collections.Generic;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class VindicatorHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(LegendaryAllianceStanceSkill, LegendaryAllianceStanceBuff),
            //new BuffGainCastFinder(LegendaryAllianceStanceUWSkill, LegendaryAllianceStanceEffect),
            new DamageCastFinder(CallOfTheAlliance, CallOfTheAlliance),
            new BuffGainCastFinder(UrnOfSaintViktorSkill, UrnOfSaintViktorBuff),
        };

        private static readonly HashSet<long> _dodges = new HashSet<long>
        {
            DeathDropDodge, ImperialImpactDodge, SaintsShieldDodge
        };

        public static bool IsVindicatorDodge(long id)
        {
            return _dodges.Contains(id);
        }

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(ForerunnerOfDeath, "Forerunner of Death", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Vindicator, ByPresence, BuffImages.ForerunnerOfDeath, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta2, GW2Builds.September2023Balance),
            new BuffOnActorDamageModifier(ForerunnerOfDeath, "Forerunner of Death", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Vindicator, ByPresence, BuffImages.ForerunnerOfDeath, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.September2023Balance),
            new BuffOnActorDamageModifier(ForerunnerOfDeath, "Forerunner of Death", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Vindicator, ByPresence, BuffImages.ForerunnerOfDeath, DamageModifierMode.PvE).WithBuilds(GW2Builds.September2023Balance),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(UrnOfSaintViktorBuff, "Urn of Saint Viktor", "-50%", DamageSource.NoPets, -50.0, DamageType.StrikeAndCondition, DamageType.All, Source.Vindicator, ByPresence, BuffImages.UrnOfSaintViktor, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2024LonelyTowerFractalRelease),
            new BuffOnActorDamageModifier(UrnOfSaintViktorBuff, "Urn of Saint Viktor", "-33%", DamageSource.NoPets, -33.0, DamageType.StrikeAndCondition, DamageType.All, Source.Vindicator, ByPresence, BuffImages.UrnOfSaintViktor, DamageModifierMode.sPvP)
                .WithBuilds(GW2Builds.May2024LonelyTowerFractalRelease),
            new BuffOnActorDamageModifier(UrnOfSaintViktorBuff, "Urn of Saint Viktor", "-50%", DamageSource.NoPets, -50.0, DamageType.StrikeAndCondition, DamageType.All, Source.Vindicator, ByPresence, BuffImages.UrnOfSaintViktor, DamageModifierMode.PvEWvW)
                .WithBuilds(GW2Builds.May2024LonelyTowerFractalRelease),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Legendary Alliance Stance", LegendaryAllianceStanceBuff, Source.Revenant, BuffClassification.Other, BuffImages.LegendaryAllianceStance),
            new Buff("Urn of Saint Viktor", UrnOfSaintViktorBuff, Source.Vindicator, BuffClassification.Other, BuffImages.UrnOfSaintViktor),
            new Buff("Saint of zu Heltzer", SaintOfzuHeltzer, Source.Vindicator, BuffClassification.Other, BuffImages.SaintOfZuHeltzer),
            new Buff("Forerunner of Death", ForerunnerOfDeath, Source.Vindicator, BuffClassification.Other, BuffImages.ForerunnerOfDeath),
            new Buff("Imperial Guard", ImperialGuard, Source.Vindicator, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.ImperialGuard).WithBuilds(GW2Builds.StartOfLife, GW2Builds.SOTOBetaAndSilentSurfNM),
        };
    }
}
