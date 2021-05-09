using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class SoulbeastHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            // Stout
            new BuffGainCastFinder(42944,40272,EIData.InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(43014,40272,EIData.InstantCastFinder.DefaultICD), // Leave Beastmode
            // Deadly
            new BuffGainCastFinder(42944,44932,EIData.InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(43014,44932,EIData.InstantCastFinder.DefaultICD), // Leave Beastmode
            // Versatile
            new BuffGainCastFinder(42944,44693,EIData.InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(43014,44693,EIData.InstantCastFinder.DefaultICD), // Leave Beastmode
            // Ferocious
            new BuffGainCastFinder(42944,41720,EIData.InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(43014,41720,EIData.InstantCastFinder.DefaultICD), // Leave Beastmode
            // Supportive
            new BuffGainCastFinder(42944,40069,EIData.InstantCastFinder.DefaultICD), // Beastmode
            new BuffLossCastFinder(43014,40069,EIData.InstantCastFinder.DefaultICD), // Leave Beastmode
            // 
            new BuffGiveCastFinder(45789,41815,EIData.InstantCastFinder.DefaultICD), // Dolyak Stance
            new BuffGiveCastFinder(45970,45038,EIData.InstantCastFinder.DefaultICD), // Moa Stance
            new BuffGiveCastFinder(40498,44651,EIData.InstantCastFinder.DefaultICD), // Vulture Stance
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(45600, "Twice as Vicious", "5% (4s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png",0 ,97950, DamageModifierMode.PvE),
            new BuffDamageModifier(45600, "Twice as Vicious", "5% (10s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png", 97950, 102321, DamageModifierMode.PvE),
            new BuffDamageModifier(45600, "Twice as Vicious", "10% (10s) after disabling foe", DamageSource.NoPets, 10.0, DamageType.All, DamageType.All, Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png", 102321, ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(45600, "Twice as Vicious", "5% (10s) after disabling foe", DamageSource.NoPets, 5.0, DamageType.All, DamageType.All, Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png", 102321, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifier(725, "Furious Strength", "7% under fury", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, Source.Soulbeast, ByStack, "https://wiki.guildwars2.com/images/c/ca/Furious_Strength.png", 0, 114788, DamageModifierMode.All),
            new BuffDamageModifier(725, "Furious Strength", "10% under fury", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, Source.Soulbeast, ByStack, "https://wiki.guildwars2.com/images/c/ca/Furious_Strength.png", 114788, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(new long[] { 40272, 44932, 44693, 41720, 40069}, "Loud Whistle", "10% while merged and hp > 90%", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, Source.Soulbeast, ByPresence, "https://wiki.guildwars2.com/images/b/b6/Loud_Whistle.png", 88541, ulong.MaxValue, DamageModifierMode.All, (x,log) => x.IsOverNinety),
            new DamageLogApproximateDamageModifier("Oppressive Superiority", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, Source.Soulbeast, "https://wiki.guildwars2.com/images/f/fc/Oppressive_Superiority.png", (x,log) =>
            {
                var selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                var dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, DamageModifierMode.All ),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Dolyak Stance",41815, Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/7/71/Dolyak_Stance.png"),
                new Buff("Griffon Stance",46280, Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/9/98/Griffon_Stance.png"),
                new Buff("Moa Stance",45038, Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.SupportBuffTable, "https://wiki.guildwars2.com/images/6/66/Moa_Stance.png"),
                new Buff("Vulture Stance",44651, Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/8/8f/Vulture_Stance.png"),
                new Buff("Bear Stance",40045, Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.DefensiveBuffTable, "https://wiki.guildwars2.com/images/f/f0/Bear_Stance.png"),
                new Buff("One Wolf Pack",44139, Source.Soulbeast, BuffStackType.Queue, 25, BuffNature.OffensiveBuffTable, "https://wiki.guildwars2.com/images/3/3b/One_Wolf_Pack.png"),
                new Buff("Deadly",44932, Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/94/Deadly_%28Archetype%29.png"),
                new Buff("Ferocious",41720, Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e9/Ferocious_%28Archetype%29.png"),
                new Buff("Supportive",40069, Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/32/Supportive_%28Archetype%29.png"),
                new Buff("Versatile",44693, Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/bb/Versatile_%28Archetype%29.png"),
                new Buff("Stout",40272, Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/80/Stout_%28Archetype%29.png"),
                new Buff("Unstoppable Union",44439, Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b2/Unstoppable_Union.png"),
                new Buff("Twice as Vicious",45600, Source.Soulbeast, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/0/00/Twice_as_Vicious.png"),
        };

    }
}
