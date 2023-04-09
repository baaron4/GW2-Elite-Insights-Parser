using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.EIData.Buffs;
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
            new BuffGainCastFinder(FlameWheelSkill, FlameWheelEffect).UsingChecker((ba, combatData, agentData, skillData) => ba.OverridenDuration == 0).WithBuilds(GW2Builds.EODBeta4), 
            new BuffGainCastFinder(IcyCoilSkill, IcyCoilEffect).UsingChecker((ba, combatData, agentData, skillData) => ba.OverridenDuration == 0).WithBuilds(GW2Builds.EODBeta4),
            new BuffGainCastFinder(CrescentWindSkill, CrescentWindEffect).UsingChecker((ba, combatData, agentData, skillData) => ba.OverridenDuration == 0).WithBuilds(GW2Builds.EODBeta4), 
            new BuffGainCastFinder(RockyLoopSkill, RockyLoopEffect).UsingChecker((ba, combatData, agentData, skillData) => ba.OverridenDuration == 0).WithBuilds(GW2Builds.EODBeta4),
            new BuffGainCastFinder(InvigoratingAirSkill, InvigoratingAirEffect).WithBuilds(GW2Builds.EODBeta4), // Invigorating Air
            new EffectCastFinder(DeployJadeSphereFire, EffectGUIDs.CatalystDeployFireJadeSphere).UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec == Spec.Catalyst),
            new EffectCastFinder(DeployJadeSphereAir, EffectGUIDs.CatalystDeployAirJadeSphere).UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec == Spec.Catalyst),
            new EffectCastFinder(DeployJadeSphereWater, EffectGUIDs.CatalystDeployWaterJadeSphere).UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec == Spec.Catalyst),
            new EffectCastFinder(DeployJadeSphereEarth, EffectGUIDs.CatalystDeployEarthJadeSphere).UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec == Spec.Catalyst)
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(FlameWheelEffect, "Flame Wheel", "5%", DamageSource.NoPets, 5.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, BuffImages.FlameWheel, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta2, GW2Builds.March2022Balance2),
            new BuffDamageModifier(FlameWheelEffect, "Flame Wheel", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, BuffImages.FlameWheel, DamageModifierMode.All).WithBuilds(GW2Builds.March2022Balance2),
            new BuffDamageModifier(RelentlessFire, "Relentless Fire", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, BuffImages.RelentlessFire, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta2, GW2Builds.March2022Balance),
            new BuffDamageModifier(RelentlessFire, "Relentless Fire", "10%", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, BuffImages.RelentlessFire, DamageModifierMode.PvE).WithBuilds(GW2Builds.March2022Balance),
            new BuffDamageModifier(FlameWheelSkill, "Relentless Fire", "15%", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByPresence, BuffImages.RelentlessFire, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.March2022Balance),
            new BuffDamageModifier(EmpoweringAuras, "Empowering Auras", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByStack, BuffImages.EmpoweringAuras, DamageModifierMode.All).WithBuilds(GW2Builds.EODBeta2, GW2Builds.November2022Balance),
            new BuffDamageModifier(EmpoweringAuras, "Empowering Auras", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByStack, BuffImages.EmpoweringAuras, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.November2022Balance),
            new BuffDamageModifier(EmpoweringAuras, "Empowering Auras", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndCondition, DamageType.All, Source.Catalyst, ByStack, BuffImages.EmpoweringAuras, DamageModifierMode.PvE).WithBuilds(GW2Builds.November2022Balance),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Flame Wheel", FlameWheelEffect, Source.Catalyst, BuffClassification.Other, BuffImages.FlameWheel),
            new Buff("Icy Coil", IcyCoilEffect, Source.Catalyst, BuffClassification.Other, BuffImages.IcyCoil),
            new Buff("Crescent Wind", CrescentWindEffect, Source.Catalyst, BuffClassification.Other, BuffImages.CrescentWind),
            new Buff("Rocky Loop", RockyLoopEffect, Source.Catalyst, BuffClassification.Other, BuffImages.RockyLoop),
            new Buff("Relentless Fire", RelentlessFire, Source.Catalyst, BuffClassification.Other, BuffImages.RelentlessFire),
            new Buff("Shattering Ice", ShatteringIce, Source.Catalyst, BuffClassification.Other, BuffImages.ShatteringIce),
            new Buff("Invigorating Air", InvigoratingAirEffect, Source.Catalyst, BuffClassification.Other, BuffImages.InvigoratingAir),
            new Buff("Immutable Stone", ImmutableStoneEffect, Source.Catalyst, BuffClassification.Other, BuffImages.ImmutableStone),
            new Buff("Fortified Earth", FortifiedEarth, Source.Catalyst, BuffClassification.Other, BuffImages.FortifiedEarth),
            new Buff("Elemental Celerity", ElementalCelerity, Source.Catalyst, BuffClassification.Other, BuffImages.ElementalCelerity),
            new Buff("Elemental Empowerment", ElementalEmpowerment, Source.Catalyst, BuffStackType.Stacking, 10, BuffClassification.Other, BuffImages.ElementalEmpowerment),
            new Buff("Empowering Auras", EmpoweringAuras, Source.Catalyst, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.EmpoweringAuras),
            new Buff("Hardened Auras", HardenedAuras, Source.Catalyst, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Other, BuffImages.HardenedAuras),
        };
    }
}
