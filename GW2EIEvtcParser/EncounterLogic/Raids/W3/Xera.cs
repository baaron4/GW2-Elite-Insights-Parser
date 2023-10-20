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
    internal class Xera : StrongholdOfTheFaithful
    {

        private long _xeraSecondPhaseStartTime = 0;
        private long _xeraFirstPhaseEndTime = 0;
        private bool _hasPreEvent = false;
        private long _xeraFirstPhaseStart = 0;

        public Xera(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new PlayerDstHitMechanic(TemporalShredOrb, "Temporal Shred", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Orb","Temporal Shred (Hit by Red Orb)", "Red Orb",0),
            new PlayerDstHitMechanic(TemporalShredAoE, "Temporal Shred", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Orb Aoe","Temporal Shred (Stood in Orb Aoe)", "Orb AoE",0),
            new PlayerDstBuffApplyMechanic(BloodstoneProtection, "Bloodstone Protection", new MechanicPlotlySetting(Symbols.HourglassOpen,Colors.DarkPurple), "In Bubble","Bloodstone Protection (Stood in Bubble)", "Inside Bubble",0),
            new EnemyCastStartMechanic(SummonFragments, "Summon Fragment Start", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","Summon Fragment (Xera Breakbar)", "Breakbar",0),
            new EnemyCastEndMechanic(SummonFragments, "Summon Fragment End", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","Summon Fragment (Failed CC)", "CC Fail",0).UsingChecker( (ce,log) => ce.ActualDuration > 11940),
            new EnemyCastEndMechanic(SummonFragments, "Summon Fragment End", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Summon Fragment (Breakbar broken)", "CCed",0).UsingChecker( (ce, log) => ce.ActualDuration <= 11940),
            new PlayerDstBuffApplyMechanic(Derangement, "Derangement", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightPurple), "Stacks","Derangement (Stacking Debuff)", "Derangement",0),
            new PlayerDstBuffApplyMechanic(BendingChaos, "Bending Chaos", new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Yellow), "Button1","Bending Chaos (Stood on 1st Button)", "Button 1",0),
            new PlayerDstBuffApplyMechanic(ShiftingChaos, "Shifting Chaos", new MechanicPlotlySetting(Symbols.TriangleNEOpen,Colors.Yellow), "Button2","Bending Chaos (Stood on 2nd Button)", "Button 2",0),
            new PlayerDstBuffApplyMechanic(TwistingChaos, "Twisting Chaos", new MechanicPlotlySetting(Symbols.TriangleNWOpen,Colors.Yellow), "Button3","Bending Chaos (Stood on 3rd Button)", "Button 3",0),
            new PlayerDstBuffApplyMechanic(InterventionSAK, "Intervention SAK", new MechanicPlotlySetting(Symbols.Square,Colors.Blue), "Shield","Intervention (got Special Action Key)", "Shield",0),
            new PlayerDstBuffApplyMechanic(GravityWellXera, "Gravity Well", new MechanicPlotlySetting(Symbols.CircleXOpen,Colors.Magenta), "Gravity Half","Half-platform Gravity Well", "Gravity Well",4000),
            new PlayerDstBuffApplyMechanic(HerosDeparture, "Hero's Depature", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "TP Out","Hero's Departure (Teleport to Platform)","TP",0),
            new PlayerDstBuffApplyMechanic(HerosReturn, "Hero's Return", new MechanicPlotlySetting(Symbols.Circle,Colors.Green), "TP Back","Hero's Return (Teleport back)", "TP back",0),
            /*new Mechanic(Intervention, "Intervention", ParseEnum.BossIDS.Xera, new MechanicPlotlySetting(Symbols.Hourglass,"rgb(128,0,128)"), "Bubble",0),*/
            //new Mechanic(Disruption, "Disruption", ParseEnum.BossIDS.Xera, new MechanicPlotlySetting(Symbols.Square,Colors.DarkGreen), "TP",0), 
            //Not sure what this (ID 350342,"Disruption") is. Looks like it is the pulsing "orb removal" from the orange circles on the 40% platform. Would fit the name although it's weird it can hit players. 
            });
            Extension = "xera";
            GenericFallBackMethod = FallBackMethod.Death | FallBackMethod.CombatExit;
            Icon = EncounterIconXera;
            EncounterCategoryInformation.InSubCategoryOrder = 3;
            EncounterID |= 0x000004;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayXera,
                            (1000, 897),
                            (-5992, -5992, 69, -522)/*,
                            (-12288, -27648, 12288, 27648),
                            (1920, 12160, 2944, 14464)*/);
        }

        internal override List<AbstractBuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
        {
            AbstractSingleActor mainTarget = GetMainTarget();
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Xera not found");
            }
            var res = new List<AbstractBuffEvent>();
            if (_xeraSecondPhaseStartTime != 0)
            {
                res.Add(new BuffRemoveAllEvent(_unknownAgent, mainTarget.AgentItem, _xeraSecondPhaseStartTime, int.MaxValue, skillData.Get(Determined762), ArcDPSEnums.IFF.Unknown, 1, int.MaxValue));
                res.Add(new BuffRemoveManualEvent(_unknownAgent, mainTarget.AgentItem, _xeraSecondPhaseStartTime, int.MaxValue, skillData.Get(Determined762), ArcDPSEnums.IFF.Unknown));
            }
            return res;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            if (_xeraSecondPhaseStartTime == 0)
            {
                return;
            }
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (fightData.Success && fightData.FightEnd < _xeraSecondPhaseStartTime)
            {
                fightData.SetSuccess(false, fightData.LogEnd);
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long fightEnd = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = GetMainTarget();
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Xera not found");
            }
            phases[0].AddTarget(mainTarget);
            if (requirePhases)
            {
                if (_xeraFirstPhaseStart > 0)
                {
                    var phasePreEvent = new PhaseData(0, _xeraFirstPhaseStart, "Pre Event");
                    phasePreEvent.AddTargets(Targets.Where(x => x.IsSpecies(ArcDPSEnums.TrashID.BloodstoneShardButton) || x.IsSpecies(ArcDPSEnums.TrashID.BloodstoneShardRift)));
                    phasePreEvent.AddTarget(Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.DummyTarget)));
                    phases.Add(phasePreEvent);
                    var phase100to0 = new PhaseData(_xeraFirstPhaseStart, log.FightData.FightEnd, "Main Fight");
                    phase100to0.AddTarget(mainTarget);
                    phases.Add(phase100to0);
                }
                AbstractBuffEvent invulXera = GetInvulXeraEvent(log, mainTarget);
                // split happened
                if (invulXera != null)
                {
                    var phase1 = new PhaseData(_xeraFirstPhaseStart, invulXera.Time, "Phase 1");
                    phase1.AddTarget(mainTarget);
                    phases.Add(phase1);

                    long glidingEndTime = _xeraSecondPhaseStartTime > 0 ? _xeraSecondPhaseStartTime : fightEnd;
                    var glidingPhase = new PhaseData(invulXera.Time, glidingEndTime, "Gliding");
                    glidingPhase.AddTargets(Targets.Where(t => t.IsSpecies(ArcDPSEnums.TrashID.ChargedBloodstone)));
                    phases.Add(glidingPhase);

                    if (_xeraSecondPhaseStartTime > 0)
                    {
                        var phase2 = new PhaseData(_xeraSecondPhaseStartTime, fightEnd, "Phase 2");
                        phase2.AddTarget(mainTarget);
                        phase2.AddTargets(Targets.Where(t => t.IsSpecies(ArcDPSEnums.TrashID.BloodstoneShardMainFight)));
                        //mainTarget.AddCustomCastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None, log);
                        phases.Add(phase2);
                    }
                }
            }
            return phases;
        }

        private AbstractSingleActor GetMainTarget() => Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Xera));

        private static AbstractBuffEvent GetInvulXeraEvent(ParsedEvtcLog log, AbstractSingleActor xera)
        {
            AbstractBuffEvent determined = log.CombatData.GetBuffData(Determined762).FirstOrDefault(x => x.To == xera.AgentItem && x is BuffApplyEvent);
            if (determined == null)
            {
                determined = log.CombatData.GetBuffData(SpawnProtection).FirstOrDefault(x => x.To == xera.AgentItem && x is BuffApplyEvent);
            }
            return determined;
        }

        internal override long GetFightOffset(int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            AgentItem target = agentData.GetNPCsByID(ArcDPSEnums.TargetID.Xera).FirstOrDefault();
            if (target == null)
            {
                throw new MissingKeyActorsException("Xera not found");
            }
            // enter combat
            CombatItem enterCombat = combatData.Find(x => x.SrcMatchesAgent(target) && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat);
            if (enterCombat != null)
            {
                AgentItem fakeXera = agentData.GetNPCsByID(ArcDPSEnums.TrashID.FakeXera).FirstOrDefault();
                if (fakeXera != null)
                {
                    _hasPreEvent = true;
                    long encounterStart = fakeXera.LastAware;
                    _xeraFirstPhaseStart = enterCombat.Time - encounterStart;
                    return encounterStart;
                }
                return enterCombat.Time;
            }
            return GetGenericFightOffset(fightData);
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            bool needsRefresh = false;
            bool needsDummy = true;
            // find target
            AgentItem firstXera = agentData.GetNPCsByID(ArcDPSEnums.TargetID.Xera).FirstOrDefault();
            if (firstXera == null)
            {
                throw new MissingKeyActorsException("Xera not found");
            }
            _xeraFirstPhaseEndTime = firstXera.LastAware;
            //
            var maxHPUpdates = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate && x.DstAgent > 0).ToList();
            //
            var bloodstoneFragments = maxHPUpdates.Where(x => x.DstAgent == 104580).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget).ToList();
            foreach (AgentItem gadget in bloodstoneFragments)
            {
                gadget.OverrideType(AgentItem.AgentType.NPC);
                gadget.OverrideID(ArcDPSEnums.TrashID.BloodstoneFragment);
                needsRefresh = true;
            }
            //
            var bloodstoneShardsMainFight = maxHPUpdates.Where(x => x.DstAgent == 343620).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget).ToList();
            foreach (AgentItem gadget in bloodstoneShardsMainFight)
            {
                gadget.OverrideType(AgentItem.AgentType.NPC);
                gadget.OverrideID(ArcDPSEnums.TrashID.BloodstoneShardMainFight);
                needsRefresh = true;
            }
            //
            var bloodstoneShardsButton = maxHPUpdates.Where(x =>  x.DstAgent == 597600).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget).ToList();
            foreach (AgentItem gadget in bloodstoneShardsButton)
            {
                gadget.OverrideType(AgentItem.AgentType.NPC);
                gadget.OverrideID(ArcDPSEnums.TrashID.BloodstoneShardButton);
                needsRefresh = true;
                needsDummy = false;
            }
            //
            var bloodstoneShardsRift = maxHPUpdates.Where(x =>  x.DstAgent == 747000).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget).ToList();
            foreach (AgentItem gadget in bloodstoneShardsRift)
            {
                gadget.OverrideType(AgentItem.AgentType.NPC);
                gadget.OverrideID(ArcDPSEnums.TrashID.BloodstoneShardRift);
                needsRefresh = true;
                needsDummy = false;
            }
            //
            var chargedBloodStones = maxHPUpdates.Where(x => x.DstAgent == 74700).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.LastAware > firstXera.LastAware).ToList();
            foreach (AgentItem gadget in chargedBloodStones)
            {
                if (!combatData.Any(x => x.IsDamage() && x.DstMatchesAgent(gadget)))
                {
                    continue;
                }
                gadget.OverrideType(AgentItem.AgentType.NPC);
                gadget.OverrideID(ArcDPSEnums.TrashID.ChargedBloodstone);
                needsRefresh = true;
            }
            if (_hasPreEvent && needsDummy)
            {
                agentData.AddCustomNPCAgent(fightData.FightStart, _xeraFirstPhaseStart, "Xera Pre Event", Spec.NPC, ArcDPSEnums.TargetID.DummyTarget, true);
                needsRefresh = false; // AddCustomNPCAgent already refreshes
            }
            if (needsRefresh)
            {
                agentData.Refresh();
            }
            // find split
            AgentItem secondXera = agentData.GetNPCsByID(ArcDPSEnums.TargetID.Xera2).FirstOrDefault();
            if (secondXera != null)
            {
                CombatItem move = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.Position && x.SrcMatchesAgent(secondXera) && x.Time >= secondXera.FirstAware + 500);
                if (move != null)
                {
                    _xeraSecondPhaseStartTime = move.Time;
                }
                else
                {
                    _xeraSecondPhaseStartTime = secondXera.FirstAware;
                }
                firstXera.OverrideAwareTimes(firstXera.FirstAware, secondXera.LastAware);
                RedirectAllEvents(combatData, extensions, agentData, secondXera, firstXera);
            }
            ComputeFightTargets(agentData, combatData, extensions);

            if (_xeraSecondPhaseStartTime > 0)
            {
                AbstractSingleActor mainTarget = GetMainTarget();
                if (mainTarget == null)
                {
                    throw new MissingKeyActorsException("Xera not found");
                }
                mainTarget.SetManualHealth(24085950);
            }
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int> {
                (int)ArcDPSEnums.TargetID.Xera,
                (int)ArcDPSEnums.TargetID.DummyTarget,
                (int)ArcDPSEnums.TrashID.BloodstoneShardMainFight,
                (int)ArcDPSEnums.TrashID.BloodstoneShardRift,
                (int)ArcDPSEnums.TrashID.BloodstoneShardButton,
                (int)ArcDPSEnums.TrashID.ChargedBloodstone,
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.WhiteMantleSeeker1,
                ArcDPSEnums.TrashID.WhiteMantleSeeker2,
                ArcDPSEnums.TrashID.WhiteMantleKnight1,
                ArcDPSEnums.TrashID.WhiteMantleKnight2,
                ArcDPSEnums.TrashID.WhiteMantleBattleMage1,
                ArcDPSEnums.TrashID.WhiteMantleBattleMage2,
                ArcDPSEnums.TrashID.BloodstoneFragment,
                ArcDPSEnums.TrashID.ExquisiteConjunction,
                ArcDPSEnums.TrashID.XerasPhantasm,
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Xera:
                    var summon = cls.Where(x => x.SkillId == SummonFragments).ToList();
                    foreach (AbstractCastEvent c in summon)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.ChargedBloodstone:
                    if (_xeraFirstPhaseEndTime != 0)
                    {
                        long end = replay.TimeOffsets.end;
                        AbstractHealthDamageEvent lastDamage = target.GetDamageTakenEvents(null, log, 0, log.FightData.FightEnd).LastOrDefault();
                        if (lastDamage != null)
                        {
                            end = lastDamage.Time;
                        }
                        replay.Trim(_xeraFirstPhaseEndTime + 12000, end);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.BloodstoneFragment:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 760, ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end), "rgba(255, 155, 0, 0.2)", new AgentConnector(target)));
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            // Derangement - 0 to 29 nothing, 30 to 59 Silver, 60 to 89 Gold, 90 to 99 Red
            IEnumerable<Segment> derangements = player.GetBuffStatus(log, Derangement, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            foreach (Segment segment in derangements)
            {
                if (segment.Value >= 90)
                {
                    replay.AddOverheadIcon(segment, player, ParserIcons.DerangementRedOverhead);
                }
                else if (segment.Value >= 60)
                {
                    replay.AddOverheadIcon(segment, player, ParserIcons.DerangementGoldOverhead);
                }
                else if (segment.Value >= 30)
                {
                    replay.AddOverheadIcon(segment, player, ParserIcons.DerangementSilverOverhead);
                }
            }
        }
    }
}
