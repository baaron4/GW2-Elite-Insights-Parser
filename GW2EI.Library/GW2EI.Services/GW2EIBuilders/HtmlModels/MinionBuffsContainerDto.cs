using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels;

internal class MinionBuffsContainerDto
{
    public List<BuffData> BoonStats { get; set; }
    public List<BuffData> BoonActiveStats { get; set; }

    public List<List<BuffData>> BoonDictionaries { get; set; }
    public List<List<BuffData>> BoonActiveDictionaries { get; set; }

    public MinionBuffsContainerDto(ParsedEvtcLog log, Minions minions, PhaseData phase)
    {
        IReadOnlyList<NPC> instances = minions.MinionList;

        BoonStats = new(instances.Count);
        BoonActiveStats = new(instances.Count);
        BoonDictionaries = new(instances.Count);
        BoonActiveDictionaries = new(instances.Count);

        foreach (NPC minion in instances)
        {
            BoonStats.Add(BuffData.BuildMinionBoonUptimeData(log, minion, phase));
            BoonActiveStats.Add(BuffData.BuildMinionActiveBoonUptimeData(log, minion, phase));
            BoonDictionaries.Add(BuffData.BuildMinionBoonDictionaryData(log, minion, phase));
            BoonActiveDictionaries.Add(BuffData.BuildMinionActiveBoonDictionaryData(log, minion, phase));
        }
    }
}
