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
        new MinionCastCastFinder(IgnitePlayerSkill, IgnitePetSkill),
        new MinionCastCastFinder(SplashPlayerSkill, SplashPetSkill),
        new MinionCastCastFinder(ZapPlayerSkill, ZapPetSkill),
        new MinionCastCastFinder(CalcifyPlayerSkill, CalcifyPetSkill),
        new EffectCastFinder(OttersCompassion, EffectGUIDs.EvokerOttersCompassion1)
            .UsingSecondaryEffectChecker(EffectGUIDs.EvokerOttersCompassion2),
    ];

    private static bool ZapChecker(HealthDamageEvent hde, ParsedEvtcLog log)
    {
        if (hde.HasCrit)
        {
            var src = log.FindActor(hde.From);
            var dst = log.FindActor(hde.To);
            return dst.HasBuff(log, src, ZapBuffPlayerToTarget, hde.Time);
        }
        return false;
    }

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers = 
    [
        // Elemental Balance
        new DamageLogDamageModifier(Mod_ElementalBalanceOutgoing5_Incoming10, "Elemental balance (Outgoing)", "5% if hp < 50%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) < 50.0, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease)
            .UsingApproximate(),
        new DamageLogDamageModifier(Mod_ElementalBalanceOutgoing10_Incoming5, "Elemental balance (Outgoing)", "10% if hp >= 50%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) >= 50.0, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease)
            .UsingApproximate(),
        // Familiar's Prowess (Fox)
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFox, FamiliarsProwessFox, "Familiar's Prowess (Fox)", "10% after familiar skill", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFox, FamiliarsProwessFox, "Familiar's Prowess (Fox)", "7% after familiar skill", DamageSource.NoPets, 7.0, DamageType.Condition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFox, FamiliarsProwessFox, "Familiar's Prowess (Fox)", "10% after familiar skill", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        // Familiar's Prowess + Familiar's Focus (Fox)
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFocusFox, FamiliarsProwessFox, "Familiar's Prowess + Focus (Fox)", "25% after familiar skill (Familiar's Focus)", DamageSource.NoPets, 25.0, DamageType.Condition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFocusFox, FamiliarsProwessFox, "Familiar's Prowess + Focus (Fox)", "20% after familiar skill (Familiar's Focus)", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFocusFox, FamiliarsProwessFox, "Familiar's Prowess + Focus (Fox)", "20% after familiar skill (Familiar's Focus)", DamageSource.NoPets, 20.0, DamageType.Condition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.All)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        // Familiar's Prowess (Hare)
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessHare, FamiliarsProwessHare, "Familiar's Prowess (Hare)", "10% after familiar skill", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessHare, FamiliarsProwessHare, "Familiar's Prowess (Hare)", "7% after familiar skill", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessHare, FamiliarsProwessHare, "Familiar's Prowess (Hare)", "10% after familiar skill", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        // Familiar's Prowess + Familiar's Focus (Hare)
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFocusHare, FamiliarsProwessHare, "Familiar's Prowess + Focus (Hare)", "25% after familiar skill (Familiar's Focus)", DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFocusHare, FamiliarsProwessHare, "Familiar's Prowess + Focus (Hare)", "20% after familiar skill (Familiar's Focus)", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFocusHare, FamiliarsProwessHare, "Familiar's Prowess + Focus (Hare)", "20% after familiar skill (Familiar's Focus)", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.All)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        // Zap
        new BuffOnFoeDamageModifier(Mod_Zap, ZapBuffPlayerToTarget, "Zap", "7% crit damage", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, SkillImages.Zap, DamageModifierMode.PvE)
            .UsingEarlyExit((actor, log) => !actor.GetBuffStatus(log, ZapBuffTargetToPlayer).Any(x => x.Value > 0))
            .UsingChecker(ZapChecker)
            .UsingApproximate()
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease),
        new BuffOnFoeDamageModifier(Mod_Zap, ZapBuffPlayerToTarget, "Zap", "4% crit damage", DamageSource.NoPets, 4.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, SkillImages.Zap, DamageModifierMode.PvE)
            .UsingEarlyExit((actor, log) => !actor.GetBuffStatus(log, ZapBuffTargetToPlayer).Any(x => x.Value > 0))
            .UsingChecker(ZapChecker)
            .UsingApproximate()
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new BuffOnFoeDamageModifier(Mod_Zap, ZapBuffPlayerToTarget, "Zap", "5% crit damage", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, SkillImages.Zap, DamageModifierMode.sPvPWvW)
            .UsingEarlyExit((actor, log) => !actor.GetBuffStatus(log, ZapBuffTargetToPlayer).Any(x => x.Value > 0))
            .UsingChecker(ZapChecker)
            .UsingApproximate(),
        // Adept
        new BuffOnFoeDamageModifier(Mod_FieryMight, Burning, "Fiery Might", "5% against burning foes", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, TraitImages.FieryMight, DamageModifierMode.All),

    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers = 
    [
        // Elemental Balance
        new DamageLogDamageModifier(Mod_ElementalBalanceOutgoing10_Incoming5, "Elemental balance (Incoming)", "-5% if hp > 50%", DamageSource.Incoming, -5.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) > 50.0, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease)
            .UsingApproximate(),
        new DamageLogDamageModifier(Mod_ElementalBalanceOutgoing5_Incoming10, "Elemental balance (Incoming)", "-10% if hp <= 50%", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Evoker, TraitImages.ElementalBalance, (x, log) => x.From.GetCurrentHealthPercent(log, x.Time) <= 50.0, DamageModifierMode.All)
            .WithBuilds(GW2Builds.August2025VoEBeta, GW2Builds.OctoberVoERelease)
            .UsingApproximate(),
        // Familiar's Prowess (Toad)
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessToad, FamiliarsProwessToad, "Familiar's Prowess (Toad)", "-15% after familiar skill", DamageSource.Incoming, -15.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessToad, FamiliarsProwessToad, "Familiar's Prowess (Toad)", "-10% after familiar skill", DamageSource.Incoming, -10.0, DamageType.Strike, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.sPvPWvW),
        // Familiar's Prowess + Familiar's Focus (Toad)
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFocusToad, FamiliarsProwessToad, "Familiar's Prowess + Focus (Toad)", "-15% after familiar skill", DamageSource.Incoming, -15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.PvE),
        new BuffOnActorDamageModifier(Mod_FamiliarsProwessFocusToad, FamiliarsProwessToad, "Familiar's Prowess + Focus (Toad)", "-10% after familiar skill", DamageSource.Incoming, -10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Evoker, ByPresence, TraitImages.FamiliarsProwess, DamageModifierMode.sPvPWvW),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs = 
    [
        new Buff("Familiar's Prowess (Fox)", FamiliarsProwessFox, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Prowess (Otter)", FamiliarsProwessOtter, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Prowess (Hare)", FamiliarsProwessHare, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Prowess (Toad)", FamiliarsProwessToad, Source.Evoker, BuffStackType.Queue, 9, BuffClassification.Other, TraitImages.FamiliarsProwess),
        new Buff("Familiar's Focus", FamiliarsFocus, Source.Evoker, BuffClassification.Other, TraitImages.FamiliarsFocus),
        new Buff("Evoker's Stone Spirit Aura (1)", EvokerStoneSpiritAura1, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (2)", EvokerStoneSpiritAura2, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (3)", EvokerStoneSpiritAura3, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (4)", EvokerStoneSpiritAura4, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Evoker's Stone Spirit Aura (5)", EvokerStoneSpiritAura5, Source.Evoker, BuffClassification.Other, BuffImages.Unknown),
        new Buff("Electric Enchantment", ElectricEnchantmentBuff, Source.Evoker, BuffStackType.StackingConditionalLoss, 25, BuffClassification.Other, SkillImages.HaresAgility),
        new Buff("Zap (To Target)", ZapBuffPlayerToTarget, Source.Evoker, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Other, SkillImages.Zap),
        new Buff("Zap (To Player)", ZapBuffTargetToPlayer, Source.Evoker, BuffStackType.StackingUniquePerSrc, 999, BuffClassification.Other, SkillImages.Zap),
        new Buff("Toad Block", ToadBlock, Source.Evoker, BuffClassification.Other, SkillImages.ToadsFortitude),
        new Buff("Elemental Balance", ElementalBalanceBuff, Source.Evoker, BuffClassification.Other, TraitImages.ElementalBalance)
            .WithBuilds(GW2Builds.OctoberVoERelease),
        new Buff("Fox Passive", FoxPassive, Source.Evoker, BuffClassification.Other, SkillImages.Ignite),
        new Buff("Otter Passive", OtterPassive, Source.Evoker, BuffClassification.Other, SkillImages.Splash),
        new Buff("Hare Passive", HarePassive, Source.Evoker, BuffClassification.Other, SkillImages.Zap),
        new Buff("Toad Passive", ToadPassive, Source.Evoker, BuffClassification.Other, SkillImages.Calcify),
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
