using System.Numerics;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData;

public class CombatReplay
{
    //TODO(Rennorb) @perf: capacity
    internal readonly List<ParametricPoint3D> Positions = [];
    internal readonly List<ParametricPoint3D> PolledPositions = [];
    internal readonly List<ParametricPoint3D> Velocities = [];
    internal readonly List<ParametricPoint3D> Rotations = [];
    internal readonly List<ParametricPoint3D> PolledRotations = [];
    internal readonly List<Segment> Hidden = [];
    private long _start = -1;
    private long _end = -1;
    internal (long start, long end) TimeOffsets => (_start, _end);
    // actors
    internal readonly CombatReplayDecorationContainer Decorations;

    internal CombatReplay(ParsedEvtcLog log)
    {
        _start = log.FightData.FightStart;
        _end = log.FightData.FightEnd;
        //TODO(Rennorb) @perf: capacity
        Decorations = new(log.FightData.Logic.DecorationCache);
    }

    internal void Trim(long start, long end)
    {
        _start = Math.Max(start, _start);
        _end = Math.Max(_start, Math.Min(end, _end));
        PolledPositions.RemoveAll(x => x.Time < _start || x.Time > _end);
        PolledRotations.RemoveAll(x => x.Time < _start || x.Time > _end);
    }

    private static int UpdateVelocityIndex(List<ParametricPoint3D> velocities, long time, int currentIndex)
    {
        if (velocities.Count == 0)
        {
            return -1;
        }
        int res = Math.Max(currentIndex, 0);
        ParametricPoint3D cuvVelocity = velocities[res];
        while (res < velocities.Count && cuvVelocity.Time < time)
        {
            res++;
            if (res < velocities.Count)
            {
                cuvVelocity = velocities[res];
            }
        }
        return res - 1;
    }

    private void PositionPolling(int rate, long fightDuration, bool forcePolling)
    {
        List<ParametricPoint3D> positions = Positions;
        if (Positions.Count == 0 && forcePolling)
        {
            positions = [ new(int.MinValue, int.MinValue, 0, 0) ];
        }
        else if (Positions.Count == 0)
        {
            return;
        }
        
        int positionTableIndex = 0;
        int velocityTableIndex = 0;

        //TODO(Rennorb) @perf: reserve PolledPositions

        for (long t = Math.Min(0, rate * ((positions[0].Time / rate) - 1)); t < fightDuration; t += rate)
        {
            ParametricPoint3D pos = positions[positionTableIndex];
            if (t <= pos.Time)
            {
                PolledPositions.Add(pos.WithChangedTime(t));
            }
            else
            {
                if (positionTableIndex == positions.Count - 1)
                {
                    PolledPositions.Add(pos.WithChangedTime(t));
                }
                else
                {
                    ParametricPoint3D nextPos = positions[positionTableIndex + 1];
                    if (nextPos.Time < t)
                    {
                        positionTableIndex++;
                        t -= rate;
                    }
                    else
                    {
                        ParametricPoint3D last = PolledPositions.Last().Time > pos.Time ? PolledPositions.Last() : pos;
                        velocityTableIndex = UpdateVelocityIndex(Velocities, t, velocityTableIndex);
                        ParametricPoint3D velocity = default;
                        if (velocityTableIndex >= 0 && velocityTableIndex < Velocities.Count)
                        {
                            velocity = Velocities[velocityTableIndex];
                        }

                        if (nextPos.Time - last.Time > ArcDPSEnums.ArcDPSPollingRate + rate && velocity.Value.Length() < 1e-3)
                        {
                            PolledPositions.Add(last.WithChangedTime(t));
                        }
                        else
                        {
                            float ratio = (float)(t - last.Time) / (nextPos.Time - last.Time);
                            PolledPositions.Add(new(Vector3.Lerp(last.Value, nextPos.Value, ratio), t));
                        }

                    }
                }
            }
        }
        PolledPositions.RemoveAll(x => x.Time < 0); //TODO(Rennorb) @perf: inline before inserting
    }

