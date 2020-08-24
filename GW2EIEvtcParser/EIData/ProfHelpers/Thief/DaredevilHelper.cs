using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;

namespace GW2EIEvtcParser.EIData
{
    internal class DaredevilHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(31600, 33162, EIData.InstantCastFinder.DefaultICD), // bound
            new DamageCastFinder(30520, 30520, EIData.InstantCastFinder.DefaultICD), // Debilitating Arc
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(32200, "Lotus Training", "10% cDam (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Condition, DamageType.All, ParserHelper.Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png", DamageModifierMode.All),
            new BuffDamageModifier(33162, "Bounding Dodger", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParserHelper.Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png", DamageModifierMode.PvE),
            new BuffDamageModifier(33162, "Bounding Dodger", "10% (4s) after dodging", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParserHelper.Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png", 0, 102321, DamageModifierMode.sPvPWvW),
            new BuffDamageModifier(33162, "Bounding Dodger", "15% (4s) after dodging", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ParserHelper.Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png", 102321, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(742, "Weakening Strikes", "7% if weakness on target", DamageSource.NoPets, 7.0, DamageType.Power, DamageType.All, ParserHelper.Source.Daredevil, ByPresence, "https://wiki.guildwars2.com/images/7/7c/Weakening_Strikes.png", 96406, ulong.MaxValue, DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Palm Strike",30423, ParserHelper.Source.Daredevil, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
                new Buff("Pulmonary Impact",30510, ParserHelper.Source.Daredevil, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/1a/Palm_Strike.png"),
                new Buff("Lotus Training", 32200, ParserHelper.Source.Daredevil, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ea/Lotus_Training.png"),
                new Buff("Unhindered Combatant", 32931, ParserHelper.Source.Daredevil, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/a/a1/Unhindered_Combatant.png"),
                new Buff("Bounding Dodger", 33162, ParserHelper.Source.Daredevil, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/30/Bounding_Dodger.png"),
                new Buff("Weakening Strikes", 34081, ParserHelper.Source.Daredevil, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7c/Weakening_Strikes.png", 96406, ulong.MaxValue),
        };

    }
}
