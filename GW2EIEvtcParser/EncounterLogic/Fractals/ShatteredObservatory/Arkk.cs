using System.Collections.Generic;
using System.Linq;
using System;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using GW2EIEvtcParser.Extensions;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Arkk : ShatteredObservatory
    {
        public Arkk(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(new long[] { HorizonStrikeArkk1, HorizonStrikeArkk2 }, "Horizon Strike", new MechanicPlotlySetting(Symbols.Circle, Colors.LightOrange), "Horizon Strike","Horizon Strike (turning pizza slices)", "Horizon Strike",0),
            new PlayerDstHitMechanic(new long[] { DiffractiveEdge1, DiffractiveEdge2 }, "Diffractive Edge", new MechanicPlotlySetting(Symbols.Star,Colors.Yellow), "5 Cone","Diffractive Edge (5 Cone Knockback)", "Five Cones",0),
            new PlayerDstHitMechanic(SolarFury, "Solar Fury", new MechanicPlotlySetting(Symbols.Circle,Colors.LightRed), "Ball","Stood in Red Overhead Ball Field", "Red Ball Aoe",0),
            new PlayerDstHitMechanic(FocusedRage, "Focused Rage", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Orange), "Cone KB","Knockback in Cone with overhead crosshair", "Knockback Cone",0),
            new PlayerDstHitMechanic(SolarDischarge, "Solar Discharge", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Shockwave","Knockback shockwave after Overhead Balls", "Shockwave",0),
            new PlayerDstHitMechanic(new long[] { StarbustCascade1, StarbustCascade2 }, "Starburst Cascade", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Float Ring","Starburst Cascade (Expanding/Retracting Lifting Ring)", "Float Ring",500),
            new PlayerDstHitMechanic(HorizonStrikeNormal, "Horizon Strike Normal", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkRed), "Horizon Strike norm","Horizon Strike (normal)", "Horizon Strike (normal)",0),
            new PlayerDstHitMechanic(OverheadSmash, "Overhead Smash", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.LightRed), "Smash","Overhead Smash","Overhead Smash",0),
            new PlayerDstBuffApplyMechanic(CorporealReassignmentBuff, "Corporeal Reassignment", new MechanicPlotlySetting(Symbols.Diamond,Colors.Red), "Skull","Exploding Skull mechanic application", "Corporeal Reassignment",0),
            new PlayerDstHitMechanic(ExplodeArkk, "Explode", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Bloom Explode","Hit by Solar Bloom explosion", "Bloom Explosion",0),
            new PlayerDstBuffApplyMechanic(new long[] {FixatedBloom1, FixatedBloom2, FixatedBloom3, FixatedBloom4}, "Fixate", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Magenta), "Bloom Fix","Fixated by Solar Bloom", "Bloom Fixate",0),
            new PlayerDstBuffApplyMechanic(CosmicMeteor, "Cosmic Meteor", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "Green","Temporal Realignment (Green) application", "Green",0),
            new PlayerDstBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)",0).UsingChecker((ba, log) => ba.AppliedDuration == 3000), // //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new EnemyCastStartMechanic(ArkkBreakbarCast, "Breakbar Start", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "Breakbar","Start Breakbar", "CC",0),
            new EnemyDstBuffApplyMechanic(Exposed31589, "Breakbar End", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC.Fail","Breakbar (Failed CC)", "CC Fail",0).UsingChecker((bae,log) => bae.To.IsSpecies(TargetID.Arkk) && !log.CombatData.GetAnimatedCastData(ArkkBreakbarCast).Any(x => bae.To == x.Caster && x.Time < bae.Time && bae.Time < x.ExpectedEndTime + ServerDelayConstant)),
            new EnemyDstBuffApplyMechanic(Exposed31589, "Breakbar End", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Breakbar broken", "CCed",0).UsingChecker((bae,log) => bae.To.IsSpecies(TargetID.Arkk) && log.CombatData.GetAnimatedCastData(ArkkBreakbarCast).Any(x => bae.To == x.Caster && x.Time < bae.Time && bae.Time < x.ExpectedEndTime + ServerDelayConstant)),
            new PlayerDstHitMechanic(OverheadSmashArchdiviner, "Overhead Smash", new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.LightRed), "A.Smsh","Overhead Smash (Arcdiviner)", "Smash (Add)",0),
            new PlayerDstHitMechanic(RollingChaos, "Rolling Chaos", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightRed), "KD Marble","Rolling Chaos (Arrow marble)", "KD Marble",0),
            new PlayerDstHitMechanic(SolarStomp, "Solar Stomp", new MechanicPlotlySetting(Symbols.TriangleUp,Colors.Magenta), "Stomp","Solar Stomp (Evading Stomp)", "Evading Jump",0),
            new EnemyCastStartMechanic(CosmicStreaks, "Cosmic Streaks", new MechanicPlotlySetting(Symbols.DiamondOpen,Colors.Pink), "DDR Beam","Triple Death Ray Cast (last phase)", "Death Ray Cast",0),
            new PlayerDstHitMechanic(WhirlingDevastation, "Whirling Devastation", new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.DarkPink), "Whirl","Whirling Devastation (Gladiator Spin)", "Gladiator Spin",300),
            new EnemyCastStartMechanic(PullCharge, "Pull Charge", new MechanicPlotlySetting(Symbols.Bowtie,Colors.DarkTeal), "Pull","Pull Charge (Gladiator Pull)", "Gladiator Pull",0), //
            new EnemyCastEndMechanic(PullCharge, "Pull Charge", new MechanicPlotlySetting(Symbols.Bowtie,Colors.Red), "Pull CC Fail","Pull Charge CC failed", "CC fail (Gladiator)",0).UsingChecker((ce,log) => ce.ActualDuration > 3200), //
            new EnemyCastEndMechanic(PullCharge, "Pull Charge", new MechanicPlotlySetting(Symbols.Bowtie,Colors.DarkGreen), "Pull CCed","Pull Charge CCed", "CCed (Gladiator)",0).UsingChecker((ce, log) => ce.ActualDuration < 3200), //
            new PlayerDstHitMechanic(SpinningCut, "Spinning Cut", new MechanicPlotlySetting(Symbols.StarSquareOpen,Colors.LightPurple), "Daze","Spinning Cut (3rd Gladiator Auto->Daze)", "Gladiator Daze",0), //
            });
            Extension = "arkk";
            Icon = EncounterIconArkk;
            EncounterCategoryInformation.InSubCategoryOrder = 2;
            EncounterID |= 0x000003;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayArkk,
                            (914, 914),
                            (-19231, -18137, -16591, -15677)/*,
                            (-24576, -24576, 24576, 24576),
                            (11204, 4414, 13252, 6462)*/);
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            var trashIDs = new List<ArcDPSEnums.TrashID>
            {
                TrashID.TemporalAnomaly2,
                TrashID.BLIGHT,
                TrashID.Fanatic,
                TrashID.SolarBloom,
                TrashID.PLINK,
                TrashID.DOC,
                TrashID.CHOP,
                TrashID.ProjectionArkk
            };
            trashIDs.AddRange(base.GetTrashMobsIDs());
            return trashIDs;
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)TargetID.Arkk,
                (int)TrashID.Archdiviner,
                (int)TrashID.EliteBrazenGladiator
            };
        }

        private void GetMiniBossPhase(int targetID, ParsedEvtcLog log, string phaseName, List<PhaseData> phases)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(targetID));
            if (target == null)
            {
                return;
            }
            var phaseData = new PhaseData(Math.Max(target.FirstAware, log.FightData.FightStart), Math.Min(target.LastAware, log.FightData.FightEnd), phaseName);
            AddTargetsToPhaseAndFit(phaseData, new List<int> { targetID }, log);
            phases.Add(phaseData);
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor arkk = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Arkk));
            if (arkk == null)
            {
                throw new MissingKeyActorsException("Arkk not found");
            }
            phases[0].AddTarget(arkk);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, arkk, false, true));
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].AddTarget(arkk);
            }
            GetMiniBossPhase((int)TrashID.Archdiviner, log, "Archdiviner", phases);
            GetMiniBossPhase((int)TrashID.EliteBrazenGladiator, log, "Brazen Gladiator", phases);
            return phases;
        }
        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                AgentItem arkk = agentData.GetNPCsByID(TargetID.Arkk).FirstOrDefault();
                if (arkk == null)
                {
                    throw new MissingKeyActorsException("Arkk not found");
                }
                long upperLimit = GetPostLogStartNPCUpdateDamageEventTime(fightData, agentData, combatData, logStartNPCUpdate.Time, arkk);
                CombatItem firstBuffApply = combatData.FirstOrDefault(x => x.IsBuffApply() && x.SrcMatchesAgent(arkk) && x.SkillID == ArkkStartBuff && x.Time <= upperLimit + TimeThresholdConstant);
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.EnterCombat && x.SrcMatchesAgent(arkk) && x.Time <= upperLimit + TimeThresholdConstant);
                return firstBuffApply != null ? Math.Min(firstBuffApply.Time, enterCombat != null ? enterCombat.Time : long.MaxValue): GetGenericFightOffset(fightData);
            }
            return GetGenericFightOffset(fightData);
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            // reward or death worked
            if (fightData.Success)
            {
                return;
            }
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Arkk));
            if (target == null)
            {
                throw new MissingKeyActorsException("Arkk not found");
            }
            HashSet<AgentItem> adjustedPlayers = GetParticipatingPlayerAgents(target, combatData, playerAgents);
            // missing buff apply events fallback, some phases will be missing
            // removes should be present
            if (SetSuccessByBuffCount(combatData, fightData, adjustedPlayers, target, Determined762, 10))
            {
                var invulsRemoveTarget = combatData.GetBuffData(Determined762).OfType<BuffRemoveAllEvent>().Where(x => x.To == target.AgentItem).ToList();
                if (invulsRemoveTarget.Count == 5)
                {
                    SetSuccessByCombatExit(new List<AbstractSingleActor> { target }, combatData, fightData, adjustedPlayers);
                }
            }
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);
            IReadOnlyList<AbstractBuffEvent> beDynamic = log.CombatData.GetBuffData(AchievementEligibilityBeDynamic);
            int counter = 0;

            if (beDynamic.Any() && log.FightData.Success)
            {
                foreach (Player p in log.PlayerList)
                {
                    if (p.HasBuff(log, AchievementEligibilityBeDynamic, log.FightData.FightEnd - ServerDelayConstant))
                    {
                        counter++;
                    }
                }
            }
            // The party must have 5 players to be eligible
            if (counter == 5)
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[AchievementEligibilityBeDynamic], 1));
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // Corporeal Reassignment
            IEnumerable<Segment> corpReass = p.GetBuffStatus(log, CorporealReassignmentBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            replay.AddOverheadIcons(corpReass, p, ParserIcons.SkullOverhead);
            // Fixations
            IEnumerable<Segment> fixations = p.GetBuffStatus(log, new long[] { FixatedBloom1, FixatedBloom2, FixatedBloom3, FixatedBloom4 }, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            List<AbstractBuffEvent> fixationEvents = GetFilteredList(log.CombatData, new long[] { FixatedBloom1, FixatedBloom2, FixatedBloom3, FixatedBloom4 }, p, true, true);
            replay.AddOverheadIcons(fixations, p, ParserIcons.FixationPurpleOverhead);
            replay.AddTether(fixationEvents, "rgba(255, 0, 255, 0.5)");
        }
    }
}