    /// <summary>
    /// The method exists only to have the same amount of rotation as positions, it's easier to do it here than
    /// in javascript
    /// </summary>
    private void RotationPolling(int rate, long fightDuration)
    {
        if (Rotations.Count == 0)
        {
            return;
        }

        //TODO(Rennorb) @perf: reserve PolledPositions

        int rotationTableIndex = 0;
        for (int t = (int)Math.Min(0, rate * ((Rotations[0].Time / rate) - 1)); t < fightDuration; t += rate)
        {
            var rot = Rotations[rotationTableIndex];
            if (t <= rot.Time)
            {
                PolledRotations.Add(rot.WithChangedTime(t));
            }
            else
            {
                if (rotationTableIndex == Rotations.Count - 1)
                {
                    PolledRotations.Add(rot.WithChangedTime(t));
                }
                else
                {
                    ParametricPoint3D nextRot = Rotations[rotationTableIndex + 1];
                    if (nextRot.Time < t)
                    {
                        rotationTableIndex++;
                        t -= rate;
                    }
                    else
                    {
                        ParametricPoint3D last = PolledRotations.Last().Time > rot.Time ? PolledRotations.Last() : rot;
                        if (nextRot.Time - last.Time > ArcDPSEnums.ArcDPSPollingRate + rate)
                        {
                            PolledRotations.Add(last.WithChangedTime(t));
                        }
                        else
                        {
                            float ratio = (float)(t - last.Time) / (nextRot.Time - last.Time);
                            PolledRotations.Add(new(Vector3.Lerp(last.Value, nextRot.Value, ratio), t));
                        }

                    }
                }
            }
        }
        PolledRotations.RemoveAll(x => x.Time < 0); //TODO(Rennorb) @perf: inline before inserting
    }

    internal void PollingRate(long fightDuration, bool forcePositionPolling)
    {
        PositionPolling(ParserHelper.CombatReplayPollingRate, fightDuration, forcePositionPolling);
        RotationPolling(ParserHelper.CombatReplayPollingRate, fightDuration);
    }


    #region DEBUG EFFECTS
    //NOTE(Rennorb): Methods used for debugging purposes. Keep unused variables.
    #if DEBUG_EFFECTS

