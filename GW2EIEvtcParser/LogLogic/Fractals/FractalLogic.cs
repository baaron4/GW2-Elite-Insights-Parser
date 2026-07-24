using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Mechanic;
using static GW2EIEvtcParser.LogLogic.LogCategories;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicTimeUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity; 
using static GW2EIEvtcParser.MechanicIDs;

namespace GW2EIEvtcParser.LogLogic;

internal abstract class FractalLogic : LogLogic
{
    protected FractalLogic(int triggerID) : base(triggerID)
    {
        ParseMode = ParseModeEnum.Instanced5;
        SkillMode = SkillModeEnum.PvE;
        MechanicList.Add(new MechanicGroup(
        [
            new MechanicGroup([
                new MechanicGroup(
                    [
                        new PlayerDstBuffApplyMechanic(FluxBombBuff, Mech_FluxBombBuff, new (Symbols.Circle,Colors.Purple,10), new ("Flux", "Flux Bomb application","Flux Bomb"), Sev2),
                        new PlayerDstHealthDamageHitMechanic(FluxBombSkill, Mech_FluxBombHit, new (Symbols.CircleOpen,Colors.Purple,10), new ("Flux dmg", "Flux Bomb hit","Flux Bomb dmg"), Sev1), // No longer tracking damage
                    ]
                ),
                new SpawnMechanic((int)TargetID.FractalVindicator, Mech_FractalVindicator, new (Symbols.StarDiamondOpen,Colors.Black,10), new ("Vindicator", "Fractal Vindicator spawned","Vindicator spawn"), Sev3),
                new MechanicGroup(
                    [
                        new PlayerDstBuffApplyMechanic(DebilitatedToxicSickness, Mech_ToxicSicknessReceived, new (Symbols.TriangleUp, Colors.Pink, 10), new ("Debil.A", "Debilitated Application (Toxic Sickness)", "Received Debilitated"), Sev2),
                        new PlayerSrcEffectMechanic([EffectGUIDs.ToxicSicknessOldIndicator, EffectGUIDs.ToxicSicknessNewIndicator], Mech_ToxicSicknessApplied, new (Symbols.TriangleUpOpen, Colors.LightOrange, 10), new ("ToxSick.A", "Toxic Sickness Application", "Toxic Sickness Application"), Sev2),
                        new PlayerSrcBuffApplyMechanic(DebilitatedToxicSickness, Mech_ToxicSicknessHitOther, new (Symbols.TriangleLeftOpen, Colors.LightOrange, 10), new ("ToxSick.HitTo", "Hit another player with Toxic Sickness", "Toxic Sickness Hit To Player"), Sev1)
                            .UsingChecker((bae, log) => bae.To.IsPlayer),
                        new PlayerDstBuffApplyMechanic(DebilitatedToxicSickness, Mech_ToxicSicknessHitByOther, new (Symbols.TriangleRightOpen, Colors.LightOrange, 10), new ("ToxSick.HitBy", "Got hit by Toxic Sickness", "Toxic Sickness Hit By Player"), Sev1)
                            .UsingChecker((bae, log) => bae.By.IsPlayer),
                    ]
                ),
                /* Not trackable due to health % damage for now
                new PlayerDstHitMechanic(ToxicTrail, "Toxic Trail", new (Symbols.CircleOpenDot, Colors.DarkGreen, 10), "ToxTrail.H", "Hit by Toxic Trail", "Toxic Trail Hit", 0),
                new PlayerDstHitMechanic(ExplodeLastLaugh, "Explode", new (Symbols.CircleOpenDot, Colors.Orange, 10), "Explode.H", "Hit by Last Laugh Explode", "Last Laugh Hit", 0),
                new PlayerDstHitMechanic(WeBleedFireBig, "We Bleed Fire", new (Symbols.Star, Colors.LightRed, 10), "BleedFireB.H", "Hit by We Bleed Fire (Big)", "Big Bleed Fire Hit", 0),
                new PlayerDstHitMechanic(WeBleedFireSmall, "We Bleed Fire", new (Symbols.StarOpen, Colors.LightRed, 10), "BleedFireS.H", "Hit by We Bleed Fire (Small)", "Small Bleed FIre Hit", 0),
                 */
            ])
        ]));
        LogCategoryInformation.Category = LogCategory.Fractal;
        LogID |= LogIDs.LogMasks.FractalMask;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        if (IsInstance)
        {
            return base.GetPhases(log, requirePhases);
        }
        // generic method for fractals
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID)) ?? throw new MissingKeyActorsException("Main target of the log not found");
        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }
        phases.AddRange(GetSubPhasesByInvul(log, Determined762, mainTarget, false, true));
        for (int i = 1; i < phases.Count; i++)
        {
            phases[i].Name = "Phase " + i;
            phases[i].AddTarget(mainTarget, log);
        }
        return phases;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.FractalAvenger,
            TargetID.FractalVindicator,
            TargetID.TheMossman,
            TargetID.InspectorEllenKiel,
            TargetID.ChampionRabbit,
            TargetID.JadeMawTentacle,
            TargetID.AwakenedAbomination,
        ];
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, LogData logData, IReadOnlyCollection<AgentItem> playerAgents, LogData.LogSuccessHandler successHandler)
    {
        if (IsInstance)
        {
            successHandler.SetSuccess(true, GetFinalMapChangeTime(logData, combatData));
            return;
        }
        // check reward
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(GenericTriggerID)) ?? throw new MissingKeyActorsException("Main target of the log not found");
        RewardEvent? reward = combatData.GetRewardEvents().LastOrDefault(x => x.RewardType == RewardTypes.Daily && x.Time > logData.LogStart);
        HealthDamageEvent? lastDamageTaken = combatData.GetDamageTakenData(mainTarget.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Any(x.From.IsMasterOrSelf));
        if (lastDamageTaken != null)
        {
            if (reward != null && Math.Abs(lastDamageTaken.Time - reward.Time) < 1000)
            {
                successHandler.SetSuccess(true, Math.Min(lastDamageTaken.Time, reward.Time));
            }
            else
            {
                NoBouncyChestGenericCheckSucess(combatData, agentData, logData, playerAgents, successHandler);
            }
        } 
        else
        {
            successHandler.SetSuccess(false, mainTarget.LastAware);
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);

        // Toxic Sickness
        var toxicSicknessGUIDs = new List<GUID>() { EffectGUIDs.ToxicSicknessOldIndicator, EffectGUIDs.ToxicSicknessNewIndicator };
        foreach (var guid in toxicSicknessGUIDs)
        {
            if (log.CombatData.TryGetEffectEventsBySrcWithGUID(p.AgentItem, guid, out var toxicSickenss))
            {
                foreach (EffectEvent effect in toxicSickenss)
                {
                    (long start, long end) lifespan = (effect.Time, effect.Time + 4000);
                    (long start, long end) lifespanPuke = (lifespan.end, lifespan.end + 200);
                    var rotation = new AgentFacingConnector(p);
                    var connector = new AgentConnector(p);
                    replay.Decorations.Add(new PieDecoration(600, 36, lifespan, Colors.DarkGreen, 0.2, connector).UsingRotationConnector(rotation));
                    replay.Decorations.Add(new PieDecoration(600, 36, lifespanPuke, Colors.DarkGreen, 0.4, connector).UsingRotationConnector(rotation));
                }
            }
        }

        // Flux Bomb on selected player
        var fluxBombApplies = p.GetBuffStatus(log, FluxBombBuff).Where(x => x.Value > 0);
        foreach (Segment segment in fluxBombApplies)
        {
            replay.Decorations.AddWithGrowing(new CircleDecoration(120, segment, Colors.LightOrange, 0.2, new AgentConnector(p)), segment.End);
        }
    }

    protected static void AddFractalScaleEvent(ulong gw2Build, EvtcVersionEvent evtcVersion, List<CombatItem> combatData, IReadOnlyList<(ulong build, byte scale)> scales)
    {
        if (combatData.Any(x => x.IsStateChange == StateChange.FractalScale))
        {
            return;
        }
        var orderedScales = scales.OrderByDescending(x => x.build);
        foreach ((ulong build, byte scale) in orderedScales)
        {
            if (gw2Build >= build)
            {
                combatData.Add(new CombatItem(0, scale, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)StateChange.FractalScale, 0, 0, 0, 0, evtcVersion));
                break;
            }
        }
    }

    internal override LogData.StartStatus GetLogStartStatus(CombatData combatData, AgentData agentData, LogData logData)
    {
        if (IsInstance)
        {
            return base.GetLogStartStatus(combatData, agentData, logData);
        }
        if (TargetHPPercentUnderThreshold(GenericTriggerID, logData.LogStart, combatData, Targets))
        {
            return LogData.StartStatus.Late;
        }
        return LogData.StartStatus.Normal;
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);

        // Flux bomb on the ground
        if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.SmallFluxBomb, out var fluxBombs))
        {
            foreach (EffectEvent effect in fluxBombs)
            {
                (long start, long end) lifespan = effect.ComputeDynamicLifespan(log, 5000);
                var circle = new CircleDecoration(120, lifespan, Colors.Blue, 0.1, new PositionConnector(effect.Position));
                environmentDecorations.Add(circle);
                environmentDecorations.Add(circle.GetBorderDecoration(Colors.Red, 0.2));

                int pulseDuration = 1000;
                long pulse = lifespan.start + pulseDuration;
                long previousPulse = lifespan.start;
                for (int pulses = 0; pulses < 5; pulses++)
                {
                    environmentDecorations.Add(new CircleDecoration(120, (previousPulse, pulse), Colors.Blue, 0.1, new PositionConnector(effect.Position)).UsingGrowingEnd(pulse));
                    previousPulse = pulse;
                    pulse += pulseDuration;
                }
            }
        }
    }

    /// <summary>
    /// Add AoE decorations based on the distance to the caster. <br></br>
    /// Used for Caustic Barrage (Siax, Ensolyss) and Solar Bolt (Skorvald).
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="environmentDecorations">The decorations list.</param>
    /// <param name="effect">The <see cref="EffectGUIDs"/>.</param>
    /// <param name="target">The target casting.</param>
    /// <param name="distanceThreshold">Threshold distance of the effect from the caster.</param>
    /// <param name="onDistanceSuccessDuration">Duration of the AoE effects closer to the caster.</param>
    /// <param name="onDistanceFailDuration">Duration of the AoE effects farther away from the caster.</param>
    protected static void AddDistanceCorrectedOrbAoEDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations, GUID effect, TargetID target, double distanceThreshold, long onDistanceSuccessDuration, long onDistanceFailDuration)
    {
        if (!log.AgentData.TryGetFirstAgentItem(target, out var agent))
        {
            return;
        }

        if (log.CombatData.TryGetEffectEventsByGUID(effect, out var effectEvents))
        {
            foreach (EffectEvent effectEvent in effectEvents)
            {
                (long start, long end) lifespan = (effectEvent.Time, effectEvent.Time + effectEvent.Duration);
                // Correcting the duration of the effects for CTBS 45, based on the distance from the target casting the mechanic.
                if (effectEvent is EffectEventCBTS45)
                {
                    if (!agent.TryGetCurrentPosition(log, effectEvent.Time, out var agentPos))
                    {
                        continue;
                    }

                    var distance = (effectEvent.Position - agentPos.Value).Length();
                    if (distance < distanceThreshold)
                    {
                        lifespan.end = effectEvent.Time + onDistanceSuccessDuration;
                    }
                    else
                    {
                        lifespan.end = effectEvent.Time + onDistanceFailDuration;
                    }
                }
                environmentDecorations.Add(new CircleDecoration(100, lifespan, Colors.Orange, 0.3, new PositionConnector(effectEvent.Position)));
            }
        }
    }
}
