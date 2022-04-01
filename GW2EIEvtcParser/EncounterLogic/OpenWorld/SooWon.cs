using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic.OpenWorld
{
    internal class SooWon : FightLogic
    {
        public SooWon(int triggerID) : base(triggerID)
        {
            Extension = "soowon";
            Icon = "https://i.imgur.com/lcZGgBC.png";
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterCategoryInformation.Category = EncounterCategory.FightCategory.OpenWorld;
            EncounterCategoryInformation.SubCategory = EncounterCategory.SubFightCategory.OpenWorld;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.SooWonOW);
            AbstractSingleActor tailTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TrashID.SooWonTail);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Soo-Won not found");
            }

            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }

            phases.AddRange(GetPhasesByInvul(log, new long[] { 757, 66242 }, mainTarget, true, true, 0,
                log.FightData.FightEnd));

            var phaseOffset = GetPhaseOffset(log, mainTarget);
            InitPhases(phases, mainTarget, tailTarget, phaseOffset);

            return phases;
        }

        /// <summary>
        /// Calculates on which phase the log started by checking Soo-Won's initial health and invulnerability buffs.
        /// </summary>
        ///
        /// <returns>An integer indicating on which phase the log started:
        /// <code>
        ///  0: 100% - 80%
        ///  1: First Greens
        ///  2: 80% - 60%
        ///  3: First Spear
        ///  4: First Champions
        ///  5: 60% - 40%
        ///  6: Second Greens
        ///  7: 40% - 20%
        ///  8: Second Spear
        ///  9: Second Champions
        /// 10: 20% - 0%
        /// </code>
        /// </returns>
        private static int GetPhaseOffset(ParsedEvtcLog log, AbstractSingleActor mainTarget)
        {
            var initialHealth = mainTarget.GetCurrentHealthPercent(log, 0);
            Func<Func<BuffApplyEvent, bool>, BuffApplyEvent> targetBuffs =
                log.CombatData.GetBuffData(mainTarget.AgentItem).OfType<BuffApplyEvent>().FirstOrDefault;
            AbstractBuffEvent initialInvuln = targetBuffs(x => x.Initial && x.BuffID == 757);
            AbstractBuffEvent initialDmgImmunity = targetBuffs(x => x.Initial && x.BuffID == 66242); // spear phase

            var offset = 0;
            if (initialHealth <= 80 && initialHealth > 60)
            {
                offset = 2;
            }
            else if (initialHealth <= 60 && initialHealth > 40)
            {
                offset = 5;
            }
            else if (initialHealth <= 40 && initialHealth > 20)
            {
                offset = 7;
            }
            else if (initialHealth <= 20 && initialHealth > 0)
            {
                offset = 10;
            }

            if (offset > 0)
            {
                if (initialInvuln != null)
                {
                    offset--;
                }
                else if (initialDmgImmunity != null)
                {
                    offset++;
                }
            }

            return offset;
        }

        private void InitPhases(List<PhaseData> phases, AbstractSingleActor mainTarget,
            AbstractSingleActor tailTarget, int phaseOffset)
        {
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                switch (i + phaseOffset)
                {
                    case 1:
                        phase.Name = "100% - 80%";
                        phase.AddTarget(mainTarget);
                        break;
                    case 2:
                        phase.Name = "First Greens";
                        phase.AddTarget(mainTarget);
                        break;
                    case 3:
                        phase.Name = "80% - 60%";
                        phase.AddTarget(mainTarget);
                        phase.AddTarget(tailTarget);
                        break;
                    case 4:
                        phase.Name = "First Spear";
                        phase.AddTarget(mainTarget);
                        break;
                    case 5:
                        phase.Name = "First Champions";
                        phase.AddTargets(Targets.Where(x =>
                            x.ID == (int)ArcDPSEnums.TrashID.VoidGiant2 ||
                            x.ID == (int)ArcDPSEnums.TrashID.VoidTimeCaster2));
                        break;
                    case 6:
                        phase.Name = "60% - 40%";
                        phase.AddTarget(mainTarget);
                        phase.AddTarget(tailTarget);
                        break;
                    case 7:
                        phase.Name = "Second Greens";
                        phase.AddTarget(mainTarget);
                        break;
                    case 8:
                        phase.Name = "40% - 20%";
                        phase.AddTarget(mainTarget);
                        phase.AddTarget(tailTarget);
                        break;
                    case 9:
                        phase.Name = "Second Spear";
                        phase.AddTarget(mainTarget);
                        break;
                    case 10:
                        phase.Name = "Second Champions";
                        phase.AddTargets(Targets.Where(x =>
                            x.ID == (int)ArcDPSEnums.TrashID.VoidBrandstalker ||
                            x.ID == (int)ArcDPSEnums.TrashID.VoidColdsteel2 ||
                            x.ID == (int)ArcDPSEnums.TrashID.VoidObliterator2));
                        break;
                    case 11:
                        phase.Name = "20% - 0%";
                        phase.AddTarget(mainTarget);
                        phase.AddTarget(tailTarget);
                        break;
                }
            }
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData,
            List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            IReadOnlyList<AgentItem> sooWons = agentData.GetGadgetsByID((int)ArcDPSEnums.TargetID.SooWonOW);
            if (!sooWons.Any())
            {
                throw new MissingKeyActorsException("Soo-Won not found");
            }

            foreach (AgentItem sooWon in sooWons)
            {
                sooWon.OverrideType(AgentItem.AgentType.NPC);
                sooWon.OverrideID(ArcDPSEnums.TargetID.SooWonOW);
            }

            IReadOnlyList<AgentItem> sooWonTails = agentData.GetGadgetsByID((int)ArcDPSEnums.TrashID.SooWonTail);
            foreach (AgentItem sooWonTail in sooWonTails)
            {
                sooWonTail.OverrideType(AgentItem.AgentType.NPC);
                sooWonTail.OverrideID(ArcDPSEnums.TrashID.SooWonTail);
            }

            agentData.Refresh();
            ComputeFightTargets(agentData, combatData, extensions);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData,
            IReadOnlyCollection<AgentItem> playerAgents)
        {
            RewardEvent reward = combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == 13);
            if (reward != null)
            {
                fightData.SetSuccess(true, reward.Time);
            }
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.SooWonOW,
                (int)ArcDPSEnums.TrashID.SooWonTail,
                (int)ArcDPSEnums.TrashID.VoidGiant2,
                (int)ArcDPSEnums.TrashID.VoidTimeCaster2,
                (int)ArcDPSEnums.TrashID.VoidBrandstalker,
                (int)ArcDPSEnums.TrashID.VoidColdsteel2,
                (int)ArcDPSEnums.TrashID.VoidObliterator2,
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.SooWonOW,
                (int)ArcDPSEnums.TrashID.SooWonTail,
                (int)ArcDPSEnums.TrashID.VoidGiant2,
                (int)ArcDPSEnums.TrashID.VoidTimeCaster2,
                (int)ArcDPSEnums.TrashID.VoidBrandstalker,
                (int)ArcDPSEnums.TrashID.VoidColdsteel2,
                (int)ArcDPSEnums.TrashID.VoidObliterator2,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.VoidAbomination,
                ArcDPSEnums.TrashID.VoidAbomination2,
                ArcDPSEnums.TrashID.VoidBomber,
                ArcDPSEnums.TrashID.VoidBrandbeast,
                ArcDPSEnums.TrashID.VoidBrandcharger1,
                ArcDPSEnums.TrashID.VoidBrandcharger2,
                ArcDPSEnums.TrashID.VoidBrandfang1,
                ArcDPSEnums.TrashID.VoidBrandfang2,
                ArcDPSEnums.TrashID.VoidBrandscale1,
                ArcDPSEnums.TrashID.VoidBrandscale2,
                ArcDPSEnums.TrashID.VoidColdsteel,
                ArcDPSEnums.TrashID.VoidColdsteel3,
                ArcDPSEnums.TrashID.VoidCorpseknitter1,
                ArcDPSEnums.TrashID.VoidCorpseknitter2,
                ArcDPSEnums.TrashID.VoidDespoiler1,
                ArcDPSEnums.TrashID.VoidDespoiler2,
                ArcDPSEnums.TrashID.VoidFiend1,
                ArcDPSEnums.TrashID.VoidFiend2,
                ArcDPSEnums.TrashID.VoidFoulmaw,
                ArcDPSEnums.TrashID.VoidFrostwing,
                ArcDPSEnums.TrashID.VoidGlacier1,
                ArcDPSEnums.TrashID.VoidGlacier2,
                ArcDPSEnums.TrashID.VoidInfested1,
                ArcDPSEnums.TrashID.VoidInfested2,
                ArcDPSEnums.TrashID.VoidMelter1,
                ArcDPSEnums.TrashID.VoidMelter2,
                ArcDPSEnums.TrashID.VoidRimewolf1,
                ArcDPSEnums.TrashID.VoidRimewolf2,
                ArcDPSEnums.TrashID.VoidRotspinner1,
                ArcDPSEnums.TrashID.VoidRotswarmer,
                ArcDPSEnums.TrashID.VoidStorm,
                ArcDPSEnums.TrashID.VoidStormseer,
                ArcDPSEnums.TrashID.VoidStormseer2,
                ArcDPSEnums.TrashID.VoidStormseer3,
                ArcDPSEnums.TrashID.VoidTangler,
                ArcDPSEnums.TrashID.VoidTangler2,
                ArcDPSEnums.TrashID.VoidThornheart1,
                ArcDPSEnums.TrashID.VoidThornheart2,
                ArcDPSEnums.TrashID.VoidWarforged2,
                ArcDPSEnums.TrashID.VoidWorm,
            };
        }
    }
}