    internal static void DebugEffects(AbstractSingleActor actor, ParsedEvtcLog log, CombatReplay replay, HashSet<long> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
    {
        var effectEventsOnAgent = log.CombatData.GetEffectEventsByDst(actor.AgentItem)
            .Where(x => !knownEffectIDs.Contains(x.EffectID) && x.Time >= start && x.Time <= end);
        var effectGUIDsOnAgent = effectEventsOnAgent.Select(x => x.GUIDEvent.HexContentGUID).ToList();
        var effectGUIDsOnAgentDistinct = effectGUIDsOnAgent.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
        foreach (EffectEvent effectEvt in effectEventsOnAgent)
        {
            (long start, long end) lifeSpan = effectEvt.ComputeDynamicLifespan(log, effectEvt.Duration);
            if (lifeSpan.end - lifeSpan.start < 100)
            {
                lifeSpan.end = lifeSpan.start + 100;
            }
            if (effectEvt.IsAroundDst)
            {
                replay.Decorations.Add(new CircleDecoration(180, lifeSpan, Colors.Blue, 0.5, new AgentConnector(log.FindActor(effectEvt.Dst))));
            }
            else
            {

                replay.Decorations.Add(new CircleDecoration(180, lifeSpan, Colors.Blue, 0.5, new PositionConnector(effectEvt.Position)));
            }
        }
        var effectEventsByAgent = log.CombatData.GetEffectEventsBySrc(actor.AgentItem)
            .Where(x => !knownEffectIDs.Contains(x.EffectID) && x.Time >= start && x.Time <= end);
        var effectGUIDsByAgent = effectEventsByAgent.Select(x => x.GUIDEvent.HexContentGUID).ToList();
        var effectGUIDsByAgentDistinct = effectGUIDsByAgent.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
        foreach (EffectEvent effectEvt in effectEventsByAgent)
        {
            (long start, long end) lifeSpan = effectEvt.ComputeDynamicLifespan(log, effectEvt.Duration);
            if (lifeSpan.end - lifeSpan.start < 100)
            {
                lifeSpan.end = lifeSpan.start + 100;
            }
            if (effectEvt.IsAroundDst)
            {
                replay.Decorations.Add(new CircleDecoration(180, lifeSpan, Colors.Green, 0.5, new AgentConnector(log.FindActor(effectEvt.Dst))));
            }
            else
            {

                replay.Decorations.Add(new CircleDecoration(180, lifeSpan, Colors.Green, 0.5, new PositionConnector(effectEvt.Position)));
            }
        }
    }

    internal static void DebugUnknownEffects(ParsedEvtcLog log, CombatReplay replay, HashSet<long> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
    {
        var allEffectEvents = log.CombatData.GetEffectEvents()
            .Where(x => !knownEffectIDs.Contains(x.EffectID) && x.Src.IsUnamedSpecies() && x.Time >= start && x.Time <= end && x.EffectID > 0);
        var effectGUIDs = allEffectEvents.Select(x => x.GUIDEvent.HexContentGUID).ToList();
        var effectGUIDsDistinct = effectGUIDs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
        foreach (EffectEvent effectEvt in allEffectEvents)
        {
            (long start, long end) lifeSpan = effectEvt.ComputeDynamicLifespan(log, effectEvt.Duration);
            if (lifeSpan.end - lifeSpan.start < 100)
            {
                lifeSpan.end = lifeSpan.start + 100;
            }
            if (effectEvt.IsAroundDst)
            {
                replay.Decorations.Add(new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new AgentConnector(log.FindActor(effectEvt.Dst))));
            }
            else
            {

                replay.Decorations.Add(new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new PositionConnector(effectEvt.Position)));
            }
        }

    }

