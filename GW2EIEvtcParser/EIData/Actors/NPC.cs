using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class NPC : AbstractSingleActor
    {
        // Constructors
        internal NPC(AgentItem agent) : base(agent)
        {
            if (agent.IsPlayer)
            {
                throw new EvtcAgentException("Agent is a player");
            }
        }

        internal override void OverrideName(string name)
        {
            Character = name;
        }
        internal override void SetManualHealth(int health)
        {
            Health = health;
        }

        public override string GetIcon()
        {
            return ParserHelper.GetNPCIcon(ID);
        }

        protected override void InitAdditionalCombatReplayData(ParsedEvtcLog log)
        {
            log.FightData.Logic.ComputeNPCCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any() && (log.FightData.Logic.Targets.Contains(this) || log.Friendlies.Contains(this)))
            {
                CombatReplay.Decorations.Add(new FacingDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }


        //

        public override AbstractSingleActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedEvtcLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return new NPCSerializable(this, log, map, CombatReplay);
        }

        protected override bool InitCombatReplay(ParsedEvtcLog log)
        {
            if (base.InitCombatReplay(log))
            {
                // Trim
                DespawnEvent despawnCheck = log.CombatData.GetDespawnEvents(AgentItem).LastOrDefault();
                SpawnEvent spawnCheck = log.CombatData.GetSpawnEvents(AgentItem).LastOrDefault();
                DeadEvent deathCheck = log.CombatData.GetDeadEvents(AgentItem).LastOrDefault();
                AliveEvent aliveCheck = log.CombatData.GetAliveEvents(AgentItem).LastOrDefault();
                if (deathCheck != null && (aliveCheck == null || aliveCheck.Time < deathCheck.Time))
                {
                    CombatReplay.Trim(AgentItem.FirstAware, deathCheck.Time);
                }
                else if (despawnCheck != null && (spawnCheck == null || spawnCheck.Time < despawnCheck.Time))
                {
                    CombatReplay.Trim(AgentItem.FirstAware, despawnCheck.Time);
                }
                else
                {
                    CombatReplay.Trim(AgentItem.FirstAware, AgentItem.LastAware);
                }
                return true;
            }
            return false;
        }
    }
}
