using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class EaterOfSouls : RaidLogic
    {
        // TODO - add CR icons/indicators (vomit, greens, etc) and some mechanics
        public EaterOfSouls(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange( new List<Mechanic>
            {

            }
            );
            Extension = "souleater";
            IconUrl = "https://wiki.guildwars2.com/images/thumb/2/24/Eater_of_Souls_%28Hall_of_Chains%29.jpg/194px-Eater_of_Souls_%28Hall_of_Chains%29.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/Owo34RS.png",
                            (710, 709),
                            (1306, -9381, 4720, -5968),
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508));
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                OrbSpider,
                SpiritHorde1,
                SpiritHorde2,
                SpiritHorde3,
                GreenSpirit
            };
        }

        public override void SetSuccess(ParsedLog log)
        {
            SetSuccessByDeath(log, TriggerID);
        }

        public override string GetFightName()
        {
            return "Statue of Death";
        }
    }
}
