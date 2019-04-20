using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class BrokenKing : RaidLogic
    {
        // TODO - add CR icons and some mechanics
        public BrokenKing(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange( new List<Mechanic>
            {

            }
            );
            Extension = "brokenking";
            IconUrl = "https://wiki.guildwars2.com/images/3/37/Mini_Broken_King.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/JRPskkX.png",
                            (999, 890),
                            (2497, 5388, 7302, 9668),
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508));
        }

        public override void SetSuccess(ParsedLog log)
        {
            SetSuccessByDeath(log, TriggerID);
        }

        public override string GetFightName() {
            return "Statue of Ice";
        }
    }
}
