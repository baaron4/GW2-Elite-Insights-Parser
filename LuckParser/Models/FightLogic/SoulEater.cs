using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class SoulEater : RaidLogic
    {
        // TODO - add CR icons/indicators (vomit, greens, etc) and some mechanics
        public SoulEater(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange( new List<Mechanic>
            {

            }
            );
            Extension = "souleater";
            IconUrl = "https://wiki.guildwars2.com/images/thumb/7/7b/Gold_River_of_Souls_Trophy.jpg/220px-Gold_River_of_Souls_Trophy.jpg";
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                // spiders - special icon
                // spirits - green dot
                // skeletons - red dot
            };
        }

        public override string GetFightName()
        {
            return "Statue of Death";
        }
    }
}
