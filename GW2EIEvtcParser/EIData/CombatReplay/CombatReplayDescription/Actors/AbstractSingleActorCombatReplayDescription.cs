using System;
using System.Collections.Generic;
using System.Linq;

namespace GW2EIEvtcParser.EIData
{
    public abstract class AbstractSingleActorCombatReplayDescription : AbstractCombatReplayDescription
    {
        public string Img { get; }
        public int ID { get; }
        public IReadOnlyList<float> Positions { get; }
        public IReadOnlyList<float> Angles { get; }
        public IReadOnlyList<long> Dead { get; private set; }
        public IReadOnlyList<long> Down { get; private set; }
        public IReadOnlyList<long> Dc { get; private set; }
        public IReadOnlyList<long> BreakbarActive { get; private set; }

        public long HitboxWidth { get; }

        private static string GetActorType(AbstractSingleActor actor, ParsedEvtcLog log)
        {
            if (actor.AgentItem.IsPlayer)
            {
                return !log.PlayerAgents.Contains(actor.AgentItem) ? "TargetPlayer" : "Player";
            }
            if (log.FightData.Logic.TargetAgents.Contains(actor.AgentItem))
            {
                return "Target";
            }
            if (log.FightData.Logic.NonPlayerFriendlyAgents.Contains(actor.AgentItem) || actor.AgentItem.GetFinalMaster().Type == ParsedData.AgentItem.AgentType.Player)
            {
                return "Friendly";
            }
            return "Mob";
        }

        internal AbstractSingleActorCombatReplayDescription(AbstractSingleActor actor, ParsedEvtcLog log, CombatReplayMap map, CombatReplay replay)
        {
            Start = replay.TimeOffsets.start;
            End = replay.TimeOffsets.end;
            Img = actor.GetIcon();
            ID = actor.UniqueID;
            var positions = new List<float>();
            Positions = positions;
            var angles = new List<float>();
            Angles = angles;
            Type = GetActorType(actor, log);
            HitboxWidth = actor.AgentItem.HitboxWidth;
            foreach (Point3D pos in replay.PolledPositions)
            {
                (float x, float y) = map.GetMapCoord(pos.X, pos.Y);
                positions.Add(x);
                positions.Add(y);
            }
            foreach (Point3D facing in replay.PolledRotations)
            {
                angles.Add(-Point3D.GetRotationFromFacing(facing));
            }
        }
        protected void SetStatus(ParsedEvtcLog log, AbstractSingleActor a)
        {
            var dead = new List<long>();
            Dead = dead;
            var down = new List<long>();
            Down = down;
            var dc = new List<long>();
            Dc = dc;
            (IReadOnlyList<Segment> deads, IReadOnlyList<Segment> downs, IReadOnlyList<Segment> dcs) = a.GetStatus(log);

            foreach (Segment seg in deads)
            {
                dead.Add(seg.Start);
                dead.Add(seg.End);
            }
            foreach (Segment seg in downs)
            {
                down.Add(seg.Start);
                down.Add(seg.End);
            }
            foreach (Segment seg in dcs)
            {
                dc.Add(seg.Start);
                dc.Add(seg.End);
            }
        }

        protected void SetBreakbarStatus(ParsedEvtcLog log, AbstractSingleActor a)
        {
            var active = new List<long>();
            BreakbarActive = active;
            (_, IReadOnlyList<Segment> actives, _, _) = a.GetBreakbarStatus(log);

            foreach (Segment seg in actives)
            {
                active.Add(seg.Start);
                active.Add(seg.End);
            }
        }

    }
}
