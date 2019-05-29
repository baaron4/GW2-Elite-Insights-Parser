using LuckParser.Models.ParseModels;
using LuckParser.Parser;
using System.Collections.Generic;

namespace LuckParser.Models.HtmlModels
{   
    public class ActorDetailsDto
    {
         public List<DmgDistributionDto> DmgDistributions;
         public List<List<DmgDistributionDto>> DmgDistributionsTargets;
         public List<DmgDistributionDto> DmgDistributionsTaken;
         public List<List<object[]>> Rotation;
         public List<List<BoonChartDataDto>> BoonGraph;
         public List<FoodDto> Food;
         public List<ActorDetailsDto> Minions;
         public List<DeathRecapDto> DeathRecap;

        // helpers

        public static object[] GetSkillData(AbstractCastEvent cl, long phaseStart)
        {
            object[] rotEntry = new object[5];
            double start = (cl.Time - phaseStart) / 1000.0;
            rotEntry[0] = start;
            rotEntry[1] = cl.SkillId;
            rotEntry[2] = cl.ActualDuration  ;
            rotEntry[3] = EncodeEndActivation(cl);
            rotEntry[4] = cl.UnderQuickness ? 1 : 0;
            return rotEntry;
        }

        private static int EncodeEndActivation(AbstractCastEvent cl)
        {
            if (cl.ReducedAnimation)
            {
                return 1;
            }
            if (cl.Interrupted)
            {
                return 2;
            }
            if (cl.FullAnimation)
            {
                return 3;
            }
            return 0;
        }
    }
}
