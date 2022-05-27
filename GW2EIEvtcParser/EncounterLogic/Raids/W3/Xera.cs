using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Xera : StrongholdOfTheFaithful
    {

        private long _xeraSecondPhaseStartTime = 0;
        private long _xeraFirstPhaseEndTime = 0;

        public Xera(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new HitOnPlayerMechanic(35128, "Temporal Shred", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Orb","Temporal Shred (Hit by Red Orb)", "Red Orb",0),
            new HitOnPlayerMechanic(34913, "Temporal Shred", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Orb Aoe","Temporal Shred (Stood in Orb Aoe)", "Orb AoE",0),
            new PlayerBuffApplyMechanic(BloodstoneProtection, "Bloodstone Protection", new MechanicPlotlySetting(Symbols.HourglassOpen,Colors.DarkPurple), "In Bubble","Bloodstone Protection (Stood in Bubble)", "Inside Bubble",0),
            new EnemyCastStartMechanic(34887, "Summon Fragment Start", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","Summon Fragment (Xera Breakbar)", "Breakbar",0),
            new EnemyCastEndMechanic(34887, "Summon Fragment End", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","Summon Fragment (Failed CC)", "CC Fail",0, (ce,log) => ce.ActualDuration > 11940),
            new EnemyCastEndMechanic(34887, "Summon Fragment End", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Summon Fragment (Breakbar broken)", "CCed",0, (ce, log) => ce.ActualDuration <= 11940),
            new PlayerBuffApplyMechanic(Derangement, "Derangement", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightPurple), "Stacks","Derangement (Stacking Debuff)", "Derangement",0),
            new PlayerBuffApplyMechanic(BendingChaos, "Bending Chaos", new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Yellow), "Button1","Bending Chaos (Stood on 1st Button)", "Button 1",0),
            new PlayerBuffApplyMechanic(ShiftingChaos, "Shifting Chaos", new MechanicPlotlySetting(Symbols.TriangleNEOpen,Colors.Yellow), "Button2","Bending Chaos (Stood on 2nd Button)", "Button 2",0),
            new PlayerBuffApplyMechanic(TwistingChaos, "Twisting Chaos", new MechanicPlotlySetting(Symbols.TriangleNWOpen,Colors.Yellow), "Button3","Bending Chaos (Stood on 3rd Button)", "Button 3",0),
            new PlayerBuffApplyMechanic(Intervention, "Intervention", new MechanicPlotlySetting(Symbols.Square,Colors.Blue), "Shield","Intervention (got Special Action Key)", "Shield",0),
            new PlayerBuffApplyMechanic(34921, "Gravity Well", new MechanicPlotlySetting(Symbols.CircleXOpen,Colors.Magenta), "Gravity Half","Half-platform Gravity Well", "Gravity Well",4000),
            new PlayerBuffApplyMechanic(34997, "Teleport Out", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "TP Out","Teleport Out (Teleport to Platform)","TP",0),
            new PlayerBuffApplyMechanic(35076, "Hero's Return", new MechanicPlotlySetting(Symbols.Circle,Colors.Green), "TP Back","Hero's Return (Teleport back)", "TP back",0),
            /*new Mechanic(35000, "Intervention", ParseEnum.BossIDS.Xera, new MechanicPlotlySetting(Symbols.Hourglass,"rgb(128,0,128)"), "Bubble",0),*/
            //new Mechanic(35034, "Disruption", ParseEnum.BossIDS.Xera, new MechanicPlotlySetting(Symbols.Square,Colors.DarkGreen), "TP",0), 
            //Not sure what this (ID 350342,"Disruption") is. Looks like it is the pulsing "orb removal" from the orange circles on the 40% platform. Would fit the name although it's weird it can hit players. 
            });
            Extension = "xera";
            GenericFallBackMethod = FallBackMethod.CombatExit;
            Icon = "https://wiki.guildwars2.com/images/4/4b/Mini_Xera.png";
            EncounterCategoryInformation.InSubCategoryOrder = 3;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/mGOHGwN.png",
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
                res.Add(new BuffRemoveAllEvent(mainTarget.AgentItem, mainTarget.AgentItem, _xeraSecondPhaseStartTime, int.MaxValue, skillData.Get(762), 1, int.MaxValue));
                res.Add(new BuffRemoveManualEvent(mainTarget.AgentItem, mainTarget.AgentItem, _xeraSecondPhaseStartTime, int.MaxValue, skillData.Get(762)));
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
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long fightDuration = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = GetMainTarget();
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Xera not found");
            }
            phases[0].AddTarget(mainTarget);
            if (requirePhases)
            {
                AbstractBuffEvent invulXera = GetInvulXeraEvent(log, mainTarget);
                // split happened
                if (invulXera != null)
                {
                    var phase1 = new PhaseData(0, invulXera.Time, "Phase 1");
                    phase1.AddTarget(mainTarget);
                    phases.Add(phase1);

                    long glidingEndTime = _xeraSecondPhaseStartTime > 0 ? _xeraSecondPhaseStartTime : fightDuration;
                    var glidingPhase = new PhaseData(invulXera.Time, glidingEndTime, "Gliding");
                    glidingPhase.AddTargets(Targets.Where(t => t.ID == (int)ArcDPSEnums.TrashID.ChargedBloodstone));
                    phases.Add(glidingPhase);

                    if (_xeraSecondPhaseStartTime > 0)
                    {
                        var phase2 = new PhaseData(_xeraSecondPhaseStartTime, fightDuration, "Phase 2");
                        phase2.AddTarget(mainTarget);
                        phase2.AddTargets(Targets.Where(t => t.ID == (int)ArcDPSEnums.TrashID.BloodstoneShard));
                        //mainTarget.AddCustomCastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None, log);
                        phases.Add(phase2);
                    }
                }
            }
            return phases;
        }

        private AbstractSingleActor GetMainTarget() => Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Xera);

        private static AbstractBuffEvent GetInvulXeraEvent(ParsedEvtcLog log, AbstractSingleActor xera)
        {
            AbstractBuffEvent determined = log.CombatData.GetBuffData(SkillIDs.Determined762).FirstOrDefault(x => x.To == xera.AgentItem && x is BuffApplyEvent);
            if (determined == null)
            {
                determined = log.CombatData.GetBuffData(SkillIDs.SpawnProtection).FirstOrDefault(x => x.To == xera.AgentItem && x is BuffApplyEvent);
            }
            return determined;
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            AgentItem target = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Xera).FirstOrDefault();
            if (target == null)
            {
                throw new MissingKeyActorsException("Xera not found");
            }
            // enter combat
            CombatItem enterCombat = combatData.Find(x => x.SrcMatchesAgent(target) && x.IsStateChange == ArcDPSEnums.StateChange.EnterCombat);
            if (enterCombat != null)
            {
                return enterCombat.Time;
            }
            return fightData.LogStart;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // find target
            AgentItem firstXera = agentData.GetNPCsByID((int)ArcDPSEnums.TargetID.Xera).FirstOrDefault();
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
            }
            //
            var bloodstoneShards = maxHPUpdates.Where(x => x.DstAgent == 343620).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget).ToList();
            foreach (AgentItem gadget in bloodstoneShards)
            {
                gadget.OverrideType(AgentItem.AgentType.NPC);
                gadget.OverrideID(ArcDPSEnums.TrashID.BloodstoneShard);
            }
            //
            var chargedBloodStones = maxHPUpdates.Where(x => x.DstAgent == 74700).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.LastAware > firstXera.LastAware).ToList();
            foreach (AgentItem gadget in chargedBloodStones)
            {
                gadget.OverrideType(AgentItem.AgentType.NPC);
                gadget.OverrideID(ArcDPSEnums.TrashID.ChargedBloodstone);

            }
            if (bloodstoneFragments.Any() || bloodstoneShards.Any() || chargedBloodStones.Any())
            {
                agentData.Refresh();
            }
            // find split
            AgentItem secondXera = agentData.GetNPCsByID(16286).FirstOrDefault();
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
                agentData.SwapMasters(secondXera, firstXera);
                // update combat data
                foreach (CombatItem c in combatData)
                {
                    if (c.SrcMatchesAgent(secondXera, extensions))
                    {
                        c.OverrideSrcAgent(firstXera.Agent);
                    }
                    if (c.DstMatchesAgent(secondXera, extensions))
                    {
                        c.OverrideDstAgent(firstXera.Agent);
                    }
                }
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
                (int)ArcDPSEnums.TrashID.BloodstoneShard,
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
                    var summon = cls.Where(x => x.SkillId == 34887).ToList();
                    foreach (AbstractCastEvent c in summon)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.ChargedBloodstone:
                    if (_xeraFirstPhaseEndTime != 0)
                    {
                        replay.Trim(_xeraFirstPhaseEndTime + 12000, replay.TimeOffsets.end);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.BloodstoneFragment:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 760, ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end), "rgba(255, 155, 0, 0.2)", new AgentConnector(target)));
                    break;
                default:
                    break;
            }
        }
    }
}
