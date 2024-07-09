using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifiersUtils;
using static GW2EIEvtcParser.EIData.ProfHelper;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    internal static class TempestHelper
    {
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            //new DamageCastFinder(30662, 30662, 10000), // "Feel the Burn!" - shockwave, fire aura indiscernable from the focus skill
            new EffectCastFinder(FeelTheBurn, EffectGUIDs.TempestFeelTheBurn)
                .UsingSrcSpecChecker(Spec.Tempest),
            new EffectCastFinder(EyeOfTheStormShout, EffectGUIDs.TempestEyeOfTheStorm1)
                .UsingSrcSpecChecker(Spec.Tempest),
        };

        internal static readonly List<DamageModifierDescriptor> OutgoingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(HarmoniousConduit, "Harmonious Conduit", "10% (4s) after overload", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Tempest, ByPresence, BuffImages.HarmoniousConduit, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.StartOfLife ,GW2Builds.October2019Balance),
            new BuffOnActorDamageModifier(TranscendentTempest, "Transcendent Tempest", "7% after overload", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, BuffImages.TranscendentTempest, DamageModifierMode.All)
                .WithBuilds(GW2Builds.October2019Balance, GW2Builds.August2022Balance),
            new BuffOnActorDamageModifier(TranscendentTempest, "Transcendent Tempest", "7% after overload", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, BuffImages.TranscendentTempest, DamageModifierMode.sPvPWvW)
                .WithBuilds(GW2Builds.August2022Balance),
            new BuffOnActorDamageModifier(TranscendentTempest, "Transcendent Tempest", "15% after overload", DamageSource.NoPets, 15.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, BuffImages.TranscendentTempest, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.August2022Balance, GW2Builds.SOTOReleaseAndBalance),
            new BuffOnActorDamageModifier(TranscendentTempest, "Transcendent Tempest", "25% after overload", DamageSource.NoPets, 25.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, BuffImages.TranscendentTempest, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.SOTOReleaseAndBalance),
            new BuffOnActorDamageModifier(TempestuousAria, "Tempestuous Aria", "7% after giving aura", DamageSource.NoPets, 7.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, BuffImages.TempestuousAria, DamageModifierMode.sPvPWvW)
                .WithBuilds(GW2Builds.June2023Balance),
            new BuffOnActorDamageModifier(TempestuousAria, "Tempestuous Aria", "10% after giving aura", DamageSource.NoPets, 10.0, DamageType.StrikeAndCondition, DamageType.All, Source.Tempest, ByPresence, BuffImages.TempestuousAria, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.June2023Balance, GW2Builds.March2024BalanceAndCerusLegendary),
            new BuffOnActorDamageModifier(TempestuousAria, "Tempestuous Aria (Strike)", "10% after giving aura", DamageSource.NoPets, 10.0, DamageType.Strike, DamageType.All, Source.Tempest, ByPresence, BuffImages.TempestuousAria, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
            new BuffOnActorDamageModifier(TempestuousAria, "Tempestuous Aria (Condition)", "5% after giving aura", DamageSource.NoPets, 5.0, DamageType.Condition, DamageType.All, Source.Tempest, ByPresence, BuffImages.TempestuousAria, DamageModifierMode.PvE)
                .WithBuilds(GW2Builds.March2024BalanceAndCerusLegendary),
        };

        internal static readonly List<DamageModifierDescriptor> IncomingDamageModifiers = new List<DamageModifierDescriptor>
        {
            new BuffOnActorDamageModifier(Protection, "Hardy Conduit", "20% extra protection effectiveness", DamageSource.NoPets, (0.604/0.67 - 1) * 100, DamageType.Strike, DamageType.All, Source.Tempest, ByPresence, BuffImages.HardyConduit, DamageModifierMode.All), // We only compute the added effectiveness
        };


        internal static readonly List<Buff> Buffs = new List<Buff>
        {
            new Buff("Rebound", Rebound, Source.Tempest, BuffClassification.Defensive, BuffImages.Rebound),
            new Buff("Harmonious Conduit", HarmoniousConduit, Source.Tempest, BuffClassification.Other, BuffImages.HarmoniousConduit)
                .WithBuilds(GW2Builds.StartOfLife, GW2Builds.October2019Balance),
            new Buff("Transcendent Tempest", TranscendentTempest, Source.Tempest, BuffClassification.Other, BuffImages.TranscendentTempest)
                .WithBuilds(GW2Builds.October2019Balance),
            new Buff("Static Charge", StaticCharge, Source.Tempest, BuffClassification.Offensive, BuffImages.OverloadAir),
            new Buff("Heat Sync", HeatSync, Source.Tempest, BuffClassification.Support, BuffImages.HeatSync),
            new Buff("Tempestuous Aria", TempestuousAria, Source.Tempest, BuffStackType.Queue, 9, BuffClassification.Other, BuffImages.TempestuousAria)
                .WithBuilds(GW2Builds.June2023Balance),
        };


        internal static void ComputeProfessionCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            Color color = Colors.Elementalist;

            // Overload Fire
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.TempestOverloadFire2, out IReadOnlyList<EffectEvent> overloadsFire))
            {
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.TempestOverloadFire1, out IReadOnlyList<EffectEvent> overloadsFireAux))
                {
                    var skill = new SkillModeDescriptor(player, Spec.Tempest, OverloadFire);
                    foreach (EffectEvent effect in overloadsFire)
                    {
                        if (!overloadsFireAux.Any(x => Math.Abs(x.Time - effect.Time) < ServerDelayConstant))
                        {
                            continue;
                        }
                        (long, long) lifespan = effect.ComputeLifespan(log, 5000);
                        AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 180, ParserIcons.EffectOverloadFire);
                    }
                }
            }

            // Overload Air
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.TempestOverloadAir2, out IReadOnlyList<EffectEvent> overloadsAir))
            {
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.TempestOverloadAir1, out IReadOnlyList<EffectEvent> overloadsAirAux))
                {
                    var skill = new SkillModeDescriptor(player, Spec.Tempest, OverloadAir);
                    foreach (EffectEvent effect in overloadsAir)
                    {
                        if (!overloadsAirAux.Any(x => Math.Abs(x.Time - effect.Time) < ServerDelayConstant))
                        {
                            continue;
                        }
                        (long, long) lifespan = effect.ComputeLifespan(log, 4000);
                        AddCircleSkillDecoration(replay, effect, color, skill, lifespan, 360, ParserIcons.EffectOverloadAir);
                    }
                }
            }
        }
    }
}
