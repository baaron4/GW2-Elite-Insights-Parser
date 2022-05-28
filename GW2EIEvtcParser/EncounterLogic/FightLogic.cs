using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    public abstract class FightLogic
    {

        public enum ParseMode { FullInstance, Instanced10, Instanced5, Benchmark, WvW, sPvP, Unknown };

        private CombatReplayMap _map;
        protected List<Mechanic> MechanicList { get; }//Resurrects (start), Resurrect
        public ParseMode Mode { get; protected set; } = ParseMode.Unknown;
        public string Extension { get; protected set; }
        public string Icon { get; protected set; }
        private readonly int _basicMechanicsCount;
        public bool HasNoFightSpecificMechanics => MechanicList.Count == _basicMechanicsCount;
        public IReadOnlyCollection<AgentItem> TargetAgents { get; protected set; }
        public IReadOnlyCollection<AgentItem> NonPlayerFriendlyAgents { get; protected set; }
        public IReadOnlyCollection<AgentItem> TrashMobAgents { get; protected set; }
        public IReadOnlyList<NPC> TrashMobs => _trashMobs;
        public IReadOnlyList<AbstractSingleActor> NonPlayerFriendlies => _nonPlayerFriendlies;
        public IReadOnlyList<AbstractSingleActor> Targets => _targets;
        protected readonly List<NPC> _trashMobs = new List<NPC>();
        protected readonly List<AbstractSingleActor> _nonPlayerFriendlies = new List<AbstractSingleActor>();
        protected readonly List<AbstractSingleActor> _targets = new List<AbstractSingleActor>();

        public bool Targetless { get; protected set; } = false;
        protected int GenericTriggerID { get; }

        public EncounterCategory EncounterCategoryInformation { get; protected set; }


        internal static Mechanic DeathMechanic = new PlayerStatusMechanic<DeadEvent>("Dead", new MechanicPlotlySetting(Symbols.X, Colors.Black), "Dead", "Dead", "Dead", 0, (log, a) => log.CombatData.GetDeadEvents(a)).UsingShowOnTable(false);
        internal static Mechanic DownMechanic = new PlayerStatusMechanic<DownEvent>("Downed", new MechanicPlotlySetting(Symbols.Cross, Colors.Red), "Downed", "Downed", "Downed", 0, (log, a) => log.CombatData.GetDownEvents(a), (evt, log) => !log.CombatData.GetBuffRemoveAllData(SkillIDs.VaporForm).Any(x => x.To == evt.Src && Math.Abs(x.Time - evt.Time) < 20)).UsingShowOnTable(false);
        internal static Mechanic AliveMechanic = new PlayerStatusMechanic<AliveEvent>("Got up", new MechanicPlotlySetting(Symbols.Cross, Colors.Green), "Got up", "Got up", "Got up", 0, (log, a) => log.CombatData.GetAliveEvents(a)).UsingShowOnTable(false);
        internal static Mechanic RespawnMechanic = new PlayerStatusMechanic<SpawnEvent>("Respawn", new MechanicPlotlySetting(Symbols.Cross, Colors.LightBlue), "Resp", "Resp", "Resp", 0, (log, a) => log.CombatData.GetSpawnEvents(a)).UsingShowOnTable(false);
        internal static Mechanic DespawnMechanic = new PlayerStatusMechanic<DespawnEvent>("Disconnected", new MechanicPlotlySetting(Symbols.X, Colors.LightGrey), "DC", "DC", "DC", 0, (log, a) => log.CombatData.GetDespawnEvents(a)).UsingShowOnTable(false);

        protected FightLogic(int triggerID)
        {
            GenericTriggerID = triggerID;
            MechanicList = new List<Mechanic>() {
                DeathMechanic,
                DownMechanic,
                new PlayerCastStartMechanic(SkillIDs.Resurrect, "Resurrect", new MechanicPlotlySetting(Symbols.CrossOpen,Colors.Teal), "Res", "Res", "Res",0).UsingShowOnTable(false),
                AliveMechanic,
                DespawnMechanic,
                RespawnMechanic
            };
            _basicMechanicsCount = MechanicList.Count;
            EncounterCategoryInformation = new EncounterCategory();
        }

        // Only used for CSV files
        public NPC GetLegacyTarget()
        {
            return Targets.OfType<NPC>().FirstOrDefault();
        }

        internal MechanicData GetMechanicData()
        {
            return new MechanicData(MechanicList);
        }

        protected virtual CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("", (800, 800), (0, 0, 0, 0)/*, (0, 0, 0, 0), (0, 0, 0, 0)*/);
        }

        public CombatReplayMap GetCombatReplayMap(ParsedEvtcLog log)
        {
            if (_map == null)
            {
                _map = GetCombatMapInternal(log);
                _map.ComputeBoundingBox(log);
            }
            return _map;
        }

        protected virtual List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                GenericTriggerID
            };
        }
        protected virtual List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>();
        }

        protected virtual List<int> GetFriendlyNPCIDs()
        {
            return new List<int>();
        }

        internal virtual string GetLogicName(CombatData combatData, AgentData agentData)
        {
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == GenericTriggerID);
            if (target == null)
            {
                return "UNKNOWN";
            }
            return target.Character;
        }

        private static void RegroupTargetsByID(int id, AgentData agentData, List<CombatItem> combatItems, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            IReadOnlyList<AgentItem> agents = agentData.GetNPCsByID(id);
            if (agents.Count > 1)
            {
                AgentItem firstItem = agents.First();
                var agentValues = new HashSet<ulong>(agents.Select(x => x.Agent));
                var newTargetAgent = new AgentItem(firstItem);
                newTargetAgent.OverrideAwareTimes(agents.Min(x => x.FirstAware), agents.Max(x => x.LastAware));
                agentData.SwapMasters(new HashSet<AgentItem>(agents), newTargetAgent);
                agentData.ReplaceAgentsFromID(newTargetAgent);
                foreach (CombatItem c in combatItems)
                {
                    if (agentValues.Contains(c.SrcAgent) && c.SrcIsAgent(extensions))
                    {
                        c.OverrideSrcAgent(newTargetAgent.Agent);
                    }
                    if (agentValues.Contains(c.DstAgent) && c.DstIsAgent(extensions))
                    {
                        c.OverrideDstAgent(newTargetAgent.Agent);
                    }
                }
            }
        }

        protected abstract HashSet<int> GetUniqueNPCIDs();

        internal virtual void ComputeFightTargets(AgentData agentData, List<CombatItem> combatItems, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            foreach (int id in GetUniqueNPCIDs())
            {
                RegroupTargetsByID(id, agentData, combatItems, extensions);
            }
            //
            List<int> targetIDs = GetTargetsIDs();
            foreach (int id in targetIDs)
            {
                IReadOnlyList<AgentItem> agents = agentData.GetNPCsByID(id);
                foreach (AgentItem agentItem in agents)
                {
                    _targets.Add(new NPC(agentItem));
                }
            }
            _targets.Sort((x, y) => x.FirstAware.CompareTo(y.FirstAware));
            //
            List<ArcDPSEnums.TrashID> trashIDs = GetTrashMobsIDs();
            var aList = agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => trashIDs.Contains(ArcDPSEnums.GetTrashID(x.ID))).ToList();
            //aList.AddRange(agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => ids2.Contains(ParseEnum.GetTrashIDS(x.ID))));
            foreach (AgentItem a in aList)
            {
                _trashMobs.Add(new NPC(a));
            }
            _trashMobs.Sort((x, y) => x.FirstAware.CompareTo(y.FirstAware));
            //
            List<int> friendlyNPCIDs = GetFriendlyNPCIDs();
            foreach (int id in friendlyNPCIDs)
            {
                IReadOnlyList<AgentItem> agents = agentData.GetNPCsByID(id);
                foreach (AgentItem agentItem in agents)
                {
                    _nonPlayerFriendlies.Add(new NPC(agentItem));
                }
            }
            _nonPlayerFriendlies.Sort((x, y) => x.FirstAware.CompareTo(y.FirstAware));
            //
            TargetAgents = new HashSet<AgentItem>(_targets.Select(x => x.AgentItem));
            NonPlayerFriendlyAgents = new HashSet<AgentItem>(_nonPlayerFriendlies.Select(x => x.AgentItem));
            TrashMobAgents = new HashSet<AgentItem>(_trashMobs.Select(x => x.AgentItem));
        }

        internal virtual List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>();
        }

        protected static List<PhaseData> GetPhasesByHealthPercent(ParsedEvtcLog log, AbstractSingleActor mainTarget, List<double> thresholds)
        {
            var phases = new List<PhaseData>();
            if (thresholds.Count == 0)
            {
                return phases;
            }
            long fightDuration = log.FightData.FightEnd;
            long start = 0;
            double offset = 100.0 / thresholds.Count;
            IReadOnlyList<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem);
            for (int i = 0; i < thresholds.Count; i++)
            {
                HealthUpdateEvent evt = hpUpdates.FirstOrDefault(x => x.HPPercent <= thresholds[i]);
                if (evt == null)
                {
                    break;
                }
                var phase = new PhaseData(start, Math.Min(evt.Time, fightDuration), (offset + thresholds[i]) + "% - " + thresholds[i] + "%");
                phase.AddTarget(mainTarget);
                phases.Add(phase);
                start = Math.Max(evt.Time, 0);
            }
            if (phases.Count > 0 && phases.Count < thresholds.Count)
            {
                var lastPhase = new PhaseData(start, fightDuration, (offset + thresholds[phases.Count]) + "% -" + thresholds[phases.Count] + "%");
                lastPhase.AddTarget(mainTarget);
                phases.Add(lastPhase);
            }
            return phases;
        }

        protected static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, IEnumerable<long> skillIDs, AbstractSingleActor mainTarget, bool addSkipPhases, bool beginWithStart, long start, long end)
        {
            var phases = new List<PhaseData>();
            long last = start;
            var invuls = skillIDs.SelectMany(skillID => GetFilteredList(log.CombatData, skillID, mainTarget, beginWithStart, true)).ToList();
            invuls.RemoveAll(x => x.Time < 0);
            invuls.Sort((event1, event2) => event1.Time.CompareTo(event2.Time)); // Sort in case there were multiple skillIDs
            for (int i = 0; i < invuls.Count; i++)
            {
                AbstractBuffEvent c = invuls[i];
                if (c is BuffApplyEvent)
                {
                    long curEnd = Math.Min(c.Time, end);
                    phases.Add(new PhaseData(last, curEnd));
                    last = curEnd;
                }
                else
                {
                    long curEnd = Math.Min(c.Time, end);
                    if (addSkipPhases)
                    {
                        phases.Add(new PhaseData(last, curEnd));
                    }
                    last = curEnd;
                }
            }
            if (end - last > ParserHelper.PhaseTimeLimit)
            {
                phases.Add(new PhaseData(last, end));
            }
            return phases.Where(x => x.DurationInMS > ParserHelper.PhaseTimeLimit).ToList();
        }

        protected static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, long skillID, AbstractSingleActor mainTarget, bool addSkipPhases, bool beginWithStart, long start, long end)
        {
            return GetPhasesByInvul(log, new[] { skillID }, mainTarget, addSkipPhases, beginWithStart, start, end);
        }

        protected static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, long skillID, AbstractSingleActor mainTarget, bool addSkipPhases, bool beginWithStart)
        {
            return GetPhasesByInvul(log, skillID, mainTarget, addSkipPhases, beginWithStart, 0, log.FightData.FightEnd);
        }

        protected static List<PhaseData> GetInitialPhase(ParsedEvtcLog log)
        {
            var phases = new List<PhaseData>();
            long fightDuration = log.FightData.FightEnd;
            phases.Add(new PhaseData(0, fightDuration, "Full Fight")
            {
                CanBeSubPhase = false
            });
            return phases;
        }

        internal List<PhaseData> GetBreakbarPhases(ParsedEvtcLog log, bool requirePhases)
        {
            if (!requirePhases)
            {
                return new List<PhaseData>();
            }
            var breakbarPhases = new List<PhaseData>();
            foreach (AbstractSingleActor target in Targets)
            {
                int i = 0;
                IReadOnlyList<BreakbarStateEvent> breakbarStateEvents = log.CombatData.GetBreakbarStateEvents(target.AgentItem);
                IReadOnlyList<BreakbarPercentEvent> breakbarPercentEvents = log.CombatData.GetBreakbarPercentEvents(target.AgentItem);
                var breakbarActiveEvents = breakbarStateEvents.Where(x => x.State == ArcDPSEnums.BreakbarState.Active).ToList();
                var breakbarNotActiveEvents = breakbarStateEvents.Where(x => x.State != ArcDPSEnums.BreakbarState.Active).ToList();
                foreach (BreakbarStateEvent active in breakbarActiveEvents)
                {
                    long start = Math.Max(active.Time - 2000, 0);
                    BreakbarStateEvent notActive = breakbarNotActiveEvents.FirstOrDefault(x => x.Time >= active.Time);
                    long end;
                    if (notActive == null)
                    {
                        DeadEvent deadEvent = log.CombatData.GetDeadEvents(target.AgentItem).LastOrDefault();
                        if (deadEvent == null)
                        {
                            end = Math.Min(target.LastAware, log.FightData.FightEnd);
                        }
                        else
                        {
                            end = Math.Min(deadEvent.Time, log.FightData.FightEnd);
                        }
                    }
                    else
                    {
                        end = Math.Min(notActive.Time, log.FightData.FightEnd);
                    }
                    var phase = new PhaseData(start, end, target.Character + " Breakbar " + ++i)
                    {
                        BreakbarPhase = true,
                        CanBeSubPhase = false
                    };
                    phase.AddTarget(target);
                    breakbarPhases.Add(phase);
                }
            }
            return breakbarPhases;
        }

        internal virtual List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            phases[0].AddTarget(mainTarget);
            return phases;
        }

        internal virtual List<ErrorEvent> GetCustomWarningMessages(FightData fightData, int arcdpsVersion)
        {
            if (arcdpsVersion >= ParserHelper.ArcDPSBuilds.DirectX11Update)
            {
                return new List<ErrorEvent>
                {
                    new ErrorEvent("As of arcdps 20210923, animated cast events' durations are broken, as such, any feature having a dependency on it are to be taken with a grain of salt. Impacted features are: <br>- Rotations <br>- Time spent in animation statistics <br>- Mechanics <br>- Phases <br>- Combat Replay Decorations")
                };
            }
            return new List<ErrorEvent>();
        }

        protected static List<ErrorEvent> GetConfusionDamageMissingMessage(int arcdpsVersion)
        {
            if (arcdpsVersion > ParserHelper.ArcDPSBuilds.ProperConfusionDamageSimulation)
            {
                return new List<ErrorEvent>();
            }
            return new List<ErrorEvent>()
            {
                new ErrorEvent("Missing confusion damage")
            };
        }

        protected void AddTargetsToPhaseAndFit(PhaseData phase, List<int> ids, ParsedEvtcLog log)
        {
            foreach (AbstractSingleActor target in Targets)
            {
                if (ids.Contains(target.ID) && phase.InInterval(Math.Max(target.FirstAware, 0)))
                {
                    phase.AddTarget(target);
                }
            }
            phase.OverrideTimes(log);
        }

        internal virtual List<AbstractBuffEvent> SpecialBuffEventProcess(CombatData combatData, SkillData skillData)
        {
            return new List<AbstractBuffEvent>();
        }

        protected static void NegateDamageAgainstBarrier(CombatData combatData, List<AgentItem> agentItems)
        {
            var dmgEvts = new List<AbstractHealthDamageEvent>();
            foreach (AgentItem agentItem in agentItems)
            {
                dmgEvts.AddRange(combatData.GetDamageTakenData(agentItem));
            }
            foreach (AbstractHealthDamageEvent de in dmgEvts)
            {
                if (de.ShieldDamage > 0)
                {
                    de.NegateShieldDamage();
                }
            }
        }

        /*protected static void AdjustTimeRefreshBuff(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, long id)
        {
            if (buffsById.TryGetValue(id, out List<AbstractBuffEvent> buffList))
            {
                var agentsToSort = new HashSet<AgentItem>();
                foreach (AbstractBuffEvent be in buffList)
                {
                    if (be is AbstractBuffRemoveEvent abre)
                    {
                        // to make sure remove events are before applications
                        abre.OverrideTime(abre.Time - 1);
                        agentsToSort.Add(abre.To);
                    }
                }
                if (buffList.Count > 0)
                {
                    buffsById[id].Sort((x, y) => x.Time.CompareTo(y.Time));
                }
                foreach (AgentItem a in agentsToSort)
                {
                    buffsByDst[a].Sort((x, y) => x.Time.CompareTo(y.Time));
                }
            }
        }*/

        internal virtual List<AbstractCastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
        {
            return new List<AbstractCastEvent>();
        }

        internal virtual List<AbstractHealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
        {
            return new List<AbstractHealthDamageEvent>();
        }

        internal virtual void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
        }

        internal virtual void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
        }

        internal virtual FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.CMStatus.NoCM;
        }

        protected void SetSuccessByDeath(CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents, bool all, int idFirst, params int[] ids)
        {
            var idsToUse = new List<int>
            {
                idFirst
            };
            idsToUse.AddRange(ids);
            SetSuccessByDeath(combatData, fightData, playerAgents, all, idsToUse);
        }

        protected void SetSuccessByDeath(CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents, bool all, List<int> idsToUse)
        {
            if (!idsToUse.Any())
            {
                return;
            }
            int success = 0;
            long maxTime = long.MinValue;
            foreach (int id in idsToUse)
            {
                AbstractSingleActor target = Targets.FirstOrDefault(x => x.ID == id);
                if (target == null)
                {
                    return;
                }
                DeadEvent killed = combatData.GetDeadEvents(target.AgentItem).LastOrDefault();
                if (killed != null)
                {
                    long time = killed.Time;
                    success++;
                    AbstractHealthDamageEvent lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                    if (lastDamageTaken != null)
                    {
                        time = Math.Min(lastDamageTaken.Time, time);
                    }
                    maxTime = Math.Max(time, maxTime);
                }
            }
            if ((all && success == idsToUse.Count) || (!all && success > 0))
            {
                fightData.SetSuccess(true, maxTime);
            }
        }

        protected static bool AtLeastOnePlayerAlive(CombatData combatData, FightData fightData, long timeToCheck, IReadOnlyCollection<AgentItem> playerAgents)
        {
            int playerDeadOrDCCount = 0;
            foreach (AgentItem playerAgent in playerAgents)
            {
                var deads = new List<(long start, long end)>();
                var downs = new List<(long start, long end)>();
                var dcs = new List<(long start, long end)>();
                playerAgent.GetAgentStatus(deads, downs, dcs, combatData, fightData);
                if (deads.Any(x => x.start <= timeToCheck && x.end >= timeToCheck))
                {
                    playerDeadOrDCCount++;
                }
                else if (dcs.Any(x => x.start <= timeToCheck && x.end >= timeToCheck))
                {
                    playerDeadOrDCCount++;
                }
            }
            if (playerDeadOrDCCount == playerAgents.Count)
            {
                return false;
            }
            return true;
        }

        protected static void SetSuccessByCombatExit(List<AbstractSingleActor> targets, CombatData combatData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            if (targets.Count == 0)
            {
                return;
            }
            var targetExits = new List<ExitCombatEvent>();
            var lastTargetDamages = new List<AbstractHealthDamageEvent>();
            foreach (AbstractSingleActor t in targets)
            {
                EnterCombatEvent enterCombat = combatData.GetEnterCombatEvents(t.AgentItem).LastOrDefault();
                ExitCombatEvent exitCombat;
                if (enterCombat != null)
                {
                    exitCombat = combatData.GetExitCombatEvents(t.AgentItem).Where(x => x.Time > enterCombat.Time).LastOrDefault();
                }
                else
                {
                    exitCombat = combatData.GetExitCombatEvents(t.AgentItem).LastOrDefault();
                }
                AbstractHealthDamageEvent lastDamage = combatData.GetDamageTakenData(t.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                if (exitCombat == null || lastDamage == null ||
                    combatData.GetAnimatedCastData(t.AgentItem).Any(x => x.Time > exitCombat.Time + ParserHelper.ServerDelayConstant) ||
                    combatData.GetDamageData(t.AgentItem).Any(x => x.Time > exitCombat.Time + ParserHelper.ServerDelayConstant && playerAgents.Contains(x.To)))
                {
                    return;
                }
                targetExits.Add(exitCombat);
                lastTargetDamages.Add(lastDamage);
            }
            ExitCombatEvent lastTargetExit = targetExits.Count > 0 ? targetExits.MaxBy(x => x.Time) : null;
            AbstractHealthDamageEvent lastDamageTaken = lastTargetDamages.Count > 0 ? lastTargetDamages.MaxBy(x => x.Time) : null;
            // Make sure the last damage has been done before last combat exit
            if (lastTargetExit != null && lastDamageTaken != null && lastTargetExit.Time + 150 >= lastDamageTaken.Time)
            {
                if (!AtLeastOnePlayerAlive(combatData, fightData, lastTargetExit.Time, playerAgents))
                {
                    return;
                }
                fightData.SetSuccess(true, lastDamageTaken.Time);
            }
        }

        protected virtual List<int> GetSuccessCheckIds()
        {
            return new List<int>
            {
                GenericTriggerID
            };
        }

        internal virtual void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            SetSuccessByDeath(combatData, fightData, playerAgents, true, GetSuccessCheckIds());
        }

        internal virtual long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return fightData.LogStart;
        }

        internal virtual void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            ComputeFightTargets(agentData, combatData, extensions);
        }

        //
        protected static List<AbstractBuffEvent> GetFilteredList(CombatData combatData, long buffID, AbstractSingleActor target, bool beginWithStart, bool padEnd)
        {
            bool needStart = beginWithStart;
            var main = combatData.GetBuffData(buffID).Where(x => x.To == target.AgentItem && (x is BuffApplyEvent || x is BuffRemoveAllEvent)).ToList();
            var filtered = new List<AbstractBuffEvent>();
            for (int i = 0; i < main.Count; i++)
            {
                AbstractBuffEvent c = main[i];
                if (needStart && c is BuffApplyEvent)
                {
                    needStart = false;
                    filtered.Add(c);
                }
                else if (!needStart && c is BuffRemoveAllEvent)
                {
                    // consider only last remove event before another application
                    if ((i == main.Count - 1) || (i < main.Count - 1 && main[i + 1] is BuffApplyEvent))
                    {
                        needStart = true;
                        filtered.Add(c);
                    }
                }
            }
            if (padEnd && filtered.Any() && filtered.Last() is BuffApplyEvent)
            {
                AbstractBuffEvent last = filtered.Last();
                filtered.Add(new BuffRemoveAllEvent(ParserHelper._unknownAgent, last.To, target.LastAware, int.MaxValue, last.BuffSkill, BuffRemoveAllEvent.FullRemoval, int.MaxValue));
            }
            return filtered;
        }

    }
}
