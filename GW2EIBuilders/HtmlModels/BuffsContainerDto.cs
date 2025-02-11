using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels;


internal class BuffsContainerDto
{
    // all
    public List<BuffData> BoonStats;
    public List<List<BuffData>> BoonDictionaries;
    public List<BuffData> BoonGenSelfStats;
    public List<BuffData> BoonGenGroupStats;
    public List<BuffData> BoonGenOGroupStats;
    public List<BuffData> BoonGenSquadStats;

    public List<BuffData> OffBuffStats;
    public List<BuffData> OffBuffGenSelfStats;
    public List<BuffData> OffBuffGenGroupStats;
    public List<BuffData> OffBuffGenOGroupStats;
    public List<BuffData> OffBuffGenSquadStats;

    public List<BuffData> SupBuffStats;
    public List<BuffData> SupBuffGenSelfStats;
    public List<BuffData> SupBuffGenGroupStats;
    public List<BuffData> SupBuffGenOGroupStats;
    public List<BuffData> SupBuffGenSquadStats;

    public List<BuffData> DefBuffStats;
    public List<BuffData> DefBuffGenSelfStats;
    public List<BuffData> DefBuffGenGroupStats;
    public List<BuffData> DefBuffGenOGroupStats;
    public List<BuffData> DefBuffGenSquadStats;

    public List<BuffData> ConditionsStats;
    public List<BuffData> PersBuffStats;
    public List<BuffData> GearBuffStats;
    public List<BuffData> NourishmentStats;
    public List<BuffData> EnhancementStats;
    public List<BuffData> OtherConsumableStats;
    public List<BuffData> DebuffStats;

    // active
    public List<BuffData> BoonActiveStats;
    public List<List<BuffData>> BoonActiveDictionaries;
    public List<BuffData> BoonGenActiveSelfStats;
    public List<BuffData> BoonGenActiveGroupStats;
    public List<BuffData> BoonGenActiveOGroupStats;
    public List<BuffData> BoonGenActiveSquadStats;

    public List<BuffData> OffBuffActiveStats;
    public List<BuffData> OffBuffGenActiveSelfStats;
    public List<BuffData> OffBuffGenActiveGroupStats;
    public List<BuffData> OffBuffGenActiveOGroupStats;
    public List<BuffData> OffBuffGenActiveSquadStats;

    public List<BuffData> SupBuffActiveStats;
    public List<BuffData> SupBuffGenActiveSelfStats;
    public List<BuffData> SupBuffGenActiveGroupStats;
    public List<BuffData> SupBuffGenActiveOGroupStats;
    public List<BuffData> SupBuffGenActiveSquadStats;

    public List<BuffData> DefBuffActiveStats;
    public List<BuffData> DefBuffGenActiveSelfStats;
    public List<BuffData> DefBuffGenActiveGroupStats;
    public List<BuffData> DefBuffGenActiveOGroupStats;
    public List<BuffData> DefBuffGenActiveSquadStats;

    public List<BuffData> ConditionsActiveStats;
    public List<BuffData> PersBuffActiveStats;
    public List<BuffData> GearBuffActiveStats;
    public List<BuffData> DebuffActiveStats;

    public List<List<BuffData>> TargetsCondiStats;
    public List<BuffData> TargetsCondiUptimes;
    public List<BuffData> TargetsBoonUptimes;

