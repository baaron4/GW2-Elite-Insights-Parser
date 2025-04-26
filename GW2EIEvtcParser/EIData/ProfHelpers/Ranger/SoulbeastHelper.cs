using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.DamageModifierIDs;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData;

internal static class SoulbeastHelper
{
    internal static readonly List<InstantCastFinder> InstantCastFinder =
    [
        // Beastmode
        new BuffGainCastFinder(EnterBeastMode, Stout),
        new BuffLossCastFinder(ExitBeastMode, Stout),
        new BuffGainCastFinder(EnterBeastMode, Deadly),
        new BuffLossCastFinder(ExitBeastMode, Deadly),
        new BuffGainCastFinder(EnterBeastMode, Versatile),
        new BuffLossCastFinder(ExitBeastMode, Versatile),
        new BuffGainCastFinder(EnterBeastMode, Ferocious),
        new BuffLossCastFinder(ExitBeastMode, Ferocious),
        new BuffGainCastFinder(EnterBeastMode, Supportive),
        new BuffLossCastFinder(ExitBeastMode, Supportive),
        // Stances
        new BuffGiveCastFinder(DolyakStanceSkill, DolyakStanceBuff),
        new BuffGiveCastFinder(MoaStanceSkill, MoaStanceBuff),
        new BuffGiveCastFinder(VultureStanceSkill, VultureStanceBuff),
        //
        new BuffGainCastFinder(SharpenSpinesBeastmode, SharpenSpinesBuff),
        new EffectCastFinder(EternalBondSkill, EffectGUIDs.SoulbeastEternalBond)
            .UsingSrcSpecChecker(Spec.Soulbeast)
            .WithBuilds(GW2Builds.October2022Balance),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> OutgoingDamageModifiers =
    [
        // Twice as Vicious
        new BuffOnActorDamageModifier(Mod_TwiceAsVicious, TwiceAsVicious, "Twice as Vicious", "5% (4s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, TraitImages.TwiceAsVicious, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        new BuffOnActorDamageModifier(Mod_TwiceAsVicious, TwiceAsVicious, "Twice as Vicious", "5% (10s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, TraitImages.TwiceAsVicious, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.July2019Balance, GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_TwiceAsVicious, TwiceAsVicious, "Twice as Vicious", "10% (10s) after disabling foe", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, TraitImages.TwiceAsVicious, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.February2020Balance),
        new BuffOnActorDamageModifier(Mod_TwiceAsVicious, TwiceAsVicious, "Twice as Vicious", "5% (10s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, TraitImages.TwiceAsVicious, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.February2020Balance),
        // Furious Strength
        new BuffOnActorDamageModifier(Mod_FuriousStrength, Fury, "Furious Strength", "7% under fury", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByStack, TraitImages.FuriousStrength, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_FuriousStrength, Fury, "Furious Strength", "7% under fury", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByStack, TraitImages.FuriousStrength, DamageModifierMode.sPvPWvW)
            .WithBuilds(GW2Builds.May2021Balance),
        new BuffOnActorDamageModifier(Mod_FuriousStrength, Fury, "Furious Strength", "10% under fury", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByStack, TraitImages.FuriousStrength, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.May2021Balance, GW2Builds.May2021BalanceHotFix),
        new BuffOnActorDamageModifier(Mod_FuriousStrength, Fury, "Furious Strength", "15% under fury", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByStack, TraitImages.FuriousStrength, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.May2021BalanceHotFix, GW2Builds.September2023Balance),
        new BuffOnActorDamageModifier(Mod_FuriousStrength, Fury, "Furious Strength", "10% under fury", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByStack, TraitImages.FuriousStrength, DamageModifierMode.PvE)
            .WithBuilds(GW2Builds.September2023Balance),
        // Loud Whistle
        new BuffOnActorDamageModifier(Mod_LoudWhistle, [Stout, Deadly, Ferocious, Supportive, Versatile], "Loud Whistle", "10% while merged and hp >=90%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByPresence, TraitImages.LoudWhistle, DamageModifierMode.All)
            .UsingChecker((x,log) => x.IsOverNinety)
            .WithBuilds(GW2Builds.May2018Balance),
        // Oppressive Superiority
        new DamageLogDamageModifier(Mod_OppressiveSuperiority, "Oppressive Superiority", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Soulbeast, TraitImages.OppressiveSuperiority, SelfHigherHPChecker, DamageModifierMode.All)
            .UsingApproximate(true),
        // One Wolf Pack
        new SkillDamageModifier(Mod_OneWolfPack,"One Wolf Pack", "per hit (max. once every 0.25s)", OneWolfPackDamage, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, SkillImages.OneWolfPack, DamageModifierMode.All),
    ];

    internal static readonly IReadOnlyList<DamageModifierDescriptor> IncomingDamageModifiers =
    [
        // Second Skin
        new BuffOnActorDamageModifier(Mod_SecondSkin, Protection, "Second Skin", "-33% under protection", DamageSource.Incoming, -33.0, DamageType.Condition, DamageType.All, Source.Soulbeast, ByPresence, TraitImages.SecondSkin, DamageModifierMode.All)
            .WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
        new BuffOnActorDamageModifier(Mod_SecondSkin, Protection, "Second Skin", "-33% under protection", DamageSource.Incoming, -33.0, DamageType.Condition, DamageType.All, Source.Soulbeast, ByPresence, TraitImages.SecondSkin, DamageModifierMode.PvEWvW)
            .WithBuilds(GW2Builds.July2019Balance),
        new BuffOnActorDamageModifier(Mod_SecondSkin, Protection, "Second Skin", "-20% under protection", DamageSource.Incoming, -20.0, DamageType.Condition, DamageType.All, Source.Soulbeast, ByPresence, TraitImages.SecondSkin, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.July2019Balance, GW2Builds.October2024Balance),
        new BuffOnActorDamageModifier(Mod_SecondSkin, Protection, "Second Skin", "-25% under protection", DamageSource.Incoming, -25.0, DamageType.Condition, DamageType.All, Source.Soulbeast, ByPresence, TraitImages.SecondSkin, DamageModifierMode.sPvP)
            .WithBuilds(GW2Builds.October2024Balance),
        // Dolyak Stance
        new BuffOnActorDamageModifier(Mod_DolyakStance, DolyakStanceBuff, "Dolyak Stance", "-33%", DamageSource.Incoming, -33.0, DamageType.StrikeAndCondition, DamageType.All, Source.Common, ByPresence, SkillImages.DolyakStance, DamageModifierMode.All)
            .WithBuilds(GW2Builds.December2018Balance),
    ];

    internal static readonly IReadOnlyList<Buff> Buffs =
    [
        new Buff("Dolyak Stance", DolyakStanceBuff, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Defensive, SkillImages.DolyakStance),
        new Buff("Griffon Stance", GriffonStance, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Support, SkillImages.GriffonStance),
        new Buff("Moa Stance", MoaStanceBuff, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Support, SkillImages.MoaStance),
        new Buff("Vulture Stance", VultureStanceBuff, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Offensive, SkillImages.VultureStance),
        new Buff("Bear Stance", BearStance, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Defensive, SkillImages.BearStance),
        new Buff("One Wolf Pack", OneWolfPackBuff, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Offensive, SkillImages.OneWolfPack),
        new Buff("Deadly", Deadly, Source.Soulbeast, BuffClassification.Other, TraitImages.DeadlyArchetype),
        new Buff("Ferocious", Ferocious, Source.Soulbeast, BuffClassification.Other, TraitImages.FerociousArchetype),
        new Buff("Supportive", Supportive, Source.Soulbeast, BuffClassification.Other, TraitImages.SupportiveArchetype),
        new Buff("Versatile", Versatile, Source.Soulbeast, BuffClassification.Other, TraitImages.VersatileArchetype),
        new Buff("Stout", Stout, Source.Soulbeast, BuffClassification.Other, TraitImages.StoutArchetype),
        new Buff("Unstoppable Union", UnstoppableUnion, Source.Soulbeast, BuffClassification.Other, TraitImages.UnstoppableUnion),
        new Buff("Twice as Vicious", TwiceAsVicious, Source.Soulbeast, BuffClassification.Other, TraitImages.TwiceAsVicious),
        new Buff("Unflinching Fortitude", UnflinchingFortitudeBuff, Source.Soulbeast, BuffClassification.Defensive, SkillImages.UnflinchingFortitude),
        new Buff("Defy Pain", DefyPainSoulbeastBuff, Source.Soulbeast, BuffClassification.Defensive, SkillImages.DefyPain),
    ];

}
