using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class CosmicObservatory : SecretOfTheObscureStrike
    {
        public CosmicObservatory(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            }
            );
            Icon = EncounterIconCosmicObservatory;
            Extension = "cosobs";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayCosmicObservatory,
                            // TODO
                            (1008, 1008),
                            (0,0,0,0));
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Dagda));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Dagda not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Cast check
            var tormentedIDs = new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.TheTormented1,
                ArcDPSEnums.TrashID.TheTormented2,
                ArcDPSEnums.TrashID.TheTormented3,
            };
            var tormentedAgents = new List<AgentItem>();
            foreach (ArcDPSEnums.TrashID tormentedID in tormentedIDs)
            {
                tormentedAgents.AddRange(log.AgentData.GetNPCsByID(tormentedID));
            }
            tormentedAgents = tormentedAgents.OrderBy(x => x.FirstAware).ToList();
            var tormentedGroups = new List<List<AgentItem>>();
            var processedAgents = new HashSet<AgentItem>();
            foreach (AgentItem tormentedAgent in tormentedAgents)
            {
                if (processedAgents.Contains(tormentedAgent))
                {
                    continue;
                }
                var group = new List<AgentItem>();
                AgentItem currentReferenceTormented = tormentedAgent;
                foreach (AgentItem tormentedAgentToBeGrouped in tormentedAgents)
                {
                    if (tormentedAgentToBeGrouped.FirstAware >= currentReferenceTormented.FirstAware && tormentedAgentToBeGrouped.FirstAware <= currentReferenceTormented.LastAware)
                    {
                        group.Add(tormentedAgentToBeGrouped);
                        currentReferenceTormented = tormentedAgentToBeGrouped;
                    }
                }
                foreach (AgentItem agent in group)
                {
                    processedAgents.Add(agent);
                }
                tormentedGroups.Add(group);
            }
            var phaseTimes = new List<(long start, long end)>();
            long previousStart = log.FightData.FightStart;
            for (int i = 0; i < tormentedGroups.Count; i++)
            {
                List<AgentItem> group = tormentedGroups[i];
                long start = Math.Max(log.FightData.FightStart, group.Min(x => x.FirstAware));
                long end = Math.Min(log.FightData.FightEnd, group.Max(x => {
                    long res = x.LastAware;
                    if (log.CombatData.GetDeadEvents(x).Any())
                    {
                        res = log.CombatData.GetDeadEvents(x).Last().Time;
                    }
                    return res;
                }));

                phaseTimes.Add((previousStart, start));
                phaseTimes.Add((start, end));
                previousStart = end;
                if (i == tormentedGroups.Count - 1)
                {
                    phaseTimes.Add((end, log.FightData.FightEnd));
                }
            }
            for (int i = 0; i < phaseTimes.Count; i++)
            {
                (long start, long end) = phaseTimes[i];
                var phase = new PhaseData(start, end);
                if (i % 2 == 1)
                {
                    phase.Name = "Tormenteds " + (i + 1) / 2;
                    var ids = new List<int>
                    {
                        (int)ArcDPSEnums.TrashID.TheTormented1,
                        (int)ArcDPSEnums.TrashID.TheTormented2,
                        (int)ArcDPSEnums.TrashID.TheTormented3,
                    };
                    AddTargetsToPhase(phase, ids);
                }
                else
                {
                    phase.Name = "Phase " + (i + 2) / 2;
                }
                phase.AddTarget(mainTarget);
                phases.Add(phase);
            }
            return phases;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>()
            {
                (int)ArcDPSEnums.TargetID.Dagda,
                (int)ArcDPSEnums.TrashID.TheTormented1,
                (int)ArcDPSEnums.TrashID.TheTormented2,
                (int)ArcDPSEnums.TrashID.TheTormented3,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>()
            {
                ArcDPSEnums.TrashID.SoulFeast
            };
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Cosmic Observatory";
        }
    }
}