    public BuffsContainerDto(PhaseData phase, ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> persBuffDict)
    {
        StatisticsHelper statistics = log.StatisticsHelper;
        //
        BoonStats             = BuffData.BuildBuffUptimeData(log, statistics.PresentBoons, phase);
        BoonDictionaries      = BuffData.BuildBuffDictionariesData(log, statistics.PresentBoons, phase);
        OffBuffStats          = BuffData.BuildBuffUptimeData(log, statistics.PresentOffbuffs, phase);
        SupBuffStats          = BuffData.BuildBuffUptimeData(log, statistics.PresentSupbuffs, phase);
        DefBuffStats          = BuffData.BuildBuffUptimeData(log, statistics.PresentDefbuffs, phase);
        PersBuffStats         = BuffData.BuildPersonalBuffUptimeData(log, persBuffDict, phase);
        GearBuffStats         = BuffData.BuildBuffUptimeData(log, statistics.PresentGearbuffs, phase);
        NourishmentStats      = BuffData.BuildBuffUptimeData(log, statistics.PresentNourishements, phase);
        EnhancementStats      = BuffData.BuildBuffUptimeData(log, statistics.PresentEnhancements, phase);
        OtherConsumableStats  = BuffData.BuildBuffUptimeData(log, statistics.PresentOtherConsumables, phase);
        DebuffStats           = BuffData.BuildBuffUptimeData(log, statistics.PresentDebuffs, phase);
        ConditionsStats       = BuffData.BuildBuffUptimeData(log, statistics.PresentConditions, phase);
        BoonGenSelfStats      = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Self);
        BoonGenGroupStats     = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Group);
        BoonGenOGroupStats    = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.OffGroup);
        BoonGenSquadStats     = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Squad);
        OffBuffGenSelfStats   = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Self);
        OffBuffGenGroupStats  = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Group);
        OffBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.OffGroup);
        OffBuffGenSquadStats  = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Squad);
        SupBuffGenSelfStats   = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Self);
        SupBuffGenGroupStats  = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Group);
        SupBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.OffGroup);
        SupBuffGenSquadStats  = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Squad);
        DefBuffGenSelfStats   = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Self);
        DefBuffGenGroupStats  = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Group);
        DefBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.OffGroup);
        DefBuffGenSquadStats  = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Squad);
        //
        BoonActiveStats             = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentBoons, phase);
        BoonActiveDictionaries      = BuffData.BuildActiveBuffDictionariesData(log, statistics.PresentBoons, phase);
        OffBuffActiveStats          = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentOffbuffs, phase);
        SupBuffActiveStats          = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentSupbuffs, phase);
        DefBuffActiveStats          = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentDefbuffs, phase);
        PersBuffActiveStats         = BuffData.BuildActivePersonalBuffUptimeData(log, persBuffDict, phase);
        GearBuffActiveStats         = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentGearbuffs, phase);
        DebuffActiveStats           = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentDebuffs, phase);
        ConditionsActiveStats       = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentConditions, phase);
        BoonGenActiveSelfStats      = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Self);
        BoonGenActiveGroupStats     = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Group);
        BoonGenActiveOGroupStats    = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.OffGroup);
        BoonGenActiveSquadStats     = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Squad);
        OffBuffGenActiveSelfStats   = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Self);
        OffBuffGenActiveGroupStats  = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Group);
        OffBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.OffGroup);
        OffBuffGenActiveSquadStats  = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Squad);
        SupBuffGenActiveSelfStats   = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Self);
        SupBuffGenActiveGroupStats  = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Group);
        SupBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.OffGroup);
        SupBuffGenActiveSquadStats  = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Squad);
        DefBuffGenActiveSelfStats   = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Self);
        DefBuffGenActiveGroupStats  = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Group);
        DefBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.OffGroup);
        DefBuffGenActiveSquadStats  = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Squad);


        TargetsCondiStats   = new (phase.Targets.Count);
        TargetsCondiUptimes = new(phase.Targets.Count);
        TargetsBoonUptimes  = new(phase.Targets.Count);
        foreach (SingleActor target in phase.Targets.Keys)
        {
            TargetsCondiStats.Add(BuffData.BuildTargetCondiData(log, phase, target));
            TargetsCondiUptimes.Add(BuffData.BuildTargetCondiUptimeData(log, phase, target));
            TargetsBoonUptimes.Add(BuffData.BuildTargetBoonUptimeData(log, phase, target));
        }
    }
}
