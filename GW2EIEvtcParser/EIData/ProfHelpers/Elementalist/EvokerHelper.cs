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

internal static class EvokerHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder = 
    [
        new MinionCastCastFinder(IgniteSkill, IgniteDamage),
        new MinionCastCastFinder(SplashSkill, 999999), // TODO Find skill id casted by the pet
        new MinionCastCastFinder(ZapSkill, ZapDamage),
        new MinionCastCastFinder(CalcifySkill, 999999), // TODO Find skill id casted by the pet
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Elemental Balance
        // TODO Verify behaviour https://wiki.guildwars2.com/wiki/Elemental_Balance
        new DamageLogDamageModifier(Mod_ElementalBalanceOutgoing5, "Elemental balance", "5% if hp <= 50%", DamageSource.All, 5.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) <= 50.0, DamageModifierMode.All),
        new DamageLogDamageModifier(Mod_ElementalBalanceOutgoing10, "Elemental balance", "10% if hp >= 50%", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 50.0, DamageModifierMode.All),
        // Familiar's Prowess
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFox, FamiliarsProwessFox, "Familiar's Prowess (Fox)", "10% condi after familiar skill", DamageSource.All, 10.0, DamageType.Condition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessHare, FamiliarsProwessHare, "Familiar's Prowess (Hare)", "10% strike after familiar skill", DamageSource.All, 10.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Elemental Balance
        // TODO Verify behaviour https://wiki.guildwars2.com/wiki/Elemental_Balance
        new DamageLogDamageModifier(Mod_ElementalBalanceIncoming5, "Elemental balance", "-5% if hp >= 50%", DamageSource.Incoming, 5.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 50.0, DamageModifierMode.All),
        new DamageLogDamageModifier(Mod_ElementalBalanceIncoming10, "Elemental balance", "-10% if hp <= 50%", DamageSource.Incoming, 10.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) <= 50.0, DamageModifierMode.All),
        // Familiar's Prowess
        // TODO Verify strike and condi if accurate
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessToad, FamiliarsProwessToad, "Familiar's Prowess (Toad)", "-15% strike and condi after familiar skill", DamageSource.Incoming, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        new Buff("Familiar's Prowess (Fox)", FamiliarsProwessFox, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Prowess (Otter)", FamiliarsProwessOtter, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Prowess (Hare)", FamiliarsProwessHare, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Prowess (Toad)", FamiliarsProwessToad, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Focus", FamiliarsFocus, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (1)", EvokerStoneSpiritAura1, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (2)", EvokerStoneSpiritAura2, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (3)", EvokerStoneSpiritAura3, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (4)", EvokerStoneSpiritAura4, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (5)", EvokerStoneSpiritAura5, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Hare's Agility", HaresAgilityBuff, Source.Evoker, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Zap (1)", ZapBuff1, Source.Evoker, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Other, SkillImages.Zap),
        new Buff("Zap (2)", ZapBuff2, Source.Evoker, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Other, SkillImages.Zap),
    ];

    private static readonly HashSet<int> Minions = 
    [
        (int)MinionID.FireFox,
        (int)MinionID.WaterOtter,
        (int)MinionID.AirHare,
        (int)MinionID.EarthToad,
        (int)MinionID.ElementalProcessionFireFox,
        (int)MinionID.ElementalProcessionWaterOtter,
        (int)MinionID.ElementalProcessionAirHare,
        (int)MinionID.ElementalProcessionEarthToad,
    ];

    internal static bool IsKnownMinionID(int id)
    {
        return Minions.Contains(id);
    }

    internal static void AdjustMinionName(AgentItem minion)
    {
        switch (minion.ID)
        {
            case (int)MinionID.FireFox:
                minion.OverrideName("Fire Fox");
                break;
            case (int)MinionID.WaterOtter:
                minion.OverrideName("Water Otter");
                break;
            case (int)MinionID.AirHare:
                minion.OverrideName("Air Hare");
                break;
            case (int)MinionID.EarthToad:
                minion.OverrideName("Earth Toad");
                break;
            case (int)MinionID.ElementalProcessionFireFox:
                minion.OverrideName("Fire Fox (EP)");
                break;
            case (int)MinionID.ElementalProcessionWaterOtter:
                minion.OverrideName("Water Otter (EP)");
                break;
            case (int)MinionID.ElementalProcessionAirHare:
                minion.OverrideName("Air Hare (EP)");
                break;
            case (int)MinionID.ElementalProcessionEarthToad:
                minion.OverrideName("Earth Toad (EP)");
                break;
            default:
                break;
        }
    }
}
