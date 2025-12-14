using System.Drawing;
using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EIData;

public class CombatReplay
{

    //TODO_PERF(Rennorb) @capacity
    internal IReadOnlyList<ParametricPoint3D> Positions => _Positions;
    internal IReadOnlyList<ParametricPoint3D> PolledPositions => _PolledPositions;
    internal IReadOnlyList<ParametricPoint3D> Velocities => _Velocities;
    internal IReadOnlyList<ParametricPoint3D> Rotations => _Rotations;
    internal IReadOnlyList<ParametricPoint3D> PolledRotations => _PolledRotations;

    protected List<ParametricPoint3D> _Positions = [];
    protected ParametricPoint3D[] _PolledPositions = [];
    protected List<ParametricPoint3D> _Velocities = [];
    protected List<ParametricPoint3D> _Rotations = [];
    protected ParametricPoint3D[] _PolledRotations = [];

    internal readonly List<Segment> Hidden = [];
    private long _start = -1;
    private long _end = -1;
    internal (long start, long end) TimeOffsets => (_start, _end);
    // actors
    internal readonly CombatReplayDecorationContainer Decorations;

    internal CombatReplay(ParsedEvtcLog log)
    {
        _start = log.LogData.LogStart;
        _end = log.LogData.LogEnd;
        //TODO_PERF(Rennorb) @capacity
        Decorations = new(log.LogData.Logic.DecorationCache);
    }

    internal virtual void AddPosition(ParametricPoint3D position)
    {
        _Positions.Add(position);
    }

    internal virtual void AddVelocity(ParametricPoint3D velocity)
    {
        _Velocities.Add(velocity);
    }

    internal virtual void AddRotation(ParametricPoint3D rotation)
    {
        _Rotations.Add(rotation);
    }

    internal virtual void CopyFrom(CombatReplay other)
    {
        _Positions = other.Positions.ToList();
        _PolledPositions = other.PolledPositions.ToArray();
        _Rotations = other.Rotations.ToList();
        _PolledRotations = other.PolledRotations.ToArray();
        _Velocities = other.Velocities.ToList();
    }

    internal virtual void CopyPositionsFrom(CombatReplay other)
    {
    }

