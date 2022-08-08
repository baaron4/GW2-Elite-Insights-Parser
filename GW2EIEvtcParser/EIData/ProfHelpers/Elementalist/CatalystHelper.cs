using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class CatalystHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(FlameWheelSkill, FlameWheelEffect).WithBuilds(GW2Builds.EODBeta4), // Flame Wheel
            new BuffGainCastFinder(IcyCoilSkill, IcyCoilEffect).WithBuilds(GW2Builds.EODBeta4), // Icy Coil
            new BuffGainCastFinder(CrescentWindSkill, CrescentWindEffect).WithBuilds(GW2Builds.EODBeta4), // Crescent Wind
            new BuffGainCastFinder(RockyLoopSkill, RockyLoopEffect).WithBuilds(GW2Builds.EODBeta4), // Rockyh Loop
            new BuffGainCastFinder(InvigoratingAirSkill, InvigoratingAirEffect).WithBuilds(GW2Builds.EODBeta4), // Invigorating Air
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(FlameWheelEffect, "Flame Wheel", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, "https://wiki.guildwars2.com/images/f/f3/Flame_Wheel.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta2, GW2Builds.March2022Balance2),
            new BuffDamageModifier(FlameWheelEffect, "Flame Wheel", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, "https://wiki.guildwars2.com/images/f/f3/Flame_Wheel.png", DamageModifierMode.All).WithBuilds(GW2Builds.March2022Balance2),
            new BuffDamageModifier(RelentlessFire, "Relentless Fire", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, "https://wiki.guildwars2.com/images/7/70/Relentless_Fire.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta2, GW2Builds.March2022Balance),
            new BuffDamageModifier(RelentlessFire, "Relentless Fire", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, "https://wiki.guildwars2.com/images/7/70/Relentless_Fire.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.March2022Balance),
            new BuffDamageModifier(FlameWheelSkill, "Relentless Fire", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, "https://wiki.guildwars2.com/images/7/70/Relentless_Fire.png", DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.March2022Balance),
            new BuffDamageModifier(EmpoweringAuras, "Empowering Auras", "2%", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByStack, "https://wiki.guildwars2.com/images/4/44/Empowering_Auras.png", DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta2),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Flame Wheel", FlameWheelEffect, Source.Catalyst, BuffClassification.Other,"https://wiki.guildwars2.com/images/f/f3/Flame_Wheel.png"),
            new Buff("Icy Coil", IcyCoilEffect, Source.Catalyst, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/87/Icy_Coil.png"),
            new Buff("Crescent Wind", CrescentWindEffect, Source.Catalyst, BuffClassification.Other,"https://wiki.guildwars2.com/images/3/3c/Crescent_Wind.png"),
            new Buff("Rocky Loop", RockyLoopEffect, Source.Catalyst, BuffClassification.Other,"https://wiki.guildwars2.com/images/3/31/Rocky_Loop.png"),
            new Buff("Relentless Fire", RelentlessFire, Source.Catalyst, BuffClassification.Other,"https://wiki.guildwars2.com/images/7/70/Relentless_Fire.png"),
            new Buff("Shattering Ice", ShatteringIce, Source.Catalyst, BuffClassification.Other,"https://wiki.guildwars2.com/images/6/63/Shattering_Ice.png"),
            new Buff("Invigorating Air", InvigoratingAirEffect, Source.Catalyst, BuffClassification.Other,"https://wiki.guildwars2.com/images/1/12/Invigorating_Air.png"),
            new Buff("Fortified Earth", FortifiedEarth, Source.Catalyst, BuffClassification.Other,"https://wiki.guildwars2.com/images/8/84/Fortified_Earth.png"),
            new Buff("Elemental Celerity", ElementalCelerity, Source.Catalyst, BuffClassification.Other,"https://wiki.guildwars2.com/images/a/a5/Elemental_Celerity.png"),
            new Buff("Elemental Empowerment", ElementalEmpowerment, Source.Catalyst, BuffStackType.Stacking, 10, BuffClassification.Other,"https://wiki.guildwars2.com/images/e/e6/Elemental_Empowerment.png"),
            new Buff("Empowering Auras", EmpoweringAuras, Source.Catalyst, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other,"https://wiki.guildwars2.com/images/4/44/Empowering_Auras.png"),
            new Buff("Hardened Auras", HardenedAuras, Source.Catalyst, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other,"https://wiki.guildwars2.com/images/2/23/Hardened_Auras.png"),

        };
    }
}
