using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

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
        new Buff("Number of Notes", NumberOfNotes, Source.Troubadour, BuffStackType.Stacking, 3, BuffClassification.Other, TraitImages.NumberOfNotes),
        new Buff("Harp Playing", HarpPlaying, Source.Troubadour, BuffClassification.Other, SkillImages.HarmoniousHarp),
        new Buff("Drum Playing", DrumPlaying, Source.Troubadour, BuffClassification.Other, SkillImages.DeafeningDrum),
        new Buff("Flute Playing", FlutePlaying, Source.Troubadour, BuffClassification.Other, SkillImages.FlusteringFlute),
        new Buff("Scion's Reprieve", ScionsReprieve, Source.Troubadour, BuffClassification.Other, SkillImages.TaleOfTheSecondScion),
        new Buff("Lute Playing", LutePlaying, Source.Troubadour, BuffClassification.Other, SkillImages.LivelyLute),
        new Buff("Altered Chord", AlteredChord, Source.Troubadour, BuffClassification.Other, TraitImages.AlteredChord),
    ];

    private static readonly HashSet<int> _afterImages =
    [
        (int)MinionID.LivelyLuteAfterimage,
        (int)MinionID.DeafeningDrumAfterimage,
        (int)MinionID.FlusteringFluteAfterimage,
        (int)MinionID.HarmoniousHarpAfterimage,
    ];

    internal static bool IsKnownMinionID(int id)
    {
        return _afterImages.Contains(id);
    }

    public static List<BuffEvent> TransformTroubadourNoteStorage(CombatData combatData, AgentData agentData, SkillData skillData, EvtcVersionEvent evtcVersion)
    {
        var noteBuffEvents = new List<BuffEvent>();
        var noteSkill = skillData.Get(NumberOfNotes);
        // Buff Applies, each slot redirected under same custom buff
        uint buffStackID = 0;
        if (combatData.TryGetEffectEventsByGUIDs([EffectGUIDs.TroubadourNote1, EffectGUIDs.TroubadourNote2, EffectGUIDs.TroubadourNote3], out var notes))
        {
            foreach (var note in notes)
            {
                noteBuffEvents.Add(new BuffApplyEvent(note.Src, note.Src, note.Time, int.MaxValue, noteSkill, IFF.Friend, ++buffStackID, true));
            }
        }
        // First sequence of note applies, to handle initial state
        var firstNoteBuffEventsBySrc = noteBuffEvents.GroupBy(x => x.By).ToDictionary(x => x.Key, x => x.Take(3).ToList());

        // All instrument buff applies
        List<BuffApplyEvent> instrumentBuffApplies = [
            ..combatData.GetBuffApplyData(LutePlaying).OfType<BuffApplyEvent>(),
            ..combatData.GetBuffApplyData(FlutePlaying).OfType<BuffApplyEvent>(),
            ..combatData.GetBuffApplyData(DrumPlaying).OfType<BuffApplyEvent>(),
            ..combatData.GetBuffApplyData(HarpPlaying).OfType<BuffApplyEvent>(),
        ];
        instrumentBuffApplies.SortByTime();
        // Regroup by time with tolerance, to handle Tale of the August Queen casts
        var tol = 30f;
        var groupedBuffApplies = instrumentBuffApplies
            .GroupBy(x => x.To)
            .ToDictionary(x => x.Key, x => x
                .GroupBy(y => MathF.Round(y.Time / tol) * tol).
                ToDictionary(y => y.Key, y => y.ToList()
        ));
        foreach (var grouped in groupedBuffApplies)
        {
            var troub = grouped.Key;
            bool doInitial = true;
            foreach (var groupedInstrumentBuffApplies in grouped.Value)
            {
                // Keep only intrument buff applies after first aware, ignore the ones applied before log start as their notes couldn't have been consummed during the log
                var troubInstrumentBuffApplies = groupedInstrumentBuffApplies.Value
                    .Where(instrumentBuffApply => instrumentBuffApply.Time - (instrumentBuffApply.OriginalAppliedDuration - instrumentBuffApply.AppliedDuration) >= troub.FirstAware).ToList();
                // If more than 1, Tale of the August Queen trigger
                if (troubInstrumentBuffApplies.Count == 1)
                {
                    var instrumentBuffApply = troubInstrumentBuffApplies[0];
                    noteBuffEvents.Add(new BuffRemoveAllEvent(troub, troub, instrumentBuffApply.Time, int.MaxValue, noteSkill, IFF.Friend, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
                    if (doInitial)
                    {
                        doInitial = false;
                        // Handle buff initial as note creation effects will not be present in the log
                        var expectedNoteCount = (int)Math.Round(instrumentBuffApply.OriginalAppliedDuration / 5000.0) - 1;
                        var missingNotes = 0;
                        // Keep only applies before the very first within log instrument buff apply
                        if (firstNoteBuffEventsBySrc.TryGetValue(troub, out var firstNoteApplies) &&
                            firstNoteApplies.Where(x => x.Time < instrumentBuffApply.Time).ToList() is List<BuffEvent> restrictedFirstNoteApplies &&
                            restrictedFirstNoteApplies.Count != expectedNoteCount)
                        {
                            missingNotes = expectedNoteCount - restrictedFirstNoteApplies.Count;
                        }
                        else
                        {
                            missingNotes = expectedNoteCount;
                        }
                        for (var i = 0; i < missingNotes; i++)
                        {
                            noteBuffEvents.Add(new BuffApplyEvent(troub, troub, troub.FirstAware, int.MaxValue, noteSkill, IFF.Friend, ++buffStackID, true, true));
                        }
                    }
                }
            }
        }
        return noteBuffEvents;
    }

    internal static void AdjustMinionName(AgentItem minion)
    {
        switch (minion.ID)
        {
            case (int)MinionID.LivelyLuteAfterimage:
                minion.OverrideName("Lively Lute " + minion.Name);
                break;
            case (int)MinionID.DeafeningDrumAfterimage:
                minion.OverrideName("Deafening Drum " + minion.Name);
                break;
            case (int)MinionID.FlusteringFluteAfterimage:
                minion.OverrideName("Flustering Flute " + minion.Name);
                break;
            case (int)MinionID.HarmoniousHarpAfterimage:
                minion.OverrideName("Harmonious Harp " + minion.Name);
                break;
            default:
                break;
        }
    }
}
