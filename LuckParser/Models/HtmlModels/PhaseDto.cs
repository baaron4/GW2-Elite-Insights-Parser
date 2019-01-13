using System.Collections.Generic;
using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{
    
    public class PhaseDto
    {
        public string Name;
        [DefaultValue(null)]
        public long Duration;
        [DefaultValue(null)]
        public double Start;
        [DefaultValue(null)]
        public double End;
        public List<int> Targets = new List<int>();

        public List<List<object>> DpsStats;
        public List<List<List<object>>> DpsStatsTargets;
        public List<List<List<object>>> DmgStatsTargets;
        public List<List<object>> DmgStats;
        public List<List<object>> DefStats;
        public List<List<object>> HealStats;

        public List<BoonData> BoonStats;
        public List<BoonData> BoonGenSelfStats;
        public List<BoonData> BoonGenGroupStats;
        public List<BoonData> BoonGenOGroupStats;
        public List<BoonData> BoonGenSquadStats;

        public List<BoonData> OffBuffStats;
        public List<BoonData> OffBuffGenSelfStats;
        public List<BoonData> OffBuffGenGroupStats;
        public List<BoonData> OffBuffGenOGroupStats;
        public List<BoonData> OffBuffGenSquadStats;

        public List<BoonData> DefBuffStats;
        public List<BoonData> DefBuffGenSelfStats;
        public List<BoonData> DefBuffGenGroupStats;
        public List<BoonData> DefBuffGenOGroupStats;
        public List<BoonData> DefBuffGenSquadStats;

        public List<BoonData> PersBuffStats;

        public List<List<object[]>> DmgModifiersCommon = new List<List<object[]>>();
        public List<List<List<object[]>>> DmgModifiersTargetsCommon = new List<List<List<object[]>>>();

        public List<List<BoonData>> TargetsCondiStats;
        public List<BoonData> TargetsCondiTotals;
        public List<BoonData> TargetsBoonTotals;

        public List<List<int[]>> MechanicStats;
        public List<List<int[]>> EnemyMechanicStats;

        public List<double> MarkupLines;
        public List<AreaLabelDto> MarkupAreas;
        public List<int> SubPhases;
    }
}
