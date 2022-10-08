using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

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
            IReadOnlyList<EffectEvent> effectEventsByAgent = log.CombatData.GetEffectEvents(actor.AgentItem).Where(x => !knownEffectIDs.Contains(x.EffectID) && x.Time >= start && x.Time <= end).ToList(); ;
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
    }
}

