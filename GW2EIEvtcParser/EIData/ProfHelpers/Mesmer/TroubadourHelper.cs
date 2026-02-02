using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class TroubadourHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        new EffectCastFinder(TaleOfTheSoulkeeper, EffectGUIDs.TroubadourTaleOfTheSoulkeeper),
        new EffectCastFinder(TaleOfTheValiantMarshal, EffectGUIDs.TroubadourTaleOfTheValiantMarshal),
        new EffectCastFinder(TaleOfTheHonorableRogue, EffectGUIDs.TroubadourTaleOfTheHonorableRogue),
        new EffectCastFinder(SyncopateDelayedWave, EffectGUIDs.TroubadourSyncopateDelayedWave1)
            .UsingSecondaryEffectSameSrcChecker(EffectGUIDs.TroubadourSyncopateDelayedWave2)
            .UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Unconditional),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Altered Chord
        new BuffOnActorDamageModifier(Mod_AlteredChord, AlteredChord, "Altered Chord", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Troubadour, ByPresence, TraitImages.AlteredChord, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_AlteredChord, AlteredChord, "Altered Chord", "15%", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Troubadour, ByPresence, TraitImages.AlteredChord, DamageModifierMode.sPvPWvW),
        
        // Lute Playing
        new BuffOnActorDamageModifier(Mod_LutePlaying, LutePlaying, "Lute Playing", "4%", DamageSource.NoPets, 4.0, DamageType.StrikeAndCondition, DamageType.All, Source.Troubadour, ByPresence, SkillImages.LivelyLute, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_LutePlaying, LutePlaying, "Lute Playing", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Troubadour, ByPresence, SkillImages.LivelyLute, DamageModifierMode.All)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        
        // Harmonize
        new BuffOnActorDamageModifier(Mod_Harmonize, [LutePlaying, FlutePlaying, DrumPlaying, HarpPlaying], "Harmonize", "4% per instrument playing", DamageSource.NoPets, 4.0, DamageType.Strike, DamageType.All, Source.Troubadour, ByMultiPresence, TraitImages.Harmonize, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        
        // Shredding
        new BuffOnActorDamageModifier(Mod_Shredding, LutePlaying, "Shredding", "20%", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Troubadour, ByPresence, TraitImages.Shredding, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_Shredding, LutePlaying, "Shredding (Replaces Lute Playing)", "25%", DamageSource.NoPets, 25.0, DamageType.StrikeAndCondition, DamageType.All, Source.Troubadour, ByPresence, TraitImages.Shredding, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_Shredding, LutePlaying, "Shredding (Replaces Lute Playing)", "20%", DamageSource.NoPets, 20.0, DamageType.StrikeAndCondition, DamageType.All, Source.Troubadour, ByPresence, TraitImages.Shredding, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        // Symphonic resonance Lute
        new BuffOnActorDamageModifier(Mod_SymphonicResonanceLute, LutePlaying, "Symphonic Resonance (Lute Playing)", "25%", DamageSource.NoPets, 25.0, DamageType.StrikeAndCondition, DamageType.All, Source.Troubadour, ByPresence, TraitImages.SymphonicResonance, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        new BuffOnActorDamageModifier(Mod_LoveSong, HarpPlaying, "Love Song", "-10%", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Troubadour, ByPresence, TraitImages.LoveSong, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_LoveSong, HarpPlaying, "Love Song", "-7%", DamageSource.Incoming, -7.0, DamageType.Strike, DamageType.All, Source.Troubadour, ByPresence, TraitImages.LoveSong, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.OctoberVoERelease),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        new Buff("Harp Playing", HarpPlaying, Source.Troubadour, BuffClassification.Other, SkillImages.HarmoniousHarp),
        new Buff("Drum Playing", DrumPlaying, Source.Troubadour, BuffClassification.Other, SkillImages.DeafeningDrum),
        new Buff("Flute Playing", FlutePlaying, Source.Troubadour, BuffClassification.Other, SkillImages.FlusteringFlute),
        new Buff("Scion's Reprieve", ScionsReprieve, Source.Troubadour, BuffClassification.Other, SkillImages.TaleOfTheSecondScion),
        new Buff("Lute Playing", LutePlaying, Source.Troubadour, BuffClassification.Other, SkillImages.LivelyLute),
        new Buff("Altered Chord", AlteredChord, Source.Troubadour, BuffClassification.Other, TraitImages.AlteredChord),
    ];
}
