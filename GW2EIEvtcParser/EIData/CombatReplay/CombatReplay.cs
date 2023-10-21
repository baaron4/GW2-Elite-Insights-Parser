using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;

namespace GW2EIEvtcParser.EIData
{
    public class CombatReplay
    {
        internal List<ParametricPoint3D> Positions { get; } = new List<ParametricPoint3D>();
        internal List<ParametricPoint3D> PolledPositions { get; private set; } = new List<ParametricPoint3D>();
        internal List<ParametricPoint3D> Velocities { get; private set; } = new List<ParametricPoint3D>();
        internal List<ParametricPoint3D> Rotations { get; } = new List<ParametricPoint3D>();
        internal List<ParametricPoint3D> PolledRotations { get; private set; } = new List<ParametricPoint3D>();
        private long _start = -1;
        private long _end = -1;
        internal (long start, long end) TimeOffsets => (_start, _end);
        // actors
        internal List<GenericDecoration> Decorations { get; } = new List<GenericDecoration>();

        internal CombatReplay(ParsedEvtcLog log)
        {
            _start = log.FightData.FightStart;
            _end = log.FightData.FightEnd;
        }

        internal void Trim(long start, long end)
        {
            PolledPositions.RemoveAll(x => x.Time < start || x.Time > end);
            PolledRotations.RemoveAll(x => x.Time < start || x.Time > end);
            _start = Math.Max(start, _start);
            _end = Math.Max(_start, Math.Min(end, _end));
        }

