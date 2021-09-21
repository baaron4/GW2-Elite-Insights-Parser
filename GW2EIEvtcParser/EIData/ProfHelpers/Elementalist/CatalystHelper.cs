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
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(-1, "Flame Wheel", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, "https://wiki.guildwars2.com/images/f/f3/Flame_Wheel.png", 119939, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(-1, "Relentless Fire", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, "https://wiki.guildwars2.com/images/7/70/Relentless_Fire.png", 119939, ulong.MaxValue, DamageModifierMode.All),
            new BuffDamageModifier(-1, "Empowering Auras", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByStack, "https://wiki.guildwars2.com/images/4/44/Empowering_Auras.png", 119939, ulong.MaxValue, DamageModifierMode.All),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Flame Wheel", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/f/f3/Flame_Wheel.png", 119939, ulong.MaxValue),
            new Buff("Icy Coil", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/8/87/Icy_Coil.png", 119939, ulong.MaxValue),
            new Buff("Crescent Wind", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/3/3c/Crescent_Wind.png", 119939, ulong.MaxValue),
            new Buff("Rocky Loop", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/3/31/Rocky_Loop.png", 119939, ulong.MaxValue),
            new Buff("Relentless Fire", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/7/70/Relentless_Fire.png", 119939, ulong.MaxValue),
            new Buff("Shattering Ice", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/6/63/Shattering_Ice.png", 119939, ulong.MaxValue),
            new Buff("Invigorating Air", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/1/12/Invigorating_Air.png", 119939, ulong.MaxValue),
            new Buff("Fortified Earth", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/8/84/Fortified_Earth.png", 119939, ulong.MaxValue),
            new Buff("Elemental Celerity", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/a/a5/Elemental_Celerity.png", 119939, ulong.MaxValue),
            new Buff("Elemental Empowerment", -1, Source.Catalyst, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/e/e6/Elemental_Empowerment.png", 119939, ulong.MaxValue),
            new Buff("Empowering Auras", -1, Source.Catalyst, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/4/44/Empowering_Auras.png", 119939, ulong.MaxValue),
            new Buff("Hardened Auras", -1, Source.Catalyst, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/2/23/Hardened_Auras.png", 119939, ulong.MaxValue),

        };
    }
}
