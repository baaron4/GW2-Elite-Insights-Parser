using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
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

                        if (nextPos.Time - last.Time > ArcDPSEnums.ArcDPSPollingRate + rate && velocity.XYZ.Length() < 1e-3)
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
                        if (nextRot.Time - last.Time > ArcDPSEnums.ArcDPSPollingRate + rate)
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

    /// <summary>
    /// Adds two rectangles over each other representing a loading bar.
    /// </summary>
    /// <param name="actor">Actor to attach the bar to.</param>
    /// <param name="segments">Time segments used to increase or decrease the bar over the background.</param>
    /// <param name="segmentMaxValue">The maximum value the segments could have. Necessary to know to calculate the bar size ratio.</param>
    /// <param name="offsetX">Horizontal offset to position more to the left or to the right.</param>
    /// <param name="offsetY">Vertical offset to position higher or lower.</param>
    /// <param name="width">The maximum width of the bar.</param>
    /// <param name="height">The maximum height of the bar.</param>
    /// <param name="angle">Rotation angle.</param>
    /// <param name="colors">Span containing the colors and opacity. They will be used in the following order:<br></br>
    /// <list type="bullet">
    /// <item>Bar Color</item>
    /// <item>Background Color</item>
    /// <item>Bar Border Color</item>
    /// </list>
    /// </param>
    /// <param name="sizeMultiplier">Multiplies the Value of the Segment to scale the size of the bar. The value cannot be 0 or less.</param>
    internal void AddDynamicBar(SingleActor actor, IReadOnlyList<GenericSegment<double>> segments, double segmentMaxValue, int offsetX, int offsetY, uint width, uint height, float angle, ReadOnlySpan<(Color color, double opacity)> colors, int sizeMultiplier = 1)
    {
        Debug.Assert(sizeMultiplier > 0, $"{nameof(sizeMultiplier)} must be greater than zero but was {sizeMultiplier}");

        uint barWidth;
        var offset = new Vector3(0, offsetY, 0);
        var offsetBackground = new Vector3(offsetX, offsetY, 0);
        var angleConnector = new AngleConnector(angle);

        var ratio = width / segmentMaxValue;

        foreach (var segment in segments)
        {
            offset.X = (float)(offsetX + (- (width / 2) + segment.Value * ratio * sizeMultiplier / 2));
            barWidth = (uint)(segment.Value * sizeMultiplier * ratio);
            var bar = (RectangleDecoration)new RectangleDecoration(barWidth, height, segment.TimeSpan, colors[0].color, colors[0].opacity, new AgentConnector(actor).WithOffset(offset, true)).UsingRotationConnector(angleConnector);
            var background = (RectangleDecoration)new RectangleDecoration(width, height, segment.TimeSpan, colors[1].color, colors[1].opacity, new AgentConnector(actor).WithOffset(offsetBackground, true)).UsingRotationConnector(angleConnector);
            AddDecorationWithBorder(bar, colors[2].color, colors[2].opacity);
            Decorations.Add(background);
        }
    }
}

