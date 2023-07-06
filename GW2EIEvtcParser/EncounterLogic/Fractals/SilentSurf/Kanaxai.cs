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
    internal class Kanaxai : FractalLogic
    {
        public Kanaxai(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(WorldCleaver, "World Cleaver", new MechanicPlotlySetting(Symbols.CircleX, Colors.Red), "WorldClv.H", "Hit by World Cleaver", "World Cleaver Hit", 150),
                new PlayerDstBuffApplyMechanic(DreadVisage, "Dread Visage", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.Black), "DreadVis.A", "Applied Dread Visage", "Dread Visage Application", 2000),
            });
            Extension = "kanaxai";
            Icon = EncounterIconKanaxai;
            EncounterCategoryInformation.SubCategory = SubFightCategory.SilentSurf;
            EncounterID |= EncounterIDs.FractalMasks.SilentSurfMask | 0x000001;
        }

        /*protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayKanaxai,
                           (0, 0),
                           (0, -0, 0, 0));
        }*/

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkus,
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkus,
                (int)ArcDPSEnums.TrashID.AspectOfTorment,
                (int)ArcDPSEnums.TrashID.AspectOfLethargy,
                (int)ArcDPSEnums.TrashID.AspectOfExposure,
                (int)ArcDPSEnums.TrashID.AspectOfDeath,
            };
        }

        //internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        //{
        //    AbstractSingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkus));
        //    if (kanaxai == null)
        //    {
        //        throw new MissingKeyActorsException("Kanaxai not found");
        //    }
        //    return kanaxai.GetHealth(combatData) > 25899216 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
        //}

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkus));
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
                        newPhases[i].Name = "Aspects 1";
                        break;
                    case 2:
                        newPhases[i].Name = "Phase 2";
                        break;
                    case 3:
                        newPhases[i].Name = "Aspects 2";
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
            AbstractSingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkus));
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
