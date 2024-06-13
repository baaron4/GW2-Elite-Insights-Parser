using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels
{

    internal class BuffsContainerDto
    {
        // all
        public List<BuffData> BoonStats { get; set; }
        public List<List<BuffData>> BoonDictionaries { get; set; }
        public List<BuffData> BoonGenSelfStats { get; set; }
        public List<BuffData> BoonGenGroupStats { get; set; }
        public List<BuffData> BoonGenOGroupStats { get; set; }
        public List<BuffData> BoonGenSquadStats { get; set; }

        public List<BuffData> OffBuffStats { get; set; }
        public List<BuffData> OffBuffGenSelfStats { get; set; }
        public List<BuffData> OffBuffGenGroupStats { get; set; }
        public List<BuffData> OffBuffGenOGroupStats { get; set; }
        public List<BuffData> OffBuffGenSquadStats { get; set; }

        public List<BuffData> SupBuffStats { get; set; }
        public List<BuffData> SupBuffGenSelfStats { get; set; }
        public List<BuffData> SupBuffGenGroupStats { get; set; }
        public List<BuffData> SupBuffGenOGroupStats { get; set; }
        public List<BuffData> SupBuffGenSquadStats { get; set; }

        public List<BuffData> DefBuffStats { get; set; }
        public List<BuffData> DefBuffGenSelfStats { get; set; }
        public List<BuffData> DefBuffGenGroupStats { get; set; }
        public List<BuffData> DefBuffGenOGroupStats { get; set; }
        public List<BuffData> DefBuffGenSquadStats { get; set; }

        public List<BuffData> ConditionsStats { get; set; }
        public List<BuffData> PersBuffStats { get; set; }
        public List<BuffData> GearBuffStats { get; set; }
        public List<BuffData> NourishmentStats { get; set; }
        public List<BuffData> EnhancementStats { get; set; }
        public List<BuffData> OtherConsumableStats { get; set; }
        public List<BuffData> DebuffStats { get; set; }

        // active
        public List<BuffData> BoonActiveStats { get; set; }
        public List<List<BuffData>> BoonActiveDictionaries { get; set; }
        public List<BuffData> BoonGenActiveSelfStats { get; set; }
        public List<BuffData> BoonGenActiveGroupStats { get; set; }
        public List<BuffData> BoonGenActiveOGroupStats { get; set; }
        public List<BuffData> BoonGenActiveSquadStats { get; set; }

        public List<BuffData> OffBuffActiveStats { get; set; }
        public List<BuffData> OffBuffGenActiveSelfStats { get; set; }
        public List<BuffData> OffBuffGenActiveGroupStats { get; set; }
        public List<BuffData> OffBuffGenActiveOGroupStats { get; set; }
        public List<BuffData> OffBuffGenActiveSquadStats { get; set; }

        public List<BuffData> SupBuffActiveStats { get; set; }
        public List<BuffData> SupBuffGenActiveSelfStats { get; set; }
        public List<BuffData> SupBuffGenActiveGroupStats { get; set; }
        public List<BuffData> SupBuffGenActiveOGroupStats { get; set; }
        public List<BuffData> SupBuffGenActiveSquadStats { get; set; }

        public List<BuffData> DefBuffActiveStats { get; set; }
        public List<BuffData> DefBuffGenActiveSelfStats { get; set; }
        public List<BuffData> DefBuffGenActiveGroupStats { get; set; }
        public List<BuffData> DefBuffGenActiveOGroupStats { get; set; }
        public List<BuffData> DefBuffGenActiveSquadStats { get; set; }

        public List<BuffData> ConditionsActiveStats { get; set; }
        public List<BuffData> PersBuffActiveStats { get; set; }
        public List<BuffData> GearBuffActiveStats { get; set; }
        public List<BuffData> DebuffActiveStats { get; set; }

        public List<List<BuffData>> TargetsCondiStats { get; set; }
        public List<BuffData> TargetsCondiUptimes { get; set; }
        public List<BuffData> TargetsBoonUptimes { get; set; }

        public BuffsContainerDto(PhaseData phase, ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> persBuffDict)
        {
            StatisticsHelper statistics = log.StatisticsHelper;
            //
            BoonStats = BuffData.BuildBuffUptimeData(log, statistics.PresentBoons, phase);
            BoonDictionaries = BuffData.BuildBuffDictionariesData(log, statistics.PresentBoons, phase);
            OffBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentOffbuffs, phase);
            SupBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentSupbuffs, phase);
            DefBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentDefbuffs, phase);
            PersBuffStats = BuffData.BuildPersonalBuffUptimeData(log, persBuffDict, phase);
            GearBuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentGearbuffs, phase);
            NourishmentStats = BuffData.BuildBuffUptimeData(log, statistics.PresentNourishements, phase);
            EnhancementStats = BuffData.BuildBuffUptimeData(log, statistics.PresentEnhancements, phase);
            OtherConsumableStats = BuffData.BuildBuffUptimeData(log, statistics.PresentOtherConsumables, phase);
            DebuffStats = BuffData.BuildBuffUptimeData(log, statistics.PresentDebuffs, phase);
            ConditionsStats = BuffData.BuildBuffUptimeData(log, statistics.PresentConditions, phase);
            BoonGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Self);
            BoonGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Group);
            BoonGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.OffGroup);
            BoonGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Squad);
            OffBuffGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Self);
            OffBuffGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Group);
            OffBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.OffGroup);
            OffBuffGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Squad);
            SupBuffGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Self);
            SupBuffGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Group);
            SupBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.OffGroup);
            SupBuffGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Squad);
            DefBuffGenSelfStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Self);
            DefBuffGenGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Group);
            DefBuffGenOGroupStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.OffGroup);
            DefBuffGenSquadStats = BuffData.BuildBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Squad);
            //
            BoonActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentBoons, phase);
            BoonActiveDictionaries = BuffData.BuildActiveBuffDictionariesData(log, statistics.PresentBoons, phase);
            OffBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentOffbuffs, phase);
            SupBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentSupbuffs, phase);
            DefBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentDefbuffs, phase);
            PersBuffActiveStats = BuffData.BuildActivePersonalBuffUptimeData(log, persBuffDict, phase);
            GearBuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentGearbuffs, phase);
            DebuffActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentDebuffs, phase);
            ConditionsActiveStats = BuffData.BuildActiveBuffUptimeData(log, statistics.PresentConditions, phase);
            BoonGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Self);
            BoonGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Group);
            BoonGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.OffGroup);
            BoonGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentBoons, phase, BuffEnum.Squad);
            OffBuffGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Self);
            OffBuffGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Group);
            OffBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.OffGroup);
            OffBuffGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentOffbuffs, phase, BuffEnum.Squad);
            SupBuffGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Self);
            SupBuffGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Group);
            SupBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.OffGroup);
            SupBuffGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentSupbuffs, phase, BuffEnum.Squad);
            DefBuffGenActiveSelfStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Self);
            DefBuffGenActiveGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Group);
            DefBuffGenActiveOGroupStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.OffGroup);
            DefBuffGenActiveSquadStats = BuffData.BuildActiveBuffGenerationData(log, statistics.PresentDefbuffs, phase, BuffEnum.Squad);


            TargetsCondiStats = new List<List<BuffData>>();
            TargetsCondiUptimes = new List<BuffData>();
            TargetsBoonUptimes = new List<BuffData>();
            foreach (AbstractSingleActor target in phase.AllTargets)
            {
                TargetsCondiStats.Add(BuffData.BuildTargetCondiData(log, phase, target));
                TargetsCondiUptimes.Add(BuffData.BuildTargetCondiUptimeData(log, phase, target));
                TargetsBoonUptimes.Add(BuffData.BuildTargetBoonUptimeData(log, phase, target));
            }
        }
    }
}
