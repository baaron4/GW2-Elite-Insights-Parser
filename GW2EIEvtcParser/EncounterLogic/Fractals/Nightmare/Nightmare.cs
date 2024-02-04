
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

        protected static void AddCascadeOfTormentDecoration(ParsedEvtcLog log, List<GenericDecoration> environmentDecorations, string guid, uint innerRadius, uint outerRadius)
        {
            int duration = 1000;
            if (log.CombatData.TryGetEffectEventsByGUID(guid, out IReadOnlyList<EffectEvent> cascadeOfTorment))
            {
                foreach (EffectEvent effect in cascadeOfTorment)
                {
                    (long start, long end) lifespanIndicator = (effect.Time, effect.Time + duration);
                    (long start, long end) lifespanDamage = (lifespanIndicator.end, lifespanIndicator.end + 150);
                    if (innerRadius == 0)
                    {
                        environmentDecorations.Add(new CircleDecoration(outerRadius, lifespanIndicator, Colors.Orange, 0.2, new PositionConnector(effect.Position)));
                        environmentDecorations.Add(new CircleDecoration(outerRadius, lifespanDamage, Colors.Orange, 0.4, new PositionConnector(effect.Position)));
                    }
                    else
                    {
                        environmentDecorations.Add(new DoughnutDecoration(innerRadius, outerRadius, lifespanIndicator, Colors.Orange, 0.2, new PositionConnector(effect.Position)));
                        environmentDecorations.Add(new DoughnutDecoration(innerRadius, outerRadius, lifespanDamage, Colors.Orange, 0.4, new PositionConnector(effect.Position)));
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
