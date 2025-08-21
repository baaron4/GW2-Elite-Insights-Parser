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
        new DamageLogDamageModifier(Mod_ElementalBalanceOutgoing5, "Elemental balance (Outgoing)", "5% if hp < 50%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) < 50.0, DamageModifierMode.All)
            .UsingApproximate(),
        new DamageLogDamageModifier(Mod_ElementalBalanceOutgoing10, "Elemental balance (Outgoing)", "10% if hp >= 50%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 50.0, DamageModifierMode.All)
            .UsingApproximate(),
        // Familiar's Prowess
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFox, FamiliarsProwessFox, "Familiar's Prowess (Fox)", "10% condition damage after familiar skill", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.All),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessHare, FamiliarsProwessHare, "Familiar's Prowess (Hare)", "10% strike damage after familiar skill", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.All),
        // Zap
        new BuffOnFoeDamageModifier(Mod_Zap, ZapBuffPlayerToTarget, "Zap", "7% crit damage", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, SkillImages.Zap, DamageModifierMode.All)
            .UsingEarlyExit((actor, log) => !actor.GetBuffStatus(log, ZapBuffTargetToPlayer).Any(x => x.Value > 0))
            .UsingChecker((hde, log) =>
            {
                if (hde.HasCrit)
                {
                    var src = log.FindActor(hde.From);
                    var dst = log.FindActor(hde.To);
                    return dst.HasBuff(log, src, ZapBuffPlayerToTarget, hde.Time);
                }
                return false;
            })
            .UsingApproximate(),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Elemental Balance
        new DamageLogDamageModifier(Mod_ElementalBalanceIncoming5, "Elemental balance (Incoming)", "-5% if hp > 50%", DamageSource.Incoming, -5.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) > 50.0, DamageModifierMode.All)
            .UsingApproximate(),
        new DamageLogDamageModifier(Mod_ElementalBalanceIncoming10, "Elemental balance (Incoming)", "-10% if hp <= 50%", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) <= 50.0, DamageModifierMode.All)
            .UsingApproximate(),
        // Familiar's Prowess
        // TODO Verify strike and condi if accurate
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessToad, FamiliarsProwessToad, "Familiar's Prowess (Toad)", "-15% strike and condi after familiar skill", DamageSource.Incoming, -15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.All),
        // TODO Add Familiar's Focus https://wiki.guildwars2.com/wiki/Familiar%27s_Focus
    ];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        new Buff("Familiar's Prowess (Fox)", FamiliarsProwessFox, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Prowess (Otter)", FamiliarsProwessOtter, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Prowess (Hare)", FamiliarsProwessHare, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Prowess (Toad)", FamiliarsProwessToad, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Focus", FamiliarsFocus, Source.Evoker, BuffClassification.Other, TraitImages.FamiliarsFocus), // TODO Verify if there are more buffs on other elements
        new Buff("Evoker's Stone Spirit Aura (1)", EvokerStoneSpiritAura1, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (2)", EvokerStoneSpiritAura2, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (3)", EvokerStoneSpiritAura3, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (4)", EvokerStoneSpiritAura4, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (5)", EvokerStoneSpiritAura5, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Hare's Agility", HaresAgilityBuff, Source.Evoker, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Zap (To Target)", ZapBuffPlayerToTarget, Source.Evoker, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Other, SkillImages.Zap),
        new Buff("Zap (To Player)", ZapBuffTargetToPlayer, Source.Evoker, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Other, SkillImages.Zap),
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
