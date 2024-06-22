using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIBuilders.HtmlModels.HTMLStats;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIBuilders.HtmlModels
{

    internal class BuffVolumesContainerDto
    {
        // all
        public List<BuffVolumeData> BoonVolumeStats { get; set; }
        public List<List<BuffVolumeData>> BoonVolumeDictionaries { get; set; }
        public List<BuffVolumeData> BoonOutgoingVolumeSelfStats { get; set; }
        public List<BuffVolumeData> BoonOutgoingVolumeGroupStats { get; set; }
        public List<BuffVolumeData> BoonOutgoingVolumeOGroupStats { get; set; }
        public List<BuffVolumeData> BoonOutgoingVolumeSquadStats { get; set; }

        public List<BuffVolumeData> OffBuffVolumeStats { get; set; }
        public List<BuffVolumeData> OffBuffOutgoingVolumeSelfStats { get; set; }
        public List<BuffVolumeData> OffBuffOutgoingVolumeGroupStats { get; set; }
        public List<BuffVolumeData> OffBuffOutgoingVolumeOGroupStats { get; set; }
        public List<BuffVolumeData> OffBuffOutgoingVolumeSquadStats { get; set; }

        public List<BuffVolumeData> SupBuffVolumeStats { get; set; }
        public List<BuffVolumeData> SupBuffOutgoingVolumeSelfStats { get; set; }
        public List<BuffVolumeData> SupBuffOutgoingVolumeGroupStats { get; set; }
        public List<BuffVolumeData> SupBuffOutgoingVolumeOGroupStats { get; set; }
        public List<BuffVolumeData> SupBuffOutgoingVolumeSquadStats { get; set; }

        public List<BuffVolumeData> DefBuffVolumeStats { get; set; }
        public List<BuffVolumeData> DefBuffOutgoingVolumeSelfStats { get; set; }
        public List<BuffVolumeData> DefBuffOutgoingVolumeGroupStats { get; set; }
        public List<BuffVolumeData> DefBuffOutgoingVolumeOGroupStats { get; set; }
        public List<BuffVolumeData> DefBuffOutgoingVolumeSquadStats { get; set; }

        public List<BuffVolumeData> ConditionsVolumeStats { get; set; }
        public List<BuffVolumeData> PersBuffVolumeStats { get; set; }
        public List<BuffVolumeData> GearBuffVolumeStats { get; set; }
        public List<BuffVolumeData> DebuffVolumeStats { get; set; }

        // active
        public List<BuffVolumeData> BoonVolumeActiveStats { get; set; }
        public List<List<BuffVolumeData>> BoonVolumeActiveDictionaries { get; set; }
        public List<BuffVolumeData> BoonOutgoingVolumeActiveSelfStats { get; set; }
        public List<BuffVolumeData> BoonOutgoingVolumeActiveGroupStats { get; set; }
        public List<BuffVolumeData> BoonOutgoingVolumeActiveOGroupStats { get; set; }
        public List<BuffVolumeData> BoonOutgoingVolumeActiveSquadStats { get; set; }

        public List<BuffVolumeData> OffBuffVolumeActiveStats { get; set; }
        public List<BuffVolumeData> OffBuffOutgoingVolumeActiveSelfStats { get; set; }
        public List<BuffVolumeData> OffBuffOutgoingVolumeActiveGroupStats { get; set; }
        public List<BuffVolumeData> OffBuffOutgoingVolumeActiveOGroupStats { get; set; }
        public List<BuffVolumeData> OffBuffOutgoingVolumeActiveSquadStats { get; set; }

        public List<BuffVolumeData> SupBuffVolumeActiveStats { get; set; }
        public List<BuffVolumeData> SupBuffOutgoingVolumeActiveSelfStats { get; set; }
        public List<BuffVolumeData> SupBuffOutgoingVolumeActiveGroupStats { get; set; }
        public List<BuffVolumeData> SupBuffOutgoingVolumeActiveOGroupStats { get; set; }
        public List<BuffVolumeData> SupBuffOutgoingVolumeActiveSquadStats { get; set; }

        public List<BuffVolumeData> DefBuffVolumeActiveStats { get; set; }
        public List<BuffVolumeData> DefBuffOutgoingVolumeActiveSelfStats { get; set; }
        public List<BuffVolumeData> DefBuffOutgoingVolumeActiveGroupStats { get; set; }
        public List<BuffVolumeData> DefBuffOutgoingVolumeActiveOGroupStats { get; set; }
        public List<BuffVolumeData> DefBuffOutgoingVolumeActiveSquadStats { get; set; }

        public List<BuffVolumeData> ConditionsVolumeActiveStats { get; set; }
        public List<BuffVolumeData> PersBuffVolumeActiveStats { get; set; }
        public List<BuffVolumeData> GearBuffVolumeActiveStats { get; set; }
        public List<BuffVolumeData> DebuffVolumeActiveStats { get; set; }

        public List<List<BuffVolumeData>> TargetsCondiVolumeStats { get; set; }
        public List<BuffVolumeData> TargetsCondiIncomingVolumeTotals { get; set; }
        public List<BuffVolumeData> TargetsBoonIncomingVolumeTotals { get; set; }

        public BuffVolumesContainerDto(PhaseData phase, ParsedEvtcLog log, IReadOnlyDictionary<Spec, IReadOnlyList<Buff>> persBuffDict)
        {
            StatisticsHelper statistics = log.StatisticsHelper;
            //
            BoonVolumeStats = BuffVolumeData.BuildBuffIncomingVolumeData(log, statistics.PresentBoons, phase);
            BoonVolumeDictionaries = BuffVolumeData.BuildBuffVolumeDictionariesData(log, statistics.PresentBoons, phase);
            OffBuffVolumeStats = BuffVolumeData.BuildBuffIncomingVolumeData(log, statistics.PresentOffbuffs, phase);
            SupBuffVolumeStats = BuffVolumeData.BuildBuffIncomingVolumeData(log, statistics.PresentSupbuffs, phase);
            DefBuffVolumeStats = BuffVolumeData.BuildBuffIncomingVolumeData(log, statistics.PresentDefbuffs, phase);
            PersBuffVolumeStats = BuffVolumeData.BuildPersonalBuffIncomingVolueData(log, persBuffDict, phase);
            GearBuffVolumeStats = BuffVolumeData.BuildBuffIncomingVolumeData(log, statistics.PresentGearbuffs, phase);
            DebuffVolumeStats = BuffVolumeData.BuildBuffIncomingVolumeData(log, statistics.PresentDebuffs, phase);
            ConditionsVolumeStats = BuffVolumeData.BuildBuffIncomingVolumeData(log, statistics.PresentConditions, phase);
            BoonOutgoingVolumeSelfStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentBoons, phase, BuffEnum.Self);
            BoonOutgoingVolumeGroupStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentBoons, phase, BuffEnum.Group);
            BoonOutgoingVolumeOGroupStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentBoons, phase, BuffEnum.OffGroup);
            BoonOutgoingVolumeSquadStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentBoons, phase, BuffEnum.Squad);
            OffBuffOutgoingVolumeSelfStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentOffbuffs, phase, BuffEnum.Self);
            OffBuffOutgoingVolumeGroupStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentOffbuffs, phase, BuffEnum.Group);
            OffBuffOutgoingVolumeOGroupStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentOffbuffs, phase, BuffEnum.OffGroup);
            OffBuffOutgoingVolumeSquadStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentOffbuffs, phase, BuffEnum.Squad);
            SupBuffOutgoingVolumeSelfStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentSupbuffs, phase, BuffEnum.Self);
            SupBuffOutgoingVolumeGroupStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentSupbuffs, phase, BuffEnum.Group);
            SupBuffOutgoingVolumeOGroupStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentSupbuffs, phase, BuffEnum.OffGroup);
            SupBuffOutgoingVolumeSquadStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentSupbuffs, phase, BuffEnum.Squad);
            DefBuffOutgoingVolumeSelfStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentDefbuffs, phase, BuffEnum.Self);
            DefBuffOutgoingVolumeGroupStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentDefbuffs, phase, BuffEnum.Group);
            DefBuffOutgoingVolumeOGroupStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentDefbuffs, phase, BuffEnum.OffGroup);
            DefBuffOutgoingVolumeSquadStats = BuffVolumeData.BuildBuffOutgoingVolumeData(log, statistics.PresentDefbuffs, phase, BuffEnum.Squad);
            //
            BoonVolumeActiveStats = BuffVolumeData.BuildActiveBuffIncomingVolumeData(log, statistics.PresentBoons, phase);
            BoonVolumeActiveDictionaries = BuffVolumeData.BuildActiveBuffVolumeDictionariesData(log, statistics.PresentBoons, phase);
            OffBuffVolumeActiveStats = BuffVolumeData.BuildActiveBuffIncomingVolumeData(log, statistics.PresentOffbuffs, phase);
            SupBuffVolumeActiveStats = BuffVolumeData.BuildActiveBuffIncomingVolumeData(log, statistics.PresentSupbuffs, phase);
            DefBuffVolumeActiveStats = BuffVolumeData.BuildActiveBuffIncomingVolumeData(log, statistics.PresentDefbuffs, phase);
            PersBuffVolumeActiveStats = BuffVolumeData.BuildActivePersonalBuffIncomingVolumeData(log, persBuffDict, phase);
            GearBuffVolumeActiveStats = BuffVolumeData.BuildActiveBuffIncomingVolumeData(log, statistics.PresentGearbuffs, phase);
            DebuffVolumeActiveStats = BuffVolumeData.BuildActiveBuffIncomingVolumeData(log, statistics.PresentDebuffs, phase);
            ConditionsVolumeActiveStats = BuffVolumeData.BuildActiveBuffIncomingVolumeData(log, statistics.PresentConditions, phase);
            BoonOutgoingVolumeActiveSelfStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentBoons, phase, BuffEnum.Self);
            BoonOutgoingVolumeActiveGroupStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentBoons, phase, BuffEnum.Group);
            BoonOutgoingVolumeActiveOGroupStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentBoons, phase, BuffEnum.OffGroup);
            BoonOutgoingVolumeActiveSquadStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentBoons, phase, BuffEnum.Squad);
            OffBuffOutgoingVolumeActiveSelfStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentOffbuffs, phase, BuffEnum.Self);
            OffBuffOutgoingVolumeActiveGroupStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentOffbuffs, phase, BuffEnum.Group);
            OffBuffOutgoingVolumeActiveOGroupStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentOffbuffs, phase, BuffEnum.OffGroup);
            OffBuffOutgoingVolumeActiveSquadStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentOffbuffs, phase, BuffEnum.Squad);
            SupBuffOutgoingVolumeActiveSelfStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentSupbuffs, phase, BuffEnum.Self);
            SupBuffOutgoingVolumeActiveGroupStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentSupbuffs, phase, BuffEnum.Group);
            SupBuffOutgoingVolumeActiveOGroupStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentSupbuffs, phase, BuffEnum.OffGroup);
            SupBuffOutgoingVolumeActiveSquadStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentSupbuffs, phase, BuffEnum.Squad);
            DefBuffOutgoingVolumeActiveSelfStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentDefbuffs, phase, BuffEnum.Self);
            DefBuffOutgoingVolumeActiveGroupStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentDefbuffs, phase, BuffEnum.Group);
            DefBuffOutgoingVolumeActiveOGroupStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentDefbuffs, phase, BuffEnum.OffGroup);
            DefBuffOutgoingVolumeActiveSquadStats = BuffVolumeData.BuildActiveBuffOutgoingVolumeData(log, statistics.PresentDefbuffs, phase, BuffEnum.Squad);


            TargetsCondiVolumeStats = new List<List<BuffVolumeData>>();
            TargetsCondiIncomingVolumeTotals = new List<BuffVolumeData>();
            TargetsBoonIncomingVolumeTotals = new List<BuffVolumeData>();
            foreach (AbstractSingleActor target in phase.AllTargets)
            {
                TargetsCondiVolumeStats.Add(BuffVolumeData.BuildTargetCondiVolumeData(log, phase, target));
                TargetsCondiIncomingVolumeTotals.Add(BuffVolumeData.BuildTargetCondiIncomingVolumeData(log, phase, target));
                TargetsBoonIncomingVolumeTotals.Add(BuffVolumeData.BuildTargetBoonIncomingVolumeData(log, phase, target));
            }
        }
    }
}
