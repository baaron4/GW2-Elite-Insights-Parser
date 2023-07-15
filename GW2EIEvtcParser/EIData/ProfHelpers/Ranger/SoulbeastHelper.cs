using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class SoulbeastHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            // Stout
            new BuffGainCastFinder(EnterBeastMode, Stout), // Beastmode
            new BuffLossCastFinder(ExitBeastMode, Stout), // Leave Beastmode
            // Deadly
            new BuffGainCastFinder(EnterBeastMode, Deadly), // Beastmode
            new BuffLossCastFinder(ExitBeastMode, Deadly), // Leave Beastmode
            // Versatile
            new BuffGainCastFinder(EnterBeastMode, Versatile), // Beastmode
            new BuffLossCastFinder(ExitBeastMode, Versatile), // Leave Beastmode
            // Ferocious
            new BuffGainCastFinder(EnterBeastMode, Ferocious), // Beastmode
            new BuffLossCastFinder(ExitBeastMode, Ferocious), // Leave Beastmode
            // Supportive
            new BuffGainCastFinder(EnterBeastMode, Supportive), // Beastmode
            new BuffLossCastFinder(ExitBeastMode, Supportive), // Leave Beastmode
            // 
            new BuffGiveCastFinder(DolyakStanceSkill, DolyakStanceEffect), // Dolyak Stance
            new BuffGiveCastFinder(MoaStanceSkill, MoaStanceEffect), // Moa Stance
            new BuffGiveCastFinder(VultureStanceSkill, VultureStanceEffect), // Vulture Stance
            //
            new BuffGainCastFinder(SharpenSpinesBeastmode, SharpenSpinesBuff), // Beastmode Sharpen Spines
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(TwiceAsVicious, "Twice as Vicious", "5% (4s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, BuffImages.TwiceAsVicious, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2019Balance),
            new BuffDamageModifier(TwiceAsVicious, "Twice as Vicious", "5% (10s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, BuffImages.TwiceAsVicious, DamageModifierMode.PvE).WithBuilds(GW2Builds.July2019Balance, GW2Builds.February2020Balance),
            new BuffDamageModifier(TwiceAsVicious, "Twice as Vicious", "10% (10s) after disabling foe", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, BuffImages.TwiceAsVicious, DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance),
            new BuffDamageModifier(TwiceAsVicious, "Twice as Vicious", "5% (10s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, BuffImages.TwiceAsVicious, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.February2020Balance),
            new BuffDamageModifier(Fury, "Furious Strength", "7% under fury", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByStack, BuffImages.FuriousStrength, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffDamageModifier(Fury, "Furious Strength", "10% under fury", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByStack, BuffImages.FuriousStrength, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance, GW2Builds.May2021BalanceHotFix),
            new BuffDamageModifier(Fury, "Furious Strength", "15% under fury", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByStack, BuffImages.FuriousStrength, DamageModifierMode.All).WithBuilds(GW2Builds.May2021BalanceHotFix),
            new BuffDamageModifier(new long[] { Stout, Deadly, Ferocious, Supportive, Versatile }, "Loud Whistle", "10% while merged and hp >=90%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByPresence, BuffImages.LoudWhistle, DamageModifierMode.All).UsingChecker((x,log) => x.IsOverNinety).WithBuilds(GW2Builds.May2018Balance),
            new DamageLogDamageModifier("Oppressive Superiority", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Soulbeast, BuffImages.OppressiveSuperiority, (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, DamageModifierMode.All ).UsingApproximate(true),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Dolyak Stance", DolyakStanceEffect, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Defensive, BuffImages.DolyakStance),
            new Buff("Griffon Stance", GriffonStance, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Support, BuffImages.GriffonStance),
            new Buff("Moa Stance", MoaStanceEffect, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Support, BuffImages.MoaStance),
            new Buff("Vulture Stance", VultureStanceEffect, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Offensive, BuffImages.VultureStance),
            new Buff("Bear Stance", BearStance, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Defensive, BuffImages.BearStance),
            new Buff("One Wolf Pack", OneWolfPackEffect, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Offensive, BuffImages.OneWolfPack),
            new Buff("Deadly", Deadly, Source.Soulbeast, BuffClassification.Other, BuffImages.DeadlyArchetype),
            new Buff("Ferocious", Ferocious, Source.Soulbeast, BuffClassification.Other, BuffImages.FerociousArchetype),
            new Buff("Supportive", Supportive, Source.Soulbeast, BuffClassification.Other, BuffImages.SupportiveArchetype),
            new Buff("Versatile", Versatile, Source.Soulbeast, BuffClassification.Other, BuffImages.VersatileArchetype),
            new Buff("Stout", Stout, Source.Soulbeast, BuffClassification.Other, BuffImages.StoutArchetype),
            new Buff("Unstoppable Union", UnstoppableUnion, Source.Soulbeast, BuffClassification.Other, BuffImages.UnstoppableUnion),
            new Buff("Twice as Vicious", TwiceAsVicious, Source.Soulbeast, BuffClassification.Other, BuffImages.TwiceAsVicious),
            new Buff("Unflinching Fortitude", UnflinchingFortitudeBuff, Source.Soulbeast, BuffClassification.Defensive, BuffImages.UnflinchingFortitude),
            new Buff("Defy Pain", DefyPainSoulbeastBuff, Source.Soulbeast, BuffClassification.Defensive, BuffImages.DefyPain),
            new Buff("Sharpen Spines", SharpenSpinesBuff, Source.Soulbeast, BuffStackType.Stacking, 25, BuffClassification.Offensive, BuffImages.SharpenSpines),
        };

    }
}
