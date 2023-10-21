using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.ParsedData
{
    public class AgentItem
    {

        private static int AgentCount = 0;
        public enum AgentType { NPC, Gadget, Player, NonSquadPlayer}

        public bool IsPlayer => Type == AgentType.Player || Type == AgentType.NonSquadPlayer;
        public bool IsNPC => Type == AgentType.NPC || Type == AgentType.Gadget;

        // Fields
        public ulong Agent { get; }
        public int ID { get; protected set; }
        public int UniqueID { get; }
        public AgentItem Master { get; protected set; }
        public ushort InstID { get; protected set; }
        public AgentType Type { get; protected set; } = AgentType.NPC;
        public long FirstAware { get; protected set; }
        public long LastAware { get; protected set; } = long.MaxValue;
        public string Name { get; protected set; } = "UNKNOWN";
        public ParserHelper.Spec Spec { get; } = ParserHelper.Spec.Unknown;
        public ParserHelper.Spec BaseSpec { get; } = ParserHelper.Spec.Unknown;
        public ushort Toughness { get; protected set; }
        public ushort Healing { get; }
        public ushort Condition { get; }
        public ushort Concentration { get; }
        public uint HitboxWidth { get; }
        public uint HitboxHeight { get; }

        public bool IsFake { get; }
        public bool IsNotInSquadFriendlyPlayer { get; private set; }

        // Constructors
        internal AgentItem(ulong agent, string name, ParserHelper.Spec spec, int id, AgentType type, ushort toughness, ushort healing, ushort condition, ushort concentration, uint hbWidth, uint hbHeight)
        {
            UniqueID = AgentCount++;
            Agent = agent;
            Name = name;
            Spec = spec;
            BaseSpec = ParserHelper.SpecToBaseSpec(spec);
            ID = id;
            Type = type;
            Toughness = toughness;
            Healing = healing;
            Condition = condition;
            Concentration = concentration;
            HitboxWidth = hbWidth;
            HitboxHeight = hbHeight;
            //
            try
            {
                if (type == AgentType.Player)
                {
                    HitboxWidth = 48;
                    HitboxHeight = 240;
                    string[] splitStr = Name.Split('\0');
                    if (splitStr.Length < 2 || (splitStr[1].Length == 0 || splitStr[2].Length == 0 || splitStr[0].Contains("-")))
                    {
                        if (!splitStr[0].Any(char.IsDigit))
                        {
                            IsNotInSquadFriendlyPlayer = true;
                        } 
                        else
                        {
                            Name = Spec.ToString() + " " + Name;
                        }
                        Type = AgentType.NonSquadPlayer;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        internal AgentItem(ulong agent, string name, ParserHelper.Spec spec, int id, ushort instid, ushort toughness, ushort healing, ushort condition, ushort concentration, uint hbWidth, uint hbHeight, long firstAware, long lastAware, bool isFake) : this(agent, name, spec, id, AgentType.NPC, toughness, healing, condition, concentration, hbWidth, hbHeight)
        {
            InstID = instid;
            FirstAware = firstAware;
            LastAware = lastAware;
            IsFake = isFake;
        }

        internal AgentItem(AgentItem other)
        {
            UniqueID = AgentCount++;
            Agent = other.Agent;
            Name = other.Name;
            Spec = other.Spec;
            BaseSpec = other.BaseSpec;
            ID = other.ID;
            Type = other.Type;
            Toughness = other.Toughness;
            Healing = other.Healing;
            Condition = other.Condition;
            Concentration = other.Concentration;
            HitboxWidth = other.HitboxWidth;
            HitboxHeight = other.HitboxHeight;
            InstID = other.InstID;
            Master = other.Master;
            IsFake = other.IsFake;
        }

        internal AgentItem()
        {
            UniqueID = AgentCount++;
        }

        internal void OverrideIsNotInSquadFriendlyPlayer(bool status)
        {
            IsNotInSquadFriendlyPlayer = status;
        }

        internal void OverrideType(AgentType type)
        {
            Type = type;
        }

        internal void OverrideName(string name)
        {
            Name = name;
        }

        internal void SetInstid(ushort instid)
        {
            InstID = instid;
        }

        internal void OverrideID(int id)
        {
            ID = id;
        }

        internal void OverrideID(ArcDPSEnums.TrashID id)
        {
            ID = (int)id;
        }

        internal void OverrideID(ArcDPSEnums.TargetID id)
        {
            ID = (int)id;
        }

        internal void OverrideID(ArcDPSEnums.MinionID id)
        {
            ID = (int)id;
        }

        internal void OverrideID(ArcDPSEnums.ChestID id)
        {
            ID = (int)id;
        }

        internal void OverrideToughness(ushort toughness)
        {
            Toughness = toughness;
        }

        internal void OverrideAwareTimes(long firstAware, long lastAware)
        {
            FirstAware = firstAware;
            LastAware = lastAware;
        }

        internal void SetMaster(AgentItem master)
        {
            if (IsPlayer)
            {
                return;
            }
            Master = master;
        }

        internal AgentItem GetMainAgentWhenAttackTarget(ParsedEvtcLog log, long time)
        {
            IReadOnlyList<AttackTargetEvent> atEvents = log.CombatData.GetAttackTargetEventsByAttackTarget(this);
            if (atEvents.Any()) // agent is attack target
            {
                return atEvents.LastOrDefault(y => time >= y.Time)?.Src;
            }
            else
            {
                return this;
            }
        }

        private static void AddValueToStatusList(List<Segment> dead, List<Segment> down, List<Segment> dc, AbstractStatusEvent cur, long nextTime, long minTime, int index)
        {
            long cTime = cur.Time; 
            
            if (cur is DownEvent)
            {
                down.Add(new Segment(cTime, nextTime, 1));
            }
            else if (cur is DeadEvent)
            {
                dead.Add(new Segment(cTime, nextTime, 1));
            }
            else if (cur is DespawnEvent)
            {
                dc.Add(new Segment(cTime, nextTime, 1));
            }
            else if (index == 0)
            {
                dc.Add(new Segment(minTime, cTime, 1));
            }
        }

        internal void GetAgentStatus(List<Segment> dead, List<Segment> down, List<Segment> dc, CombatData combatData, FightData fightData)
        {
            // State changes are not reliable
            if (Type == AgentType.NonSquadPlayer)
            {
                return;
            }
            var status = new List<AbstractStatusEvent>();
            status.AddRange(combatData.GetDownEvents(this));
            status.AddRange(combatData.GetAliveEvents(this));
            status.AddRange(combatData.GetDeadEvents(this));
            status.AddRange(combatData.GetSpawnEvents(this));
            status.AddRange(combatData.GetDespawnEvents(this));
            dc.Add(new Segment(long.MinValue, FirstAware, 1));
            if (!status.Any())
            {
                dc.Add(new Segment(LastAware, long.MaxValue, 1));
                return;
            }
            status = status.OrderBy(x => x.Time).ToList();
            for (int i = 0; i < status.Count - 1; i++)
            {
                AbstractStatusEvent cur = status[i];
                AbstractStatusEvent next = status[i + 1];
                AddValueToStatusList(dead, down, dc, cur, next.Time, FirstAware, i);
            }
            // check last value
            if (status.Count > 0)
            {
                AbstractStatusEvent cur = status.Last();
                AddValueToStatusList(dead, down, dc, cur, LastAware, FirstAware, status.Count - 1);
                if (cur is DeadEvent)
                {
                    dead.Add(new Segment(LastAware, long.MaxValue, 1));
                } 
                else
                {
                    dc.Add(new Segment(LastAware, long.MaxValue, 1));
                }
            }
        }

        internal void GetAgentBreakbarStatus(List<Segment> nones, List<Segment> actives, List<Segment> immunes, List<Segment> recovering, CombatData combatData, FightData fightData)
        {
            // State changes are not reliable
            if (Type == AgentType.NonSquadPlayer)
            {
                return;
            }
            var status = new List<BreakbarStateEvent>();
            status.AddRange(combatData.GetBreakbarStateEvents(this));
            if (!status.Any())
            {
                nones.Add(new Segment(FirstAware, LastAware, 1));
                return;
            }
            status = status.OrderBy(x => x.Time).ToList();
            for (int i = 0; i < status.Count - 1; i++)
            {
                BreakbarStateEvent cur = status[i];
                if (i == 0 && cur.Time > FirstAware)
                {
                    nones.Add(new Segment(FirstAware, cur.Time, 1));
                }
                BreakbarStateEvent next = status[i + 1];
                switch (cur.State)
                {
                    case ArcDPSEnums.BreakbarState.Active:
                        actives.Add(new Segment(cur.Time, next.Time, 1));
                        break;
                    case ArcDPSEnums.BreakbarState.Immune:
                        immunes.Add(new Segment(cur.Time, next.Time, 1));
                        break;
                    case ArcDPSEnums.BreakbarState.None:
                        nones.Add(new Segment(cur.Time, next.Time, 1));
                        break;
                    case ArcDPSEnums.BreakbarState.Recover:
                        recovering.Add(new Segment(cur.Time, next.Time, 1));
                        break;
                }
            }
            // check last value
            if (status.Count > 0)
            {
                BreakbarStateEvent cur = status.Last();
                if (LastAware - cur.Time >= ParserHelper.ServerDelayConstant)
                {
                    switch (cur.State)
                    {
                        case ArcDPSEnums.BreakbarState.Active:
                            actives.Add(new Segment(cur.Time, LastAware, 1));
                            break;
                        case ArcDPSEnums.BreakbarState.Immune:
                            immunes.Add(new Segment(cur.Time, LastAware, 1));
                            break;
                        case ArcDPSEnums.BreakbarState.None:
                            nones.Add(new Segment(cur.Time, LastAware, 1));
                            break;
                        case ArcDPSEnums.BreakbarState.Recover:
                            recovering.Add(new Segment(cur.Time, LastAware, 1));
                            break;
                    }
                }
                
            }
        }

        public AgentItem GetFinalMaster()
        {
            AgentItem cur = this;
            while (cur.Master != null)
            {
                cur = cur.Master;
            }
            return cur;
        }

        public bool InAwareTimes(long time)
        {
            return FirstAware <= time && LastAware >= time;
        }

        /// <summary>
        /// Checks if a buff is present on the actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        /// <param name="log"></param>
        /// <param name="buffId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool HasBuff(ParsedEvtcLog log, long buffId, long time)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.HasBuff(log, buffId, time);
        }

        /// <summary>
        /// Checks if a buff is present on the actor and applied by given actor. Given buff id must be in the buff simulator, throws <see cref="InvalidOperationException"/> otherwise
        /// </summary>
        /// <param name="log"></param>
        /// <param name="by"></param>
        /// <param name="buffId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool HasBuff(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long time)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.HasBuff(log, by, buffId, time);
        }

        public Segment GetBuffStatus(ParsedEvtcLog log, long buffId, long time)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.GetBuffStatus(log, buffId, time);
        }

        public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, long buffId, long start, long end)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.GetBuffStatus(log, buffId, start, end);
        }

        public Segment GetBuffStatus(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long time)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.GetBuffStatus(log, by, buffId, time);
        }

        public IReadOnlyList<Segment> GetBuffStatus(ParsedEvtcLog log, AbstractSingleActor by, long buffId, long start, long end)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.GetBuffStatus(log, by, buffId, start, end);
        }

        /// <summary>
        /// Checks if agent is downed at given time
        /// </summary>
        /// <param name="log"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool IsDowned(ParsedEvtcLog log, long time)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.IsDowned(log, time);
        }

        /// <summary>
        /// Checks if agent is dead at given time
        /// </summary>
        /// <param name="log"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool IsDead(ParsedEvtcLog log, long time)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.IsDead(log, time);
        }

        /// <summary>
        /// Checks if agent is dc/not spawned at given time
        /// </summary>
        /// <param name="log"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool IsDC(ParsedEvtcLog log, long time)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.IsDC(log, time);
        }

        public double GetCurrentHealthPercent(ParsedEvtcLog log, long time)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.GetCurrentHealthPercent(log, time);
        }

        public double GetCurrentBarrierPercent(ParsedEvtcLog log, long time)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.GetCurrentBarrierPercent(log, time);
        }

        public Point3D GetCurrentPosition(ParsedEvtcLog log, long time)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.GetCurrentPosition(log, time);
        }

        public ArcDPSEnums.BreakbarState GetCurrentBreakbarState(ParsedEvtcLog log, long time)
        {
            AbstractSingleActor actor = log.FindActor(this);
            return actor.GetCurrentBreakbarState(log, time);
        }

        public bool IsSpecies(int id)
        {
            return !IsPlayer && ID == id;
        }

        public bool IsSpecies(ArcDPSEnums.TrashID id)
        {
            return IsSpecies((int)id);
        }

        public bool IsSpecies(ArcDPSEnums.TargetID id)
        {
            return IsSpecies((int)id);
        }

        public bool IsSpecies(ArcDPSEnums.MinionID id)
        {
            return IsSpecies((int)id);
        }

        public bool IsSpecies(ArcDPSEnums.ChestID id)
        {
            return IsSpecies((int)id);
        }

        public bool IsAnySpecies(IReadOnlyList<ArcDPSEnums.TrashID> ids)
        {
            return ids.Any(x => IsSpecies(x));
        }

        public bool IsAnySpecies(IReadOnlyList<ArcDPSEnums.TargetID> ids)
        {
            return ids.Any(x => IsSpecies(x));
        }

        public bool IsAnySpecies(IReadOnlyList<ArcDPSEnums.MinionID> ids)
        {
            return ids.Any(x => IsSpecies(x));
        }

        public bool IsAnySpecies(IReadOnlyList<ArcDPSEnums.ChestID> ids)
        {
            return ids.Any(x => IsSpecies(x));
        }
    }
}
