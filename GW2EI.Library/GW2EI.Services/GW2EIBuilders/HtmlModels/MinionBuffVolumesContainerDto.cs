using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels;

internal class MinionBuffVolumesContainerDto
{
    public List<BuffVolumeData> BoonVolumeStats { get; set; }
    public List<BuffVolumeData> BoonVolumeActiveStats { get; set; }

    public List<List<BuffVolumeData>> BoonVolumeDictionaries { get; set; }
    public List<List<BuffVolumeData>> BoonVolumeActiveDictionaries { get; set; }

    public MinionBuffVolumesContainerDto(ParsedEvtcLog log, Minions minions, PhaseData phase)
    {
        IReadOnlyList<NPC> instances = minions.MinionList;

        BoonVolumeStats = new(instances.Count);
        BoonVolumeActiveStats = new(instances.Count);
        BoonVolumeDictionaries = new(instances.Count);
        BoonVolumeActiveDictionaries = new(instances.Count);

        foreach (NPC minion in instances)
        {
            BoonVolumeStats.Add(BuffVolumeData.BuildMinionBoonIncomingVolumeData(log, minion, phase));
            BoonVolumeActiveStats.Add(BuffVolumeData.BuildMinionActiveBoonIncomingVolumeData(log, minion, phase));
            BoonVolumeDictionaries.Add(BuffVolumeData.BuildMinionBoonVolumeDictionaryData(log, minion, phase));
            BoonVolumeActiveDictionaries.Add(BuffVolumeData.BuildMinionActiveBoonVolumeDictionaryData(log, minion, phase));
        }
    }
}