        private static int UpdateVelocityIndex(List<ParametricPoint3D> velocities, int time, int currentIndex)
        {
            if (!velocities.Any())
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

        private void PositionPolling(int rate, long fightDuration)
        {
            if (Positions.Count == 0)
            {
                Positions.Add(new ParametricPoint3D(int.MinValue, int.MinValue, 0, 0));
            }
            int positionTablePos = 0;
            int velocityTablePos = 0;
            //
            for (int i = (int)Math.Min(0, rate * ((Positions[0].Time / rate) - 1)); i < fightDuration; i += rate)
            {
                ParametricPoint3D pt = Positions[positionTablePos];
                if (i <= pt.Time)
                {
                    PolledPositions.Add(new ParametricPoint3D(pt.X, pt.Y, pt.Z, i));
                }
                else
                {
                    if (positionTablePos == Positions.Count - 1)
                    {
                        PolledPositions.Add(new ParametricPoint3D(pt.X, pt.Y, pt.Z, i));
                    }
                    else
                    {
                        ParametricPoint3D ptn = Positions[positionTablePos + 1];
                        if (ptn.Time < i)
                        {
                            positionTablePos++;
                            i -= rate;
                        }
                        else
                        {
                            ParametricPoint3D last = PolledPositions.Last().Time > pt.Time ? PolledPositions.Last() : pt;
                            velocityTablePos = UpdateVelocityIndex(Velocities, i, velocityTablePos);
                            ParametricPoint3D velocity = null;
                            if (velocityTablePos >= 0 && velocityTablePos < Velocities.Count)
                            {
                                velocity = Velocities[velocityTablePos];
                            }
                            if (velocity == null || (Math.Abs(velocity.X) <= 1e-1 && Math.Abs(velocity.Y) <= 1e-1))
                            {
                                PolledPositions.Add(new ParametricPoint3D(last.X, last.Y, last.Z, i));
                            }
                            else
                            {
                                float ratio = (float)(i - last.Time) / (ptn.Time - last.Time);
                                PolledPositions.Add(new ParametricPoint3D(last, ptn, ratio, i));
                            }

                        }
                    }
                }
            }
            PolledPositions = PolledPositions.Where(x => x.Time >= 0).ToList();
        }
        /// <summary>
        /// The method exists only to have the same amount of rotation as positions, it's easier to do it here than
        /// in javascript
        /// </summary>
        /// <param name="rate"></param>
        /// <param name="fightDuration"></param>
        /// <param name="forceInterpolate"></param>
        private void RotationPolling(int rate, long fightDuration)
        {
            if (Rotations.Count == 0)
            {
                return;
            }
            int rotationTablePos = 0;
            for (int i = (int)Math.Min(0, rate * ((Rotations[0].Time / rate) - 1)); i < fightDuration; i += rate)
            {
                ParametricPoint3D pt = Rotations[rotationTablePos];
                if (i <= pt.Time)
                {
                    PolledRotations.Add(new ParametricPoint3D(pt.X, pt.Y, pt.Z, i));
                }
                else
                {
                    if (rotationTablePos == Rotations.Count - 1)
                    {
                        PolledRotations.Add(new ParametricPoint3D(pt.X, pt.Y, pt.Z, i));
                    }
                    else
                    {
                        ParametricPoint3D ptn = Rotations[rotationTablePos + 1];
                        if (ptn.Time < i)
                        {
                            rotationTablePos++;
                            i -= rate;
                        }
                        else
                        {
                            PolledRotations.Add(new ParametricPoint3D(pt.X, pt.Y, pt.Z, i));
                        }
                    }
                }
            }
            PolledRotations = PolledRotations.Where(x => x.Time >= 0).ToList();
        }

        internal void PollingRate(long fightDuration)
        {
            PositionPolling(ParserHelper.CombatReplayPollingRate, fightDuration);
            RotationPolling(ParserHelper.CombatReplayPollingRate, fightDuration);
        }


        #region DEBUG EFFECTS

        internal static void DebugEffects(AbstractSingleActor actor, ParsedEvtcLog log, CombatReplay replay, HashSet<long> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
        {
            IReadOnlyList<EffectEvent> effectEventsOnAgent = log.CombatData.GetEffectEventsByDst(actor.AgentItem).Where(x => !knownEffectIDs.Contains(x.EffectID) && x.Time >= start && x.Time <= end).ToList();
            var effectGUIDsOnAgent = effectEventsOnAgent.Select(x => log.CombatData.GetEffectGUIDEvent(x.EffectID).ContentGUID).ToList();
            var effectGUIDsOnAgentDistinct = effectGUIDsOnAgent.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
            foreach (EffectEvent effectEvt in effectEventsOnAgent)
            {
                if (effectEvt.IsAroundDst)
                {
                    replay.Decorations.Insert(0, new CircleDecoration(true, 0, 180, ((int)effectEvt.Time, (int)effectEvt.Time + 100), "rgba(0, 0, 255, 0.5)", new AgentConnector(log.FindActor(effectEvt.Dst))));
                }
                else
                {

                    replay.Decorations.Insert(0, new CircleDecoration(true, 0, 180, ((int)effectEvt.Time, (int)effectEvt.Time + 100), "rgba(0, 0, 255, 0.5)", new PositionConnector(effectEvt.Position)));
                }
            }
            IReadOnlyList<EffectEvent> effectEventsByAgent = log.CombatData.GetEffectEventsBySrc(actor.AgentItem).Where(x => !knownEffectIDs.Contains(x.EffectID) && x.Time >= start && x.Time <= end).ToList(); ;
            var effectGUIDsByAgent = effectEventsByAgent.Select(x => log.CombatData.GetEffectGUIDEvent(x.EffectID).ContentGUID).ToList();
            var effectGUIDsByAgentDistinct = effectGUIDsByAgent.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
            foreach (EffectEvent effectEvt in effectEventsByAgent)
            {
                if (effectEvt.IsAroundDst)
                {
                    replay.Decorations.Insert(0, new CircleDecoration(true, 0, 180, ((int)effectEvt.Time, (int)effectEvt.Time + 100), "rgba(0, 255, 0, 0.5)", new AgentConnector(log.FindActor(effectEvt.Dst))));
                }
                else
                {

                    replay.Decorations.Insert(0, new CircleDecoration(true, 0, 180, ((int)effectEvt.Time, (int)effectEvt.Time + 100), "rgba(0, 255, 0, 0.5)", new PositionConnector(effectEvt.Position)));
                }
            }
        }

        internal static void DebugUnknownEffects(ParsedEvtcLog log, CombatReplay replay, HashSet<long> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
        {
            IReadOnlyList<EffectEvent> allEffectEvents = log.CombatData.GetEffectEvents().Where(x => !knownEffectIDs.Contains(x.EffectID) && x.Src == ParserHelper._unknownAgent && x.Time >= start && x.Time <= end && !x.IsAroundDst && x.EffectID > 0).ToList(); ;
            var effectGUIDs = allEffectEvents.Select(x => log.CombatData.GetEffectGUIDEvent(x.EffectID).ContentGUID).ToList();
            var effectGUIDsDistinct = effectGUIDs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
            foreach (EffectEvent effectEvt in allEffectEvents)
            {
                if (effectEvt.IsAroundDst)
                {
                    replay.Decorations.Insert(0, new CircleDecoration(true, 0, 180, ((int)effectEvt.Time, (int)effectEvt.Time + 100), "rgba(0, 255, 255, 0.5)", new AgentConnector(log.FindActor(effectEvt.Dst))));
                }
                else
                {

                    replay.Decorations.Insert(0, new CircleDecoration(true, 0, 180, ((int)effectEvt.Time, (int)effectEvt.Time + 100), "rgba(0, 255, 255, 0.5)", new PositionConnector(effectEvt.Position)));
                }
            }

        }

        internal static void DebugAllNPCEffects(ParsedEvtcLog log, CombatReplay replay, HashSet<long> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
        {
            IReadOnlyList<EffectEvent> allEffectEvents = log.CombatData.GetEffectEvents().Where(x => !knownEffectIDs.Contains(x.EffectID) && !x.Src.GetFinalMaster().IsPlayer && (!x.IsAroundDst || !x.Dst.GetFinalMaster().IsPlayer) && x.Time >= start && x.Time <= end && x.EffectID > 0).ToList(); ;
            var effectGUIDs = allEffectEvents.Select(x => log.CombatData.GetEffectGUIDEvent(x.EffectID).ContentGUID).ToList();
            var effectGUIDsDistinct = effectGUIDs.GroupBy(x => x).ToDictionary(x => x.Key, x => x.ToList().Count);
            foreach (EffectEvent effectEvt in allEffectEvents)
            {
                if (effectEvt.IsAroundDst)
                {
                    replay.Decorations.Insert(0, new CircleDecoration(true, 0, 180, ((int)effectEvt.Time, (int)effectEvt.Time + 100), "rgba(0, 255, 255, 0.5)", new AgentConnector(log.FindActor(effectEvt.Dst))));
                }
                else
                {

                    replay.Decorations.Insert(0, new CircleDecoration(true, 0, 180, ((int)effectEvt.Time, (int)effectEvt.Time + 100), "rgba(0, 255, 255, 0.5)", new PositionConnector(effectEvt.Position)));
                }
            }
        }

        #endregion DEBUG EFFECTS

        /// <summary>
        /// Add an overhead icon decoration
        /// </summary>
        /// <param name="segment">Lifespan interval</param>
        /// <param name="actor">actor to which the decoration will be attached to</param>
        /// <param name="icon">URL of the icon</param>
        /// <param name="pixelSize">Size in pixel of the icon</param>
        /// <param name="opacity">Opacity of the icon</param>
        internal void AddOverheadIcon(Segment segment, AbstractSingleActor actor, string icon, int pixelSize = ParserHelper.CombatReplayOverheadDefaultSizeInPixel, float opacity = ParserHelper.CombatReplayOverheadDefaultOpacity)
        {
            Decorations.Add(new IconOverheadDecoration(icon, pixelSize, opacity, segment, new AgentConnector(actor)));
        }

        /// <summary>
        /// Add overhead icon decorations
        /// </summary>
        /// <param name="segments">Lifespan intervals</param>
        /// <param name="actor">actor to which the decoration will be attached to</param>
        /// <param name="icon">URL of the icon</param>
        /// <param name="pixelSize">Size in pixel of the icon</param>
        /// <param name="opacity">Opacity of the icon</param>
        internal void AddOverheadIcons(IEnumerable<Segment> segments, AbstractSingleActor actor, string icon, int pixelSize = ParserHelper.CombatReplayOverheadDefaultSizeInPixel, float opacity = ParserHelper.CombatReplayOverheadDefaultOpacity)
        {
            foreach (Segment segment in segments)
            {
                AddOverheadIcon(segment, actor, icon, pixelSize, opacity);
            }
        }

        /// <summary>
        /// Add tether decorations which src and dst are defined by tethers parameter using <see cref="AbstractBuffEvent"/>.
        /// </summary>
        /// <param name="tethers">Buff events of the tethers.</param>
        /// <param name="color">color of the tether</param>
        internal void AddTether(IReadOnlyList<AbstractBuffEvent> tethers, string color)
        {
            int tetherStart = 0;
            AgentItem src = ParserHelper._unknownAgent;
            AgentItem dst = ParserHelper._unknownAgent;
            foreach (AbstractBuffEvent tether in tethers)
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
                        Decorations.Add(new LineDecoration(0, (tetherStart, tetherEnd), color, new AgentConnector(dst), new AgentConnector(src)));
                        src = ParserHelper._unknownAgent;
                        dst = ParserHelper._unknownAgent;
                    }
                }
            }
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

            (int, int) lifespan;
            if (overrideDuration == false)
            {
                lifespan = ProfHelper.ComputeEffectLifespan(log, effect, effect.Duration);
            }
            else
            {
                lifespan = ((int)effect.Time, (int)effect.Time + duration);
            }

            if (effect.Src != ParserHelper._unknownAgent && effect.Dst != ParserHelper._unknownAgent)
            {
                Decorations.Add(new LineDecoration(0, lifespan, color, new AgentConnector(effect.Dst), new AgentConnector(effect.Src)));
            }
        }
    }
}

