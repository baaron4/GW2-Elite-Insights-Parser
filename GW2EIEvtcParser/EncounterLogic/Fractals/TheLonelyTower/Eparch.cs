using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Eparch : LonelyTower
    {
        public Eparch(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            });
            Extension = "eparch";
            Icon = EncounterIconEparch;
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            const int healthCMRelease = 32_618_906;
            const int healthThreshold = (int)(0.95 * healthCMRelease); // fractals lose hp as their scale lowers
            AbstractSingleActor eparch = GetEparchActor();
            if (combatData.GetBuildEvent().Build >= GW2Builds.June2024LonelyTowerCMRelease && eparch.GetHealth(combatData) >= healthThreshold)
            {
                return FightData.EncounterMode.CM;
            }
            else
            {
                return FightData.EncounterMode.Normal;
            }
        }

        internal override string GetLogicName(CombatData combatData, AgentData agentData)
        {
            return "Eparch";
        }

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
            
            int crueltyCount = 1;
            int judgementCount = 1;
            int avatarCount = 1;
            foreach (NPC target in _targets)
            {
                switch (target.ID) {
                    case (int)TrashID.IncarnationOfCruelty:
                        target.OverrideName(target.Character + " " + crueltyCount++);
                        break;
                    case (int)TrashID.IncarnationOfJudgement:
                        target.OverrideName(target.Character + " " + judgementCount++);
                        break;
                    case (int)TrashID.AvatarOfSpite:
                        target.OverrideName(target.Character + " " + avatarCount++);
                        break;
                }
            }
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor eparch = GetEparchActor();
            var determinedApplies = combatData.GetBuffDataByIDByDst(Determined762, eparch.AgentItem).OfType<BuffApplyEvent>().ToList();
            if (determinedApplies.Count >= 3)
            {
                fightData.SetSuccess(true, determinedApplies[2].Time);
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor eparch = GetEparchActor();
            phases[0].AddTarget(eparch);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, eparch, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0) {
                    phase.Name = "Split " + i / 2;
                    var ids = new List<int>
                    {
                        (int)TrashID.IncarnationOfCruelty,
                        (int)TrashID.IncarnationOfJudgement,
                    };
                    AddTargetsToPhase(phase, ids);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(eparch);
                }
            }
            return phases;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)TargetID.EparchLonelyTower,
                (int)TrashID.IncarnationOfCruelty,
                (int)TrashID.IncarnationOfJudgement,
                (int)TrashID.AvatarOfSpite,
            };
        }

        protected override Dictionary<int, int> GetTargetsSortIDs()
        {
            return new Dictionary<int, int>()
            {
                {(int)TargetID.EparchLonelyTower, 0},
                {(int)TrashID.IncarnationOfCruelty, 1},
                {(int)TrashID.IncarnationOfJudgement, 1},
                {(int)TrashID.AvatarOfSpite, 2},
            };
        }

        protected override List<TrashID> GetTrashMobsIDs()
        {
            return new List<TrashID>
            {
                TrashID.TheTormentedLonelyTower,
                TrashID.TheCravenLonelyTower,
            };
        }

        private AbstractSingleActor GetEparchActor()
        {
            AbstractSingleActor eparch = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.EparchLonelyTower));
            if (eparch == null)
            {
                throw new MissingKeyActorsException("Eparch not found");
            }
            return eparch;
        }
    }
}
