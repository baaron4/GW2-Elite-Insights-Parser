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
            if (CombatReplay.Rotations.Any() && (log.FightData.Logic.TargetAgents.Contains(AgentItem) || log.FriendlyAgents.Contains(AgentItem)))
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
        protected override void TrimCombatReplay(ParsedEvtcLog log)
        {
            if (!log.FriendlyAgents.Contains(AgentItem))
            {
                TrimCombatReplay(log, CombatReplay, AgentItem);
            }
        }
    }
}
