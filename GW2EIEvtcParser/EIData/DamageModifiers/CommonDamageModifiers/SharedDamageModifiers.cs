using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

internal static class SharedDamageModifiers
{

    private static bool VulnerabilityActiveCheck(HealthDamageEvent evt, ParsedEvtcLog log)
    {
        if (evt.To.HasBuff(log, Resistance, evt.Time))
        {
            return false;
        }
        // Ion shield disabled vulnerability, so we check the presence of the buff when hitting Sabir 
        if (evt.To.IsSpecies(TargetID.Sabir) && evt.To.HasBuff(log, IonShield, evt.Time))
        {
            return false;
        }
        // Malicious Shadows in CM have a unique Resistance buff
        if (evt.To.IsSpecies(TrashID.MaliciousShadowCM) && evt.To.HasBuff(log, ResistanceUnremovable, evt.Time))
        {
            return false;
        }
        return true;
    }

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnFoeDamageModifier(Mod_Exposed, Exposed31589, "Exposed", "50%", DamageSource.All, 50.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new BuffOnFoeDamageModifier(Mod_ExposedStrike, Exposed31589, "Exposed (Strike)", "30%", DamageSource.All, 30.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
            .WithBuilds( GW2Builds.May2021Balance, GW2Builds.March2022Balance2),
        new BuffOnFoeDamageModifier(Mod_ExposedCondition, Exposed31589, "Exposed (Condition)", "100%", DamageSource.All, 100.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.March2022Balance2),
        new BuffOnFoeDamageModifier(Mod_ExposedStrike, Exposed31589, "Exposed (Strike)", "10%", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
            .WithBuilds(GW2Builds.March2022Balance2),
        new BuffOnFoeDamageModifier(Mod_ExposedCondition, Exposed31589, "Exposed (Condition)", "20%", DamageSource.All, 20.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
            .WithBuilds(GW2Builds.March2022Balance2),
        new BuffOnFoeDamageModifier(Mod_OldExposedStrike, OldExposed, "Old Exposed (Strike)", "30%", DamageSource.All, 30.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
            .WithBuilds(GW2Builds.March2022Balance2),
        new BuffOnFoeDamageModifier(Mod_OldExposedCondition, OldExposed, "Old Exposed (Condition)", "100%", DamageSource.All, 100.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Exposed, DamageModifierMode.All)
            .WithBuilds(GW2Builds.March2022Balance2),
        new BuffOnFoeDamageModifier(Mod_Vulnerability, Vulnerability, "Vulnerability", "1% per Stack", DamageSource.All, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByStack, BuffImages.Vulnerability, DamageModifierMode.All)
            .UsingChecker(VulnerabilityActiveCheck),
        new BuffOnFoeDamageModifier(Mod_Protection, Protection, "Protection", "-33%", DamageSource.All, -33.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Protection, DamageModifierMode.All),
        new BuffOnFoeDamageModifier(Mod_Resolution, Resolution, "Resolution", "-33%", DamageSource.All, -33.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Resolution, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_Emboldened, Emboldened, "Emboldened", "10% per stack", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByStack, BuffImages.Emboldened, DamageModifierMode.All)
            .WithBuilds(GW2Builds.June2022Balance),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_Vulnerability, Vulnerability, "Vulnerability", "1% per Stack", DamageSource.All, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByStack, BuffImages.Vulnerability, DamageModifierMode.All)
            .UsingChecker(VulnerabilityActiveCheck),
        new BuffOnActorDamageModifier(Mod_Protection, Protection, "Protection", "-33%", DamageSource.All, -33.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.Protection, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_Resolution, Resolution, "Resolution", "-33%", DamageSource.All, -33.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.Resolution, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_FrostAura, FrostAura, "Frost Aura", "-10%", DamageSource.All, -10.0, DamageType.Strike, DamageType.All, Source.Common, ByPresence, BuffImages.FrostAura, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_LightAura, LightAura, "Light Aura", "-10%", DamageSource.All, -10.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.LightAura, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_DarkAura, DarkAura, "Dark Aura", "-20%", DamageSource.All, -20.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.DarkAura, DamageModifierMode.All),
    ];
}
