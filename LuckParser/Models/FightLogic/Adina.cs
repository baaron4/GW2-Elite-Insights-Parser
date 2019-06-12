using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;
using System.Drawing;

namespace LuckParser.Models.Logic
{
    public class Adina : RaidLogic
    {
        public Adina(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            });
            Extension = "adina";
            IconUrl = "";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return null;
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return 0;
        }
    }
}
