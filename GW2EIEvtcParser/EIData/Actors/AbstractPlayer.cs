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
        public bool IsFriendlyPlayer => AgentItem.Type == AgentItem.AgentType.Player || AgentItem.IsNotInSquadFriendlyPlayer;

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
        internal override void OverrideName(string name)
        {
            throw new InvalidOperationException("Players' name can't be overriden");
        }
        internal override void SetManualHealth(int health)
        {
            throw new InvalidOperationException("Players' health can't be overriden");
        }

        public override string GetIcon()
        {
            return !IsFriendlyPlayer ? GetHighResolutionProfIcon(Spec) : GetProfIcon(Spec);
        }

        protected override void InitAdditionalCombatReplayData(ParsedEvtcLog log)
        {
            // Fight related stuff
            log.FightData.Logic.ComputePlayerCombatReplayActors(this, log, CombatReplay);
            ProfHelper.ComputeProfessionCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any())
            {
                CombatReplay.Decorations.Add(new ActorOrientationDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), AgentItem));
            }
        }

        //

        public override AbstractSingleActorCombatReplayDescription GetCombatReplayDescription(CombatReplayMap map, ParsedEvtcLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return new PlayerCombatReplayDescription(this, log, map, CombatReplay);
        }
    }
}
