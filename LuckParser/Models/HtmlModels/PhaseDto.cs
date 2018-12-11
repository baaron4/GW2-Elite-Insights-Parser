using System.Collections.Generic;
using System.ComponentModel;

namespace LuckParser.Models.HtmlModels
{
    
    public class PhaseDto
    {
        public string name;
        [DefaultValue(null)]
        public long duration;
        [DefaultValue(null)]
        public double start;
        [DefaultValue(null)]
        public double end;
        public bool needsLastPoint;
        public List<int> targets = new List<int>();

        public List<List<object>> dpsStats;
        public List<List<List<object>>> dpsStatsTargets;
        public List<List<List<object>>> dmgStatsTargets;
        public List<List<object>> dmgStats;
        public List<List<object>> defStats;
        public List<List<object>> healStats;

        public List<BoonData> boonStats;
        public List<BoonData> boonGenSelfStats;
        public List<BoonData> boonGenGroupStats;
        public List<BoonData> boonGenOGroupStats;
        public List<BoonData> boonGenSquadStats;

        public List<BoonData> offBuffStats;
        public List<BoonData> offBuffGenSelfStats;
        public List<BoonData> offBuffGenGroupStats;
        public List<BoonData> offBuffGenOGroupStats;
        public List<BoonData> offBuffGenSquadStats;

        public List<BoonData> defBuffStats;
        public List<BoonData> defBuffGenSelfStats;
        public List<BoonData> defBuffGenGroupStats;
        public List<BoonData> defBuffGenOGroupStats;
        public List<BoonData> defBuffGenSquadStats;

        public List<BoonData> persBuffStats;

        public List<List<object[]>> dmgModifiersCommon = new List<List<object[]>>();
        public List<List<List<object[]>>> dmgModifiersTargetsCommon = new List<List<List<object[]>>>();

        public List<List<BoonData>> targetsCondiStats;
        public List<BoonData> targetsCondiTotals;
        public List<BoonData> targetsBoonTotals;

        public List<List<int[]>> mechanicStats;
        public List<List<int[]>> enemyMechanicStats;

        public List<double> markupLines;
        public List<AreaLabelDto> markupAreas;
        public List<int> subPhases;
    }
}
