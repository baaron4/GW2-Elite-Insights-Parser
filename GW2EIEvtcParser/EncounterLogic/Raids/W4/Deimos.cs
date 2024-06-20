using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Deimos : BastionOfThePenitent
    {

        private long _deimos10PercentTime = 0;

        private bool _hasPreEvent = false;

        private long _deimos100PercentTime = 0;

        private const long PreEventConsiderationConstant = 5000;

        public Deimos(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(RapidDecay, "Rapid Decay", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Black), "Oil","Rapid Decay (Black expanding oil)", "Black Oil",0),
            new PlayerDstFirstHitMechanic(RapidDecay, "Rapid Decay", new MechanicPlotlySetting(Symbols.Circle,Colors.Black), "Oil T.","Rapid Decay Trigger (Black expanding oil)", "Black Oil Trigger",0).UsingChecker((ce, log) => {
                AbstractSingleActor actor = log.FindActor(ce.To);
                if (actor == null)
                {
                    return false;
                }
                (_, IReadOnlyList<Segment> downs , _) = actor.GetStatus(log);
                bool hitInDown = downs.Any(x => x.ContainsPoint(ce.Time));
                return !hitInDown;
            }),
            new EnemyCastStartMechanic(OffBalance, "Off Balance", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "TP CC","Off Balance (Saul TP Breakbar)", "Saul TP Start",0),
            new EnemyCastEndMechanic(OffBalance, "Off Balance", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "TP CC Fail","Failed Saul TP CC", "Failed CC (TP)",0).UsingChecker((ce,log) => ce.ActualDuration >= 2200),
            new EnemyCastEndMechanic(OffBalance, "Off Balance", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "TP CCed","Saul TP CCed", "CCed (TP)",0).UsingChecker((ce, log) => ce.ActualDuration < 2200),
            new EnemyCastStartMechanic(BoonThief, "Boon Thief", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.DarkTeal), "Thief CC","Boon Thief (Saul Breakbar)", "Boon Thief Start",0),
            new EnemyCastEndMechanic(BoonThief, "Boon Thief", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.Red), "Thief CC Fail","Failed Boon Thief CC", "Failed CC (Thief)",0).UsingChecker((ce,log) => ce.ActualDuration >= 4400),
            new EnemyCastEndMechanic(BoonThief, "Boon Thief", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.DarkGreen), "Thief CCed","Boon Thief CCed", "CCed (Thief)",0).UsingChecker((ce, log) => ce.ActualDuration < 4400),
            new PlayerDstHitMechanic(new long[] {Annihilate2, Annihilate1 }, "Annihilate", new MechanicPlotlySetting(Symbols.Hexagon,Colors.Yellow), "Pizza","Annihilate (Cascading Pizza attack)", "Boss Smash",0),
            new PlayerDstHitMechanic(DemonicShockWaveRight, "Demonic Shock Wave", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.Red), "10% RSmash","Knockback (right hand) in 10% Phase", "10% Right Smash",0),
            new PlayerDstHitMechanic(DemonicShockWaveLeft, "Demonic Shock Wave", new MechanicPlotlySetting(Symbols.TriangleLeftOpen,Colors.Red), "10% LSmash","Knockback (left hand) in 10% Phase", "10% Left Smash",0),
            new PlayerDstHitMechanic(DemonicShockWaveCenter, "Demonic Shock Wave", new MechanicPlotlySetting(Symbols.Bowtie,Colors.Red), "10% DSmash","Knockback (both hands) in 10% Phase", "10% Double Smash",0),
            new PlayerDstBuffApplyMechanic(TearInstability, "Tear Instability", new MechanicPlotlySetting(Symbols.Diamond,Colors.DarkTeal), "Tear","Collected a Demonic Tear", "Tear",0),
            new PlayerDstHitMechanic(MindCrush, "Mind Crush", new MechanicPlotlySetting(Symbols.Square,Colors.Blue), "Mind Crush","Hit by Mind Crush without Bubble Protection", "Mind Crush",0).UsingChecker( (de,log) => de.HealthDamage > 0),
            new PlayerDstBuffApplyMechanic(WeakMinded, "Weak Minded", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightPurple), "Weak Mind","Weak Minded (Debuff after Mind Crush)", "Weak Minded",0),
            new PlayerDstBuffApplyMechanic(DeimosSelectedByGreen, "Chosen by Eye of Janthir", new MechanicPlotlySetting(Symbols.Circle,Colors.Green), "Green","Chosen by the Eye of Janthir", "Chosen (Green)",0),
            new PlayerDstBuffApplyMechanic(GreenTeleport, "Teleported", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "TP","Teleport to/from Demonic Realm", "Teleport",0),
            new EnemyDstBuffApplyMechanic(UnnaturalSignet, "Unnatural Signet", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Teal), "DMG Debuff","Double Damage Debuff on Deimos", "+100% Dmg Buff",0)
            });
            Extension = "dei";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = EncounterIconDeimos;
            EncounterCategoryInformation.InSubCategoryOrder = 3;
            EncounterID |= 0x000004;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayDeimos,
                            (765, 1000),
                            (-9542, 1932, -7004, 5250)/*,
                            (-27648, -9216, 27648, 12288),
                            (11774, 4480, 14078, 5376)*/);
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(DemonicAura, DemonicAura),
            };
        }
        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.Deimos,
                (int)ArcDPSEnums.TrashID.Saul,
                (int)ArcDPSEnums.TrashID.Thief,
                (int)ArcDPSEnums.TrashID.Drunkard,
                (int)ArcDPSEnums.TrashID.Gambler,
            };
        }

        protected override List<int> GetFriendlyNPCIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TrashID.Saul,
                (int)ArcDPSEnums.TrashID.ShackledPrisoner
            };
        }

        private static void MergeWithGadgets(AgentItem deimos, HashSet<AgentItem> gadgets, List<CombatItem> combatData, AgentData agentData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            foreach (AgentItem gadget in gadgets)
            {
                RedirectAllEvents(combatData, extensions, agentData, gadget, deimos,
                    (evt, from, to) =>
                    {
                        // skip events before last aware that are not attack target related
                        if (evt.Time < deimos.LastAware && evt.IsStateChange != ArcDPSEnums.StateChange.AttackTarget && evt.IsStateChange != ArcDPSEnums.StateChange.Targetable)
                        {
                            return false;
                        }
                        if (evt.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate)
                        {
                            return false;
                        }
                        // Deimos can't go above 10% hp during that phase
                        if (evt.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && HealthUpdateEvent.GetHealthPercent(evt) > 1001)
                        {
                            return false;
                        }
                        if (evt.Time < to.FirstAware)
                        {
                            evt.OverrideTime(to.FirstAware);
                        }
                        return true;
                    }
                );
            }
        }

        internal override List<AbstractBuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Deimos)) ?? throw new MissingKeyActorsException("Deimos not found");
            var res = new List<AbstractBuffEvent>();
            IReadOnlyList<AbstractBuffEvent> signets = combatData.GetBuffData(UnnaturalSignet);
            foreach (AbstractBuffEvent bfe in signets)
            {
                if (bfe is BuffApplyEvent ba)
                {
                    AbstractBuffEvent removal = signets.FirstOrDefault(x => x is BuffRemoveAllEvent && x.Time > bfe.Time && x.Time < bfe.Time + 30000);
                    if (removal == null)
                    {
                        res.Add(new BuffRemoveAllEvent(_unknownAgent, target.AgentItem, ba.Time + ba.AppliedDuration, 0, skillData.Get(UnnaturalSignet), ArcDPSEnums.IFF.Unknown, 1, 0));
                        res.Add(new BuffRemoveManualEvent(_unknownAgent, target.AgentItem, ba.Time + ba.AppliedDuration, 0, skillData.Get(UnnaturalSignet), ArcDPSEnums.IFF.Unknown));
                    }
                }
                else if (bfe is BuffRemoveAllEvent)
                {
                    AbstractBuffEvent apply = signets.FirstOrDefault(x => x is BuffApplyEvent && x.Time < bfe.Time && x.Time > bfe.Time - 30000);
                    if (apply == null)
                    {
                        res.Add(new BuffApplyEvent(_unknownAgent, target.AgentItem, bfe.Time - 10000, 10000, skillData.Get(UnnaturalSignet), ArcDPSEnums.IFF.Unknown, uint.MaxValue, true));
                    }
                }
            }
            return res;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success && _deimos10PercentTime > 0)
            {
                AbstractSingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Deimos)) ?? throw new MissingKeyActorsException("Deimos not found");
                if (!agentData.TryGetFirstAgentItem(ArcDPSEnums.TrashID.Saul, out AgentItem saul))
                {
                    throw new MissingKeyActorsException("Saul not found");
                }
                if (combatData.GetDeadEvents(saul).Any())
                {
                    return;
                }
                IReadOnlyList<AttackTargetEvent> attackTargets = combatData.GetAttackTargetEvents(deimos.AgentItem);
                if (attackTargets.Count == 0)
                {
                    return;
                }
                AgentItem attackTarget = attackTargets.Last().AttackTarget;
                // sanity check
                TargetableEvent attackableEvent = combatData.GetTargetableEvents(attackTarget).LastOrDefault(x => x.Targetable && x.Time > _deimos10PercentTime - ServerDelayConstant);
                if (attackableEvent == null)
                {
                    return;
                }
                TargetableEvent notAttackableEvent = combatData.GetTargetableEvents(attackTarget).LastOrDefault(x => !x.Targetable && x.Time > attackableEvent.Time);
                if (notAttackableEvent == null)
                {
                    return;
                }
                AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(deimos.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && x.Time > _deimos10PercentTime && playerAgents.Contains(x.From.GetFinalMaster()) && !x.ToFriendly);
                if (lastDamageTaken != null)
                {
                    // This means Deimos received damage after becoming non attackable, that means it did not die
                    AbstractHealthDamageEvent friendlyDamageToDeimos = combatData.GetDamageTakenData(deimos.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && x.Time > _deimos10PercentTime && x.Time > lastDamageTaken.Time && x.ToFriendly);
                    if (friendlyDamageToDeimos != null || !AtLeastOnePlayerAlive(combatData, fightData, notAttackableEvent.Time, playerAgents))
                    {
                        return;
                    }
                    fightData.SetSuccess(true, notAttackableEvent.Time);
                }
            }
        }

        private static long AttackTargetSpecialParse(CombatItem targetable, AgentData agentData, List<CombatItem> combatData, HashSet<AgentItem> gadgetAgents)
        {
            if (targetable == null)
            {
                return 0;
            }
            long firstAware = targetable.Time;
            AgentItem targetAgent = agentData.GetAgent(targetable.SrcAgent, targetable.Time);
            if (targetAgent == _unknownAgent)
            {
                return 0;
            }
            CombatItem attackTargetEvent = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.AttackTarget && x.SrcMatchesAgent(targetAgent));
            if (attackTargetEvent == null)
            {
                return 0;
            }
            AgentItem deimosStructBody = agentData.GetAgent(attackTargetEvent.DstAgent, attackTargetEvent.Time);
            if (deimosStructBody == _unknownAgent)
            {
                return 0;
            }
            gadgetAgents.Add(deimosStructBody);
            CombatItem armDeimosDamageEvent = combatData.FirstOrDefault(x => x.Time >= firstAware && x.IsDamage() && (x.SkillID == DemonicShockWaveRight || x.SkillID == DemonicShockWaveCenter || x.SkillID == DemonicShockWaveLeft) && x.SrcAgent != 0 && x.SrcInstid != 0);
            if (armDeimosDamageEvent != null)
            {
                gadgetAgents.Add(agentData.GetAgent(armDeimosDamageEvent.SrcAgent, armDeimosDamageEvent.Time));
            }
            return firstAware;
        }

        private static AgentItem GetShackledPrisoner(AgentData agentData, List<CombatItem> combatData)
        {
            CombatItem shackledPrisonerMaxHP = combatData.FirstOrDefault(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 1000980 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate);
            if (shackledPrisonerMaxHP != null)
            {
                AgentItem shackledPrisoner = agentData.GetAgent(shackledPrisonerMaxHP.SrcAgent, shackledPrisonerMaxHP.Time);
                if (shackledPrisoner.ID > 0) // sanity check against unknown
                {
                    return shackledPrisoner;
                }
            }
            return null;
        }

        internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            IReadOnlyList<AgentItem> deimosAgents = agentData.GetNPCsByID(ArcDPSEnums.TargetID.Deimos);
            long start = long.MinValue;
            long genericStart = GetGenericFightOffset(fightData);
            foreach (AgentItem deimos in deimosAgents)
            {
                // enter combat
                CombatItem spawnProtectionRemove = combatData.FirstOrDefault(x => x.DstMatchesAgent(deimos) && x.IsBuffRemove == ArcDPSEnums.BuffRemove.All && x.SkillID == SpawnProtection);
                if (spawnProtectionRemove != null)
                {
                    start = Math.Max(start, spawnProtectionRemove.Time);
                    if (start - genericStart > PreEventConsiderationConstant)
                    {
                        AgentItem shackledPrisoner = GetShackledPrisoner(agentData, combatData);
                        if (shackledPrisoner != null)
                        {
                            CombatItem firstGreen = combatData.FirstOrDefault(x => x.IsBuffApply() && x.SkillID == DeimosSelectedByGreen);
                            CombatItem firstHPUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate && x.SrcMatchesAgent(shackledPrisoner));
                            if (firstGreen != null && firstGreen.Time < start && firstHPUpdate != null && HealthUpdateEvent.GetHealthPercent(firstHPUpdate) == 100) // sanity check
                            {
                                _hasPreEvent = true;
                                _deimos100PercentTime = start - firstHPUpdate.Time;
                                start = firstHPUpdate.Time;
                            }
                        }
                    }
                }
            }
            return start >= 0 ? start : genericStart;
        }

        internal override List<ErrorEvent> GetCustomWarningMessages(FightData fightData, EvtcVersionEvent evtcVersion)
        {
            List<ErrorEvent> res = base.GetCustomWarningMessages(fightData, evtcVersion);
            if (!fightData.IsCM)
            {
                res.Add(new ErrorEvent("Missing outgoing Saul damage due to % based damage"));
            }
            return res;
        }

        private static bool HandleDemonicBonds(AgentData agentData, List<CombatItem> combatData)
        {
            var maxHPUpdates = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 239040 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).ToList();
            var demonicBonds = maxHPUpdates.Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Distinct().Where(x => x.Type == AgentItem.AgentType.Gadget).ToList();
            foreach (AgentItem demonicBond in demonicBonds)
            {
                demonicBond.OverrideID(ArcDPSEnums.TrashID.DemonicBond);
                demonicBond.OverrideType(AgentItem.AgentType.NPC);
            }
            return demonicBonds.Count != 0;
        }

        internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            bool needsRefresh = _hasPreEvent && HandleDemonicBonds(agentData, combatData);
            bool needsDummy = !needsRefresh;
            AgentItem shackledPrisoner = GetShackledPrisoner(agentData, combatData);
            AgentItem saul = agentData.GetNPCsByID(ArcDPSEnums.TrashID.Saul).FirstOrDefault();
            if (shackledPrisoner != null && (saul == null || saul.FirstAware > shackledPrisoner.FirstAware + PreEventConsiderationConstant))
            {
                shackledPrisoner.OverrideID(ArcDPSEnums.TrashID.ShackledPrisoner);
                shackledPrisoner.OverrideType(AgentItem.AgentType.NPC);
                needsRefresh = true;
            }
            if (_hasPreEvent && needsDummy)
            {
                agentData.AddCustomNPCAgent(fightData.FightStart, _deimos100PercentTime, "Deimos Pre Event", Spec.NPC, ArcDPSEnums.TargetID.DummyTarget, true);
                needsRefresh = false; // AddCustomNPCAgent already refreshes
            }
            if (needsRefresh)
            {
                agentData.Refresh();
            }
            ComputeFightTargets(agentData, combatData, extensions);
            // Find target
            AbstractSingleActor deimos = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Deimos)) ?? throw new MissingKeyActorsException("Deimos not found");
            // invul correction
            CombatItem invulApp = combatData.FirstOrDefault(x => x.DstMatchesAgent(deimos.AgentItem) && x.IsBuffApply() && x.SkillID == Determined762);
            if (invulApp != null)
            {
                invulApp.OverrideValue((int)(deimos.LastAware - invulApp.Time));
            }
            // Deimos gadgets
            CombatItem targetable = combatData.LastOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.Targetable && x.DstAgent > 0 && x.Time >= deimos.FirstAware);
            var gadgetAgents = new HashSet<AgentItem>();
            long firstAware = AttackTargetSpecialParse(targetable, agentData, combatData, gadgetAgents);
            // legacy method
            if (firstAware == 0)
            {
                CombatItem armDeimosDamageEvent = combatData.FirstOrDefault(x => x.Time >= deimos.LastAware && (x.SkillID == DemonicShockWaveRight || x.SkillID == DemonicShockWaveCenter || x.SkillID == DemonicShockWaveLeft) && x.SrcAgent != 0 && x.SrcInstid != 0 && !x.IsExtension);
                if (armDeimosDamageEvent != null)
                {
                    var deimosGadgets = agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => x.Name.Contains("Deimos") && x.LastAware > armDeimosDamageEvent.Time).ToList();
                    if (deimosGadgets.Count > 0)
                    {
                        firstAware = deimosGadgets.Max(x => x.FirstAware);
                        gadgetAgents = new HashSet<AgentItem>(deimosGadgets);
                    }
                }
            }
            if (gadgetAgents.Count > 0)
            {
                _deimos10PercentTime = (firstAware >= deimos.LastAware ? firstAware : deimos.LastAware);
                MergeWithGadgets(deimos.AgentItem, gadgetAgents, combatData, agentData, extensions);
                // Add custom spawn event
                combatData.Add(new CombatItem(_deimos10PercentTime, deimos.AgentItem.Agent, 0, 0, 0, 0, 0, deimos.AgentItem.InstID, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 0, 0, 0, 0));
            }
            deimos.AgentItem.OverrideAwareTimes(deimos.FirstAware, fightData.FightEnd);
            deimos.OverrideName("Deimos");
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.IsSpecies(ArcDPSEnums.TrashID.Thief) || target.IsSpecies(ArcDPSEnums.TrashID.Drunkard) || target.IsSpecies(ArcDPSEnums.TrashID.Gambler))
                {

                    string name = (target.IsSpecies(ArcDPSEnums.TrashID.Thief) ? "Thief" : (target.IsSpecies(ArcDPSEnums.TrashID.Drunkard) ? "Drunkard" : (target.IsSpecies(ArcDPSEnums.TrashID.Gambler) ? "Gambler" : "")));
                    target.OverrideName(name);
                }
            }
        }

        internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
        {
            // We expect pre event with logs with LogStartNPCUpdate events
            if (!_hasPreEvent && combatData.GetLogNPCUpdateEvents().Any())
            {
                return FightData.EncounterStartStatus.NoPreEvent;
            }
            else
            {
                return FightData.EncounterStartStatus.Normal;
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Deimos)) ?? throw new MissingKeyActorsException("Deimos not found");
            phases[0].AddTarget(mainTarget);

            if (requirePhases)
            {
                if (_deimos100PercentTime > 0)
                {
                    var phasePreEvent = new PhaseData(0, _deimos100PercentTime, "Pre Event");
                    phasePreEvent.AddTargets(Targets.Where(x => x.IsSpecies(ArcDPSEnums.TrashID.DemonicBond)));
                    phasePreEvent.AddTarget(Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.DummyTarget)));
                    phases.Add(phasePreEvent);
                    var phase100to0 = new PhaseData(_deimos100PercentTime, log.FightData.FightEnd, "Main Fight");
                    phase100to0.AddTarget(mainTarget);
                    phases.Add(phase100to0);
                }
                phases = AddBossPhases(phases, log, mainTarget);
                phases = AddAddPhases(phases, log, mainTarget);
                phases = AddBurstPhases(phases, log, mainTarget);
            }

            return phases;
        }

        private List<PhaseData> AddBossPhases(List<PhaseData> phases, ParsedEvtcLog log, AbstractSingleActor mainTarget)
        {
            // Determined + additional data on inst change
            AbstractBuffEvent invulDei = log.CombatData.GetBuffDataByIDByDst(Determined762, mainTarget.AgentItem).FirstOrDefault(x => x is BuffApplyEvent);

            if (invulDei != null || _deimos10PercentTime > 0)
            {
                long npcDeimosEnd = _deimos10PercentTime;
                var mainDeimosPhaseName = "100% - 10%";
                if (invulDei != null)
                {
                    npcDeimosEnd = invulDei.Time;
                }
                else if (log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem).Any())
                {
                    HealthUpdateEvent prevHPUpdate = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem).LastOrDefault(x => x.Time <= _deimos10PercentTime);
                    if (prevHPUpdate != null)
                    {
                        mainDeimosPhaseName = $"100% - {Math.Round(prevHPUpdate.HealthPercent)}%";
                        npcDeimosEnd = prevHPUpdate.Time;
                    }
                }
                var phase100to10 = new PhaseData(_deimos100PercentTime, npcDeimosEnd, mainDeimosPhaseName);
                phase100to10.AddTarget(mainTarget);
                phases.Add(phase100to10);

                if (_deimos10PercentTime > 0 && log.FightData.FightEnd - _deimos10PercentTime > PhaseTimeLimit)
                {
                    var phase10to0 = new PhaseData(_deimos10PercentTime, log.FightData.FightEnd, "10% - 0%");
                    phase10to0.AddTarget(mainTarget);
                    phases.Add(phase10to0);
                }
                //mainTarget.AddCustomCastLog(end, -6, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None, log);
            }

            return phases;
        }

        private List<PhaseData> AddAddPhases(List<PhaseData> phases, ParsedEvtcLog log, AbstractSingleActor mainTarget)
        {
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.IsSpecies(ArcDPSEnums.TrashID.Thief) || target.IsSpecies(ArcDPSEnums.TrashID.Drunkard) || target.IsSpecies(ArcDPSEnums.TrashID.Gambler))
                {
                    var addPhase = new PhaseData(target.FirstAware - 1000, Math.Min(target.LastAware + 1000, log.FightData.FightEnd), target.Character);
                    addPhase.AddTarget(target);
                    addPhase.OverrideTimes(log);
                    // override first then add Deimos so that it does not disturb the override process
                    addPhase.AddTarget(mainTarget);
                    phases.Add(addPhase);
                }
            }

            return phases;
        }

        private static List<PhaseData> AddBurstPhases(List<PhaseData> phases, ParsedEvtcLog log, AbstractSingleActor mainTarget)
        {
            var signets = mainTarget.GetBuffStatus(log, UnnaturalSignet, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            int burstID = 1;
            foreach (Segment signet in signets)
            {
                long sigStart = Math.Max(signet.Start, log.FightData.FightStart);
                long sigEnd = Math.Min(signet.End, log.FightData.FightEnd);
                var burstPhase = new PhaseData(sigStart, sigEnd, "Burst " + burstID++);
                burstPhase.AddTarget(mainTarget);
                phases.Add(burstPhase);
            }
            return phases;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Deimos,
                (int)ArcDPSEnums.TargetID.DummyTarget,
                (int)ArcDPSEnums.TrashID.Thief,
                (int)ArcDPSEnums.TrashID.Drunkard,
                (int)ArcDPSEnums.TrashID.Gambler,
                (int)ArcDPSEnums.TrashID.DemonicBond
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.GamblerClones,
                ArcDPSEnums.TrashID.GamblerReal,
                ArcDPSEnums.TrashID.Greed,
                ArcDPSEnums.TrashID.Pride,
                ArcDPSEnums.TrashID.Oil,
                ArcDPSEnums.TrashID.Tear,
                ArcDPSEnums.TrashID.Hands
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Deimos:
                    var mindCrush = cls.Where(x => x.SkillId == MindCrush).ToList();
                    foreach (AbstractCastEvent c in mindCrush)
                    {
                        start = (int)c.Time;
                        end = start + 5000;
                        var circle = new CircleDecoration(180, (start, end), Colors.Red, 0.5, new AgentConnector(target));
                        replay.AddDecorationWithFilledWithGrowing(circle.UsingFilled(false), true, end);
                        if (!log.FightData.IsCM)
                        {
                            replay.Decorations.Add(new CircleDecoration(180, (start, end), Colors.Blue, 0.3, new PositionConnector(new Point3D(-8421.818f, 3091.72949f, -9.818082e8f))));
                        }
                    }
                    var annihilate = cls.Where(x => (x.SkillId == Annihilate2) || (x.SkillId == Annihilate1)).ToList();
                    foreach (AbstractCastEvent c in annihilate)
                    {
                        start = (int)c.Time;
                        int delay = 1000;
                        end = start + 2400;
                        int duration = 120;
                        Point3D facing = target.GetCurrentRotation(log, start);
                        if (facing == null)
                        {
                            continue;
                        }
                        float initialAngle = Point3D.GetZRotationFromFacing(facing);
                        var connector = new AgentConnector(target);
                        for (int i = 0; i < 6; i++)
                        {
                            var rotationConnector1 = new AngleConnector(initialAngle + i * 360 / 10);
                            replay.Decorations.Add(new PieDecoration(900, 360 / 10, (start + delay + i * duration, end + i * duration), Colors.Yellow, 0.5, connector).UsingRotationConnector(rotationConnector1));
                            replay.Decorations.Add(new PieDecoration(900, 360 / 10, (start + delay + i * duration, end + i * 120), Colors.LightOrange, 0.5, connector).UsingFilled(false).UsingRotationConnector(rotationConnector1));
                            if (i % 5 != 0)
                            {
                                var rotationConnector2 = new AngleConnector(initialAngle - i * 360 / 10);
                                replay.Decorations.Add(new PieDecoration(900, 360 / 10, (start + delay + i * duration, end + i * 120), Colors.Yellow, 0.5, connector).UsingRotationConnector(rotationConnector2));
                                replay.Decorations.Add(new PieDecoration(900, 360 / 10, (start + delay + i * duration, end + i * 120), Colors.LightOrange, 0.5, connector).UsingFilled(false).UsingRotationConnector(rotationConnector2));
                            }
                        }
                    }
                    var signets = target.GetBuffStatus(log, UnnaturalSignet, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
                    foreach (Segment seg in signets)
                    {
                        replay.Decorations.Add(new CircleDecoration(120, seg, Colors.Teal, 0.4, new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Gambler:
                case (int)ArcDPSEnums.TrashID.Thief:
                case (int)ArcDPSEnums.TrashID.Drunkard:
                    break;
                case (int)ArcDPSEnums.TrashID.GamblerClones:
                case (int)ArcDPSEnums.TrashID.GamblerReal:
                case (int)ArcDPSEnums.TrashID.Greed:
                case (int)ArcDPSEnums.TrashID.Pride:
                case (int)ArcDPSEnums.TrashID.Tear:
                    break;
                case (int)ArcDPSEnums.TrashID.Hands:
                    replay.Decorations.Add(new CircleDecoration(90, (start, end), Colors.Red, 0.2, new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.Oil:
                    int delayOil = 3000;
                    replay.Decorations.Add(new CircleDecoration(200, (start, start + delayOil), Colors.Orange, 0.5, new AgentConnector(target)).UsingGrowingEnd(start + delayOil));
                    replay.Decorations.Add(new CircleDecoration(200, (start + delayOil, end), Colors.Black, 0.5, new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.ShackledPrisoner:
                    AbstractSingleActor Saul = NonPlayerFriendlies.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TrashID.Saul));
                    if (Saul != null)
                    {
                        replay.Trim(replay.TimeOffsets.start, Saul.FirstAware);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.DemonicBond:
                    replay.Trim(replay.TimeOffsets.start, _deimos100PercentTime);
                    var demonicCenter = new Point3D(-8092.57f, 4176.98f);
                    replay.Decorations.Add(new LineDecoration((replay.TimeOffsets.start, replay.TimeOffsets.end), Colors.Teal, 0.4, new AgentConnector(target), new PositionConnector(demonicCenter)));
                    AbstractSingleActor shackledPrisoner = NonPlayerFriendlies.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TrashID.ShackledPrisoner));
                    if (shackledPrisoner != null)
                    {
                        Point3D shackledPos = shackledPrisoner.GetCurrentPosition(log, replay.TimeOffsets.start + ServerDelayConstant);
                        if (shackledPos != null)
                        {
                            float diffX = 0;
                            float diffY = 0;
                            if (replay.PolledPositions[0].X - demonicCenter.X > 0)
                            {
                                if (replay.PolledPositions[0].Y - demonicCenter.Y > 0)
                                {
                                    // top
                                    diffX = 55;
                                    diffY = 1080;
                                }
                                else
                                {
                                    // right
                                    diffX = 1115;
                                    diffY = -35;
                                }
                            }
                            else
                            {
                                if (replay.PolledPositions[0].Y - demonicCenter.Y > 0)
                                {
                                    // left 
                                    diffX = -1100;
                                    diffY = 40;
                                }
                                else
                                {
                                    // bottom
                                    diffX = -38;
                                    diffY = -1130;
                                }
                            }
                            Point3D pos = shackledPos + new Point3D(diffX, diffY);
                            replay.Decorations.Add(new LineDecoration((replay.TimeOffsets.start, replay.TimeOffsets.end), Colors.Teal, 0.4, new AgentConnector(shackledPrisoner), new PositionConnector(pos)));
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            // teleport zone
            var tpDeimos = p.GetBuffStatus(log, DeimosSelectedByGreen, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in tpDeimos)
            {
                var circle = new CircleDecoration(180, seg, "rgba(0, 150, 0, 0.3)", new AgentConnector(p));
                replay.AddDecorationWithGrowing(circle, seg.End);
            }
            // Tear Instability
            IEnumerable<Segment> tearInstabs = p.GetBuffStatus(log, TearInstability, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
            replay.AddOverheadIcons(tearInstabs, p, ParserIcons.TearInstabilityOverhead);
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Deimos)) ?? throw new MissingKeyActorsException("Deimos not found");
            FightData.EncounterMode cmStatus = (target.GetHealth(combatData) > 40e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;

            // Deimos gains additional health during the last 10% so the max-health needs to be corrected
            // done here because this method will get called during the creation of the ParsedEvtcLog and the ParsedEvtcLog should contain complete and correct values after creation
            if (cmStatus == FightData.EncounterMode.CM)
            {
                target.SetManualHealth(42804900, new List<(long hpValue, double percent)>()
                    {
                        (42000000 , 100),
                        (50049000, 10)
                    });
            }
            else
            {
                target.SetManualHealth(37388210, new List<(long hpValue, double percent)>()
                    {
                        (35981456 , 100),
                        (50049000, 10)
                    });
            }

            return cmStatus;
        }
    }
}
