using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class ConduitHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder = 
    [
        new BuffGainCastFinder(LegendaryEntityStanceSkill, LegendaryEntityStanceBuff),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = [];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Shielding Hands
        new BuffOnActorDamageModifier(Mod_ShieldingHands, ShieldingHandsBuff, "Shielding Hands", "-75%", DamageSource.Incoming, -75, DamageType.StrikeAndCondition, DamageType.All, Source.Conduit, ByPresence, SkillImages.ShieldingHands, DamageModifierMode.All),
    ];

    // TODO: check if new buffs with the rework
    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Legendary Entity Stance", LegendaryEntityStanceBuff, Source.Conduit, BuffClassification.Other, SkillImages.LegendaryEntityStance),
        new Buff("Form of the Dervish (Razah Active)", FormOfTheDervishRazahActiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheDervish),
        new Buff("Form of the Dervish (Razah Passive)", FormOfTheDervishRazahPassiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheDervish),
        new Buff("Form of the Assassin (Shiro Active)", FormOfTheAssassinShiroActiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheAssassin),
        new Buff("Form of the Assassin (Shiro Passive)", FormOfTheAssassinShiroPassiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheAssassin),
        new Buff("Form of the Mesmer (Mallyx Active)", FormOfTheMesmerMallyxActiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheMesmer),
        new Buff("Form of the Mesmer (Mallyx Passive)", FormOfTheMesmerMallyxPassiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheMesmer),
        new Buff("Form of the Monk (Ventari Active)", FormOfTheMonkVentariActiveBuff, Source.Conduit, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.FormOfTheMonk)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new Buff("Form of the Monk (Ventari Passive)", FormOfTheMonkVentariPassiveBuff, Source.Conduit, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.FormOfTheMonk)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new Buff("Form of the Monk (Ventari Active)", FormOfTheMonkVentariActiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheMonk)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new Buff("Form of the Monk (Ventari Passive)", FormOfTheMonkVentariPassiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheMonk)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new Buff("Form of the Warrior (Jalis Active)", FormOfTheWarriorJalisActiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheWarrior),
        new Buff("Form of the Warrior (Jalis Passive)", FormOfTheWarriorJalisPassiveBuff, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheWarrior),
        new Buff("Shielding Hands", ShieldingHandsBuff, Source.Conduit, BuffClassification.Other, SkillImages.ShieldingHands),
        new Buff("Lingering Determination", LingeringDetermination, Source.Conduit, BuffClassification.Other, TraitImages.LingeringDetermination),
    ];

    public static bool IsLegendSwap(long id)
    {
        return LegendaryEntityStanceSkill == id;
    }
}
