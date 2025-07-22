using System.Drawing;
using System.Numerics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.SpeciesIDs;

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

                        if (nextPos.Time - last.Time > ArcDPSPollingRate + rate && velocity.XYZ.Length() < 1e-3)
                        {
                            PolledPositions.Add(last.WithChangedTime(t));
                        }
                        else
                        {
                            float ratio = (float)(t - last.Time) / (nextPos.Time - last.Time);
                            PolledPositions.Add(new(Vector3.Lerp(last.XYZ, nextPos.XYZ, ratio), t));
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
                        if (nextRot.Time - last.Time > ArcDPSPollingRate + rate)
                        {
                            PolledRotations.Add(last.WithChangedTime(t));
                        }
                        else
                        {
                            float ratio = (float)(t - last.Time) / (nextRot.Time - last.Time);
                            PolledRotations.Add(new(Vector3.Lerp(last.XYZ, nextRot.XYZ, ratio), t));
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
                if (log.FriendlyAgents.Contains(dstActor.AgentItem) || log.FightData.Logic.TargetAgents.Contains(dstActor.AgentItem) || log.FightData.Logic.TrashMobAgents.Contains(dstActor.AgentItem))
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
                if (log.FriendlyAgents.Contains(dstActor.AgentItem) || log.FightData.Logic.TargetAgents.Contains(dstActor.AgentItem) || log.FightData.Logic.TrashMobAgents.Contains(dstActor.AgentItem))
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
                if (log.FriendlyAgents.Contains(dstActor.AgentItem) || log.FightData.Logic.TargetAgents.Contains(dstActor.AgentItem) || log.FightData.Logic.TrashMobAgents.Contains(dstActor.AgentItem))
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
                if (log.FriendlyAgents.Contains(dstActor.AgentItem) || log.FightData.Logic.TargetAgents.Contains(dstActor.AgentItem) || log.FightData.Logic.TrashMobAgents.Contains(dstActor.AgentItem))
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
                if (log.FriendlyAgents.Contains(dstActor.AgentItem) || log.FightData.Logic.TargetAgents.Contains(dstActor.AgentItem) || log.FightData.Logic.TrashMobAgents.Contains(dstActor.AgentItem))
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
            .Where(x => x.Time >= start && x.Time <= end && x.SkillID > 0 && x.Src.IsNPC);
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

