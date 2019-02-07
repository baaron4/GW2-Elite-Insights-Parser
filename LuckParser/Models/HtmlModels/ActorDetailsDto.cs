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

        public static object[] GetSkillData(CastLog cl, long phaseStart)
        {
            object[] rotEntry = new object[5];
            double offset = 0.0;
            double start = (cl.Time - phaseStart) / 1000.0;
            rotEntry[0] = start;
            if (start < 0.0)
            {
                offset = -1000.0 * start;
                rotEntry[0] = 0;
            }
            rotEntry[1] = cl.SkillId;
            rotEntry[2] = cl.ActualDuration - offset; ;
            rotEntry[3] = EncodeEndActivation(cl.EndActivation);
            rotEntry[4] = cl.StartActivation == ParseEnum.Activation.Quickness ? 1 : 0;
            return rotEntry;
        }

        private static int EncodeEndActivation(ParseEnum.Activation endActivation)
        {
            switch (endActivation)
            {
                case ParseEnum.Activation.CancelFire: return 1;
                case ParseEnum.Activation.CancelCancel: return 2;
                case ParseEnum.Activation.Reset: return 3;
                default: return 0;
            }
        }
    }
}
