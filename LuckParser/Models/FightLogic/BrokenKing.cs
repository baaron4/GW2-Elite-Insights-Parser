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
        public BrokenKing(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange( new List<Mechanic>
            {

            }
            );
            Extension = "brokenking";
            IconUrl = "https://wiki.guildwars2.com/images/3/37/Mini_Broken_King.png";
        }

        /*protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/YBtiFnH.png",
                            (4145, 1603),
                            (-12201, -4866, 7742, 2851),
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508));
        }*/

        public override string GetFightName() {
            return "Statue of Ice";
        }
    }
}
