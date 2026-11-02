using GW2EIBuilders.HtmlModels.HTMLCharts;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIBuilders.HtmlModels.LogDataDto;

namespace GW2EIBuilders.HtmlModels.HTMLActors;

internal class MinionInstanceDetailsDto
{
    // This is fine
    public int UniqueID { get; set; }
    // These should be in MinionDto, Name's already there
    public string Name { get; set; }
    public string Icon { get; set; }
    // These should be stored under PlayerChartDto/TargetChartDto, create MinionChartDto and have a Dictionary of MinionChartDtos
    public List<List<double[]>?> HealthStates { get; set; }
    public List<List<double[]>?> BarrierStates { get; set; }
    // This is fine
    public List<List<BuffChartDataDto>> BoonGraph { get; set; }
    // These should be moved under BuffsContainerDto, create a MinionBuffsContainerDto, as an array of size friendlies, a dictionary of MinionBuffsContainerDtos on each index (+ do the same for enemies, if necessary). Content should repeat the pattern see in BuffsContainerDto, ex BuffsContainerDto.BoonStats, an array[friendlies.Count], yours should be an array[minionList.Count]
    public List<BuffData> BoonStats { get; set; }
    public List<BuffData> BoonActiveStats { get; set; }
    public List<List<BuffData>> BoonDictionaries { get; set; }
    public List<List<BuffData>> BoonActiveDictionaries { get; set; }
    // These should be moved under BuffVolumesContainerDto, create a MinionBuffVolumesContainerDto, as an array of size friendlies, a dictionary of MinionBuffsContainerDto (+ do the same for enemies, if necessary)
    public List<BuffVolumeData> BoonVolumeStats { get; set; }
    public List<BuffVolumeData> BoonVolumeActiveStats { get; set; }
    public List<List<BuffVolumeData>> BoonVolumeDictionaries { get; set; }
    public List<List<BuffVolumeData>> BoonVolumeActiveDictionaries { get; set; }

    public MinionInstanceDetailsDto(ParsedEvtcLog log, NPC minion, UsedHtmlData used)
    {
        var phases = log.LogData.GetPhases(log);
        var phaseCount = phases.Count;

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
        foreach (PhaseData phase in phases)
        {
            HealthStates.Add(ChartDataDto.BuildHealthStates(log, minion, phase, true));
            BarrierStates.Add(ChartDataDto.BuildBarrierStates(log, minion, phase));
            BoonGraph.Add(BuffChartDataDto.BuildBuffGraphData(log, minion, phase, used));
            BoonStats.Add(BuffData.BuildMinionBoonUptimeData(log, minion, phase));
            BoonActiveStats.Add(BuffData.BuildMinionActiveBoonUptimeData(log, minion, phase));
            BoonDictionaries.Add(BuffData.BuildMinionBoonDictionaryData(log, minion, phase));
            BoonActiveDictionaries.Add(BuffData.BuildMinionActiveBoonDictionaryData(log, minion, phase));
            BoonVolumeStats.Add(BuffVolumeData.BuildMinionBoonIncomingVolumeData(log, minion, phase));
            BoonVolumeActiveStats.Add(BuffVolumeData.BuildMinionActiveBoonIncomingVolumeData(log, minion, phase));
            BoonVolumeDictionaries.Add(BuffVolumeData.BuildMinionBoonVolumeDictionaryData(log, minion, phase));
            BoonVolumeActiveDictionaries.Add(BuffVolumeData.BuildMinionActiveBoonVolumeDictionaryData(log, minion, phase));
        }
    }
}
