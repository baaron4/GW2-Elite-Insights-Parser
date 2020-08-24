using System;
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;

namespace GW2EIEvtcParser.EIData
{
    internal class MesmerHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(10212, 10212, EIData.InstantCastFinder.DefaultICD), // Power spike
            new BuffLossCastFinder(10233, 10233, EIData.InstantCastFinder.DefaultICD, (brae, combatData) => {
                return combatData.GetBuffData(brae.To).Exists(x => 
                                    x is BuffApplyEvent bae &&
                                    bae.BuffID == 13017 &&
                                    Math.Abs(bae.AppliedDuration - 2000) <= ParserHelper.ServerDelayConstant &&
                                    bae.By == brae.To &&
                                    Math.Abs(brae.Time - bae.Time) <= ParserHelper.ServerDelayConstant
                                 );
                }
            ), // Signet of Midnight
        };


        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(49058, "Compounding Power", "2% per stack (8s) after creating an illusion ", DamageSource.NoPets, 2.0, DamageType.Power, DamageType.All, ParserHelper.Source.Mesmer, ByStack, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png", DamageModifierMode.All),
            new BuffDamageModifierTarget(738, "Fragility", "0.5% per stack vuln on target", DamageSource.NoPets, 0.5, DamageType.Power, DamageType.All, ParserHelper.Source.Mesmer, ByStack, "https://wiki.guildwars2.com/images/3/33/Fragility.png", DamageModifierMode.All),
            // Phantasmal Force would require activating buff tracking on minions, huge performance impact and some code impact
            // TOCHECK Superiority Complex
            new BuffDamageModifierTarget(Buff.NumberOfBoonsID, "Vicious Expression", "25% on boonless target",  DamageSource.All, 25.0, DamageType.Power, DamageType.All, ParserHelper.Source.Mesmer, ByAbsence, "https://wiki.guildwars2.com/images/f/f6/Confounding_Suggestions.png", 102321, 102389, DamageModifierMode.PvE),
            new BuffDamageModifierTarget(Buff.NumberOfBoonsID, "Vicious Expression", "15% on boonless target",  DamageSource.All, 15.0, DamageType.Power, DamageType.All, ParserHelper.Source.Mesmer, ByAbsence, "https://wiki.guildwars2.com/images/f/f6/Confounding_Suggestions.png", 102389, ulong.MaxValue, DamageModifierMode.All),
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            
                //signets
                new Buff("Signet of the Ether", 21751, ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/7/7a/Signet_of_the_Ether.png"),
                new Buff("Signet of Domination",10231, ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/3/3b/Signet_of_Domination.png"),
                new Buff("Signet of Illusions",10246, ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Signet_of_Illusions.png"),
                new Buff("Signet of Inspiration",10235, ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/ed/Signet_of_Inspiration.png"),
                new Buff("Signet of Midnight",10233, ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/24/Signet_of_Midnight.png"),
                new Buff("Signet of Humility",30739, ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b5/Signet_of_Humility.png"),
                //skills
                new Buff("Distortion",10243, ParserHelper.Source.Mesmer, BuffStackType.Queue, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Blur", 10335 , ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/2/22/Distortion.png"),
                new Buff("Mirror",10357, ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b8/Mirror.png"),
                new Buff("Echo",29664, ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/ce/Echo.png"),
                new Buff("Illusionary Counter",10278, ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e5/Illusionary_Counter.png"),
                new Buff("Illusionary Riposte",10279, ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/9/91/Illusionary_Riposte.png"),
                new Buff("Illusionary Leap",10353, ParserHelper.Source.Mesmer, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/1/18/Illusionary_Leap.png"),
                //traits
                new Buff("Fencer's Finesse", 30426 , ParserHelper.Source.Mesmer, BuffStackType.Stacking, 10, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e7/Fencer%27s_Finesse.png"),
                new Buff("Illusionary Defense",49099, ParserHelper.Source.Mesmer, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e0/Illusionary_Defense.png"),
                new Buff("Compounding Power",49058, ParserHelper.Source.Mesmer, BuffStackType.Stacking, 5, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/e/e5/Compounding_Power.png"),
                new Buff("Phantasmal Force", 44691 , ParserHelper.Source.Mesmer, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/5/5f/Mistrust.png"),
        };

    }
}
