using System.Collections.Generic;
using GW2EIEvtcParser.EIData.Buffs;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class RenegadeHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(LegendaryRenegadeStanceSkill, LegendaryRenegadeStanceBuff), // Legendary Renegade Stance
            new DamageCastFinder(CallOfTheRenegade, CallOfTheRenegade), // Call of the Renegade
            new EffectCastFinder(OrdersFromAbove, EffectGUIDs.RenegadeOrdersFromAbove).UsingChecker((evt, combatData, agentData, skillData) => evt.Src.Spec == Spec.Renegade)
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(KallasFervor, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, BuffImages.KallasFervor, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(ImprovedKallasFervor, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, BuffImages.KallasFervor, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(KallasFervor, "Kalla's Fervor", "2% per stack", DamageSource.NoPets, 2.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, BuffImages.KallasFervor, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(ImprovedKallasFervor, "Improved Kalla's Fervor", "3% per stack", DamageSource.NoPets, 3.0, DamageType.StrikeAndConditionAndLifeLeech, DamageType.All, Source.Renegade, ByStack, BuffImages.KallasFervor, DamageModifierMode.PvE).WithBuilds(GW2Builds.May2021Balance),
            new SkillDamageModifier("Soulcleave's Summit", "per hit (no ICD)", SoulcleavesSummitBuff, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, BuffImages.SoulcleavesSummit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.May2021Balance),
            new SkillDamageModifier("Soulcleave's Summit", "per hit (1s ICD per target)", SoulcleavesSummitBuff, DamageSource.NoPets, DamageType.Power, DamageType.All, Source.Common, BuffImages.SoulcleavesSummit, DamageModifierMode.All)
                .WithBuilds(GW2Builds.May2021Balance),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(BreakrazorsBastionBuff, "Breakrazor's Bastion", "-50%", DamageSource.NoPets, -50.0, DamageType.Condition, DamageType.All, Source.Common, ByPresence, BuffImages.BreakrazorsBastion, DamageModifierMode.All),
            new BuffOnActorDamageModifier(KallasFervor, "Righteous Rebel", "-33%", DamageSource.NoPets, -33.0, DamageType.Condition, DamageType.All, Source.Renegade, ByPresence, BuffImages.RighteousRebel, DamageModifierMode.All).WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2018Balance),
            new BuffOnActorDamageModifier(KallasFervor, "Righteous Rebel", "-7% per stack", DamageSource.NoPets, -7.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, BuffImages.RighteousRebel, DamageModifierMode.All).WithBuilds(GW2Builds.October2018Balance, GW2Builds.May2021Balance),
            new BuffOnActorDamageModifier(KallasFervor, "Righteous Rebel", "-4% per stack", DamageSource.NoPets, -4.0, DamageType.Condition, DamageType.All, Source.Renegade, ByStack, BuffImages.RighteousRebel, DamageModifierMode.All).WithBuilds(GW2Builds.May2021Balance),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Legendary Renegade Stance", LegendaryRenegadeStanceBuff, Source.Renegade, BuffClassification.Other, BuffImages.LegendaryRenegadeStance),
            new Buff("Breakrazor's Bastion", BreakrazorsBastionBuff, Source.Renegade, BuffClassification.Defensive, BuffImages.BreakrazorsBastion),
            new Buff("Razorclaw's Rage", RazorclawsRageBuff, Source.Renegade, BuffClassification.Offensive, BuffImages.RazorclawsRage),
            new Buff("Soulcleave's Summit", SoulcleavesSummitBuff, Source.Renegade, BuffClassification.Offensive, BuffImages.SoulcleavesSummit),
            new Buff("Kalla's Fervor", KallasFervor, Source.Renegade, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.KallasFervor),
            new Buff("Improved Kalla's Fervor", ImprovedKallasFervor, Source.Renegade, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.KallasFervor),
        };

        private static HashSet<int> Minions = new HashSet<int>()
        {
            (int)MinionID.JasRazorclaw,
            (int)MinionID.ViskIcerazor,
            (int)MinionID.KusDarkrazor,
            (int)MinionID.EraBreakrazor,
            (int)MinionID.OfelaSoulcleave,
        };
        internal static bool IsKnownMinionID(int id)
        {
            return Minions.Contains(id);
        }
    }
}
