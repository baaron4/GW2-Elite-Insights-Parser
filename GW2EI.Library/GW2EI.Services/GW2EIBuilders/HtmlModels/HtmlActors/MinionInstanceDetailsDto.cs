using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIBuilders.HtmlModels.LogDataDto;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class MinionInstanceDetailsDto
{
    public int UniqueID { get; set; }
    public string Name { get; set; }
    public string Icon { get; set; }
    public List<List<double[]>?> HealthStates { get; set; }
    public List<List<double[]>?> BarrierStates { get; set; }
    public List<List<BuffChartDataDto>> BoonGraph { get; set; }
    public List<BuffData> BoonStats { get; set; }
    public List<BuffData> BoonActiveStats { get; set; }
    public List<List<BuffData>> BoonDictionaries { get; set; }
    public List<List<BuffData>> BoonActiveDictionaries { get; set; }
    public List<BuffVolumeData> BoonVolumeStats { get; set; }
    public List<BuffVolumeData> BoonVolumeActiveStats { get; set; }
    public List<List<BuffVolumeData>> BoonVolumeDictionaries { get; set; }
    public List<List<BuffVolumeData>> BoonVolumeActiveDictionaries { get; set; }

    private MinionInstanceDetailsDto(NPC minion, int phaseCount)
    {
        UniqueID = minion.UniqueID;
        Name = minion.Character;
        Icon = minion.GetIcon();
        HealthStates = new(phaseCount);
        BarrierStates = new(phaseCount);
        BoonGraph = new(phaseCount);
        BoonStats = new(phaseCount);
        BoonActiveStats = new(phaseCount);
        BoonDictionaries = new(phaseCount);
        BoonActiveDictionaries = new(phaseCount);
        BoonVolumeStats = new(phaseCount);
        BoonVolumeActiveStats = new(phaseCount);
        BoonVolumeDictionaries = new(phaseCount);
        BoonVolumeActiveDictionaries = new(phaseCount);
    }

    public static MinionInstanceDetailsDto Build(ParsedEvtcLog log, NPC minion, UsedData used)
    {
        var phases = log.LogData.GetPhases(log);
        var dto = new MinionInstanceDetailsDto(minion, phases.Count);
        foreach (PhaseData phase in phases)
        {
            dto.HealthStates.Add(ChartDataDto.BuildHealthStates(log, minion, phase, true));
            dto.BarrierStates.Add(ChartDataDto.BuildBarrierStates(log, minion, phase));
            dto.BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, minion, phase, used));
            dto.BoonStats.Add(BuffData.BuildMinionBoonUptimeData(log, minion, phase));
            dto.BoonActiveStats.Add(BuffData.BuildMinionActiveBoonUptimeData(log, minion, phase));
            dto.BoonDictionaries.Add(BuffData.BuildMinionBoonDictionaryData(log, minion, phase));
            dto.BoonActiveDictionaries.Add(BuffData.BuildMinionActiveBoonDictionaryData(log, minion, phase));
            dto.BoonVolumeStats.Add(BuffVolumeData.BuildMinionBoonIncomingVolumeData(log, minion, phase));
            dto.BoonVolumeActiveStats.Add(BuffVolumeData.BuildMinionActiveBoonIncomingVolumeData(log, minion, phase));
            dto.BoonVolumeDictionaries.Add(BuffVolumeData.BuildMinionBoonVolumeDictionaryData(log, minion, phase));
            dto.BoonVolumeActiveDictionaries.Add(BuffVolumeData.BuildMinionActiveBoonVolumeDictionaryData(log, minion, phase));
        }
        return dto;
    }
}
