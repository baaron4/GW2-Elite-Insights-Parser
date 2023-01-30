using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;

namespace GW2EIEvtcParser.EncounterLogic
{
    public abstract class FightLogic
    {

        public enum ParseMode { FullInstance, Instanced10, Instanced5, Benchmark, WvW, sPvP, OpenWorld, Unknown };
        protected enum FallBackMethod { None, Death, DeathOrCombatExit, ChestGadget, DeathOrChestGadget, CombatExitOrChestGadget, DeathOrCombatExitOrChestGadget }


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
        protected List<NPC> _trashMobs { get; } = new List<NPC>();
        protected List<AbstractSingleActor> _nonPlayerFriendlies { get; } = new List<AbstractSingleActor>();
        protected List<AbstractSingleActor> _targets { get; } = new List<AbstractSingleActor>();

        protected ArcDPSEnums.ChestID ChestID { get; set; } = ArcDPSEnums.ChestID.None;

        protected List<(Buff buff, int stack)> InstanceBuffs { get; private set; } = null;

        public bool Targetless { get; protected set; } = false;
        protected int GenericTriggerID { get; }

        public long EncounterID { get; protected set; } = EncounterIDs.Unknown;

        public EncounterCategory EncounterCategoryInformation { get; protected set; }
        protected FallBackMethod GenericFallBackMethod { get; set; } = FallBackMethod.Death;


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

        protected virtual void SetInstanceBuffs(ParsedEvtcLog log)
        {
            InstanceBuffs = new List<(Buff buff, int stack)>();
            foreach (Buff fractalInstability in log.Buffs.BuffsBySource[ParserHelper.Source.FractalInstability])
            {
                if (log.CombatData.GetBuffData(fractalInstability.ID).Any(x => x.To.IsPlayer))
                {
                    InstanceBuffs.Add((fractalInstability, 1));
                }
            }
            int emboldenedStacks = (int)log.PlayerList.Select(x => {
                if (x.GetBuffGraphs(log).TryGetValue(SkillIDs.Emboldened, out BuffsGraphModel graph))
                {
                    return graph.BuffChart.Max(y => y.Value);
                }
                else
                {
                    return 0;
                }
            }).Max();
            if (emboldenedStacks > 0)
            {
                InstanceBuffs.Add((log.Buffs.BuffsByIds[SkillIDs.Emboldened], emboldenedStacks));
            }
        }

        public virtual IReadOnlyList<(Buff buff, int stack)> GetInstanceBuffs(ParsedEvtcLog log)
        {
            if (InstanceBuffs == null)
            {
                SetInstanceBuffs(log);
            }
            return InstanceBuffs;
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
            AbstractSingleActor target = Targets.FirstOrDefault(x => x.IsSpecy(GenericTriggerID));
            if (target == null)
            {
                return "UNKNOWN";
            }
            return target.Character;
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
#if DEBUG2
            var unknownAList = agentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.InstID != 0 && x.LastAware - x.FirstAware > 1000 && !trashIDs.Contains(GetTrashID(x.ID)) && !targetIDs.Contains(x.ID)).ToList();
            unknownAList.AddRange(agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => x.LastAware - x.FirstAware > 1000));
            foreach (AgentItem a in unknownAList)
            {
                _trashMobs.Add(new NPC(a));
            }
#endif
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
        
        internal void InvalidateEncounterID()
        {
            EncounterID = EncounterIDs.EncounterMasks.Unsupported;
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
                var breakbarActiveEvents = breakbarStateEvents.Where(x => x.State == ArcDPSEnums.BreakbarState.Active).ToList();
                var breakbarNotActiveEvents = breakbarStateEvents.Where(x => x.State != ArcDPSEnums.BreakbarState.Active).ToList();
                foreach (BreakbarStateEvent active in breakbarActiveEvents)
                {
                    long start = Math.Max(active.Time - 2000, log.FightData.FightStart);
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
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecy(GenericTriggerID));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Main target of the fight not found");
            }
            phases[0].AddTarget(mainTarget);
            return phases;
        }

        internal virtual List<ErrorEvent> GetCustomWarningMessages(FightData fightData, int arcdpsVersion)
        {
            if (arcdpsVersion >= ArcDPSBuilds.DirectX11Update)
            {
                return new List<ErrorEvent>
                {
                    new ErrorEvent("As of arcdps 20210923, animated cast events' durations are broken, as such, any feature having a dependency on it are to be taken with a grain of salt. Impacted features are: <br>- Rotations <br>- Time spent in animation statistics <br>- Mechanics <br>- Phases <br>- Combat Replay Decorations")
                };
            }
            return new List<ErrorEvent>();
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

        internal virtual FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.Normal;
        }

        protected virtual List<int> GetSuccessCheckIDs()
        {
            return new List<int>
            {
                GenericTriggerID
            };
        }

        internal virtual void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            NoBouncyChestGenericCheckSucess(combatData, agentData, fightData, playerAgents);
        }

        protected IReadOnlyList<AbstractSingleActor> GetSuccessCheckTargets()
        {
            return Targets.Where(x => GetSuccessCheckIDs().Contains(x.ID)).ToList();
        }

        protected void NoBouncyChestGenericCheckSucess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            switch (GenericFallBackMethod)
            {
                case FallBackMethod.Death:
                    SetSuccessByDeath(GetSuccessCheckTargets(), combatData, fightData, playerAgents, true);
                    break;
                case FallBackMethod.DeathOrCombatExit:
                    SetSuccessByDeath(GetSuccessCheckTargets(), combatData, fightData, playerAgents, true);
                    if (!fightData.Success)
                    {
                        SetSuccessByCombatExit(GetSuccessCheckTargets(), combatData, fightData, playerAgents);
                    }
                    break;
                case FallBackMethod.ChestGadget:
                    SetSuccessByChestGadget(ChestID, agentData, fightData);
                    break;
                case FallBackMethod.DeathOrChestGadget:
                    SetSuccessByDeath(GetSuccessCheckTargets(), combatData, fightData, playerAgents, true);
                    if (!fightData.Success)
                    {
                        SetSuccessByChestGadget(ChestID, agentData, fightData);
                    }
                    break;
                case FallBackMethod.CombatExitOrChestGadget:
                    SetSuccessByCombatExit(GetSuccessCheckTargets(), combatData, fightData, playerAgents);
                    if (!fightData.Success)
                    {
                        SetSuccessByChestGadget(ChestID, agentData, fightData);
                    }
                    break;
                case FallBackMethod.DeathOrCombatExitOrChestGadget:
                    SetSuccessByDeath(GetSuccessCheckTargets(), combatData, fightData, playerAgents, true);
                    if (!fightData.Success)
                    {
                        SetSuccessByCombatExit(GetSuccessCheckTargets(), combatData, fightData, playerAgents);
                        if (!fightData.Success)
                        {
                            SetSuccessByChestGadget(ChestID, agentData, fightData);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        internal long GetEnterCombatTime(FightData fightData, AgentData agentData, List<CombatItem> combatData, long upperLimit)
        {
            return EncounterLogicTimeUtils.GetEnterCombatTime(fightData, agentData, combatData, upperLimit, GenericTriggerID);
        }

        internal virtual long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            long startToUse = GetGenericFightOffset(fightData);
            CombatItem logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogStartNPCUpdate);
            if (logStartNPCUpdate != null)
            {
                startToUse = GetEnterCombatTime(fightData, agentData, combatData, logStartNPCUpdate.Time);
            }
            return startToUse;
        }

        internal virtual void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            ComputeFightTargets(agentData, combatData, extensions);
        }

    }
}
