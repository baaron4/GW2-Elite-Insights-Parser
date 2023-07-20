using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Kanaxai : SilentSurf
    {
        public Kanaxai(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(RendingStorm, "Rending Storm", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Red), "RendStm.H", "Hit by Rending Storm (Axe AoE)", "Rending Storm Hit", 0),
                new PlayerDstHitMechanic(new long [] { HarrowshotDeath, HarrowshotExposure, HarrowshotFear, HarrowshotLethargy, HarrowshotTorment }, "Harrowshot", new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "Harrowshot.H", "Harrowshot (Lost all boons)", "Harrowshot (Boonstrip)", 0),
                new PlayerDstBuffApplyMechanic(ExtremeVulnerability, "Extreme Vulnerability", new MechanicPlotlySetting(Symbols.X, Colors.DarkRed), "ExtVuln.A", "Applied Extreme Vulnerability", "Extreme Vulnerability Application", 150),
                new PlayerDstBuffApplyMechanic(ExposedEODStrike, "Exposed", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Pink), "Expo.A", "Applied Exposed", "Exposed Application (Player)", 0),
                new PlayerDstBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Yellow), "Fear.A", "Fear Applied", "Fear Application", 150),
                new PlayerDstBuffApplyMechanic(Phantasmagoria, "Phantasmagoria", new MechanicPlotlySetting(Symbols.Diamond, Colors.Pink), "Phant.A", "Phantasmagoria Applied (Aspect visible on Island)", "Phantasmagoria Application", 150),
                new EnemyDstBuffApplyMechanic(Exposed31589, "Exposed", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Pink), "Expo.A", "Applied Exposed to Kanaxai", "Exposed Application (Kanaxai)", 150),
            });
            Extension = "kanaxai";
            Icon = EncounterIconKanaxai;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayKanaxai,
                           (334, 370),
                           (-6195, -295, -799, 5685));
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM,
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM,
                (int)ArcDPSEnums.TrashID.AspectOfTorment,
                (int)ArcDPSEnums.TrashID.AspectOfLethargy,
                (int)ArcDPSEnums.TrashID.AspectOfExposure,
                (int)ArcDPSEnums.TrashID.AspectOfDeath,
                (int)ArcDPSEnums.TrashID.AspectOfFear,
            };
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM));
            if (kanaxai == null)
            {
                throw new MissingKeyActorsException("Kanaxai not found");
            }
            phases[0].AddTarget(kanaxai);
            if (!requirePhases)
            {
                return phases;
            }

            // Phases
            List<PhaseData> newPhases = GetPhasesByInvul(log, DeterminedToDestroy, kanaxai, true, true);
            for (int i = 0; i < newPhases.Count; i++)
            {
                switch (i)
                {
                    case 0:
                        newPhases[i].Name = "Phase 1";
                        break;
                    case 1:
                        newPhases[i].Name = "World Cleaver 1";
                        break;
                    case 2:
                        newPhases[i].Name = "Phase 2";
                        break;
                    case 3:
                        newPhases[i].Name = "World Cleaver 2";
                        break;
                    case 4:
                        newPhases[i].Name = "Phase 3";
                        break;
                    default:
                        break;
                }
                newPhases[i].AddTarget(kanaxai);
            }
            phases.AddRange(newPhases);

            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM));
            if (kanaxai == null)
            {
                throw new MissingKeyActorsException("Kanaxai not found");
            }
            BuffApplyEvent invul762Gain = combatData.GetBuffData(Determined762).OfType<BuffApplyEvent>().Where(x => x.To == kanaxai.AgentItem).FirstOrDefault();
            if (invul762Gain != null)
            {
                fightData.SetSuccess(true, invul762Gain.Time);
            }
        }

    }
}
