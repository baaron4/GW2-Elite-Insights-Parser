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
            new BuffDamageModifier(-1, "Flame Wheel", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, "", DamageModifierMode.All),
            new BuffDamageModifier(-1, "Relentless Fire", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, "", DamageModifierMode.All),
            new BuffDamageModifier(-1, "Empowering Auras", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByStack, "", DamageModifierMode.All),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Flame Wheel", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,""),
            new Buff("Icy Coil", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,""),
            new Buff("Crescent Wind", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,""),
            new Buff("Rocky Loop", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,""),
            new Buff("Relentless Fire", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,""),
            new Buff("Shattering Ice", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,""),
            new Buff("Invigorating Air", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,""),
            new Buff("Fortified Earth", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,""),
            new Buff("Elemental Celerity", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,""),
            new Buff("Invigorating Air", -1, Source.Catalyst, BuffNature.GraphOnlyBuff,""),
            new Buff("Elemental Empowerment", -1, Source.Catalyst, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff,""),
            new Buff("Empowering Auras", -1, Source.Catalyst, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff,""),
            new Buff("Hardened Auras", -1, Source.Catalyst, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff,""),

        };
    }
}
