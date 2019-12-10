using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Builders.HtmlModels
{
    public class ActorDetailsDto
    {
        public List<DmgDistributionDto> DmgDistributions { get; set; }
        public List<List<DmgDistributionDto>> DmgDistributionsTargets { get; set; }
        public List<DmgDistributionDto> DmgDistributionsTaken { get; set; }
        public List<List<object[]>> Rotation { get; set; }
        public List<List<BuffChartDataDto>> BoonGraph { get; set; }
        public List<FoodDto> Food { get; set; }
        public List<ActorDetailsDto> Minions { get; set; }
        public List<DeathRecapDto> DeathRecap { get; set; }

        // helpers

        public static object[] GetSkillData(AbstractCastEvent cl, long phaseStart)
        {
            object[] rotEntry = new object[5];
            double start = (cl.Time - phaseStart) / 1000.0;
            rotEntry[0] = start;
            rotEntry[1] = cl.SkillId;
            rotEntry[2] = cl.ActualDuration;
            rotEntry[3] = (int) cl.Status;
            rotEntry[4] = cl.UnderQuickness ? 1 : 0;
            return rotEntry;
        }
    }
}
