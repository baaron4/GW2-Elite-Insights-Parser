using System.Collections.Generic;
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
            new BuffGainCastFinder(EnterBeastMode,Stout,EIData.InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(ExitBeastMode,Stout,EIData.InstantCastFinder.DefaultICD), // Leave Beastmode
            // Deadly
            new BuffGainCastFinder(EnterBeastMode,Deadly,EIData.InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(ExitBeastMode,Deadly,EIData.InstantCastFinder.DefaultICD), // Leave Beastmode
            // Versatile
            new BuffGainCastFinder(EnterBeastMode,Versatile,EIData.InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(ExitBeastMode,Versatile,EIData.InstantCastFinder.DefaultICD), // Leave Beastmode
            // Ferocious
            new BuffGainCastFinder(EnterBeastMode,Ferocious,EIData.InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(ExitBeastMode,Ferocious,EIData.InstantCastFinder.DefaultICD), // Leave Beastmode
            // Supportive
            new BuffGainCastFinder(EnterBeastMode,Supportive,EIData.InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(ExitBeastMode,Supportive,EIData.InstantCastFinder.DefaultICD), // Leave Beastmode
            // 
            new BuffGiveCastFinder(DolyakStanceSkill,DolyakStanceEffect,EIData.InstantCastFinder.DefaultICD), // Dolyak Stance
            new BuffGiveCastFinder(MoaStanceSkill,MoaStanceEffect,EIData.InstantCastFinder.DefaultICD), // Moa Stance
            new BuffGiveCastFinder(VultureStanceSkill,VultureStanceEffect,EIData.InstantCastFinder.DefaultICD), // Vulture Stance
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(TwiceAsVicious, "Twice as Vicious", "5% (4s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png",0 ,GW2Builds.July2019Balance, DamageModifierMode.PvE),
            new BuffDamageModifier(TwiceAsVicious, "Twice as Vicious", "5% (10s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png", GW2Builds.July2019Balance, GW2Builds.February2020Balance, DamageModifierMode.PvE),
            new BuffDamageModifier(TwiceAsVicious, "Twice as Vicious", "10% (10s) after disabling foe", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png", GW2Builds.February2020Balance, GW2Builds.EndOfLife, DamageModifierMode.PvE),
            new BuffDamageModifier(TwiceAsVicious, "Twice as Vicious", "5% (10s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png", GW2Builds.February2020Balance, GW2Builds.EndOfLife, DamageModifierMode.sPvPWvW),
            new BuffDamageModifier(Fury, "Furious Strength", "7% under fury", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByStack, "https://wiki.guildwars2.com/images/c/ca/Furious_Strength.png", 0, GW2Builds.May2021Balance, DamageModifierMode.All),
            new BuffDamageModifier(Fury, "Furious Strength", "10% under fury", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByStack, "https://wiki.guildwars2.com/images/c/ca/Furious_Strength.png", GW2Builds.May2021Balance, GW2Builds.May2021BalanceHotFix, DamageModifierMode.All),
            new BuffDamageModifier(Fury, "Furious Strength", "15% under fury", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByStack, "https://wiki.guildwars2.com/images/c/ca/Furious_Strength.png", GW2Builds.May2021BalanceHotFix, GW2Builds.EndOfLife, DamageModifierMode.All),
            new BuffDamageModifier(new long[] { Stout, Deadly, Ferocious, Supportive, Versatile}, "Loud Whistle", "10% while merged and hp > 90%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/b/b6/Loud_Whistle.png", GW2Builds.May2018Balance, GW2Builds.EndOfLife, DamageModifierMode.All, (x,log) => x.IsOverNinety),
            new DamageLogDamageModifier("Oppressive Superiority", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Soulbeast, "https://wiki.guildwars2.com/images/f/fc/Oppressive_Superiority.png", (x,log) =>
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
                new Buff("Dolyak Stance",DolyakStanceEffect, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/7/71/Dolyak_Stance.png"),
                new Buff("Griffon Stance",GriffonStance, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Support, "https://wiki.guildwars2.com/images/9/98/Griffon_Stance.png"),
                new Buff("Moa Stance",MoaStanceEffect, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Support, "https://wiki.guildwars2.com/images/6/66/Moa_Stance.png"),
                new Buff("Vulture Stance",VultureStanceEffect, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/8/8f/Vulture_Stance.png"),
                new Buff("Bear Stance",BearStance, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Defensive, "https://wiki.guildwars2.com/images/f/f0/Bear_Stance.png"),
                new Buff("One Wolf Pack",OneWolfPackEffect, Source.Soulbeast, BuffStackType.Queue, 25, BuffClassification.Offensive, "https://wiki.guildwars2.com/images/3/3b/One_Wolf_Pack.png"),
                new Buff("Deadly",Deadly, Source.Soulbeast, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/94/Deadly_%28Archetype%29.png"),
                new Buff("Ferocious",Ferocious, Source.Soulbeast, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e9/Ferocious_%28Archetype%29.png"),
                new Buff("Supportive",Supportive, Source.Soulbeast, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/32/Supportive_%28Archetype%29.png"),
                new Buff("Versatile",Versatile, Source.Soulbeast, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/bb/Versatile_%28Archetype%29.png"),
                new Buff("Stout",Stout, Source.Soulbeast, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/80/Stout_%28Archetype%29.png"),
                new Buff("Unstoppable Union",UnstoppableUnion, Source.Soulbeast, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b2/Unstoppable_Union.png"),
                new Buff("Twice as Vicious",TwiceAsVicious, Source.Soulbeast, BuffClassification.Other, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png"),
        };

    }
}
