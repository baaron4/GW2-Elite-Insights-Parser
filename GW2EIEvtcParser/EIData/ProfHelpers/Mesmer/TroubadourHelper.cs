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
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Altered Chord
        // TODO Verify if only strike damage or also condition
        new BuffOnActorDamageModifier(Mod_AlteredChord, AlteredChord, "Altered Chord", "25%", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Troubadour, ByPresence, TraitImages.AlteredChord, DamageModifierMode.All),
        // Lute Playing
        new BuffOnActorDamageModifier(Mod_LutePlaying, LutePlaying, "Lute Playing", "4% strike and condition damage", DamageSource.NoPets, 4.0, DamageType.StrikeAndCondition, DamageType.All, Source.Troubadour, ByPresence, TraitImages.ReverberatingLute, DamageModifierMode.All),
        // Harmonize
        // TODO Verify if only strike damage or also condition
        new BuffOnActorDamageModifier(Mod_Harmonize, [LutePlaying, FlutePlaying, DrumPlaying, HarpPlaying], "Harmonize", "4% per instrument playing", DamageSource.NoPets, 4.0, DamageType.Strike, DamageType.All, Source.Troubadour, ByMultiPresence, TraitImages.Harmonize, DamageModifierMode.All),
        // Shredding
        new BuffOnActorDamageModifier(Mod_Shredding, LutePlaying, "Shredding", "20% strike damage", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Troubadour, ByPresence, TraitImages.Shredding, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = [];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        new Buff("Harp Playing", HarpPlaying, Source.Troubadour, BuffClassification.Other, SkillImages.HarmoniousHarp),
        new Buff("Drum Playing", DrumPlaying, Source.Troubadour, BuffClassification.Other, SkillImages.DeafeningDrum),
        new Buff("Flute Playing", FlutePlaying, Source.Troubadour, BuffClassification.Other, SkillImages.FlusteringFlute),
        new Buff("Scion's Reprieve", ScionsReprieve, Source.Troubadour, BuffClassification.Other, SkillImages.TaleOfTheSecondScion),
        new Buff("Lute Playing", LutePlaying, Source.Troubadour, BuffClassification.Other, TraitImages.ReverberatingLute),
        new Buff("Altered Chord", AlteredChord, Source.Troubadour, BuffClassification.Other, TraitImages.AlteredChord),
    ];
}