    internal void Trim(long start, long end)
    {
        _start = Math.Max(start, _start);
        _end = Math.Max(_start, Math.Min(end, _end));
        if (_PolledPositions.Length > 0 && (_PolledPositions[0].Time < _start || _PolledPositions[^1].Time > _end))
        {
            _PolledPositions = _PolledPositions.Where(x => x.Time >= _start && x.Time <= _end).ToArray();
        }
        if (_PolledRotations.Length > 0 && (_PolledRotations[0].Time < _start || _PolledRotations[^1].Time > _end))
        {
            _PolledRotations = _PolledRotations.Where(x => x.Time >= _start && x.Time <= _end).ToArray();
        }
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

    protected void HandlePosition(long t, ref int polledPositionTableIndex, ref int positionTableIndex, ref int velocityTableIndex, int rate)
    {
        ParametricPoint3D pos = _Positions[positionTableIndex];
        if (t <= pos.Time)
        {
            _PolledPositions[polledPositionTableIndex++] = pos.WithChangedTime(t);
        }
        else
        {
            if (positionTableIndex == _Positions.Count - 1)
            {
                _PolledPositions[polledPositionTableIndex++] = pos.WithChangedTime(t);
            }
            else
            {
                ParametricPoint3D nextPos = _Positions[positionTableIndex + 1];
                if (nextPos.Time < t)
                {
                    positionTableIndex++;
                    HandlePosition(t, ref polledPositionTableIndex, ref positionTableIndex, ref velocityTableIndex, rate);
                }
                else
                {
                    ParametricPoint3D last = _PolledPositions.Last().Time > pos.Time ? _PolledPositions.Last() : pos;
                    velocityTableIndex = UpdateVelocityIndex(_Velocities, t, velocityTableIndex);
                    ParametricPoint3D velocity = default;
                    if (velocityTableIndex >= 0 && velocityTableIndex < Velocities.Count)
                    {
                        velocity = Velocities[velocityTableIndex];
                    }

                    if (nextPos.Time - last.Time > ArcDPSPollingRate + rate && velocity.XYZ.Length() < 1e-3)
                    {
                        _PolledPositions[polledPositionTableIndex++] = pos.WithChangedTime(t);
                    }
                    else
                    {
                        float ratio = (float)(t - last.Time) / (nextPos.Time - last.Time);
                        _PolledPositions[polledPositionTableIndex++] = new(Vector3.Lerp(last.XYZ, nextPos.XYZ, ratio), t);
                    }

                }
            }
        }
    }

    protected void HandleRotation(long t, ref int polledRotationTableIndex, ref int rotationTableIndex, int rate)
    {
        var rot = _Rotations[rotationTableIndex];
        if (t <= rot.Time)
        {
            _PolledRotations[polledRotationTableIndex++] = rot.WithChangedTime(t);
        }
        else
        {
            if (rotationTableIndex == _Rotations.Count - 1)
            {
                _PolledRotations[polledRotationTableIndex++] = rot.WithChangedTime(t);
            }
            else
            {
                ParametricPoint3D nextRot = _Rotations[rotationTableIndex + 1];
                if (nextRot.Time < t)
                {
                    rotationTableIndex++;
                    HandleRotation(t, ref polledRotationTableIndex, ref rotationTableIndex, rate);
                }
                else
                {
                    ParametricPoint3D last = _PolledRotations.Last().Time > rot.Time ? _PolledRotations.Last() : rot;
                    if (nextRot.Time - last.Time > ArcDPSPollingRate + rate)
                    {
                        _PolledRotations[polledRotationTableIndex++] = rot.WithChangedTime(t);
                    }
                    else
                    {
                        float ratio = (float)(t - last.Time) / (nextRot.Time - last.Time);
                        _PolledRotations[polledRotationTableIndex++] = new(Vector3.Lerp(last.XYZ, nextRot.XYZ, ratio), t);
                    }

                }
            }
        }
    }

    internal virtual void PollingRate(long logDuration, bool forcePolling)
    {
        if (_Positions.Count == 0 && forcePolling)
        {
            _Positions = [new(int.MinValue, int.MinValue, 0, 0)];
        }
        else if (_Positions.Count == 0)
        {
            return;
        }

        int rate = ParserHelper.CombatReplayPollingRate;

        bool doRotation = _Rotations.Count > 0;

        int positionTableIndex = 0;
        int polledPositionTableIndex = 0;
        int velocityTableIndex = 0;
        int rotationTableIndex = 0;
        int polledRotationTableIndex = 0;

        long posStartOffset = Math.Min(0, rate * ((_Positions[0].Time / rate) - 1));
        long rotStartOffset = doRotation ? Math.Min(0, rate * ((_Rotations[0].Time / rate) - 1)) : 0;
        long startOffset = Math.Min(posStartOffset, rotStartOffset);
        int capacity = (int)(logDuration - startOffset) / rate + 1;
        _PolledPositions = new ParametricPoint3D[capacity];
        _PolledRotations = doRotation ? new ParametricPoint3D[capacity] : [];

        if (doRotation)
        {
            for (long t = startOffset; t < logDuration; t += rate)
            {
                HandlePosition(t, ref polledPositionTableIndex, ref positionTableIndex, ref velocityTableIndex, rate);
                HandleRotation(t, ref polledRotationTableIndex, ref rotationTableIndex, rate);
            }
        } 
        else
        {
            for (long t = startOffset; t < logDuration; t += rate)
            {
                HandlePosition(t, ref polledPositionTableIndex, ref positionTableIndex, ref velocityTableIndex, rate);
            }
        }
    }

#if DEBUG
    #region DEBUG EFFECTS
    private static uint DebugRadius = 100;
    private static uint DebugOpeningAngle = 120;
    //NOTE(Rennorb): Methods used for debugging purposes. Keep unused variables.
    internal static void DebugEffects(SingleActor actor, ParsedEvtcLog log, CombatReplayDecorationContainer decorations, HashSet<GUID> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
    {
        var effectEventsOnAgent = log.CombatData.GetEffectEventsByDst(actor.AgentItem)
            .Where(x => !knownEffectIDs.Contains(x.GUIDEvent.ContentGUID) && x.Time >= start && x.Time <= end)
            .ToList();
        var effectGUIDsOnAgent = effectEventsOnAgent.Select(x => x.GUIDEvent).ToList();
        var effectGUIDsOnAgentDistinct = effectGUIDsOnAgent.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
        foreach (EffectEvent effectEvt in effectEventsOnAgent)
        {
            (long start, long end) lifeSpan = effectEvt.ComputeDynamicLifespan(log, effectEvt.Duration);
            if (lifeSpan.end - lifeSpan.start < 100)
            {
                lifeSpan.end = lifeSpan.start + 100;
            }
            GeographicalConnector positionConnector;
            Color color;
            if (effectEvt.IsAroundDst)
            {
                var dstActor = log.FindActor(effectEvt.Dst);
                if (log.FriendlyAgents.Contains(dstActor.AgentItem) || log.LogData.Logic.TargetAgents.Contains(dstActor.AgentItem) || log.LogData.Logic.TrashMobAgents.Contains(dstActor.AgentItem))
                {
                    color = Colors.Teal;
                    positionConnector = new AgentConnector(dstActor);
                }
                else
                {
                    if (dstActor.TryGetCurrentPosition(log, effectEvt.Time, out var position))
                    {
                        color = Colors.DarkBlue;
                        positionConnector = new PositionConnector(position);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                color = Colors.LightBlue;
                positionConnector = new PositionConnector(effectEvt.Position);
            }
            if (effectEvt.Rotation.Z != 0)
            {
                decorations.Add(new PieDecoration(DebugRadius, DebugOpeningAngle, lifeSpan, color, 0.5, positionConnector).UsingRotationConnector(new AngleConnector(effectEvt.Rotation.Z)));
            }
            else
            {
                decorations.Add(new CircleDecoration(DebugRadius, lifeSpan, color, 0.5, positionConnector));
            }
        }
        var effectEventsByAgent = log.CombatData.GetEffectEventsBySrc(actor.AgentItem)
            .Where(x => !knownEffectIDs.Contains(x.GUIDEvent.ContentGUID) && x.Time >= start && x.Time <= end)
            .ToList();
        var effectGUIDsByAgent = effectEventsByAgent.Select(x => x.GUIDEvent).ToList();
        var effectGUIDsByAgentDistinct = effectGUIDsByAgent.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
        foreach (EffectEvent effectEvt in effectEventsByAgent)
        {
            (long start, long end) lifeSpan = effectEvt.ComputeDynamicLifespan(log, effectEvt.Duration);
            if (lifeSpan.end - lifeSpan.start < 100)
            {
                lifeSpan.end = lifeSpan.start + 100;
            }
            GeographicalConnector positionConnector;
            Color color;
            if (effectEvt.IsAroundDst)
            {
                var dstActor = log.FindActor(effectEvt.Dst);
                if (log.FriendlyAgents.Contains(dstActor.AgentItem) || log.LogData.Logic.TargetAgents.Contains(dstActor.AgentItem) || log.LogData.Logic.TrashMobAgents.Contains(dstActor.AgentItem))
                {
                    color = Colors.GreenishYellow;
                    positionConnector = new AgentConnector(dstActor);
                }
                else
                {
                    if (dstActor.TryGetCurrentPosition(log, effectEvt.Time, out var position))
                    {
                        color = Colors.DarkGreen;
                        positionConnector = new PositionConnector(position);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                color = Colors.Green;
                positionConnector = new PositionConnector(effectEvt.Position);
            }
            if (effectEvt.Rotation.Z != 0)
            {
                decorations.Add(new PieDecoration(DebugRadius, DebugOpeningAngle, lifeSpan, color, 0.5, positionConnector).UsingRotationConnector(new AngleConnector(effectEvt.Rotation.Z)));
            }
            else
            {
                decorations.Add(new CircleDecoration(DebugRadius, lifeSpan, color, 0.5, positionConnector));
            }
        }
    }

    internal static void DebugUnknownEffects(ParsedEvtcLog log, CombatReplayDecorationContainer decorations, HashSet<GUID> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
    {
        var allEffectEvents = log.CombatData.GetEffectEvents()
            .Where(x => !knownEffectIDs.Contains(x.GUIDEvent.ContentGUID) && x.Src.IsUnamedSpecies() && x.Time >= start && x.Time <= end && x.EffectID > 0)
            .ToList();
        var effectGUIDs = allEffectEvents.Select(x => x.GUIDEvent).ToList();
        var effectGUIDsDistinct = effectGUIDs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
        foreach (EffectEvent effectEvt in allEffectEvents)
        {
            (long start, long end) lifeSpan = effectEvt.ComputeDynamicLifespan(log, effectEvt.Duration);
            if (lifeSpan.end - lifeSpan.start < 100)
            {
                lifeSpan.end = lifeSpan.start + 100;
            }
            GeographicalConnector positionConnector;
            Color color;
            if (effectEvt.IsAroundDst)
            {
                var dstActor = log.FindActor(effectEvt.Dst);
                if (log.FriendlyAgents.Contains(dstActor.AgentItem) || log.LogData.Logic.TargetAgents.Contains(dstActor.AgentItem) || log.LogData.Logic.TrashMobAgents.Contains(dstActor.AgentItem))
                {
                    color = Colors.Teal;
                    positionConnector = new AgentConnector(dstActor);
                }
                else
                {
                    if (dstActor.TryGetCurrentPosition(log, effectEvt.Time, out var position))
                    {
                        color = Colors.DarkBlue;
                        positionConnector = new PositionConnector(position);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                color = Colors.LightBlue;
                positionConnector = new PositionConnector(effectEvt.Position);
            }
            if (effectEvt.Rotation.Z != 0)
            {
                decorations.Add(new PieDecoration(DebugRadius, DebugOpeningAngle, lifeSpan, color, 0.5, positionConnector).UsingRotationConnector(new AngleConnector(effectEvt.Rotation.Z)));
            }
            else
            {
                decorations.Add(new CircleDecoration(DebugRadius, lifeSpan, color, 0.5, positionConnector));
            }
        }

    }

    internal static void DebugAllNPCEffects(ParsedEvtcLog log, CombatReplayDecorationContainer decorations, HashSet<GUID> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
    {
        var allEffectEvents = log.CombatData.GetEffectEvents()
            .Where(x => !knownEffectIDs.Contains(x.GUIDEvent.ContentGUID) && !x.Src.GetFinalMaster().IsPlayer && (!x.IsAroundDst || !x.Dst.GetFinalMaster().IsPlayer) && x.Time >= start && x.Time <= end && x.EffectID > 0)
            .ToList();
        var effectGUIDs = allEffectEvents.Select(x => x.GUIDEvent).ToList();
        var effectGUIDsDistinct = effectGUIDs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
        foreach (EffectEvent effectEvt in allEffectEvents)
        {
            (long start, long end) lifeSpan = effectEvt.ComputeDynamicLifespan(log, effectEvt.Duration);
            if (lifeSpan.end - lifeSpan.start < 100)
            {
                lifeSpan.end = lifeSpan.start + 100;
            }
            GeographicalConnector positionConnector;
            Color color;
            if (effectEvt.IsAroundDst)
            {
                var dstActor = log.FindActor(effectEvt.Dst);
                if (log.FriendlyAgents.Contains(dstActor.AgentItem) || log.LogData.Logic.TargetAgents.Contains(dstActor.AgentItem) || log.LogData.Logic.TrashMobAgents.Contains(dstActor.AgentItem))
                {
                    color = Colors.Teal;
                    positionConnector = new AgentConnector(dstActor);
                }
                else
                {
                    if (dstActor.TryGetCurrentPosition(log, effectEvt.Time, out var position))
                    {
                        color = Colors.DarkBlue;
                        positionConnector = new PositionConnector(position);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                color = Colors.LightBlue;
                positionConnector = new PositionConnector(effectEvt.Position);
            }
            if (effectEvt.Rotation.Z != 0)
            {
                decorations.Add(new PieDecoration(DebugRadius, DebugOpeningAngle, lifeSpan, color, 0.5, positionConnector).UsingRotationConnector(new AngleConnector(effectEvt.Rotation.Z)));
            }
            else
            {
                decorations.Add(new CircleDecoration(DebugRadius, lifeSpan, color, 0.5, positionConnector));
            }
        }
    }

    internal static void DebugAllEffects(ParsedEvtcLog log, CombatReplayDecorationContainer decorations, HashSet<GUID> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
    {
        var allEffectEvents = log.CombatData.GetEffectEvents()
            .Where(x => !knownEffectIDs.Contains(x.GUIDEvent.ContentGUID) && x.Time >= start && x.Time <= end && x.EffectID > 0)
            .ToList();
        var effectGUIDs = allEffectEvents.Select(x => x.GUIDEvent).ToList();
        var effectGUIDsDistinct = effectGUIDs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
        foreach (EffectEvent effectEvt in allEffectEvents)
        {
            (long start, long end) lifeSpan = effectEvt.ComputeDynamicLifespan(log, effectEvt.Duration);
            if (lifeSpan.end - lifeSpan.start < 100)
            {
                lifeSpan.end = lifeSpan.start + 100;
            }
            GeographicalConnector positionConnector;
            Color color;
            if (effectEvt.IsAroundDst)
            {
                var dstActor = log.FindActor(effectEvt.Dst);
                if (log.FriendlyAgents.Contains(dstActor.AgentItem) || log.LogData.Logic.TargetAgents.Contains(dstActor.AgentItem) || log.LogData.Logic.TrashMobAgents.Contains(dstActor.AgentItem))
                {
                    color = Colors.Teal;
                    positionConnector = new AgentConnector(dstActor);
                }
                else
                {
                    if (dstActor.TryGetCurrentPosition(log, effectEvt.Time, out var position))
                    {
                        color = Colors.DarkBlue;
                        positionConnector = new PositionConnector(position);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else
            {
                color = Colors.LightBlue;
                positionConnector = new PositionConnector(effectEvt.Position);
            }
            if (effectEvt.Rotation.Z != 0)
            {
                decorations.Add(new PieDecoration(DebugRadius, DebugOpeningAngle, lifeSpan, color, 0.5, positionConnector).UsingRotationConnector(new AngleConnector(effectEvt.Rotation.Z)));
            }
            else
            {
                decorations.Add(new CircleDecoration(DebugRadius, lifeSpan, color, 0.5, positionConnector));
            }
        }
    }

    #endregion DEBUG EFFECTS

    #region DEBUG MISSILES
    private static uint DebugMissileRadius = 40;
    internal static void DebugMissiles(SingleActor actor, ParsedEvtcLog log, CombatReplayDecorationContainer decorations, long start = long.MinValue, long end = long.MaxValue)
    {
        var allMissileEvents = log.CombatData.GetMissileEventsBySrc(actor.AgentItem)
            .Where(x => x.Time >= start && x.Time <= end && x.SkillID > 0);
        decorations.AddNonHomingMissiles(log, allMissileEvents, Colors.Red, 0.5, DebugMissileRadius);
    }
    internal static void DebugAllMissiles(ParsedEvtcLog log, CombatReplayDecorationContainer decorations, long start = long.MinValue, long end = long.MaxValue)
    {
        var allMissileEvents = log.CombatData.GetMissileEvents()
            .Where(x => x.Time >= start && x.Time <= end && x.SkillID > 0);
        decorations.AddNonHomingMissiles(log, allMissileEvents, Colors.Red, 0.5, DebugMissileRadius);
    }
    internal static void DebugAllNPCMissiles(ParsedEvtcLog log, CombatReplayDecorationContainer decorations, long start = long.MinValue, long end = long.MaxValue)
    {
        var allMissileEvents = log.CombatData.GetMissileEvents()
            .Where(x => x.Time >= start && x.Time <= end && x.SkillID > 0 && x.Src.GetFinalMaster().IsNPC);
        decorations.AddNonHomingMissiles(log, allMissileEvents, Colors.Red, 0.5, DebugMissileRadius);
    }
    #endregion DEBUG MISSILES
#endif
    /// <summary>
    /// Add hide based on buff's presence
    /// </summary>
    internal void AddHideByBuff(SingleActor actor, ParsedEvtcLog log, long buffID)
    {
        Hidden.AddRange(actor.GetBuffStatus(log, buffID).Where(x => x.Value > 0));
    }
}

