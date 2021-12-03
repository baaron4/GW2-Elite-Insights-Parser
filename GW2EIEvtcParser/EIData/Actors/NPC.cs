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
            AgentItem master = AgentItem.GetFinalMaster();
            bool isMinionOfPlayer = master.Type == AgentItem.AgentType.Player && ArcDPSEnums.IsKnownMinionID(ID, master.Spec);
            if (CombatReplay.Rotations.Any() && (log.FightData.Logic.TargetAgents.Contains(AgentItem) || log.FriendlyAgents.Contains(AgentItem) || isMinionOfPlayer))
            {
                CombatReplay.Decorations.Add(new FacingDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
            if (isMinionOfPlayer)
            {
                ProfHelper.LinkMinionCombatReplayToMaster(this, log.FindActor(master), log, CombatReplay);
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
