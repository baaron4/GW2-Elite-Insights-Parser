using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class UnknownFightLogic : FightLogic
    {
        public UnknownFightLogic(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            Extension = "boss";
            IconUrl = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>();
        }
    }
}
