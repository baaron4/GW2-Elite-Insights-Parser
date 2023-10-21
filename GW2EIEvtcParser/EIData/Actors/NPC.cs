using System.IO;
using System.Linq;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{
    public class NPC : AbstractSingleActor
    {
        // Constructors
        internal NPC(AgentItem agent) : base(agent)
        {
            if (agent.IsPlayer)
            {
                throw new InvalidDataException("Agent is a player");
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
                CombatReplay.Decorations.Add(new ActorOrientationDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), AgentItem));
            }
            // Don't put minions of NPC into the minion display system
            AgentItem master = AgentItem.GetFinalMaster();
            if (master != AgentItem && master.IsPlayer)
            {
                AbstractSingleActor masterActor = log.FindActor(master);
                // Basic linkage
                CombatReplay.Decorations.Add(new LineDecoration((CombatReplay.TimeOffsets.start, CombatReplay.TimeOffsets.end), "rgba(0, 255, 0, 0.5)", new AgentConnector(this), new AgentConnector(masterActor)));
                // Prof specific treatment
                ProfHelper.ComputeMinionCombatReplayActors(this, masterActor, log, CombatReplay);
            }
        }


        //

        public override AbstractSingleActorCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return new NPCCombatReplayDescription(this, log, map, CombatReplay);
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
