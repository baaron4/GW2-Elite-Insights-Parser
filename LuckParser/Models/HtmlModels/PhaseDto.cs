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
        [DataMember] public List<List<Object>> dpsStats;
        [DataMember] public List<List<Object>> dmgStatsBoss;
        [DataMember] public List<List<Object>> dmgStats;
        [DataMember] public List<List<Object>> defStats;
        [DataMember] public List<List<Object>> healStats;
        [DataMember] public List<BoonData> boonStats;
        [DataMember] public List<BoonData> offBuffStats;
        [DataMember] public List<BoonData> defBuffStats;

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

        [DataMember] public List<BoonData> bossCondiStats;
        [DataMember] public BoonData bossCondiTotals;
        [DataMember] public BoonData bossBoonTotals;
        [DataMember] public bool bossHasBoons;

        [DataMember] public List<long> deaths;

        [DataMember] public List<List<int[]>> mechanicStats;
        [DataMember] public List<List<int[]>> enemyMechanicStats;

        [DataMember] public List<double> markupLines;
        [DataMember] public List<AreaLabelDto> markupAreas;

        public PhaseDto() { }

        public PhaseDto(string name, long duration)
        {
            this.name = name;
            this.duration = duration;
        }
    }
}
