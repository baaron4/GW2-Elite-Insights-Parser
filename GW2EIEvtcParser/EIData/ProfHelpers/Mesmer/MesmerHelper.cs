using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal static class MesmerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(10212, 10212, EIData.InstantCastFinder.DefaultICD), // Power spike
            new BuffLossCastFinder(10234, 10233, EIData.InstantCastFinder.DefaultICD, (brae, combatData) => {
                return combatData.GetBuffData(brae.To).Any(x =>
                                    x is BuffApplyEvent bae &&
                                    bae.BuffID == 13017 &&
                                    Math.Abs(bae.AppliedDuration - 2000) <= ServerDelayConstant &&
                                    bae.CreditedBy == brae.To &&
                                    Math.Abs(brae.Time - bae.Time) <= ServerDelayConstant
                                 );
                }
            ), // Signet of Midnight
            new BuffGainCastFinder(10197, 10198, EIData.InstantCastFinder.DefaultICD), // Portal Entre
            new DamageCastFinder(30192, 30192, EIData.InstantCastFinder.DefaultICD), // Lesser Phantasmal Defender
            /*new BuffGainCastFinder(10192, 10243, EIData.InstantCastFinder.DefaultICD, 92715, 97950, (evt, combatData) => {
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
            new BuffGainCastFinder(10192, 10243, EIData.InstantCastFinder.DefaultICD, 97950, 104844, (evt, combatData) => {
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
            new BuffGainCastFinder(10192, 10243, EIData.InstantCastFinder.DefaultICD, 104844, ulong.MaxValue, (evt, combatData) => {
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
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            // Domination
            // Empowered illusions require knowing all illusion species ID
            // We need illusion species ID to enable Vicious Expression on All
            new BuffDamageModifierTarget(NumberOfBoonsID, "Vicious Expression", "25% on boonless target",  DamageSource.NoPets, 25.0, DamageType.Power, DamageType.All, Source.Mesmer, ByAbsence, "https://wiki.guildwars2.com/images/f/f6/Confounding_Suggestions.png", 102321, 102389, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(NumberOfBoonsID, "Vicious Expression", "15% on boonless target",  DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, Source.Mesmer, ByAbsence, "https://wiki.guildwars2.com/images/f/f6/Confounding_Suggestions.png", 102389, ulong.MaxValue, DamageModifierMode.All),
            new DamageLogApproximateDamageModifier("Egotism", "10% if target hp% lower than self hp%", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, Source.Mesmer, "https://wiki.guildwars2.com/images/7/78/Temporal_Enchanter.png", (x,log) =>
            {
                var selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                var dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, 92715, ulong.MaxValue, DamageModifierMode.PvE),
            new DamageLogApproximateDamageModifier("Egotism", "5% if target hp% lower than self hp%", DamageSource.NoPets, 5.0, DamageType.Power, DamageType.All, Source.Mesmer, "https://wiki.guildwars2.com/images/7/78/Temporal_Enchanter.png", (x,log) =>
            {
                var selfHP = x.From.GetCurrentHealthPercent(log, x.Time);
                var dstHP = x.To.GetCurrentHealthPercent(log, x.Time);
                if (selfHP < 0.0 || dstHP < 0.0)
                {
                    return false;
                }
                return selfHP > dstHP;
            }, ByPresence, 92715, ulong.MaxValue, DamageModifierMode.sPvPWvW),
            new BuffDamageModifierTarget(738, "Fragility", "0.5% per stack vuln on target", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, Source.Mesmer, ByStack, "https://wiki.guildwars2.com/images/3/33/Fragility.png", DamageModifierMode.All),
            // Dueling
            // Superiority Complex can all the conditions be tracked?
            // Illusions
            new BuffDamageModifier(49058, "Compounding Power", "2% per stack (8s) after creating an illusion ", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, Source.Mesmer, ByStack, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png", DamageModifierMode.All),
            // Phantasmal Force: the current infrastructure is not capable of checking buffs on minions, once we have that, this does not require knowing illusion species id
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            
                //signets
                new Buff("Signet of the Ether", 21751, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7a/Signet_of_the_Ether.png"),
                new Buff("Signet of Domination",10231, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3b/Signet_of_Domination.png"),
                new Buff("Signet of Illusions",10246, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Illusions.png"),
                new Buff("Signet of Inspiration",10235, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ed/Signet_of_Inspiration.png"),
                new Buff("Signet of Midnight",10233, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/24/Signet_of_Midnight.png"),
                new Buff("Signet of Humility",30739, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b5/Signet_of_Humility.png"),
                //skills
                new Buff("Distortion",10243, Source.Mesmer, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Blur", 10335 , Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Mirror",10357, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b8/Mirror.png"),
                new Buff("Echo",29664, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Echo.png"),
                new Buff("Illusionary Counter",10278, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e5/Illusionary_Counter.png"),
                new Buff("Illusionary Riposte",10279, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/91/Illusionary_Riposte.png"),
                new Buff("Illusionary Leap",10353, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Illusionary_Leap.png"),
                new Buff("Portal Weaving",10198, Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/8/81/Portal_Entre.png"),
                //traits
                new Buff("Fencer's Finesse", 30426 , Source.Mesmer, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e7/Fencer%27s_Finesse.png"),
                new Buff("Illusionary Defense",49099, Source.Mesmer, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e0/Illusionary_Defense.png"),
                new Buff("Compounding Power",49058, Source.Mesmer, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png"),
                new Buff("Phantasmal Force", 44691 , Source.Mesmer, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5f/Mistrust.png"),
        };

    }
}
