using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class MesmerHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(PowerSpike, PowerSpike).WithBuilds(GW2Builds.StartOfLife ,GW2Builds.May2021Balance), // Power spike
            new DamageCastFinder(MantraOfPain, MantraOfPain).WithBuilds(GW2Builds.May2021Balance), // Mantra of Pain
            new EXTHealingCastFinder(MantraOfRecovery, MantraOfRecovery).WithBuilds(GW2Builds.May2021Balance), // Mantra of Recovery
            new BuffLossCastFinder(SignetOfMidnightSkill, SignetOfMidnightEffect).UsingChecker((brae, combatData) => {
                return combatData.GetBuffData(brae.To).Any(x =>
                                    x is BuffApplyEvent bae &&
                                    bae.BuffID == SkillIDs.HideInShadows &&
                                    Math.Abs(bae.AppliedDuration - 2000) <= ServerDelayConstant &&
                                    bae.CreditedBy == brae.To &&
                                    Math.Abs(brae.Time - bae.Time) <= ServerDelayConstant
                                 );
                }), // Signet of Midnight
            new BuffGainCastFinder(PortalEntre, PortalWeaving), // Portal Entre
            new DamageCastFinder(LesserPhantasmalDefender, LesserPhantasmalDefender), // Lesser Phantasmal Defender
            /*new BuffGainCastFinder(10192, 10243, GW2Builds.October2018Balance, GW2Builds.July2019Balance, (evt, combatData) => {
                var buffsLossToCheck = new List<long>
                {
                    10235, 30739, 21751, 10231, 10246, 10233
                }; // signets
                foreach (long buffID in buffsLossToCheck)
                {
                    if (combatData.GetBuffData(buffID).Where(x => x.Time >= evt.Time - ParserHelper.ServerDelayConstant && x.Time <= evt.Time + ParserHelper.ServerDelayConstant && x is BuffRemoveAllEvent).Any())
                    {
                        return false;
                    }
                }
                return true;

            }), // Distortion
            new BuffGainCastFinder(10192, 10243, GW2Builds.July2019Balance, 104844, (evt, combatData) => {
                if (evt.To.Prof == "Chronomancer")
                {
                    return false;
                }
                var buffsLossToCheck = new List<long>
                {
                    10235, 30739, 21751, 10231, 10246, 10233
                }; // signets
                foreach (long buffID in buffsLossToCheck)
                {
                    if (combatData.GetBuffData(buffID).Where(x => x.Time >= evt.Time - ParserHelper.ServerDelayConstant && x.Time <= evt.Time + ParserHelper.ServerDelayConstant && x is BuffRemoveAllEvent).Any())
                    {
                        return false;
                    }
                }
                return true;

            }), // Distortion
            new BuffGainCastFinder(10192, 10243, 104844, GW2Builds.EndOfLife, (evt, combatData) => {
                var buffsLossToCheck = new List<long>
                {
                    10235, 30739, 21751, 10231, 10246, 10233
                }; // signets
                foreach (long buffID in buffsLossToCheck)
                {
                    if (combatData.GetBuffData(buffID).Where(x => x.Time >= evt.Time - ParserHelper.ServerDelayConstant && x.Time <= evt.Time + ParserHelper.ServerDelayConstant && x is BuffRemoveAllEvent).Any()) 
                    {
                        return false;
                    }
                }
                return true;
                
            }), // Distortion*/
            new EffectCastFinder(Feedback, EffectGUIDs.MesmerFeedback).UsingChecker((evt, log) => evt.Src.BaseSpec == Spec.Mesmer),
            new EffectCastFinderByDst(Blink, EffectGUIDs.MesmerBlink).UsingChecker((evt, log) => evt.Dst.BaseSpec == Spec.Mesmer),
            new EffectCastFinder(MindWrack, EffectGUIDs.MesmerMindWrack).UsingChecker((evt, log) => !log.GetBuffData(DistortionEffect).Any(x => x.To == evt.Src && Math.Abs(x.Time - evt.Time) < ServerDelayConstant) && (evt.Src.Spec == Spec.Mesmer || evt.Src.Spec == Spec.Mirage)),
            new EffectCastFinder(CryOfFrustration, EffectGUIDs.MesmerCryOfFrustration).UsingChecker((evt, log) => (evt.Src.Spec == Spec.Mesmer || evt.Src.Spec == Spec.Mirage)),
            new EffectCastFinder(Diversion, EffectGUIDs.MesmerDiversion).UsingChecker((evt, log) => (evt.Src.Spec == Spec.Mesmer || evt.Src.Spec == Spec.Mirage)),
            new EffectCastFinder(DistortionSkill, EffectGUIDs.MesmerDistortion).UsingChecker((evt, log) => log.GetBuffData(DistortionEffect).Any(x => x.To == evt.Src && Math.Abs(x.Time - evt.Time) < ServerDelayConstant) && (evt.Src.Spec == Spec.Mesmer || evt.Src.Spec == Spec.Mirage)),
            new EffectCastFinder(MantraOfResolve, EffectGUIDs.MesmerMantraOfResolve).UsingChecker((evt, log) => evt.Src.BaseSpec == Spec.Mesmer),
            new EffectCastFinderByDst(MantraOfConcentration, EffectGUIDs.MesmerMantraOfConcentration).UsingChecker((evt, log) => evt.Dst.BaseSpec == Spec.Mesmer),
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Domination
            // Empowered illusions require knowing all illusion species ID
            // We need illusion species ID to enable Vicious Expression on All
            new BuffDamageModifierTarget(NumberOfBoons, "Vicious Expression", "25% on boonless target",  DamageSource.NoPets, 25.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByAbsence, "https://wiki.guildwars2.com/images/f/f6/Confounding_Suggestions.png", DamageModifierMode.PvE).WithBuilds(GW2Builds.February2020Balance, 102389),
            new BuffDamageModifierTarget(NumberOfBoons, "Vicious Expression", "15% on boonless target",  DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByAbsence, "https://wiki.guildwars2.com/images/f/f6/Confounding_Suggestions.png", DamageModifierMode.All).WithBuilds(102389),
            new DamageLogDamageModifier("Egotism", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Mesmer, "https://wiki.guildwars2.com/images/7/78/Temporal_Enchanter.png", (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, DamageModifierMode.PvE).WithBuilds(GW2Builds.October2018Balance).UsingApproximate(true),
            new DamageLogDamageModifier("Egotism", "5% if target hp% lower than self hp%", DamageSource.NoPets, 5.0, DamageType.Strike, DamageType.All, Source.Mesmer, "https://wiki.guildwars2.com/images/7/78/Temporal_Enchanter.png", (x,log) =>
            {
                double selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                double dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, DamageModifierMode.sPvPWvW).WithBuilds(GW2Builds.October2018Balance).UsingApproximate(true),
            new BuffDamageModifierTarget(Vulnerability, "Fragility", "0.5% per stack vuln on target", DamageSource.NoPets, 0.5, DamageType.Strike, DamageType.All, Source.Mesmer, ByStack, "https://wiki.guildwars2.com/images/3/33/Fragility.png", DamageModifierMode.All),
            // Dueling
            // Superiority Complex can all the conditions be tracked?
            // Illusions
            new BuffDamageModifier(CompoundingPower, "Compounding Power", "2% per stack (8s) after creating an illusion ", DamageSource.NoPets, 2.0, DamageType.Strike, DamageType.All, Source.Mesmer, ByStack, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png", DamageModifierMode.All),
            // Phantasmal Force: the current infrastructure is not capable of checking buffs on minions, once we have that, this does not require knowing illusion species id
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            
                //signets
                new Buff("Signet of the Ether", SignetOfTheEther, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/7/7a/Signet_of_the_Ether.png"),
                new Buff("Signet of Domination",SignetOfDomination, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/3/3b/Signet_of_Domination.png"),
                new Buff("Signet of Illusions",SignetOfIllusions, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Illusions.png"),
                new Buff("Signet of Inspiration",SignetOfInspirationEffect, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/ed/Signet_of_Inspiration.png"),
                new Buff("Signet of Midnight",SignetOfMidnightEffect, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/24/Signet_of_Midnight.png"),
                new Buff("Signet of Humility",SignetOfHumility, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b5/Signet_of_Humility.png"),
                //skills
                new Buff("Distortion",DistortionEffect, Source.Mesmer, BuffStackType.Queue, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Blur", Blur , Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Mirror",Mirror, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/b/b8/Mirror.png"),
                new Buff("Echo",Echo, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/c/ce/Echo.png"),
                new Buff("Illusionary Counter",IllusionaryCounter, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e5/Illusionary_Counter.png"),
                new Buff("Illusionary Riposte",IllusionaryRiposte, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/91/Illusionary_Riposte.png"),
                new Buff("Illusionary Leap",IllusionaryLeap, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/1/18/Illusionary_Leap.png"),
                new Buff("Portal Weaving",PortalWeaving, Source.Mesmer, BuffClassification.Other, "https://wiki.guildwars2.com/images/8/81/Portal_Entre.png"),
                new Buff("Illusion of Life",IllusionOfLife, Source.Mesmer, BuffClassification.Support, "https://wiki.guildwars2.com/images/9/92/Illusion_of_Life.png"),
                //traits
                new Buff("Fencer's Finesse", FencersFinesse , Source.Mesmer, BuffStackType.Stacking, 10, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e7/Fencer%27s_Finesse.png"),
                new Buff("Illusionary Defense",IllusionaryDefense, Source.Mesmer, BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e0/Illusionary_Defense.png"),
                new Buff("Compounding Power",CompoundingPower, Source.Mesmer, BuffStackType.Stacking, 5, BuffClassification.Other, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png"),
                new Buff("Phantasmal Force", PhantasmalForce , Source.Mesmer, BuffStackType.Stacking, 25, BuffClassification.Other, "https://wiki.guildwars2.com/images/5/5f/Mistrust.png"),
                new Buff("Reflection", Reflection , Source.Mesmer, BuffStackType.Queue, 9, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9d/Arcane_Shield.png"),
                new Buff("Reflection 2", Reflection2 , Source.Mesmer, BuffStackType.Queue, 9, BuffClassification.Other, "https://wiki.guildwars2.com/images/9/9d/Arcane_Shield.png"),
        };


        private static readonly HashSet<long> _cloneIDs = new HashSet<long>()
        {
            (int)MinionID.Clone1,
            (int)MinionID.Clone2,
            (int)MinionID.Clone3,
            (int)MinionID.Clone4,
            (int)MinionID.Clone5,
            (int)MinionID.Clone6,
            (int)MinionID.Clone7,
            (int)MinionID.Clone8,
            (int)MinionID.Clone9,
            (int)MinionID.Clone10,
            (int)MinionID.Clone11,
            (int)MinionID.Clone12,
            (int)MinionID.Clone13,
            (int)MinionID.Clone14,
            (int)MinionID.Clone15,
            (int)MinionID.Clone16,
            (int)MinionID.Clone17,
            (int)MinionID.Clone18,
            (int)MinionID.Clone19,
            (int)MinionID.Clone20,
            (int)MinionID.Clone21,
            (int)MinionID.Clone22,
            (int)MinionID.Clone23,
            (int)MinionID.Clone24,
            (int)MinionID.Clone25,
            (int)MinionID.Clone26,
        };
        internal static bool IsClone(AgentItem agentItem)
        {
            if (agentItem.Type == AgentItem.AgentType.Gadget)
            {
                return false;
            }
            return _cloneIDs.Contains(agentItem.ID);
        }

        private static bool IsClone(long id)
        {
            return _cloneIDs.Contains(id);
        }

        private static HashSet<long> NonCloneMinions = new HashSet<long>()
        {
            (int)MinionID.IllusionaryWarlock,
            (int)MinionID.IllusionaryWarden,
            (int)MinionID.IllusionarySwordsman,
            (int)MinionID.IllusionaryMage,
            (int)MinionID.IllusionaryDuelist,
            (int)MinionID.IllusionaryBerserker,
            (int)MinionID.IllusionaryDisenchanter,
            (int)MinionID.IllusionaryRogue,
            (int)MinionID.IllusionaryDefender,
        };
        internal static bool IsKnownMinionID(long id)
        {
            return NonCloneMinions.Contains(id) || IsClone(id);
        }

    }
}
