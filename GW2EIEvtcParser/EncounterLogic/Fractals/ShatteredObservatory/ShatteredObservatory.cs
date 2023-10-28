using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class ShatteredObservatory : FractalLogic
    {
        public ShatteredObservatory(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.ShatteredObservatory;
            EncounterID |= EncounterIDs.FractalMasks.ShatteredObservatoryMask;
        }

        protected static HashSet<AgentItem> GetParticipatingPlayerAgents(AbstractSingleActor target, CombatData combatData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            if (target == null)
            {
                return new HashSet<AgentItem>();
            }
            var participatingPlayerAgents = new HashSet<AgentItem>(combatData.GetDamageTakenData(target.AgentItem).Where(x => playerAgents.Contains(x.From.GetFinalMaster())).Select(x => x.From.GetFinalMaster()));
            participatingPlayerAgents.UnionWith(combatData.GetDamageData(target.AgentItem).Where(x => playerAgents.Contains(x.To.GetFinalMaster())).Select(x => x.To.GetFinalMaster()));
            return participatingPlayerAgents;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // Set manual FractalScale for old logs without the event
            AddFractalScaleEvent(gw2Build, combatData, new List<(ulong, byte)>
            {
                ( GW2Builds.July2017ShatteredObservatoryRelease, 100),
                ( GW2Builds.September2020SunquaPeakRelease, 99),
                ( GW2Builds.SOTOBetaAndSilentSurfNM, 98),
            });
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
        }

        /// <summary>
        /// Returns true if the buff count was not reached so that another method can be called, if necessary
        /// </summary>
        protected static bool SetSuccessByBuffCount(CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents, AbstractSingleActor target, long buffID, int count)
        {
            if (target == null)
            {
                return false;
            }
            var invulsTarget = GetFilteredList(combatData, buffID, target, true, false).Where(x => x.Time >= 0).ToList();
            if (invulsTarget.Count == count)
            {
                AbstractBuffEvent last = invulsTarget.Last();
                if (!(last is BuffApplyEvent))
                {
                    SetSuccessByCombatExit(new List<AbstractSingleActor> { target }, combatData, fightData, playerAgents);
                    return false;
                }
            }
            return true;
        }

        protected void AddCorporealReassignmentDecorations(ParsedEvtcLog log)
        {
            // Corporeal Reassignment domes & explosions
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CorporealReassignmentDome, out IReadOnlyList<EffectEvent> domes))
            {
                foreach (EffectEvent effect in domes)
                {
                    if (log.CombatData.TryGetEffectEndByTrackingId(effect.TrackingID, effect.Time, out long end))
                    {
                        int start = (int)effect.Time;
                        EnvironmentDecorations.Add(new CircleDecoration(220, (start, (int)end), "rgba(0, 50, 200, 0.4)", new PositionConnector(effect.Position)).UsingFilled(false));
                    }
                }
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CorporealReassignmentExplosionDome, out IReadOnlyList<EffectEvent> domeExplosions))
            {
                foreach (EffectEvent effect in domeExplosions)
                {
                    int start = (int)effect.Time - 500;
                    int end = (int)effect.Time;
                    EnvironmentDecorations.Add(new CircleDecoration(220, (start, end), "rgba(0, 50, 200, 0.4)", new PositionConnector(effect.Position)).UsingGrowingEnd(end));
                }
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CorporealReassignmentExplosion1, out IReadOnlyList<EffectEvent> explosions))
            {
                foreach (EffectEvent effect in explosions)
                {
                    int start = (int)effect.Time;
                    int end = start + 500;
                    EnvironmentDecorations.Add(new CircleDecoration(2000, (start, end), "rgba(200, 50, 0, 0.2)", new PositionConnector(effect.Position)).UsingGrowingEnd(end));
                }
            }
        }
    }
}
