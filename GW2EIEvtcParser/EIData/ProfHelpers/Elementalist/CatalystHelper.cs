using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class CatalystHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            //new BuffGainCastFinder(62982, 62726, EIData.InstantCastFinder.DefaultICD), // 62726
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(62931, "Flame Wheel", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, "https://wiki.guildwars2.com/images/f/f3/Flame_Wheel.png", 119939, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(62805, "Relentless Fire", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, "https://wiki.guildwars2.com/images/7/70/Relentless_Fire.png", 119939, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(62939, "Empowering Auras", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByStack, "https://wiki.guildwars2.com/images/4/44/Empowering_Auras.png", 119939, ulong.MaxValue, DamageModifierMode.All),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Flame Wheel", 62931, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/f3/Flame_Wheel.png"),
            new Buff("Icy Coil", 62984, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/8/87/Icy_Coil.png"),
            new Buff("Crescent Wind", 62707, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/3/3c/Crescent_Wind.png"),
            new Buff("Rocky Loop", 62768, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/3/31/Rocky_Loop.png"),
            new Buff("Relentless Fire", 62805, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/7/70/Relentless_Fire.png"),
            new Buff("Shattering Ice", 62686, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/6/63/Shattering_Ice.png"),
            new Buff("Invigorating Air", 62726, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/1/12/Invigorating_Air.png"),
            new Buff("Fortified Earth", 62858, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/8/84/Fortified_Earth.png"),
            new Buff("Elemental Celerity", 62915, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/a/a5/Elemental_Celerity.png"),
            new Buff("Elemental Empowerment", 62733, Source.Catalyst, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/e6/Elemental_Empowerment.png"),
            new Buff("Empowering Auras", 62939, Source.Catalyst, BuffStackType.StackingConditionalLoss, 5, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/4/44/Empowering_Auras.png"),
            new Buff("Hardened Auras", 62986, Source.Catalyst, BuffStackType.StackingConditionalLoss, 5, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/23/Hardened_Auras.png"),

        };
    }
}
