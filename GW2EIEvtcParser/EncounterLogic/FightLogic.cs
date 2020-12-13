using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    public abstract class FightLogic
    {

        public enum ParseMode { Instanced10, Instanced5, Benchmark, WvW, sPvP, Unknown };

        private CombatReplayMap _map;
        protected List<Mechanic> MechanicList { get; }//Resurrects (start), Resurrect
        public ParseMode Mode { get; protected set; } = ParseMode.Unknown;
        public string Extension { get; protected set; }
        public string Icon { get; protected set; }
        private readonly int _basicMechanicsCount;
        public bool HasNoFightSpecificMechanics => MechanicList.Count == _basicMechanicsCount;
        public List<NPC> TrashMobs { get; } = new List<NPC>();
        public List<NPC> Targets { get; } = new List<NPC>();

        public bool Targetless { get; protected set; } = false;
        protected int GenericTriggerID { get; }

        protected FightLogic(int triggerID)
        {
            GenericTriggerID = triggerID;
            MechanicList = new List<Mechanic>() {
                new PlayerStatusMechanic(SkillItem.DeathId, "Dead", new MechanicPlotlySetting("x","rgb(0,0,0)"), "Dead",0),
                new PlayerStatusMechanic(SkillItem.DownId, "Downed", new MechanicPlotlySetting("cross","rgb(255,0,0)"), "Downed",0),
                new PlayerStatusMechanic(SkillItem.ResurrectId, "Resurrect", new MechanicPlotlySetting("cross-open","rgb(0,255,255)"), "Res",0),
                new PlayerStatusMechanic(SkillItem.AliveId, "Got up", new MechanicPlotlySetting("cross","rgb(0,255,0)"), "Got up",0),
                new PlayerStatusMechanic(SkillItem.DCId, "Disconnected", new MechanicPlotlySetting("x","rgb(120,120,120)"), "DC",0),
                new PlayerStatusMechanic(SkillItem.RespawnId, "Respawn", new MechanicPlotlySetting("cross","rgb(120,120,255)"), "Resp",0)
            };
            _basicMechanicsCount = MechanicList.Count;
        }

        // Only used for CSV files
        public NPC GetLegacyTarget()
        {
            return Targets.FirstOrDefault();
        }

        public MechanicData GetMechanicData()
        {
            return new MechanicData(MechanicList);
        }

        protected virtual CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("", (800, 800), (0, 0, 0, 0), (0, 0, 0, 0), (0, 0, 0, 0));
        }

        public CombatReplayMap GetCombatMap(ParsedEvtcLog log)
        {
            if (_map == null)
            {
                _map = GetCombatMapInternal(log);
                _map.ComputeBoundingBox(log);
            }
            return _map;
        }

        protected virtual List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                GenericTriggerID
            };
        }

        internal virtual string GetLogicName(ParsedEvtcLog log)
        {
            NPC target = Targets.Find(x => x.ID == GenericTriggerID);
            if (target == null)
            {
                return "UNKNOWN";
            }
            return target.Character;
        }

        private static void RegroupTargetsByID(int id, AgentData agentData, List<CombatItem> combatItems)
        {
            List<AgentItem> agents = agentData.GetNPCsByID(id);
            if (agents.Count > 1)
            {
                AgentItem firstItem = agents.First();
                var agentValues = new HashSet<ulong>(agents.Select(x => x.Agent));
                var newTargetAgent = new AgentItem(firstItem);
                newTargetAgent.OverrideAwareTimes(agents.Min(x => x.FirstAware), agents.Max(x => x.LastAware));
                agentData.SwapMasters(new HashSet<AgentItem>(agents), firstItem);
                agentData.OverrideID(id, newTargetAgent);
                foreach (CombatItem c in combatItems)
                {
                    if (agentValues.Contains(c.SrcAgent) && c.IsStateChange.SrcIsAgent())
                    {
                        c.OverrideSrcAgent(newTargetAgent.Agent);
                    }
                    if (agentValues.Contains(c.DstAgent) && c.IsStateChange.DstIsAgent())
                    {
                        c.OverrideDstAgent(newTargetAgent.Agent);
                    }
                }
            }
        }

        protected abstract HashSet<int> GetUniqueTargetIDs();

        internal virtual void ComputeFightTargets(AgentData agentData, List<CombatItem> combatItems)
        {
            foreach (int id in GetUniqueTargetIDs())
            {
                RegroupTargetsByID(id, agentData, combatItems);
            }
            List<int> ids = GetFightTargetsIDs();
            foreach (int id in ids)
            {
                List<AgentItem> agents = agentData.GetNPCsByID(id);
                foreach (AgentItem agentItem in agents)
                {
                    Targets.Add(new NPC(agentItem));
                }
            }
            List<ArcDPSEnums.TrashID> ids2 = GetTrashMobsIDS();
            var aList = agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => ids2.Contains(ArcDPSEnums.GetTrashID(x.ID))).ToList();
            //aList.AddRange(agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => ids2.Contains(ParseEnum.GetTrashIDS(x.ID))));
            foreach (AgentItem a in aList)
            {
                TrashMobs.Add(new NPC(a));
            }
        }

        protected static List<PhaseData> GetPhasesByHealthPercent(ParsedEvtcLog log, NPC mainTarget, List<double> thresholds)
        {
            var phases = new List<PhaseData>();
            if (thresholds.Count == 0)
            {
                return phases;
            }
            long fightDuration = log.FightData.FightEnd;
            long start = 0;
            double offset = 100.0 / thresholds.Count;
            List<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(mainTarget.AgentItem);
            for (int i = 0; i < thresholds.Count; i++)
            {
                HealthUpdateEvent evt = hpUpdates.FirstOrDefault(x => x.HPPercent <= thresholds[i]);
                if (evt == null)
                {
                    break;
                }
                var phase = new PhaseData(start, Math.Min(evt.Time, fightDuration), (offset + thresholds[i]) + "% - " + thresholds[i] + "%");
                phase.Targets.Add(mainTarget);
                phases.Add(phase);
                start = Math.Max(evt.Time, 0);
            }
            if (phases.Count > 0 && phases.Count < thresholds.Count)
            {
                var lastPhase = new PhaseData(start, fightDuration, (offset + thresholds[phases.Count]) + "% -" + thresholds[phases.Count] + "%");
                lastPhase.Targets.Add(mainTarget);
                phases.Add(lastPhase);
            }
            return phases;
        }

        protected static List<PhaseData> GetPhasesByInvul(ParsedEvtcLog log, long skillID, NPC mainTarget, bool addSkipPhases, bool beginWithStart)
        {
            long fightDuration = log.FightData.FightEnd;
            var phases = new List<PhaseData>();
            long last = 0;
            List<AbstractBuffEvent> invuls = GetFilteredList(log.CombatData, skillID, mainTarget, beginWithStart);
            invuls.RemoveAll(x => x.Time < 0);
            for (int i = 0; i < invuls.Count; i++)
            {
                AbstractBuffEvent c = invuls[i];
                if (c is BuffApplyEvent)
                {
                    long end = Math.Min(c.Time, fightDuration);
                    phases.Add(new PhaseData(last, end));
                    /*if (i == invuls.Count - 1)
                    {
                        mainTarget.AddCustomCastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None, log);
                    }*/
                    last = end;
                }
                else
                {
                    long end = Math.Min(c.Time, fightDuration);
                    if (addSkipPhases)
                    {
                        phases.Add(new PhaseData(last, end));
                    }
                    //mainTarget.AddCustomCastLog(last, -5, (int)(end - last), ParseEnum.Activation.None, (int)(end - last), ParseEnum.Activation.None, log);
                    last = end;
                }
            }
            if (fightDuration - last > ParserHelper.PhaseTimeLimit)
            {
                phases.Add(new PhaseData(last, fightDuration));
            }
            return phases;
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
            foreach (NPC target in Targets)
            {
                int i = 0;
                List<BreakbarStateEvent> breakbarStateEvents = log.CombatData.GetBreakbarStateEvents(target.AgentItem);
                List<BreakbarPercentEvent> breakbarPercentEvents = log.CombatData.GetBreakbarPercentEvents(target.AgentItem);
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
                    phase.Targets.Add(target);
                    breakbarPhases.Add(phase);
                }
            }
            return breakbarPhases;
        }

        internal virtual List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == GenericTriggerID);
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            return phases;
        }

        internal virtual List<ErrorEvent> GetCustomWarningMessages()
        {
            return new List<ErrorEvent>();
        }

        protected void AddTargetsToPhase(PhaseData phase, List<int> ids, ParsedEvtcLog log)
        {
            foreach (NPC target in Targets)
            {
                if (ids.Contains(target.ID) && phase.InInterval(Math.Max(target.FirstAware, 0)))
                {
                    phase.Targets.Add(target);
                }
            }
            phase.OverrideTimes(log);
        }

        internal virtual List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
        {
            return new List<AbstractBuffEvent>();
        }

        protected static void NegateDamageAgainstBarrier(List<AgentItem> agentItems, Dictionary<AgentItem, List<AbstractHealthDamageEvent>> damageByDst)
        {
            var dmgEvts = new List<AbstractHealthDamageEvent>();
            foreach (AgentItem agentItem in agentItems)
            {
                if (damageByDst.TryGetValue(agentItem, out List<AbstractHealthDamageEvent> list))
                {
                    dmgEvts.AddRange(list);
                }
            }
            foreach (AbstractHealthDamageEvent de in dmgEvts)
            {
                if (de.ShieldDamage > 0)
                {
                    de.NegateShieldDamage();
                }
            }
        }

        protected static void AdjustTimeRefreshBuff(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, long id)
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
        }

        internal virtual List<AbstractCastEvent> SpecialCastEventProcess(Dictionary<AgentItem, List<AnimatedCastEvent>> animatedCastEvent, Dictionary<AgentItem, List<WeaponSwapEvent>> weaponSwapData, Dictionary<long, List<AbstractCastEvent>> _castDataById, SkillData skillData)
        {
            return new List<AbstractCastEvent>();
        }

        internal virtual List<AbstractHealthDamageEvent> SpecialDamageEventProcess(Dictionary<AgentItem, List<AbstractHealthDamageEvent>> damageBySrc, Dictionary<AgentItem, List<AbstractHealthDamageEvent>> damageByDst, Dictionary<long, List<AbstractHealthDamageEvent>> damageById, SkillData skillData)
        {
            return new List<AbstractHealthDamageEvent>();
        }

        internal virtual void ComputePlayerCombatReplayActors(Player p, ParsedEvtcLog log, CombatReplay replay)
        {
        }

        internal virtual void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
        }

        protected virtual List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>();
        }

        internal virtual FightData.CMStatus IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.CMStatus.NoCM;
        }

        protected void SetSuccessByDeath(CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents, bool all, int idFirst, params int[] ids)
        {
            var idsToUse = new List<int>
            {
                idFirst
            };
            idsToUse.AddRange(ids);
            SetSuccessByDeath(combatData, fightData, playerAgents, all, idsToUse);
        }

        protected void SetSuccessByDeath(CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents, bool all, List<int> idsToUse)
        {
            int success = 0;
            long maxTime = long.MinValue;
            foreach (int id in idsToUse)
            {
                NPC target = Targets.Find(x => x.ID == id);
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

        protected static void SetSuccessByCombatExit(List<NPC> targets, CombatData combatData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            if (targets.Count == 0)
            {
                return;
            }
            var targetExits = new List<ExitCombatEvent>();
            var lastTargetDamages = new List<AbstractHealthDamageEvent>();
            foreach (NPC t in targets)
            {
                EnterCombatEvent enterCombat = combatData.GetEnterCombatEvents(t.AgentItem).LastOrDefault();
                if (enterCombat != null)
                {
                    targetExits.AddRange(combatData.GetExitCombatEvents(t.AgentItem).Where(x => x.Time > enterCombat.Time));
                }
                else
                {
                    targetExits.AddRange(combatData.GetExitCombatEvents(t.AgentItem));
                }
                AbstractHealthDamageEvent lastDamage = combatData.GetDamageTakenData(t.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                if (lastDamage != null)
                {
                    lastTargetDamages.Add(lastDamage);
                }
            }
            ExitCombatEvent lastTargetExit = targetExits.Count > 0 ? targetExits.MaxBy(x => x.Time) : null;
            AbstractHealthDamageEvent lastDamageTaken = lastTargetDamages.Count > 0 ? lastTargetDamages.MaxBy(x => x.Time) : null;
            // Make sure the last damage has been done before last combat exit
            if (lastTargetExit != null && lastDamageTaken != null && lastTargetExit.Time + 100 >= lastDamageTaken.Time)
            {
                int playerDeadOrDCCount = 0;
                foreach (AgentItem playerAgent in playerAgents)
                {
                    var deads = new List<(long start, long end)>();
                    var downs = new List<(long start, long end)>();
                    var dcs = new List<(long start, long end)>();
                    playerAgent.GetAgentStatus(deads, downs, dcs, combatData, fightData);
                    if (deads.Any(x => x.start <= lastTargetExit.Time && x.end >= lastTargetExit.Time))
                    {
                        playerDeadOrDCCount++;
                    }
                    else if (dcs.Any(x => x.start <= lastTargetExit.Time && x.end >= lastTargetExit.Time))
                    {
                        playerDeadOrDCCount++;
                    }
                }
                if (playerDeadOrDCCount == playerAgents.Count)
                {
                    return;
                }
                fightData.SetSuccess(true, lastDamageTaken.Time);
            }
        }

        internal virtual void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            SetSuccessByDeath(combatData, fightData, playerAgents, true, GenericTriggerID);
        }

        internal virtual long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return fightData.FightOffset;
        }

        internal virtual void EIEvtcParse(FightData fightData, AgentData agentData, List<CombatItem> combatData, List<Player> playerList)
        {
            ComputeFightTargets(agentData, combatData);
        }

        //
        protected static List<AbstractBuffEvent> GetFilteredList(CombatData combatData, long buffID, AbstractSingleActor target, bool beginWithStart)
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
            return filtered;
        }

    }
}
