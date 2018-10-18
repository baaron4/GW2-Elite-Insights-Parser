using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace LuckParser.Models.HtmlModels
{
    [DataContract]
    public class PhaseDto
    {
        [DataMember] public string name;
        [DataMember] public long duration;
        [DataMember] public double start;
        [DataMember] public double end;
        [DataMember] public List<int> targets = new List<int>();

        [DataMember] public List<List<object>> dpsStats;
        [DataMember] public List<List<List<object>>> dpsStatsTargets;
        [DataMember] public List<List<List<object>>> dmgStatsTargets;
        [DataMember] public List<List<object>> dmgStats;
        [DataMember] public List<List<object>> defStats;
        [DataMember] public List<List<object>> healStats;

        [DataMember] public List<BoonData> boonStats;
        [DataMember] public List<BoonData> offBuffStats;
        [DataMember] public List<BoonData> defBuffStats;
        [DataMember] public List<BoonData> persBuffStats;

        [DataMember] public List<Dictionary<long, List<object[]>>> extraBuffStats;

        [DataMember] public List<BoonData> boonGenSelfStats;
        [DataMember] public List<BoonData> boonGenGroupStats;
        [DataMember] public List<BoonData> boonGenOGroupStats;
        [DataMember] public List<BoonData> boonGenSquadStats;
        [DataMember] public List<BoonData> offBuffGenSelfStats;
        [DataMember] public List<BoonData> offBuffGenGroupStats;
        [DataMember] public List<BoonData> offBuffGenOGroupStats;
        [DataMember] public List<BoonData> offBuffGenSquadStats;
        [DataMember] public List<BoonData> defBuffGenSelfStats;
        [DataMember] public List<BoonData> defBuffGenGroupStats;
        [DataMember] public List<BoonData> defBuffGenOGroupStats;
        [DataMember] public List<BoonData> defBuffGenSquadStats;

        [DataMember] public List<List<BoonData>> targetsCondiStats;
        [DataMember] public List<BoonData> targetsCondiTotals;
        [DataMember] public List<BoonData> targetsBoonTotals;

        [DataMember] public List<List<int[]>> mechanicStats;
        [DataMember] public List<List<int[]>> enemyMechanicStats;

        [DataMember(EmitDefaultValue = false)] public List<double> markupLines;
        [DataMember(EmitDefaultValue = false)] public List<AreaLabelDto> markupAreas;

        public PhaseDto() { }

        public PhaseDto(string name, long duration)
        {
            this.name = name;
            this.duration = duration;
        }
    }
}
