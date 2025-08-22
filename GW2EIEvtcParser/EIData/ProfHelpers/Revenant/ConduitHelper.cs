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
        // TODO Verify game mode splits https://wiki.guildwars2.com/wiki/Shielding_Hands
        new BuffOnActorDamageModifier(Mod_ShieldingHands, ShieldingHandsBuff, "Shielding Hands", "-75% strike and condition damage", DamageSource.Incoming, -75, DamageType.StrikeAndCondition, DamageType.All, Source.Conduit, ByPresence, SkillImages.ShieldingHands, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Legendary Entity Stance", LegendaryEntityStanceBuff, Source.Conduit, BuffClassification.Other, SkillImages.LegendaryEntityStance),
        new Buff("Form of the Assassin", FormOfTheAssassin, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheAssassin),
        new Buff("Form of the Dervish (1)", FormOfTheDervishBuff1, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheDervish),
        new Buff("Form of the Dervish (2)", FormOfTheDervishBuff2, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheDervish),
        new Buff("Form of the Mesmer (1)", FormOfTheMesmer1, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheMesmer),
        new Buff("Form of the Mesmer (2)", FormOfTheMesmer2, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheMesmer),
        new Buff("Form of the Monk", FormOfTheMonk, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheMonk),
        new Buff("Form of the Warrior", FormOfTheWarrior, Source.Conduit, BuffClassification.Other, BuffImages.FormOfTheWarrior),
        new Buff("Shielding Hands", ShieldingHandsBuff, Source.Conduit, BuffClassification.Other, SkillImages.ShieldingHands),
    ];

    public static bool IsLegendSwap(long id)
    {
        return LegendaryEntityStanceSkill == id;
    }
}
