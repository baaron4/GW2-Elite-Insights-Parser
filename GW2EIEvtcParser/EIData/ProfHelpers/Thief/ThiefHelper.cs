using System;
using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class ThiefHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(Shadowstep, Infiltration),
            new BuffLossCastFinder(ShadowReturn, Infiltration).UsingChecker((evt, combatData, agentData, skillData) => evt.RemovedDuration > ServerDelayConstant),
            new DamageCastFinder(Mug, Mug),
            new DamageCastFinder(InfiltratorsStrike, InfiltratorsStrike),
            new BuffGainCastFinder(AssassinsSignet, AssassinsSignetActive),
            new BuffGiveCastFinder(DevourerVenomSkill, DevourerVenomEffect),
            new BuffGiveCastFinder(IceDrakeVenomSkill, IceDrakeVenomEffect),
            new BuffGiveCastFinder(SkaleVenomSkill, SkaleVenomEffect),
            new BuffGiveCastFinder(SoulStoneVenomSkill,SoulStoneVenomEffect),
            new BuffGiveCastFinder(SpiderVenomSkill,SpiderVenomEffect).UsingChecker((evt, combatData, agentData, skillData) => evt.To != evt.By || Math.Abs(evt.AppliedDuration - 24000) < ServerDelayConstant).UsingNotAccurate(true), // same id as leeching venom trait?
            new EffectCastFinder(Pitfall, EffectGUIDs.ThiefPitfallAoE).UsingSrcBaseSpecChecker(Spec.Thief),
            new EffectCastFinder(ThousandNeedles, EffectGUIDs.ThiefThousandNeedlesAoE1).UsingSrcBaseSpecChecker(Spec.Thief),
            new EffectCastFinder(SealArea, EffectGUIDs.ThiefSealAreaAoE).UsingSrcBaseSpecChecker(Spec.Thief),
            new BuffGainCastFinder(ShadowPortal, ShadowPortalOpenedEffect),
            new EffectCastFinderByDst(InfiltratorsSignetSkill, EffectGUIDs.ThiefInfiltratorsSignet1).UsingDstBaseSpecChecker(Spec.Thief),
            new EffectCastFinderByDst(SignetOfAgilitySkill, EffectGUIDs.ThiefSignetOfAgility).UsingDstBaseSpecChecker(Spec.Thief),
            new EffectCastFinderByDst(SignetOfShadowsSkill, EffectGUIDs.ThiefSignetOfShadows).UsingDstBaseSpecChecker(Spec.Thief),
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Deadly arts
            new BuffDamageModifierTarget(NumberOfConditions, "Exposed Weakness", "2% per condition on target", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Thief, ByStack, BuffImages.ExposedWeakness, DamageModifierMode.All).WithBuilds(GW2Builds.July2018Balance),
            new BuffDamageModifierTarget(NumberOfConditions, "Exposed Weakness", "10% if condition on target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Thief, ByPresence, BuffImages.ExposedWeakness, DamageModifierMode.PvE).WithBuilds(GW2Builds.StartOfLife, GW2Builds.July2018Balance),
            new DamageLogDamageModifier("Executioner", "20% if target <50% HP", DamageSource.NoPets, 20.0, DamageType.Strike, DamageType.All, Source.Thief, BuffImages.Executioner, (x, log) => x.AgainstUnderFifty, ByPresence, DamageModifierMode.All),
            // Critical Strikes
            new DamageLogDamageModifier("Twin Fangs","7% if hp >=90%", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Thief, BuffImages.FerociousStrikes, (x, log) => x.IsOverNinety && x.HasCrit, ByPresence, DamageModifierMode.All),
            new DamageLogDamageModifier("Ferocious Strikes", "10% on critical strikes if target >50%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Thief, BuffImages.FerociousStrikes, (x, log) => !x.AgainstUnderFifty && x.HasCrit, ByPresence, DamageModifierMode.All),
            // Trickery
            new BuffDamageModifier(LeadAttacks, "Lead Attacks", "1% (10s) per initiative spent", DamageSource.NoPets, 1.0, DamageType.StrikeAndCondition, DamageType.All, Source.Thief, ByStack, BuffImages.LeadAttacks, DamageModifierMode.All), // It's not always possible to detect the presence of pistol and the trait is additive with itself. Staff master is worse as we can't detect endurance at all
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            // Skills
            new Buff("Shadow Portal (Prepared)", ShadowPortalPreparedEffect, Source.Thief, BuffClassification.Other, BuffImages.PrepareShadowPortal),
            new Buff("Shadow Portal (Open)", ShadowPortalOpenedEffect, Source.Thief, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.ShadowPortal),
            // Signets
            new Buff("Signet of Malice", SignetOfMalice, Source.Thief, BuffClassification.Other, BuffImages.SignetOfMalice),
            new Buff("Assassin's Signet (Passive)", AssassinsSignetPassive, Source.Thief, BuffClassification.Other, BuffImages.AssassinsSignet),
            new Buff("Assassin's Signet (Active)", AssassinsSignetActive, Source.Thief, BuffClassification.Other, BuffImages.AssassinsSignet),
            new Buff("Infiltrator's Signet", InfiltratorsSignetEffect, Source.Thief, BuffClassification.Other, BuffImages.InfiltratorsSignet),
            new Buff("Signet of Agility", SignetOfAgilityEffect, Source.Thief, BuffClassification.Other, BuffImages.SignetOfAgility),
            new Buff("Signet of Shadows", SignetOfShadowsEffect, Source.Thief, BuffClassification.Other, BuffImages.SignetOfShadows),
            // Venoms // src is always the user, makes generation data useless
            new Buff("Skelk Venom", SkelkVenom, Source.Thief, BuffStackType.StackingConditionalLoss, 5, BuffClassification.Defensive, BuffImages.SkelkVenom),
            new Buff("Ice Drake Venom", IceDrakeVenomEffect, Source.Thief, BuffStackType.StackingConditionalLoss, 4, BuffClassification.Support, BuffImages.IceDrakeVenom),
            new Buff("Devourer Venom", DevourerVenomEffect, Source.Thief, BuffStackType.StackingConditionalLoss, 2, BuffClassification.Support, BuffImages.DevourerVenom),
            new Buff("Skale Venom", SkaleVenomEffect, Source.Thief, BuffStackType.StackingConditionalLoss, 4, BuffClassification.Offensive, BuffImages.SkaleVenom),
            new Buff("Spider Venom", SpiderVenomEffect, Source.Thief, BuffStackType.StackingConditionalLoss, 6, BuffClassification.Offensive, BuffImages.SpiderVenom),
            new Buff("Soul Stone Venom", SoulStoneVenomEffect, Source.Thief, BuffStackType.Stacking, 25, BuffClassification.Offensive, BuffImages.SoulStoneVenom),
            new Buff("Basilisk Venom", BasiliskVenom, Source.Thief, BuffStackType.StackingConditionalLoss, 2, BuffClassification.Support, BuffImages.BasiliskVenom),
            new Buff("Petrified 1", Petrified1, Source.Thief, BuffClassification.Other, BuffImages.Stun),
            new Buff("Petrified 2", Petrified2, Source.Thief, BuffClassification.Other, BuffImages.Stun),
            new Buff("Infiltration", Infiltration, Source.Thief, BuffClassification.Other, BuffImages.Shadowstep),
            // Transforms
            new Buff("Dagger Storm", DaggerStorm, Source.Thief, BuffClassification.Other, BuffImages.DaggerStorm),
            // Traits
            new Buff("Hidden Killer", HiddenKiller, Source.Thief, BuffClassification.Other, BuffImages.Hiddenkiller),
            new Buff("Lead Attacks", LeadAttacks, Source.Thief, BuffStackType.Stacking, 15, BuffClassification.Other, BuffImages.LeadAttacks),
            new Buff("Instant Reflexes", InstantReflexes, Source.Thief, BuffClassification.Other, BuffImages.InstantReflexes),
        };

        private static HashSet<long> Minions = new HashSet<long>()
        {
            (int)MinionID.Thief1,
            (int)MinionID.Thief2,
            (int)MinionID.Thief3,
            (int)MinionID.Thief4,
            (int)MinionID.Thief5,
            (int)MinionID.Thief6,
            (int)MinionID.Thief7,
            (int)MinionID.Thief8,
            (int)MinionID.Thief9,
            (int)MinionID.Thief10,
            (int)MinionID.Thief11,
            (int)MinionID.Thief12,
            (int)MinionID.Thief13,
            (int)MinionID.Thief14,
            (int)MinionID.Thief15,
            (int)MinionID.Thief16,
            (int)MinionID.Thief17,
            (int)MinionID.Thief18,
            (int)MinionID.Thief19,
            (int)MinionID.Thief20,
            (int)MinionID.Thief21,
            (int)MinionID.Thief22,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return Minions.Contains(id);
        }

    }
}
