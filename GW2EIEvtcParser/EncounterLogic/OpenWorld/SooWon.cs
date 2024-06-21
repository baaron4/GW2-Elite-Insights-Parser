using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic.OpenWorld
{
    internal class SooWon : OpenWorldLogic
    {
        public SooWon(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(TsunamiSlamOW, "Tsunami Slam", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.DarkRed), "Slam", "Soo-Won slams the ground in front of her creating a circular tsunami", "Tsunami Slam", 0),
            new PlayerDstHitMechanic(VoidPurgeOW, "Void Purge", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkPurple), "Acid", "Player took damage from an acid pool", "Acid Pool", 0),
            new PlayerDstSkillMechanic(ClawSlapOW, "Claw Slap", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Orange), "Claw Slap", "Soo-Won swipes in an arc in front of her knocking players back", "Claw Slap", 0).UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant) ^ de.HasDowned ^ de.HasKilled),
            new PlayerDstHitMechanic(TailSlap, "Tail Slap", new MechanicPlotlySetting(Symbols.Square,Colors.Orange), "Tail Slap", "Soo-Won slaps the majority of the platform, opposite her head, with her tail", "Tail Slap", 0),
            new PlayerDstHitMechanic(BiteOW, "Bite", new MechanicPlotlySetting(Symbols.Diamond,Colors.Orange), "Bite", "Soo-Won bites half the platform while swapping sides", "Bite", 0),
            new PlayerDstSkillMechanic(NightmareDevastationOW1, "Nightmare Devastation", new MechanicPlotlySetting(Symbols.Square,Colors.Purple), "Wave (Half)", "Tidal wave that covers one half of the platform", "Tidal Wave (Half)", 0),
            new PlayerDstSkillMechanic(NightmareDevastationOW2, "Nightmare Devastation", new MechanicPlotlySetting(Symbols.Square,Colors.DarkPurple), "Wave (Full)", "Tidal wave that covers the entire platform", "Tidal Wave (Full)", 0),
            new PlayerDstBuffApplyMechanic(WispForm, "Wisp Form", new MechanicPlotlySetting(Symbols.Circle,Colors.Green), "Wisp", "Wisp Form from standing in a green circle", "Wisp Form", 0),
            new PlayerDstSkillMechanic(SeveredFromBody, "Severed from Body", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Failed Green", "Player failed to return to the top of the Harvest Temple after becoming a wisp", "Failed Green", 0).UsingChecker((de, log) => de.HasKilled),
            new PlayerDstBuffApplyMechanic(Drown1, "Drown", new MechanicPlotlySetting(Symbols.Circle,Colors.LightBlue), "Bubble", "Player was trapped in a bubble by Soo-Won's Tail", "Bubble", 0),
            new PlayerDstBuffApplyMechanic(Drown2, "Drown", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkTeal), "Whirlpool", "Player was trapped in a whirlpool", "Whirlpool", 0),
            new EnemyDstBuffApplyMechanic(HardenedShell, "Hardened Shell", new MechanicPlotlySetting(Symbols.DiamondWide, Colors.DarkTeal), "Tail", "Soo-Won's Tail spawned", "Tail", 0),
            new EnemyDstBuffRemoveMechanic(HardenedShell, "Hardened Shell", new MechanicPlotlySetting(Symbols.DiamondWide, Colors.DarkGreen), "Tail Killed", "Soo-Won's Tail killed", "Tail Killed", 0).UsingChecker((bre, log) => !bre.To.HasBuff(log, Invulnerability757, bre.Time - ParserHelper.ServerDelayConstant + 500)),
            new EnemyDstBuffRemoveMechanic(HardenedShell, "Hardened Shell", new MechanicPlotlySetting(Symbols.DiamondWide, Colors.Yellow), "Tail Despawned", "Soo-Won's Tail despawned due to phase change", "Tail Despawned", 0).UsingChecker((bre, log) => bre.To.HasBuff(log, Invulnerability757, bre.Time - ParserHelper.ServerDelayConstant + 500)),
            new EnemyDstBuffApplyMechanic(DamageImmunitySooWonBite, "Damage Immunity", new MechanicPlotlySetting(Symbols.Diamond, Colors.Pink), "Side Swap", "Soo-Won breifly becomes invulnerable and switches sides of the arena", "Side Swap", 0),
            new EnemyDstBuffApplyMechanic(OldExposed, "Exposed", new MechanicPlotlySetting(Symbols.DiamondTall, Colors.DarkGreen), "CCed", "Breakbar successfully broken", "CCed", 0).UsingChecker((bae, log) => bae.To.IsSpecies(ArcDPSEnums.TargetID.SooWonOW) & !bae.To.HasBuff(log, OldExposed, bae.Time - ParserHelper.ServerDelayConstant)),
            });
            Extension = "soowon";
            Icon = EncounterIconSooWon;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000401;
        }

        internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                return GetFirstDamageEventTime(fightData, agentData, combatData, agentData.GetGadgetsByID(ArcDPSEnums.TargetID.SooWonOW).FirstOrDefault());
            }
            return startToUse;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.SooWonOW));
            AbstractSingleActor tailTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TrashID.SooWonTail));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Soo-Won not found");
            }

            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }

            phases.AddRange(GetPhasesByInvul(log, new long[] { Invulnerability757, SooWonSpearPhaseInvul }, mainTarget, true, true, log.FightData.FightStart,
                log.FightData.FightEnd));

            int phaseOffset = GetPhaseOffset(log, mainTarget);
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
            double initialHealth = mainTarget.GetCurrentHealthPercent(log, 0);
            Func<Func<BuffApplyEvent, bool>, BuffApplyEvent> targetBuffs =
                log.CombatData.GetBuffDataByDst(mainTarget.AgentItem).OfType<BuffApplyEvent>().FirstOrDefault;
            AbstractBuffEvent initialInvuln = targetBuffs(x => x.Initial && x.BuffID == Invulnerability757);
            AbstractBuffEvent initialDmgImmunity = targetBuffs(x => x.Initial && x.BuffID == SooWonSpearPhaseInvul); // spear phase

            int offset = 0;
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
                            x.IsSpecies(ArcDPSEnums.TrashID.VoidGiant2) ||
                            x.IsSpecies(ArcDPSEnums.TrashID.VoidTimeCaster2)));
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
                            x.IsSpecies(ArcDPSEnums.TrashID.VoidBrandstalker) ||
                            x.IsSpecies(ArcDPSEnums.TrashID.VoidColdsteel2) ||
                            x.IsSpecies(ArcDPSEnums.TrashID.VoidObliterator2)));
                        break;
                    case 11:
                        phase.Name = "20% - 0%";
                        phase.AddTarget(mainTarget);
                        phase.AddTarget(tailTarget);
                        break;
                }
            }
        }

        internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData,
            AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            IReadOnlyList<AgentItem> sooWons = agentData.GetGadgetsByID(ArcDPSEnums.TargetID.SooWonOW);
            if (!sooWons.Any())
            {
                throw new MissingKeyActorsException("Soo-Won not found");
            }

            foreach (AgentItem sooWon in sooWons)
            {
                sooWon.OverrideType(AgentItem.AgentType.NPC);
                sooWon.OverrideID(ArcDPSEnums.TargetID.SooWonOW);
            }

            IReadOnlyList<AgentItem> sooWonTails = agentData.GetGadgetsByID(ArcDPSEnums.TrashID.SooWonTail);
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
            RewardEvent reward = combatData.GetRewardEvents().FirstOrDefault(x => x.RewardType == ArcDPSEnums.RewardTypes.Daily && x.Time > fightData.FightStart);
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
