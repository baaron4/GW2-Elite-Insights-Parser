using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    public class AbstractPlayer : AbstractSingleActor
    {

        // Constructors
        internal AbstractPlayer(AgentItem agent) : base(agent)
        {
            if (agent.IsNPC)
            {
                throw new EvtcAgentException("Agent is NPC");
            }
            if (IsFakeActor)
            {
                throw new EvtcAgentException("Players can't be fake actors");
            }
        }

        public override string GetIcon()
        {
            return AgentItem.Type == AgentItem.AgentType.NonSquadPlayer && !AgentItem.IsNotInSquadFriendlyPlayer ? GetHighResolutionProfIcon(Prof) : GetProfIcon(Prof);
        }

        protected override void InitAdditionalCombatReplayData(ParsedEvtcLog log)
        {
            // Fight related stuff
            log.FightData.Logic.ComputePlayerCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any())
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
            return new PlayerSerializable(this, log, map, CombatReplay);
        }
    }
}
