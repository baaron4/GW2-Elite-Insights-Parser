
using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class Nightmare : FractalLogic
    {
        public Nightmare(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.Nightmare;
            EncounterID |= EncounterIDs.FractalMasks.NightmareMask;
        }

        protected static void AddCascadeOfTormentDecoration(ParsedEvtcLog log, CombatReplay replay, string cascadeOfTormentEffectGUID, int cotDuration, uint innerRadius, uint outerRadius)
        {
            if (log.CombatData.TryGetEffectEventsByGUID(cascadeOfTormentEffectGUID, out IReadOnlyList<EffectEvent> expulsionEffects))
            {
                foreach (EffectEvent effect in expulsionEffects)
                {
                    long endTime = effect.Time + cotDuration;
                    if (innerRadius == 0)
                    {
                        replay.Decorations.Add(new CircleDecoration(outerRadius, (effect.Time, endTime), Colors.Orange, 0.2, new PositionConnector(effect.Position)));
                        replay.Decorations.Add(new CircleDecoration(outerRadius, (endTime, endTime + 150), Colors.Orange, 0.4, new PositionConnector(effect.Position)));
                    }
                    else
                    {
                        replay.Decorations.Add(new DoughnutDecoration(innerRadius, outerRadius, (effect.Time, endTime), Colors.Orange, 0.2, new PositionConnector(effect.Position)));
                        replay.Decorations.Add(new DoughnutDecoration(innerRadius, outerRadius, (endTime, endTime + 150), Colors.Orange, 0.4, new PositionConnector(effect.Position)));
                    }
                }
            }
        }

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // Set manual FractalScale for old logs without the event
            AddFractalScaleEvent(gw2Build, combatData, new List<(ulong, byte)>
            {
                ( GW2Builds.November2016NightmareRelease, 100),
                ( GW2Builds.July2017ShatteredObservatoryRelease, 99),
                ( GW2Builds.September2020SunquaPeakRelease, 98),
                ( GW2Builds.SOTOBetaAndSilentSurfNM, 97),
            });
            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetFightOffsetByFirstInvulFilter(fightData, agentData, combatData, GenericTriggerID, Determined762);
        }
    }
}
