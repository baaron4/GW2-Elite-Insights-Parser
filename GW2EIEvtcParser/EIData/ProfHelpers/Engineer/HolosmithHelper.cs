using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class HolosmithHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new BuffGainCastFinder(EnterPhotonForge, PhotonForge)
            .UsingBeforeWeaponSwap(true),
        new BuffLossCastFinder(ExitPhotonForge, PhotonForge)
            .UsingBeforeWeaponSwap(true),
        new BuffGainCastFinder(OverheatSkill, OverheatBuff)
            .UsingBeforeWeaponSwap(true),
        new BuffGainCastFinder(SpectrumShieldSkill, SpectrumShieldBuff),
        new DamageCastFinder(ThermalReleaseValve, ThermalReleaseValve)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
        new EffectCastFinderByDst(FlashSpark, EffectGUIDs.HolosmithFlashSpark)
            .UsingDstSpecChecker(Spec.Holosmith),
        new EffectCastFinderByDst(BladeBurstOrParticleAccelerator, EffectGUIDs.HolosmitBladeBurstParticleAccelerator1)
            .UsingDstSpecChecker(Spec.Holosmith)
            .UsingSecondaryEffectChecker(EffectGUIDs.HolosmitBladeBurstParticleAccelerator2),
    ];

    private static readonly HashSet<long> _photonForgeCast =
    [
        EnterPhotonForge, ExitPhotonForge
    ];

    public static bool IsPhotonForgeTransform(long id)
    {
        return _photonForgeCast.Contains(id);
    }

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_LasersEdge, LasersEdge, "Laser's Edge", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Holosmith, ByPresence, TraitImages.LasersEdge, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        new DamageLogDamageModifier(Mod_SolarFocusingLens, "Solar Focusing Lens", "10%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Holosmith, TraitImages.SolarFocusingLens, (x, log) => log.CombatData.HasLostBuff(Afterburner, x.From, x.Time, ServerDelayConstant), DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        new BuffOnActorDamageModifier(Mod_SpectrumShield, SpectrumShieldBuff, "Spectrum Shield", "-50%", DamageSource.Incoming, -50, DamageType.StrikeAndCondition, DamageType.All, Source.Holosmith, ByPresence, SkillImages.SpectrumShield, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_SpectrumShield, SpectrumShieldBuff, "Spectrum Shield", "-50%", DamageSource.Incoming, -50, DamageType.StrikeAndCondition, DamageType.All, Source.Holosmith, ByPresence, SkillImages.SpectrumShield, DamageModifierMode.PvEWvW)
            .WithBuilds(GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_SpectrumShield, SpectrumShieldBuff, "Spectrum Shield", "-33%", DamageSource.Incoming, -33, DamageType.StrikeAndCondition, DamageType.All, Source.Holosmith, ByPresence, SkillImages.SpectrumShield, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_LightDensityAmplifier, PhotonForge, "Light Density Amplifier", "-15%", DamageSource.Incoming, -15, DamageType.Strike, DamageType.All, Source.Holosmith, ByPresence, TraitImages.LightDensityAmplifier, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Cooling Vapor", CoolingVapor, Source.Holosmith, BuffClassification.Other, SkillImages.CoolantBlast),
        new Buff("Photon Wall Deployed", PhotonWallDeployed, Source.Holosmith, BuffClassification.Other, SkillImages.PhotonWall),
        new Buff("Spectrum Shield", SpectrumShieldBuff, Source.Holosmith, BuffClassification.Other, SkillImages.SpectrumShield),
        new Buff("Photon Forge", PhotonForge, Source.Holosmith, BuffClassification.Other, SkillImages.EngagePhotonForge),
        new Buff("Laser's Edge", LasersEdge, Source.Holosmith, BuffClassification.Other, TraitImages.LasersEdge)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        new Buff("Afterburner", Afterburner, Source.Holosmith, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, TraitImages.SolarFocusingLens),
        new Buff("Heat Therapy", HeatTherapy, Source.Holosmith, BuffStackType.Stacking, 10, BuffClassification.Other, TraitImages.HeatTherapy),
        new Buff("Overheat", OverheatBuff, Source.Holosmith, BuffClassification.Other, SkillImages.Overheat),
        new Buff("Photon Barrier", PhotonBarrierBuff, Source.Holosmith, BuffClassification.Defensive, SkillImages.PhotonWall),
        // heat buffs only present in forge
        new Buff("0-50 Heat (Photon Forge)", PhotonForgeAbove0Heat, Source.Holosmith, BuffClassification.Other, TraitImages.HeatLvl1),
        new Buff("50-100 Heat (Photon Forge)", PhotonForgeAbove50Heat, Source.Holosmith, BuffClassification.Other, TraitImages.HeatLvl2),
        new Buff("100-150 Heat (Photon Forge)", PhotonForgeAbove100Heat, Source.Holosmith, BuffClassification.Other, TraitImages.HeatLvl3),
    ];
}
