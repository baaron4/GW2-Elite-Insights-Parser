
using System;
using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class DecimaTheStormsinger : MountBalrior
{
    public DecimaTheStormsinger(int triggerID) : base(triggerID)
    {
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
            ArcDPSEnums.TrashID.GreenOrb1Person,
            ArcDPSEnums.TrashID.GreenOrb2Persons,
            ArcDPSEnums.TrashID.GreenOrb3Persons,
            ArcDPSEnums.TrashID.EnlightenedConduit,
            ArcDPSEnums.TrashID.DecimaBeamEnd,
            ArcDPSEnums.TrashID.DecimaBeamStart,
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
                break;
            // TODO: find all greens and their proper sizes
            case (int)ArcDPSEnums.TrashID.GreenOrb1Person:
                replay.AddOverheadIcon(lifespan, target, ParserIcons.TargetOrder1Overhead);
                //replay.Decorations.Add(new CircleDecoration(100, lifespan, Colors.Green, 0.3, new AgentConnector(target)));
                break;
            case (int)ArcDPSEnums.TrashID.GreenOrb2Persons:
                replay.AddOverheadIcon(lifespan, target, ParserIcons.TargetOrder2Overhead);
                //replay.Decorations.Add(new CircleDecoration(200, lifespan, Colors.Green, 0.3, new AgentConnector(target)));
                break;
            case (int)ArcDPSEnums.TrashID.GreenOrb3Persons:
                replay.AddOverheadIcon(lifespan, target, ParserIcons.TargetOrder3Overhead);
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
                        replay.AddOverheadIcon(aoeLifeSpan, target, BuffImages.InvokeLightning);
                        //replay.Decorations.Add(new CircleDecoration(150, aoeLifeSpan, Colors.DarkPurple, 0.3, new PositionConnector(effect.Position)));
                    }
                }
                var walls = GetFilteredList(log.CombatData, DecimaConduitWallBuff, target, true, true);
                replay.AddTether(walls, Colors.Purple, 0.4, 60, true);
                break;
            case (int)ArcDPSEnums.TrashID.DecimaBeamStart:
                SingleActor decima = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Decima)) ?? throw new MissingKeyActorsException("Decima not found");
                var decimaConnector = new AgentConnector(decima);
                const uint beamLength = 2900;
                const uint yellowBeamWidth = 80;
                const uint redBeamWidth = 160;
                var yellowBeams = GetFilteredList(log.CombatData, DecimaBeamTargeting, target.AgentItem, true, true);
                AddBeamWarning(log, target, replay, decima, DecimaBeamLoading, yellowBeamWidth, beamLength, yellowBeams.OfType<BuffApplyEvent>(), Colors.Yellow);
                replay.addTetherWithCustomConnectors(log, yellowBeams, Colors.Yellow, 0.5, 
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
                    yellowBeamWidth, true);
                var redBeams = GetFilteredList(log.CombatData, DecimaRedBeamTargeting, target.AgentItem, true, true);
                AddBeamWarning(log, target, replay, decima, DecimaRedBeamLoading, redBeamWidth, beamLength, redBeams.OfType<BuffApplyEvent>(), Colors.Red);
                replay.addTetherWithCustomConnectors(log, redBeams, Colors.Red, 0.5,
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

        replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder1JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder1Overhead);
        replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder2JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder2Overhead);
        replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder3JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder3Overhead);
        replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder4JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder4Overhead);
        replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder5JW, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder5Overhead);
    }
}
