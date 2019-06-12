using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;
using System.Drawing;

namespace LuckParser.Models.Logic
{
    public class PeerlessQadim : RaidLogic
    {
        public PeerlessQadim(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            });
            Extension = "prlqadim";
            IconUrl = "";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://wiki.guildwars2.com/images/5/52/The_Key_of_Ahdashim_map.jpg",
                            (1920, 1664),
                            (-21504, -21504, 24576, 24576),
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970));
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return 0;
        }
    }
}