    internal static void DebugAllNPCEffects(ParsedEvtcLog log, CombatReplay replay, HashSet<long> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
    {
        var allEffectEvents = log.CombatData.GetEffectEvents()
            .Where(x => !knownEffectIDs.Contains(x.EffectID) && !x.Src.GetFinalMaster().IsPlayer && (!x.IsAroundDst || !x.Dst.GetFinalMaster().IsPlayer) && x.Time >= start && x.Time <= end && x.EffectID > 0);
        var effectGUIDs = allEffectEvents.Select(x => x.GUIDEvent.HexContentGUID).ToList();
        var effectGUIDsDistinct = effectGUIDs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
        foreach (EffectEvent effectEvt in allEffectEvents)
        {
            (long start, long end) lifeSpan = effectEvt.ComputeDynamicLifespan(log, effectEvt.Duration);
            if (lifeSpan.end - lifeSpan.start < 100)
            {
                lifeSpan.end = lifeSpan.start + 100;
            }
            if (effectEvt.IsAroundDst)
            {
                replay.Decorations.Add(new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new AgentConnector(log.FindActor(effectEvt.Dst))));
            }
            else
            {

                replay.Decorations.Add(new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new PositionConnector(effectEvt.Position)));
            }
        }
    }

    internal static void DebugAllEffects(ParsedEvtcLog log, CombatReplay replay, HashSet<long> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
    {
        var allEffectEvents = log.CombatData.GetEffectEvents()
            .Where(x => !knownEffectIDs.Contains(x.EffectID) && x.Time >= start && x.Time <= end && x.EffectID > 0);
        var effectGUIDs = allEffectEvents.Select(x => x.GUIDEvent.HexContentGUID).ToList();
        var effectGUIDsDistinct = effectGUIDs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
        foreach (EffectEvent effectEvt in allEffectEvents)
        {
            (long start, long end) lifeSpan = effectEvt.ComputeDynamicLifespan(log, effectEvt.Duration);
            if (lifeSpan.end - lifeSpan.start < 100)
            {
                lifeSpan.end = lifeSpan.start + 100;
            }
            if (effectEvt.IsAroundDst)
            {
                replay.Decorations.Add(new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new AgentConnector(log.FindActor(effectEvt.Dst))));
            }
            else
            {

                replay.Decorations.Add(new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new PositionConnector(effectEvt.Position)));
            }
        }
    }

    #endif
    #endregion DEBUG EFFECTS

    /// <summary>
    /// Add an overhead icon decoration
    /// </summary>
    /// <param name="segment">Lifespan interval</param>
    /// <param name="actor">actor to which the decoration will be attached to</param>
    /// <param name="icon">URL of the icon</param>
    /// <param name="pixelSize">Size in pixel of the icon</param>
    /// <param name="opacity">Opacity of the icon</param>
    internal void AddOverheadIcon(Segment segment, SingleActor actor, string icon, uint pixelSize = ParserHelper.CombatReplayOverheadDefaultSizeInPixel, float opacity = ParserHelper.CombatReplayOverheadDefaultOpacity)
    {
        Decorations.Add(new IconOverheadDecoration(icon, pixelSize, opacity, segment, new AgentConnector(actor)));
    }

    /// <summary>
    /// Add an overhead icon decoration
    /// </summary>
    /// <param name="segment">Lifespan interval</param>
    /// <param name="actor">actor to which the decoration will be attached to</param>
    /// <param name="icon">URL of the icon</param>
    /// <param name="rotation">rotation of the icon</param>
    /// <param name="pixelSize">Size in pixel of the icon</param>
    /// <param name="opacity">Opacity of the icon</param>
    internal void AddRotatedOverheadIcon(Segment segment, SingleActor actor, string icon, float rotation, uint pixelSize = ParserHelper.CombatReplayOverheadDefaultSizeInPixel, float opacity = ParserHelper.CombatReplayOverheadDefaultOpacity)
    {
        Decorations.Add(new IconOverheadDecoration(icon, pixelSize, opacity, segment, new AgentConnector(actor)).UsingRotationConnector(new AngleConnector(rotation)));
    }

    /// <summary>
    /// Add an overhead squad marker
    /// </summary>
    /// <param name="segment">Lifespan interval</param>
    /// <param name="actor">actor to which the decoration will be attached to</param>
    /// <param name="icon">URL of the icon</param>
    /// <param name="rotation">rotation of the icon</param>
    /// <param name="pixelSize">Size in pixel of the icon</param>
    /// <param name="opacity">Opacity of the icon</param>
    internal void AddRotatedOverheadMarkerIcon(Segment segment, SingleActor actor, string icon, float rotation, uint pixelSize = ParserHelper.CombatReplayOverheadDefaultSizeInPixel, float opacity = ParserHelper.CombatReplayOverheadDefaultOpacity)
    {
        Decorations.Add(new IconOverheadDecoration(icon, pixelSize, opacity, segment, new AgentConnector(actor)).UsingSquadMarker(true).UsingRotationConnector(new AngleConnector(rotation)));
    }

    /// <summary>
    /// Add overhead icon decorations
    /// </summary>
    /// <param name="segments">Lifespan intervals</param>
    /// <param name="actor">actor to which the decoration will be attached to</param>
    /// <param name="icon">URL of the icon</param>
    /// <param name="pixelSize">Size in pixel of the icon</param>
    /// <param name="opacity">Opacity of the icon</param>
    internal void AddOverheadIcons(IEnumerable<Segment> segments, SingleActor actor, string icon, uint pixelSize = ParserHelper.CombatReplayOverheadDefaultSizeInPixel, float opacity = ParserHelper.CombatReplayOverheadDefaultOpacity)
    {
        foreach (Segment segment in segments)
        {
            AddOverheadIcon(segment, actor, icon, pixelSize, opacity);
        }
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a copy using given extra parameters
    /// </summary>
    /// <param name="decoration"></param>
    /// <param name="filled"></param>
    /// <param name="growingEnd"></param>
    /// <param name="reverseGrowing"></param>
    internal void AddDecorationWithFilledWithGrowing(FormDecoration decoration, bool filled, long growingEnd, bool reverseGrowing = false)
    {
        Decorations.Add(decoration);
        Decorations.Add(decoration.Copy().UsingFilled(filled).UsingGrowingEnd(growingEnd, reverseGrowing));
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a copy using given extra parameters
    /// </summary>
    /// <param name="decoration"></param>
    /// <param name="growingEnd"></param>
    /// <param name="reverseGrowing"></param>
    internal void AddDecorationWithGrowing(FormDecoration decoration, long growingEnd, bool reverseGrowing = false)
    {
        Decorations.Add(decoration);
        Decorations.Add(decoration.Copy().UsingGrowingEnd(growingEnd, reverseGrowing));
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a copy using given extra parameters
    /// </summary>
    /// <param name="decoration"></param>
    /// <param name="filled"></param>
    internal void AddDecorationWithFilled(FormDecoration decoration, bool filled)
    {
        Decorations.Add(decoration);
        Decorations.Add(decoration.Copy().UsingFilled(filled));
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a non filled copy using given extra parameters
    /// </summary>
    /// <param name="decoration">Must be filled</param>
    /// <param name="color"></param>
    internal void AddDecorationWithBorder(FormDecoration decoration, string? color = null)
    {
        Decorations.Add(decoration);
        Decorations.Add(decoration.GetBorderDecoration(color));
    }
    /// <summary>
    /// Add the decoration twice, the 2nd one being a non filled copy using given extra parameters
    /// </summary>
    /// <param name="decoration">Must be filled</param>
    /// <param name="color"></param>
    /// <param name="opacity"></param>
    internal void AddDecorationWithBorder(FormDecoration decoration, Color color, double opacity)
    {
        AddDecorationWithBorder(decoration, color.WithAlpha(opacity).ToString(true));
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a non filled copy using given extra parameters
    /// </summary>
    /// <param name="decoration">Must be filled</param>
    /// <param name="color"></param>
    /// <param name="growingEnd"></param>
    /// <param name="reverseGrowing"></param>
    internal void AddDecorationWithBorder(FormDecoration decoration, long growingEnd, string? color = null, bool reverseGrowing = false)
    {
        Decorations.Add(decoration);
        Decorations.Add(decoration.GetBorderDecoration(color).UsingGrowingEnd(growingEnd, reverseGrowing));
    }

    /// <summary>
    /// Add the decoration twice, the 2nd one being a non filled copy using given extra parameters
    /// </summary>
    /// <param name="decoration">Must be filled</param>
    /// <param name="color"></param>
    /// <param name="growingEnd"></param>
    /// <param name="reverseGrowing"></param>
    internal void AddDecorationWithBorder(FormDecoration decoration, long growingEnd, Color color, double opacity, bool reverseGrowing = false)
    {
        AddDecorationWithBorder(decoration, growingEnd, color.WithAlpha(opacity).ToString(true), reverseGrowing);
    }

    /// <summary>
    /// Add tether decorations which src and dst are defined by tethers parameter using <see cref="BuffEvent"/>.
    /// </summary>
    /// <param name="tethers">Buff events of the tethers.</param>
    /// <param name="color">color of the tether</param>
    internal void AddTether(IEnumerable<BuffEvent> tethers, string color)
    {
        int tetherStart = 0;
        AgentItem src = ParserHelper._unknownAgent;
        AgentItem dst = ParserHelper._unknownAgent;
        foreach (BuffEvent tether in tethers)
        {
            if (tether is BuffApplyEvent)
            {
                tetherStart = (int)tether.Time;
                src = tether.By;
                dst = tether.To;
            }
            else if (tether is BuffRemoveAllEvent)
            {
                int tetherEnd = (int)tether.Time;
                if (src != ParserHelper._unknownAgent && dst != ParserHelper._unknownAgent)
                {
                    Decorations.Add(new LineDecoration((tetherStart, tetherEnd), color, new AgentConnector(dst), new AgentConnector(src)));
                    src = ParserHelper._unknownAgent;
                    dst = ParserHelper._unknownAgent;
                }
            }
        }
    }
    /// <summary>
    /// Add tether decorations which src and dst are defined by tethers parameter using <see cref="BuffEvent"/>.
    /// </summary>
    /// <param name="tethers">Buff events of the tethers.</param>
    /// <param name="color">color of the tether</param>
    internal void AddTether(IEnumerable<BuffEvent> tethers, Color color, double opacity)
    {
        AddTether(tethers, color.WithAlpha(opacity).ToString(true));
    }

    /// <summary>
    /// Add tether decorations which src and dst are defined by tethers parameter using <see cref="EffectEvent"/>.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="effect">Tether effect.</param>
    /// <param name="color">Color of the tether decoration.</param>
    /// <param name="duration">Manual set duration to use as override of the <paramref name="effect"/> duration.</param>
    /// <param name="overrideDuration">Wether to override the duration or not.</param>
    internal void AddTetherByEffectGUID(ParsedEvtcLog log, EffectEvent effect, string color, int duration = 0, bool overrideDuration = false)
    {
        if (!effect.IsAroundDst) { return; }

        (long, long) lifespan;
        if (overrideDuration == false)
        {
            lifespan = effect.ComputeLifespan(log, effect.Duration);
        }
        else
        {
            lifespan = (effect.Time, effect.Time + duration);
        }

        if (effect.Src != ParserHelper._unknownAgent && effect.Dst != ParserHelper._unknownAgent)
        {
            Decorations.Add(new LineDecoration(lifespan, color, new AgentConnector(effect.Dst), new AgentConnector(effect.Src)));
        }
    }

    /// <summary>
    /// Add tether decorations which src and dst are defined by tethers parameter using <see cref="EffectEvent"/>.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="effect">Tether effect.</param>
    /// <param name="color">Color of the tether decoration.</param>
    /// <param name="opacity">Opacity of the tether decoration.</param>
    /// <param name="duration">Manual set duration to use as override of the <paramref name="effect"/> duration.</param>
    /// <param name="overrideDuration">Wether to override the duration or not.</param>
    internal void AddTetherByEffectGUID(ParsedEvtcLog log, EffectEvent effect, Color color, double opacity, int duration = 0, bool overrideDuration = false)
    {
        AddTetherByEffectGUID(log, effect, color.WithAlpha(opacity).ToString(true), duration, overrideDuration);
    }

    /// <summary>
    /// Add tether decoration connecting a player to an agent.<br></br>
    /// The <paramref name="buffId"/> is sourced by an agent that isn't the one to tether to.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="player">The player to tether to <paramref name="toTetherAgentId"/>.</param>
    /// <param name="buffId">ID of the buff sourced by <paramref name="buffSrcAgentId"/>.</param>
    /// <param name="buffSrcAgentId">ID of the agent sourcing the <paramref name="buffId"/>. Either <see cref="ArcDPSEnums.TargetID"/> or <see cref="ArcDPSEnums.TrashID"/>.</param>
    /// <param name="toTetherAgentId">ID of the agent to tether to the <paramref name="player"/>. Either <see cref="ArcDPSEnums.TargetID"/> or <see cref="ArcDPSEnums.TrashID"/>.</param>
    /// <param name="color">Color of the tether.</param>
    /// <param name="firstAwareThreshold">Time threshold in case the agent spawns before the buff application.</param>
    internal void AddTetherByThirdPartySrcBuff(ParsedEvtcLog log, PlayerActor player, long buffId, int buffSrcAgentId, int toTetherAgentId, string color, int firstAwareThreshold = 2000)
    {
        var buffEvents = log.CombatData.GetBuffDataByIDByDst(buffId, player.AgentItem).Where(x => x.CreditedBy.IsSpecies(buffSrcAgentId)).ToList();
        var buffApplies = buffEvents.OfType<BuffApplyEvent>().ToList();
        var buffRemoves = buffEvents.OfType<BuffRemoveAllEvent>().ToList();
        var agentsToTether = log.AgentData.GetNPCsByID(toTetherAgentId).ToList();

        foreach (BuffApplyEvent buffApply in buffApplies)
        {
            BuffRemoveAllEvent remove = buffRemoves.FirstOrDefault(x => x.Time > buffApply.Time);
            long removalTime = remove != null ? remove.Time : log.FightData.LogEnd;
            (long, long) lifespan = (buffApply.Time, removalTime);

            foreach (AgentItem agent in agentsToTether)
            {
                if ((Math.Abs(agent.FirstAware - buffApply.Time) < firstAwareThreshold || agent.FirstAware >= buffApply.Time) && agent.FirstAware < removalTime)
                {
                    Decorations.Add(new LineDecoration(lifespan, color, new AgentConnector(agent), new AgentConnector(player)));
                }
            }
        }
    }
    /// <summary>
    /// Add tether decoration connecting a player to an agent.<br></br>
    /// The <paramref name="buffId"/> is sourced by an agent that isn't the one to tether to.
    /// </summary>
    /// <param name="log">The log.</param>
    /// <param name="player">The player to tether to <paramref name="toTetherAgentId"/>.</param>
    /// <param name="buffId">ID of the buff sourced by <paramref name="buffSrcAgentId"/>.</param>
    /// <param name="buffSrcAgentId">ID of the agent sourcing the <paramref name="buffId"/>. Either <see cref="ArcDPSEnums.TargetID"/> or <see cref="ArcDPSEnums.TrashID"/>.</param>
    /// <param name="toTetherAgentId">ID of the agent to tether to the <paramref name="player"/>. Either <see cref="ArcDPSEnums.TargetID"/> or <see cref="ArcDPSEnums.TrashID"/>.</param>
    /// <param name="color">Color of the tether.</param>
    /// <param name="opacity">Opacity of the tether.</param>
    /// <param name="firstAwareThreshold">Time threshold in case the agent spawns before the buff application.</param>
    internal void AddTetherByThirdPartySrcBuff(ParsedEvtcLog log, PlayerActor player, long buffId, int buffSrcAgentId, int toTetherAgentId, Color color, double opacity, int firstAwareThreshold = 2000)
    {
        AddTetherByThirdPartySrcBuff(log, player, buffId, buffSrcAgentId, toTetherAgentId, color.WithAlpha(opacity).ToString(true), firstAwareThreshold);
    }

    /// <summary>
    /// Adds a moving circle resembling a projectile from a <paramref name="startingPoint"/> to an <paramref name="endingPoint"/>.
    /// </summary>
    /// <param name="startingPoint">Starting position.</param>
    /// <param name="endingPoint">Ending position.</param>
    /// <param name="lifespan">Duration of the animation.</param>
    /// <param name="color">Color of the decoration.</param>
    /// <param name="opacity">Opacity of the color.</param>
    /// <param name="radius">Radius of the circle.</param>
    internal void AddProjectile(in Vector3 startingPoint, in Vector3 endingPoint, (long start, long end) lifespan, Color color, double opacity = 0.2, uint radius = 50)
    {
        AddProjectile(startingPoint, endingPoint, lifespan, color.WithAlpha(opacity).ToString(true), radius);
    }

    /// <summary>
    /// Adds a moving circle resembling a projectile from a <paramref name="startingPoint"/> to an <paramref name="endingPoint"/>.
    /// </summary>
    /// <param name="startingPoint">Starting position.</param>
    /// <param name="endingPoint">Ending position.</param>
    /// <param name="lifespan">Duration of the animation.</param>
    /// <param name="color">Color of the decoration.</param>
    /// <param name="radius">Radius of the circle.</param>
    internal void AddProjectile(in Vector3 startingPoint, in Vector3 endingPoint, (long start, long end) lifespan, string color, uint radius = 50)
    {
        if (startingPoint == null || endingPoint == null)
        {
            return;
        }
        var startPoint = new ParametricPoint3D(startingPoint, lifespan.start);
        var endPoint = new ParametricPoint3D(endingPoint, lifespan.end);
        var shootingCircle = new CircleDecoration(radius, lifespan, color, new InterpolationConnector([startPoint, endPoint]));
        Decorations.Add(shootingCircle);
    }

    /// <summary>
    /// Adds a non-filled growing circle resembling a shockwave.
    /// </summary>
    /// <param name="connector">Starting position point.</param>
    /// <param name="lifespan">Lifespan of the shockwave.</param>
    /// <param name="color">Color.</param>
    /// <param name="opacity">Opacity of the <paramref name="color"/>.</param>
    /// <param name="radius">Radius of the shockwave.</param>
    /// <remarks>Uses <see cref="GeographicalConnector"/> which allows us to use <see cref="AgentConnector"/> and <see cref="PositionConnector"/>.</remarks>
    internal void AddShockwave(GeographicalConnector connector, (long start, long end) lifespan, Color color, double opacity, uint radius)
    {
        AddShockwave(connector, lifespan, color.WithAlpha(opacity).ToString(true), radius);
    }

    /// <summary>
    /// Adds a non-filled growing circle resembling a shockwave.
    /// </summary>
    /// <param name="connector">Starting position point.</param>
    /// <param name="lifespan">Lifespan of the shockwave.</param>
    /// <param name="color">Color.</param>
    /// <param name="radius">Radius of the shockwave.</param>
    /// <remarks>Uses <see cref="GeographicalConnector"/> which allows us to use <see cref="AgentConnector"/> and <see cref="PositionConnector"/>.</remarks>
    internal void AddShockwave(GeographicalConnector connector, (long start, long end) lifespan, string color, uint radius)
    {
        Decorations.Add(new CircleDecoration(radius, lifespan, color, connector).UsingFilled(false).UsingGrowingEnd(lifespan.end));
    }

    /// <summary>
    /// Add hide based on buff's presence
    /// </summary>
    internal void AddHideByBuff(SingleActor actor, ParsedEvtcLog log, long buffID)
    {
        Hidden.AddRange(actor.GetBuffStatus(log, buffID, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0));
    }

    /// <summary>
    /// Adds concentric doughnuts.
    /// </summary>
    /// <param name="minRadius">Starting radius.</param>
    /// <param name="radiusIncrease">Radius increase for each concentric ring.</param>
    /// <param name="lifespan">Lifespan of the decoration.</param>
    /// <param name="position">Starting position.</param>
    /// <param name="color">Color of the rings.</param>
    /// <param name="initialOpacity">Starting opacity of the rings' color.</param>
    /// <param name="rings">Total number of rings.</param>
    /// <param name="inverted">Inverts the opacity direction.</param>
    internal void AddContrenticRings(uint minRadius, uint radiusIncrease, (long, long) lifespan, in Vector3 position, Color color, float initialOpacity = 0.5f, int rings = 8, bool inverted = false)
    {
        var positionConnector = new PositionConnector(position);

        for (int i = 1; i <= rings; i++)
        {
            uint maxRadius = minRadius + radiusIncrease;
            float opacity = inverted ? initialOpacity * i : initialOpacity / i;
            var circle = new DoughnutDecoration(minRadius, maxRadius, lifespan, color, opacity, positionConnector);
            AddDecorationWithBorder(circle, color, 0.2);
            minRadius = maxRadius;
        }
        
    }
}

