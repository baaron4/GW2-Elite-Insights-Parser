using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.EIData.SkillModeDescriptor;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class SpellbreakerHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new BuffGainCastFinder(SightBeyondSightSkill, SightBeyondSightBuff),
            new BuffGiveCastFinder(MagebaneTetherSkill, MagebaneTetherBuff).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),
            new DamageCastFinder(LossAversion, LossAversion).UsingOrigin(EIData.InstantCastFinder.InstantCastOrigin.Trait),

        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnFoeDamageModifier(NumberOfBoons, "Pure Strike (boons)", "7% crit damage", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, BuffImages.PureStrike, DamageModifierMode.All).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new BuffOnFoeDamageModifier(NumberOfBoons, "Pure Strike (boons)", "7% crit damage", DamageSource.NoPets, 7.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, BuffImages.PureStrike, DamageModifierMode.sPvPWvW).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.August2022Balance),
            new BuffOnFoeDamageModifier(NumberOfBoons, "Pure Strike (boons)", "7.5% crit damage", DamageSource.NoPets, 7.5, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, BuffImages.PureStrike, DamageModifierMode.PvE).UsingChecker((x, log) => x.HasCrit).WithBuilds(GW2Builds.August2022Balance),
            new BuffOnFoeDamageModifier(NumberOfBoons, "Pure Strike (no boons)", "14% crit damage", DamageSource.NoPets, 14.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByAbsence, BuffImages.PureStrike, DamageModifierMode.All).UsingChecker( (x, log) => x.HasCrit).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new BuffOnFoeDamageModifier(NumberOfBoons, "Pure Strike (no boons)", "14% crit damage", DamageSource.NoPets, 14.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByAbsence, BuffImages.PureStrike, DamageModifierMode.sPvPWvW).UsingChecker( (x, log) => x.HasCrit).WithBuilds(GW2Builds.August2022Balance),
            new BuffOnFoeDamageModifier(NumberOfBoons, "Pure Strike (no boons)", "15% crit damage", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByAbsence, BuffImages.PureStrike, DamageModifierMode.PvE).UsingChecker( (x, log) => x.HasCrit).WithBuilds(GW2Builds.August2022Balance),
            new BuffOnFoeDamageModifier(MagebaneTetherBuff, "Magebane Tether", "10% to tethered target", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, BuffImages.MagebaneTether, DamageModifierMode.PvEInstanceOnly).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return log.FindActor(dst).HasBuff(log, log.FindActor(src), MagebaneTetherBuff, x.Time);
            }).WithBuilds(GW2Builds.StartOfLife, GW2Builds.August2022Balance),
            new BuffOnFoeDamageModifier(MagebaneTetherBuff, "Magebane Tether", "15% to tethered target", DamageSource.NoPets, 15.0, DamageType.Strike, DamageType.All, Source.Spellbreaker, ByPresence, BuffImages.MagebaneTether, DamageModifierMode.PvEInstanceOnly).UsingChecker((x, log) => {
                AgentItem src = x.From;
                AgentItem dst = x.To;
                return log.FindActor(dst).HasBuff(log, log.FindActor(src), MagebaneTetherBuff, x.Time);
            }).WithBuilds(GW2Builds.August2022Balance),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Sight beyond Sight", SightBeyondSightBuff, Source.Spellbreaker, BuffClassification.Other, BuffImages.SightBeyondSight),
            new Buff("Full Counter", FullCounterBuff, Source.Spellbreaker, BuffClassification.Other, BuffImages.FullCounter),
            new Buff("Disenchantment", Disenchantment, Source.Spellbreaker, BuffClassification.Other, BuffImages.WindsOfDisenchantment),
            new Buff("Attacker's Insight", AttackersInsight, Source.Spellbreaker, BuffStackType.Stacking, 5, BuffClassification.Other, BuffImages.AttackersInsight),
            new Buff("Magebane Tether", MagebaneTetherBuff, Source.Spellbreaker, BuffStackType.Stacking, 25, BuffClassification.Other, BuffImages.MagebaneTether),
        };

        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Warrior;

            // Winds of Disenchantment
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.SpellbreakerWindsOfDisenchantment, out IReadOnlyList<EffectEvent> windsOfDisenchantments))
            {
                var skill = new SkillModeDescriptor(player, Spec.Spellbreaker, WindsOfDisenchantment, SkillModeCategory.Strip | SkillModeCategory.ProjectileManagement);
                foreach (EffectEvent effect in windsOfDisenchantments)
                {
                    (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                    AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, ParserIcons.EffectWindsOfDisenchantment);
                }
            }
        }
    }
}
