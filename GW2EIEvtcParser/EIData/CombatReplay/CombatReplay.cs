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
                            if (ptn.Time - last.Time > ArcDPSEnums.ArcDPSPollingRate + rate && (velocity == null || velocity.Length() < 1e-3))
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
                            ParametricPoint3D last = PolledRotations.Last().Time > pt.Time ? PolledRotations.Last() : pt;
                            if (ptn.Time - last.Time > ArcDPSEnums.ArcDPSPollingRate + rate)
                            {
                                PolledRotations.Add(new ParametricPoint3D(last.X, last.Y, last.Z, i));
                            }
                            else
                            {
                                float ratio = (float)(i - last.Time) / (ptn.Time - last.Time);
                                PolledRotations.Add(new ParametricPoint3D(last, ptn, ratio, i));
                            }

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
            var effectGUIDsOnAgent = effectEventsOnAgent.Select(x => log.CombatData.GetEffectGUIDEvent(x.EffectID).HexContentGUID).ToList();
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
                    replay.Decorations.Insert(0, new CircleDecoration(180, lifeSpan, Colors.Blue, 0.5, new AgentConnector(log.FindActor(effectEvt.Dst))));
                }
                else
                {

                    replay.Decorations.Insert(0, new CircleDecoration(180, lifeSpan, Colors.Blue, 0.5, new PositionConnector(effectEvt.Position)));
                }
            }
            IReadOnlyList<EffectEvent> effectEventsByAgent = log.CombatData.GetEffectEventsBySrc(actor.AgentItem).Where(x => !knownEffectIDs.Contains(x.EffectID) && x.Time >= start && x.Time <= end).ToList(); ;
            var effectGUIDsByAgent = effectEventsByAgent.Select(x => log.CombatData.GetEffectGUIDEvent(x.EffectID).HexContentGUID).ToList();
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
                    replay.Decorations.Insert(0, new CircleDecoration(180, lifeSpan, Colors.Green, 0.5, new AgentConnector(log.FindActor(effectEvt.Dst))));
                }
                else
                {

                    replay.Decorations.Insert(0, new CircleDecoration(180, lifeSpan, Colors.Green, 0.5, new PositionConnector(effectEvt.Position)));
                }
            }
        }

        internal static void DebugUnknownEffects(ParsedEvtcLog log, CombatReplay replay, HashSet<long> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
        {
            IReadOnlyList<EffectEvent> allEffectEvents = log.CombatData.GetEffectEvents().Where(x => !knownEffectIDs.Contains(x.EffectID) && x.Src.IsSpecies(ArcDPSEnums.NonIdentifiedSpecies) && x.Time >= start && x.Time <= end && x.EffectID > 0).ToList(); ;
            var effectGUIDs = allEffectEvents.Select(x => log.CombatData.GetEffectGUIDEvent(x.EffectID).HexContentGUID).ToList();
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
                    replay.Decorations.Insert(0, new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new AgentConnector(log.FindActor(effectEvt.Dst))));
                }
                else
                {

                    replay.Decorations.Insert(0, new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new PositionConnector(effectEvt.Position)));
                }
            }

        }

        internal static void DebugAllNPCEffects(ParsedEvtcLog log, CombatReplay replay, HashSet<long> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
        {
            IReadOnlyList<EffectEvent> allEffectEvents = log.CombatData.GetEffectEvents().Where(x => !knownEffectIDs.Contains(x.EffectID) && !x.Src.GetFinalMaster().IsPlayer && (!x.IsAroundDst || !x.Dst.GetFinalMaster().IsPlayer) && x.Time >= start && x.Time <= end && x.EffectID > 0).ToList(); ;
            var effectGUIDs = allEffectEvents.Select(x => log.CombatData.GetEffectGUIDEvent(x.EffectID).HexContentGUID).ToList();
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
                    replay.Decorations.Insert(0, new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new AgentConnector(log.FindActor(effectEvt.Dst))));
                }
                else
                {

                    replay.Decorations.Insert(0, new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new PositionConnector(effectEvt.Position)));
                }
            }
        }

        internal static void DebugAllEffects(ParsedEvtcLog log, CombatReplay replay, HashSet<long> knownEffectIDs, long start = long.MinValue, long end = long.MaxValue)
        {
            IReadOnlyList<EffectEvent> allEffectEvents = log.CombatData.GetEffectEvents().Where(x => !knownEffectIDs.Contains(x.EffectID) && x.Time >= start && x.Time <= end && x.EffectID > 0).ToList(); ;
            var effectGUIDs = allEffectEvents.Select(x => log.CombatData.GetEffectGUIDEvent(x.EffectID).HexContentGUID).ToList();
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
                    replay.Decorations.Insert(0, new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new AgentConnector(log.FindActor(effectEvt.Dst))));
                }
                else
                {

                    replay.Decorations.Insert(0, new CircleDecoration(180, lifeSpan, Colors.Teal, 0.5, new PositionConnector(effectEvt.Position)));
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
        internal void AddOverheadIcon(Segment segment, AbstractSingleActor actor, string icon, uint pixelSize = ParserHelper.CombatReplayOverheadDefaultSizeInPixel, float opacity = ParserHelper.CombatReplayOverheadDefaultOpacity)
        {
            Decorations.Add(new IconOverheadDecoration(icon, pixelSize, opacity, segment, new AgentConnector(actor)));
        }

        /// <summary>
        /// Add an overhead icon decoration
        /// </summary>
        /// <param name="segment">Lifespan interval</param>
        /// <param name="actor">actor to which the decoration will be attached to</param>
        /// <param name="icon">URL of the icon</param>
        /// <param name="rotation">URL of the icon</param>
        /// <param name="pixelSize">Size in pixel of the icon</param>
        /// <param name="opacity">Opacity of the icon</param>
        internal void AddRotatedOverheadIcon(Segment segment, AbstractSingleActor actor, string icon, float rotation, uint pixelSize = ParserHelper.CombatReplayOverheadDefaultSizeInPixel, float opacity = ParserHelper.CombatReplayOverheadDefaultOpacity)
        {
            Decorations.Add(new IconOverheadDecoration(icon, pixelSize, opacity, segment, new AgentConnector(actor)).UsingRotationConnector(new AngleConnector(rotation)));
        }

        /// <summary>
        /// Add overhead icon decorations
        /// </summary>
        /// <param name="segments">Lifespan intervals</param>
        /// <param name="actor">actor to which the decoration will be attached to</param>
        /// <param name="icon">URL of the icon</param>
        /// <param name="pixelSize">Size in pixel of the icon</param>
        /// <param name="opacity">Opacity of the icon</param>
        internal void AddOverheadIcons(IEnumerable<Segment> segments, AbstractSingleActor actor, string icon, uint pixelSize = ParserHelper.CombatReplayOverheadDefaultSizeInPixel, float opacity = ParserHelper.CombatReplayOverheadDefaultOpacity)
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
        internal void AddDecorationWithBorder(FormDecoration decoration, string color = null)
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
        internal void AddDecorationWithBorder(FormDecoration decoration, long growingEnd, string color = null, bool reverseGrowing = false)
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
                        Decorations.Add(new LineDecoration((tetherStart, tetherEnd), color, new AgentConnector(dst), new AgentConnector(src)));
                        src = ParserHelper._unknownAgent;
                        dst = ParserHelper._unknownAgent;
                    }
                }
            }
        }
        /// <summary>
        /// Add tether decorations which src and dst are defined by tethers parameter using <see cref="AbstractBuffEvent"/>.
        /// </summary>
        /// <param name="tethers">Buff events of the tethers.</param>
        /// <param name="color">color of the tether</param>
        internal void AddTether(IReadOnlyList<AbstractBuffEvent> tethers, Color color, double opacity)
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
        internal void AddTetherByThirdPartySrcBuff(ParsedEvtcLog log, AbstractPlayer player, long buffId, int buffSrcAgentId, int toTetherAgentId, string color, int firstAwareThreshold = 2000)
        {
            var buffEvents = log.CombatData.GetBuffData(buffId).Where(x => x.To == player.AgentItem && x.CreditedBy.IsSpecies(buffSrcAgentId)).ToList();
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
        internal void AddTetherByThirdPartySrcBuff(ParsedEvtcLog log, AbstractPlayer player, long buffId, int buffSrcAgentId, int toTetherAgentId, Color color, double opacity, int firstAwareThreshold = 2000)
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
        internal void AddProjectile(Point3D startingPoint, Point3D endingPoint, (long start, long end) lifespan, Color color, double opacity = 0.2, uint radius = 50)
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
        internal void AddProjectile(Point3D startingPoint, Point3D endingPoint, (long start, long end) lifespan, string color, uint radius = 50)
        {
            var startPoint = new ParametricPoint3D(startingPoint, lifespan.start);
            var endPoint = new ParametricPoint3D(endingPoint, lifespan.end);
            var shootingCircle = new CircleDecoration(radius, lifespan, color, new InterpolationConnector(new List<ParametricPoint3D>() { startPoint, endPoint }));
            Decorations.Add(shootingCircle);
        }
    }
}

