using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class SharedDamageModifiers
    {
        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnFoeDamageModifier(Exposed31589, "Exposed", "50%", DamageSource.All, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffOnFoeDamageModifier(Exposed31589, "Exposed (Strike)", "30%", DamageSource.All, 30.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds( GW2Builds.May2021Balance, GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(Exposed31589, "Exposed (Condition)", "100%", DamageSource.All, 100.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2021Balance, GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(Exposed31589, "Exposed (Strike)", "10%", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(Exposed31589, "Exposed (Condition)", "20%", DamageSource.All, 20.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(OldExposed, "Old Exposed (Strike)", "30%", DamageSource.All, 30.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(OldExposed, "Old Exposed (Condition)", "100%", DamageSource.All, 100.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
                .WithBuilds(GW2Builds.March2022Balance2),
            new BuffOnFoeDamageModifier(Vulnerability, "Vulnerability", "1% per Stack", DamageSource.All, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByStack, BuffImages.Vulnerability, DamageModifierMode.All)
                .UsingChecker((evt, log) => !evt.To.HasBuff(log, Resistance, evt.Time) && (!evt.To.IsSpecies(TargetID.Sabir) || !evt.To.HasBuff(log, IonShield, evt.Time))), // Ion shield disabled vulnerability, so we check the presence of the buff when hitting Sabir 
            new BuffOnFoeDamageModifier(Protection, "Protection", "-33%", DamageSource.All, -33.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Protection, DamageModifierMode.All),
            new BuffOnFoeDamageModifier(Resolution, "Resolution", "-33%", DamageSource.All, -33.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Resolution, DamageModifierMode.All),
            new BuffOnActorDamageModifier(FrostSpiritBuff, "Frost Spirit", "5%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.FrostSpirit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2018Balance, GW2Builds.June2022Balance),
            new SkillDamageModifier("Soulcleave's Summit", "per hit (no ICD)", SoulcleavesSummitBuff, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, BuffImages.SoulcleavesSummit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new SkillDamageModifier("Soulcleave's Summit", "per hit (1s ICD per target)", SoulcleavesSummitBuff, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, BuffImages.SoulcleavesSummit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2021Balance),
            new SkillDamageModifier("One Wolf Pack", "per hit (max. once every 0.25s)", OneWolfPackDamage, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, BuffImages.OneWolfPack, DamageModifierMode.All),
            new BuffOnActorDamageModifier(Emboldened, "Emboldened", "10% per stack", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByStack, BuffImages.Emboldened, DamageModifierMode.All)
                .WithBuilds(GW2Builds.June2022Balance),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(Vulnerability, "Vulnerability", "1% per Stack", DamageSource.All, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByStack, BuffImages.Vulnerability, DamageModifierMode.All),
            new BuffOnActorDamageModifier(Protection, "Protection", "-33%", DamageSource.All, -33.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Protection, DamageModifierMode.All),
            new BuffOnActorDamageModifier(Resolution, "Resolution", "-33%", DamageSource.All, -33.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Resolution, DamageModifierMode.All),
            new BuffOnActorDamageModifier(FrostAura, "Frost Aura", "-10%", DamageSource.All, -10.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.FrostAura, DamageModifierMode.All),
            new BuffOnActorDamageModifier(LightAura, "Light Aura", "-33%", DamageSource.All, -10.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.LightAura, DamageModifierMode.All),
            new BuffOnActorDamageModifier(DarkAura, "Dark Aura", "-33%", DamageSource.All, -20.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.DarkAura, DamageModifierMode.All),
            new BuffOnActorDamageModifier(DolyakStanceBuff, "Dolyak Stance", "-33%", DamageSource.All, -20.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByPresence, BuffImages.DolyakStance, DamageModifierMode.All),
        };
    }
}
