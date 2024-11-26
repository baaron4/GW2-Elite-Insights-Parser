
using System;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class DecimaTheStormsinger : MountBalrior
{
    public DecimaTheStormsinger(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(new List<Mechanic>()
        {
            new PlayerDstHitMechanic(Fluxlance, "Fluxlance", new MechanicPlotlySetting(Symbols.StarSquare, Colors.LightOrange), "Fluxlance.H", "Hit by Fluxlance (Single Orange Arrow)", "Fluxlance Hit", 0),
            new PlayerDstHitMechanic(FluxlanceFusillade, "Fluxlance Fusillade", new MechanicPlotlySetting(Symbols.StarDiamond, Colors.LightOrange), "FluxFusi.H", "Hit by Fluxlance Fusillade (Sequential Orange Arrows)", "Fluxlance Fusillade Hit", 0),
            new PlayerDstHitMechanic([FluxlanceSalvo1, FluxlanceSalvo2, FluxlanceSalvo3, FluxlanceSalvo4, FluxlanceSalvo5], "Fluxlance Salvo", new MechanicPlotlySetting(Symbols.StarDiamondOpen, Colors.LightOrange), "FluxSalvo.H", "Hit by Fluxlance Salvo (Simultaneous Orange Arrows)", "Fluxlance Salvo Hit", 0),
            new PlayerDstBuffApplyMechanic([TargetOrder1JW, TargetOrder2JW, TargetOrder3JW, TargetOrder4JW, TargetOrder5JW], "Target Order", new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.LightOrange), "FluxOrder.T", "Targeted by Fluxlance (Target Order)", "Fluxlance Target (Sequential)", 0),
            new PlayerDstBuffApplyMechanic(FluxlanceTargetBuff1, "Fluxlance", new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.Orange), "Fluxlance.T", "Targeted by Fluxlance", "Fluxlance Target", 0),
            new PlayerDstBuffApplyMechanic(FluxlanceRedArrowTargetBuff, "Fluxlance", new MechanicPlotlySetting(Symbols.StarTriangleDown, Colors.Red), "FluxRed.T", "Targeted by Fluxlance (Red Arrow)", "Fluxlance (Red Arrow)", 0),
        });
        Extension = "decima";
        Icon = EncounterIconDecima;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000002;
    }
    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayDecimaTheStormsinger,
                        (1602, 1602),
                        (-12668, 10500, -7900, 15268));
    }

    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)ArcDPSEnums.TargetID.Decima,
            //(int)ArcDPSEnums.TrashID.EnlightenedConduit,
        ];
    }

    protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
    {
        return
        [
            ArcDPSEnums.TrashID.GreenOrb1Player,
            ArcDPSEnums.TrashID.GreenOrb2Players,
            ArcDPSEnums.TrashID.GreenOrb3Players,
            ArcDPSEnums.TrashID.EnlightenedConduit,
            ArcDPSEnums.TrashID.DecimaBeamStart,
            ArcDPSEnums.TrashID.DecimaBeamEnd,
        ];
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(ThrummingPresenceBuff, ThrummingPresenceDamage),
        ];
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor decima = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Decima)) ?? throw new MissingKeyActorsException("Decima not found");
        phases[0].AddTarget(decima);
        if (!requirePhases)
        {
            return phases;
        }
        // Invul check
        phases.AddRange(GetPhasesByInvul(log, NovaShield, decima, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            if (i % 2 == 0)
            {
                phase.Name = "Split " + (i) / 2;
                phase.AddTarget(decima);
            }
            else
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(decima);
            }
        }
        return phases;
    }


    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        var lifespan = ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end);
        switch (target.ID)
        {
            case (int)ArcDPSEnums.TargetID.Decima:
                var casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd).ToList();

                AddRedRing(target, log, replay, casts, DecimaSpawnsConduitsP1);
                AddRedRing(target, log, replay, casts, DecimaSpawnsConduitsP2);
                AddRedRing(target, log, replay, casts, DecimaSpawnsConduitsP3);

                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaMainshockIndicator, out var mainshockSlices))
                {
                    foreach (EffectEvent effect in mainshockSlices)
                    {
                        long duration = 2300;
                        long growing = effect.Time + duration;
                        (long start, long end) lifespan2 = effect.ComputeLifespan(log, duration);
                        var rotation = new AngleConnector(effect.Rotation.Z + 90);
                        var slice = (PieDecoration)new PieDecoration(1200, 32, lifespan2, Colors.LightOrange, 0.4, new PositionConnector(effect.Position)).UsingRotationConnector(rotation);
                        replay.Decorations.AddWithBorder(slice, Colors.LightOrange, 0.6);
                    }
                }
                break;
            // TODO: find all greens and their proper sizes
            case (int)ArcDPSEnums.TrashID.GreenOrb1Player:
                replay.Decorations.AddOverheadIcon(lifespan, target, ParserIcons.TargetOrder1Overhead);
                //replay.Decorations.Add(new CircleDecoration(100, lifespan, Colors.Green, 0.3, new AgentConnector(target)));
                break;
            case (int)ArcDPSEnums.TrashID.GreenOrb2Players:
                replay.Decorations.AddOverheadIcon(lifespan, target, ParserIcons.TargetOrder2Overhead);
                //replay.Decorations.Add(new CircleDecoration(200, lifespan, Colors.Green, 0.3, new AgentConnector(target)));
                break;
            case (int)ArcDPSEnums.TrashID.GreenOrb3Players:
                replay.Decorations.AddOverheadIcon(lifespan, target, ParserIcons.TargetOrder3Overhead);
                //replay.Decorations.Add(new CircleDecoration(200, lifespan, Colors.Green, 0.3, new AgentConnector(target)));
                break;
            case (int)ArcDPSEnums.TrashID.EnlightenedConduit:
                if (log.CombatData.TryGetEffectEventsBySrcWithGUID(target.AgentItem, EffectGUIDs.DecimaEnlightenedConduitPurpleAoE, out var effects))
                {
                    // TODO: We need to find a way to handle sizing
                    foreach (var effect in effects)
                    {
                        var aoeLifeSpan = effect.ComputeDynamicLifespan(log, 1200000);
                        // Placeholder to indicate activated conduits, until we can find proper sizes
                        replay.Decorations.AddOverheadIcon(aoeLifeSpan, target, BuffImages.InvokeLightning);
                        //replay.Decorations.Add(new CircleDecoration(150, aoeLifeSpan, Colors.DarkPurple, 0.3, new PositionConnector(effect.Position)));
                    }
                }
                var walls = GetFilteredList(log.CombatData, DecimaConduitWallBuff, target, true, true);
                replay.Decorations.AddTether(walls, Colors.Purple, 0.4, 60, true);
                break;
            case (int)ArcDPSEnums.TrashID.DecimaBeamStart:
                SingleActor decima = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Decima)) ?? throw new MissingKeyActorsException("Decima not found");
                var decimaConnector = new AgentConnector(decima);
                const uint beamLength = 2900;
                const uint orangeBeamWidth = 80;
                const uint redBeamWidth = 160;
                var orangeBeams = GetFilteredList(log.CombatData, DecimaBeamTargeting, target.AgentItem, true, true);
                AddBeamWarning(log, target, replay, decima, DecimaBeamLoading, orangeBeamWidth, beamLength, orangeBeams.OfType<BuffApplyEvent>(), Colors.LightOrange);
                replay.Decorations.AddTetherWithCustomConnectors(log, orangeBeams, Colors.LightOrange, 0.5, 
                    (log, agent, start, end) =>
                    {
                        if (agent.TryGetCurrentInterpolatedPosition(log, start, out var pos))
                        {
                            return new PositionConnector(pos);
                        }
                        return null;
                    }, 
                    (log, agent, start, end) =>
                    {
                        return decimaConnector;
                    }, 
                    orangeBeamWidth, true);
                var redBeams = GetFilteredList(log.CombatData, DecimaRedBeamTargeting, target.AgentItem, true, true);
                AddBeamWarning(log, target, replay, decima, DecimaRedBeamLoading, redBeamWidth, beamLength, redBeams.OfType<BuffApplyEvent>(), Colors.Red);
                replay.Decorations.AddTetherWithCustomConnectors(log, redBeams, Colors.Red, 0.5,
                    (log, agent, start, end) =>
                    {
                        if (agent.TryGetCurrentInterpolatedPosition(log, start, out var pos))
                        {
                            return new PositionConnector(pos);
                        }
                        return null;
                    },
                    (log, agent, start, end) =>
                    {
                        return decimaConnector;
                    },
                    redBeamWidth, true);
                break;
            default:
                break;
        }
    }

    private static void AddBeamWarning(ParsedEvtcLog log, SingleActor target, CombatReplay replay, SingleActor attachActor, long buffID, uint beamWidth, uint beamLength, IEnumerable<BuffApplyEvent> beamFireds, Color color)
    {
        var beamWarnings = target.AgentItem.GetBuffStatus(log, buffID, log.FightData.FightStart, log.FightData.FightEnd);
        foreach (var beamWarning in beamWarnings)
        {
            if (beamWarning.Value > 0)
            {
                long start = beamWarning.Start;
                long end = beamFireds.FirstOrDefault(x => x.Time >= start)?.Time ?? beamWarning.End;
                var connector = (AgentConnector)new AgentConnector(attachActor).WithOffset(new(beamLength / 2, 0, 0), true);
                var rotationConnector = new AgentFacingConnector(target);
                replay.Decorations.Add(new RectangleDecoration(beamLength, beamWidth, (start, end), color, 0.2, connector).UsingRotationConnector(rotationConnector));
            }
        }
    }


    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);

        // Target Overhead
        // In phase 2 you get the Fluxlance Target Buff but also Target Order, in game only Target Order is displayed overhead, so we filter those out.
        var p2Targets = player.GetBuffStatus(log, [TargetOrder1JW, TargetOrder2JW, TargetOrder3JW, TargetOrder4JW, TargetOrder5JW], log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        var allTargets = player.GetBuffStatus(log, FluxlanceTargetBuff1, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
        var filtered = allTargets.Where(x => !p2Targets.Any(y => Math.Abs(x.Start - y.Start) < ServerDelayConstant));
        foreach (var segment in filtered)
        {
            replay.Decorations.AddOverheadIcon(segment, player, ParserIcons.TargetOverhead);
        }

        // Target Order Overhead
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder1JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder1Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder2JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder2Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder3JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder3Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder4JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder4Overhead);
        replay.Decorations.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder5JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder5Overhead);
    }

    /// <summary>
    /// The ring appears when Decima spawns conduits and ends when she starts the breakbar (Flux Nova).
    /// </summary>
    private static void AddRedRing(NPC target, ParsedEvtcLog log, CombatReplay replay, List<CastEvent> casts, long skillId)
    {
        var conduitsSpawn = casts.FirstOrDefault(x => x.SkillId == skillId);

        // Return only if P2 and P3 are null
        if (conduitsSpawn == null && skillId != DecimaSpawnsConduitsP1)
        {
            return;
        }

        // The spawn of the first conduits might be missing in the log, we use FightStart.
        long start = conduitsSpawn != null ? conduitsSpawn.Time : log.FightData.FightStart;
        var breakbar = casts.FirstOrDefault(x => x.SkillId == FluxNova && x.Time > start);
        long end = breakbar != null ? breakbar.Time : log.FightData.FightEnd;

        (long start, long end) lifespan = (start, end);
        replay.Decorations.Add(new CircleDecoration(700, lifespan, Colors.Red, 0.2, new AgentConnector(target)).UsingFilled(false));
    }
}
