using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class DarkMaze : RaidLogic
    {
        // TODO - add CR icons and some mechanics
        public DarkMaze(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange( new List<Mechanic>
            {

            }
            );
            Extension = "eyes";
            IconUrl = "https://wiki.guildwars2.com/images/thumb/a/a7/Eye_of_Fate.jpg/188px-Eye_of_Fate.jpg";
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                // skeletons - red dot
                // minotaur - special icon
            };
        }


        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.TargetIDS.EyeOfFate,
                (ushort)ParseEnum.TargetIDS.EyeOfJudgement
            };
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)ParseEnum.TargetIDS.EyeOfFate,
                (ushort)ParseEnum.TargetIDS.EyeOfJudgement
            };
        }

        public override string GetFightName() {
            return "Statue of Darkness";
        }
    }
}
