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
        public EaterOfSouls(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange( new List<Mechanic>
            {

            }
            );
            Extension = "souleater";
            IconUrl = "https://wiki.guildwars2.com/images/thumb/2/24/Eater_of_Souls_%28Hall_of_Chains%29.jpg/194px-Eater_of_Souls_%28Hall_of_Chains%29.jpg";
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
