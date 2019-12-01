using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Logic
{
    public class UnknownFightLogic : FightLogic
    {
        public UnknownFightLogic(ushort triggerID) : base(triggerID)
        {
            Extension = "boss";
            Icon = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>();
        }

        protected override void ComputeFightTargets(AgentData agentData, List<CombatItem> combatItems)
        {
            ushort id = GetFightTargetsIDs().First();
            AgentItem agentItem = agentData.GetNPCsByID(id).FirstOrDefault();
            // Trigger ID is not NPC
            if (agentItem == null)
            {
                agentItem = agentData.GetGadgetsByID(id).FirstOrDefault();
                if (agentItem != null)
                {
                    Targets.Add(new NPC(agentItem));
                }
            } else
            {
                Targets.Add(new NPC(agentItem));
            }
        }
    }
}
