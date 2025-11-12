using System.Diagnostics.CodeAnalysis;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParserHelpers;
using Tracing;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.ParsedData;

public partial class CombatData
{
    public readonly bool HasMovementData;

    //private List<CombatItem> _healingData;
    //private List<CombatItem> _healingReceivedData;
    private readonly StatusEventsContainer _statusEvents = new();
    private readonly MetaEventsContainer _metaDataEvents = new();
    private readonly HashSet<long> _skillIDs;

    private readonly Dictionary<long, List<BuffEvent>> _buffData;
    private readonly Dictionary<AgentItem, List<BuffEvent>> _buffDataByDst;
    private readonly Dictionary<AgentItem, List<BuffEvent>> _buffDataBySrc;
    private Dictionary<long, Dictionary<AgentItem, List<BuffEvent>>> _buffDataByIDByDst;
    private Dictionary<long, Dictionary<uint, List<BuffEvent>>> _buffDataByInstanceID;

    private Dictionary<long, List<AbstractBuffApplyEvent>> _buffApplyData;
    private Dictionary<AgentItem, List<AbstractBuffApplyEvent>> _buffApplyDataByDst;
    private Dictionary<long, Dictionary<AgentItem, List<BuffApplyEvent>>> _buffApplyDataByIDBySrc;
    private Dictionary<long, Dictionary<AgentItem, List<AbstractBuffApplyEvent>>> _buffApplyDataByIDByDst;

    private Dictionary<long, List<BuffRemoveAllEvent>> _buffRemoveAllData;
    private Dictionary<long, Dictionary<AgentItem, List<BuffRemoveAllEvent>>> _buffRemoveAllDataByIDBySrc;
    private Dictionary<long, Dictionary<AgentItem, List<BuffRemoveAllEvent>>> _buffRemoveAllDataByIDByDst;
    private Dictionary<AgentItem, List<BuffRemoveAllEvent>> _buffRemoveAllDataBySrc;
    private Dictionary<AgentItem, List<BuffRemoveAllEvent>> _buffRemoveAllDataByDst;


    private Dictionary<long, List<BuffExtensionEvent>> _buffExtensionData;

    private readonly Dictionary<AgentItem, List<HealthDamageEvent>> _damageData;
    private readonly Dictionary<long, List<HealthDamageEvent>> _damageDataByID;
    private readonly Dictionary<AgentItem, List<HealthDamageEvent>> _damageTakenData;

    private readonly Dictionary<AgentItem, List<BreakbarRecoveryEvent>> _breakbarRecoveredData;
    private readonly Dictionary<long, List<BreakbarRecoveryEvent>> _breakbarRecoveredDataByID;

    private readonly Dictionary<AgentItem, List<BreakbarDamageEvent>> _breakbarDamageData;
    private readonly Dictionary<long, List<BreakbarDamageEvent>> _breakbarDamageDataByID;
    private readonly Dictionary<AgentItem, List<BreakbarDamageEvent>> _breakbarDamageTakenData;

    private readonly Dictionary<AgentItem, List<CrowdControlEvent>> _crowControlData;
    private readonly Dictionary<long, List<CrowdControlEvent>> _crowControlDataByID;
    private readonly Dictionary<AgentItem, List<CrowdControlEvent>> _crowControlTakenData;

    private readonly Dictionary<AgentItem, List<AnimatedCastEvent>> _animatedCastData;
    private readonly Dictionary<long, List<AnimatedCastEvent>> _animatedCastDataByID;

    private readonly Dictionary<AgentItem, List<InstantCastEvent>> _instantCastData;
    private readonly Dictionary<long, List<InstantCastEvent>> _instantCastDataByID;

    private readonly Dictionary<AgentItem, List<WeaponSwapEvent>> _weaponSwapData;

    private readonly List<RewardEvent> _rewardEvents = [];
    // EXTENSIONS
    public EXTHealingCombatData EXTHealingCombatData { get; internal set; }
    public EXTBarrierCombatData EXTBarrierCombatData { get; internal set; }
    public bool HasEXTHealing => EXTHealingCombatData != null;
    public bool HasEXTBarrier => EXTBarrierCombatData != null;

    internal readonly bool UseBuffInstanceSimulator = false;

    internal readonly bool HasStackIDs;

    public readonly bool HasBreakbarDamageData = false;
    public readonly bool HasCrowdControlData = false;
    public readonly bool HasEffectData = false;
    public readonly bool HasMarkerData = false;
    public readonly bool HasSpeciesAndSkillGUIDs = false;
    public readonly bool HasMissileData = false;

    private void EIBuffParse(IReadOnlyList<AgentItem> players, SkillData skillData, LogData logData, EvtcVersionEvent evtcVersion)
    {
        //TODO_PERF(Rennorb) @find average complexity
        var toAdd = new List<BuffEvent>(players.Count * 10);
        foreach (AgentItem p in players)
        {
            if (p.Spec == Spec.Weaver)
            {
                toAdd.AddRange(WeaverHelper.TransformWeaverAttunements(GetBuffDataByDst(p), _buffData, p, skillData));
            }
            if (p.Spec == Spec.Virtuoso)
            {
                toAdd.AddRange(VirtuosoHelper.TransformVirtuosoBladeStorage(GetBuffDataByDst(p), p, skillData, evtcVersion));
            }
            if (p.Spec == Spec.Scourge && TryGetEffectEventsBySrcWithGUIDs(p, [EffectGUIDs.ScourgeShadeSandSavant, EffectGUIDs.ScourgeShade], out var shades))
            {
                toAdd.AddRange(ScourgeHelper.AddShadeBuffsFromEffects(shades, logData, skillData, GetGW2BuildEvent(), evtcVersion));
            }
            if (p.BaseSpec == Spec.Elementalist && p.Spec != Spec.Weaver)
            {
                ElementalistHelper.RemoveDualBuffs(GetBuffDataByDst(p), _buffData, skillData);
            }
        }
        toAdd.AddRange(logData.Logic.SpecialBuffEventProcess(this, skillData));

        var buffIDsToSort = new HashSet<long>(toAdd.Count);
        var buffDstAgentsToSort = new HashSet<AgentItem>(toAdd.Count);
        var buffSrcAgentsToSort = new HashSet<AgentItem>(toAdd.Count);
        foreach (BuffEvent bf in toAdd)
        {
            //TODO_PERF(Rennorb) @find average complexity
            _buffDataByDst.AddToList(bf.To, bf, toAdd.Count / 4);
            buffDstAgentsToSort.Add(bf.To);
            if (bf is not BuffExtensionEvent)
            {
                _buffDataBySrc.AddToList(bf.By, bf, toAdd.Count / 4);
                buffSrcAgentsToSort.Add(bf.By);
            }

            //TODO_PERF(Rennorb) @find average complexity
            _buffData.AddToList(bf.BuffID, bf, toAdd.Count / 4);
            buffIDsToSort.Add(bf.BuffID);
        }

        foreach (long buffID in buffIDsToSort)
        {
            _buffData[buffID].SortByTime();
        }

        foreach (AgentItem a in buffDstAgentsToSort)
        {
            _buffDataByDst[a].SortByTime();
        }

        foreach (AgentItem a in buffSrcAgentsToSort)
        {
            _buffDataBySrc[a].SortByTime();
        }

        if (toAdd.Count != 0)
        {
            BuildBuffDependentContainers();
        }
    }

    private void EIDamageParse(SkillData skillData, AgentData agentData, LogData logData)
    {
        var toAdd = logData.Logic.SpecialDamageEventProcess(this, agentData, skillData);

        var idsToSort = new HashSet<long>(toAdd.Count);
        var dstToSort = new HashSet<AgentItem>(toAdd.Count);
        var srcToSort = new HashSet<AgentItem>(toAdd.Count);
        foreach (HealthDamageEvent de in toAdd)
        {
            //TODO_PERF(Rennorb) @find average complexity
            _damageTakenData.AddToList(de.To, de, toAdd.Count / 4);
            dstToSort.Add(de.To);

            //TODO_PERF(Rennorb) @find average complexity
            _damageData.AddToList(de.From, de, toAdd.Count / 4);
            srcToSort.Add(de.From);

            //TODO_PERF(Rennorb) @find average complexity
            _damageDataByID.AddToList(de.SkillID, de);
            idsToSort.Add(de.SkillID);
        }

        foreach (long buffID in idsToSort)
        {
            _damageDataByID[buffID].SortByTime();
        }

        foreach (AgentItem a in dstToSort)
        {
            _damageTakenData[a].SortByTime();
        }

        foreach (AgentItem a in srcToSort)
        {
            _damageData[a].SortByTime();
        }
    }

    private List<InstantCastEvent> ComputeInstantCastEventsFromFinders(AgentData agentData, SkillData skillData, HashSet<InstantCastFinder> instantCastFinders)
    {
        //TODO_PERF(Rennorb) @find average complexity
        var res = new List<InstantCastEvent>();
        foreach (InstantCastFinder icf in instantCastFinders)
        {
            if (icf.Available(this))
            {
                if (icf.NotAccurate)
                {
                    skillData.NotAccurate.Add(icf.SkillID);
                }
                switch (icf.CastOrigin)
                {
                    case InstantCastFinder.InstantCastOrigin.Trait:
                        skillData.TraitProc.Add(icf.SkillID);
                        break;
                    case InstantCastFinder.InstantCastOrigin.Gear:
                        skillData.GearProc.Add(icf.SkillID);
                        break;
                    case InstantCastFinder.InstantCastOrigin.Unconditional:
                        skillData.UnconditionalProc.Add(icf.SkillID);
                        break;
                    case InstantCastFinder.InstantCastOrigin.Skill:
                    default:
                        break;
                }
                res.AddRange(icf.ComputeInstantCast(this, skillData, agentData));
            }
        }
        return res;
    }

    private void EICastParse(IReadOnlyList<AgentItem> players, SkillData skillData, LogData logData, AgentData agentData)
    {
        List<CastEvent> toAdd = logData.Logic.SpecialCastEventProcess(this, agentData, skillData);
        ulong gw2Build = GetGW2BuildEvent().Build;
        // Redirections
        {
            ConduitHelper.RedirectGladiatorsDefenseCastEvents(this, skillData, _animatedCastDataByID);
        }

        foreach (AgentItem p in players)
        {
            switch (p.Spec)
            {
                case Spec.Willbender:
                    toAdd.AddRange(ProfHelper.ComputeEndWithBuffApplyCastEvents(p, this, skillData, FlowingResolveSkill, 440, 500, FlowingResolveBuff));
                    break;
                default:
                    break;
            }
            switch (p.BaseSpec)
            {
                case Spec.Necromancer:
                    if (gw2Build < GW2Builds.March2024BalanceAndCerusLegendary)
                    {
                        toAdd.AddRange(ProfHelper.ComputeEndWithBuffApplyCastEvents(p, this, skillData, PathOfGluttony, 750, 750, PathOfGluttonyFlipBuff));
                    }
                    break;
                case Spec.Ranger:
                    toAdd.AddRange(ProfHelper.ComputeUnderBuffCastEvents(p, this, skillData, AncestralGraceSkill, AncestralGraceBuff));
                    break;
                case Spec.Elementalist:
                    toAdd.AddRange(ProfHelper.ComputeEffectCastEvents(p, this, skillData, Updraft, EffectGUIDs.ElementalistUpdraft2, 0, 1000));
                    toAdd.AddRange(ProfHelper.ComputeUnderBuffCastEvents(p, this, skillData, RideTheLightningSkill, RideTheLightningBuff));
                    break;
                case Spec.Engineer:
                    // Collides with Detonate Supply Crate Turrets
                    //toAdd.AddRange(ProfHelper.ComputeEffectCastEvents(p, this, skillData, Devastator, EffectGUIDs.EngineerSpearDevastator1, -1000, 1000));
                    toAdd.AddRange(ProfHelper.ComputeUnderBuffCastEvents(p, this, skillData, ConduitSurge, ConduitSurgeBuff));
                    break;
                case Spec.Revenant:
                    toAdd.AddRange(ProfHelper.ComputeEffectCastEvents(p, this, skillData, AbyssalBlitz, EffectGUIDs.RevenantSpearAbyssalBlitz1, 0, 3000, 
                        (abyssalBlitz, effect, combatData, skllData) =>
                        {
                            return !abyssalBlitz.Where(x => x.Time < effect.Time && Math.Abs(x.Time - effect.Time) < 300).Any();
                        }));
                    break;
                default:
                    break;
            }
        }
        // Generic instant cast finders
        var instantCastsFinder = new HashSet<InstantCastFinder>(ProfHelper.GetProfessionInstantCastFinders(players));
        foreach(var x in logData.Logic.GetInstantCastFinders()) { instantCastsFinder.Add(x); }
        toAdd.AddRange(ComputeInstantCastEventsFromFinders(agentData, skillData, instantCastsFinder));


        var castIDsToSort       = new HashSet<long>(toAdd.Count / 3);
        var castAgentsToSort    = new HashSet<AgentItem>(toAdd.Count / 3);
        var wepSwapAgentsToSort = new HashSet<AgentItem>(toAdd.Count / 3);
        var instantAgentsToSort = new HashSet<AgentItem>(toAdd.Count / 3);
        var instantIDsToSort    = new HashSet<long>(toAdd.Count / 3);
        foreach (CastEvent cast in toAdd)
        {
            if (cast is AnimatedCastEvent ace)
            {
                //TODO_PERF(Rennorb) @find average complexity
                _animatedCastData.AddToList(ace.Caster, ace, toAdd.Count / (players.Count + 2));
                castAgentsToSort.Add(ace.Caster);

                //TODO_PERF(Rennorb) @find average complexity
                _animatedCastDataByID.AddToList(ace.SkillID, ace, 10);
                castIDsToSort.Add(ace.SkillID);
            }

            if (cast is WeaponSwapEvent wse)
            {
                //TODO_PERF(Rennorb) @find average complexity
                _weaponSwapData.AddToList(wse.Caster, wse, toAdd.Count / (players.Count + 2));
                wepSwapAgentsToSort.Add(wse.Caster);
            }

            if (cast is InstantCastEvent ice)
            {
                //TODO_PERF(Rennorb) @find average complexity
                _instantCastData.AddToList(ice.Caster, ice, toAdd.Count / (players.Count + 2));
                instantAgentsToSort.Add(ice.Caster);

                //TODO_PERF(Rennorb) @find average complexity
                _instantCastDataByID.AddToList(ice.SkillID, ice, 10);
                instantIDsToSort.Add(ice.SkillID);
            }
        }

        foreach (long castID in castIDsToSort)
        {
            _animatedCastDataByID[castID].SortByTime();
        }

        foreach (AgentItem a in castAgentsToSort)
        {
            _animatedCastData[a].SortByTime();
        }

        foreach (AgentItem a in wepSwapAgentsToSort)
        {
            _weaponSwapData[a].SortByTime();
        }

        foreach (AgentItem a in instantAgentsToSort)
        {
            _instantCastData[a].SortByTime();
        }

        foreach (long instantID in instantIDsToSort)
        {
            _instantCastDataByID[instantID].SortByTime();
        }
    }

    private void EIMetaAndStatusParse(LogData logData, AgentData agentData, EvtcVersionEvent evtcVersion)
    {
        foreach (var (agent, events) in _damageTakenData)
        {
            if (agent.IsNonIdentifiedSpecies())
            {
                continue;
            }
            
            bool setDeads = false;
            if (!_statusEvents.DeadEvents.TryGetValue(agent, out var agentDeaths))
            {
                agentDeaths = [];
            }

            bool setDowns = false;
            if (!_statusEvents.DownEvents.TryGetValue(agent, out var agentDowns))
            {
                agentDowns = [];
            }

            foreach (HealthDamageEvent evt in events)
            {
                if (evt.HasKilled)
                {
                    if (!agentDeaths.Exists(x => Math.Abs(x.Time - evt.Time) < 500))
                    {
                        setDeads = true;
                        agentDeaths.Add(new DeadEvent(agent, evt.Time));
                    }
                }
                if (evt.HasDowned)
                {
                    if (!agentDowns.Exists(x => Math.Abs(x.Time - evt.Time) < 500))
                    {
                        setDowns = true;
                        agentDowns.Add(new DownEvent(agent, evt.Time));
                    }
                }
            }

            if (setDeads)
            {
                agentDeaths.SortByTime();
                _statusEvents.DeadEvents[agent] = agentDeaths;
            }

            if (setDowns)
            {
                agentDowns.SortByTime();
                _statusEvents.DownEvents[agent] = agentDowns;
            }
        }
        _metaDataEvents.ErrorEvents.AddRange(logData.Logic.GetCustomWarningMessages(logData, agentData, this, evtcVersion));
    }

    private void EIExtraEventProcess(SkillData skillData, AgentData agentData, LogData logData, ParserController operation, EvtcVersionEvent evtcVersion)
    {
        using var _t = new AutoTrace("Process Extra Events");

        // Add missing breakbar active state
        foreach (var pair in _statusEvents.BreakbarStateEvents)
        {
            var first = pair.Value.FirstOrDefault();
            if (first != null && first.State != BreakbarState.Active && first.Time > pair.Key.FirstAware + 500)
            {
                pair.Value.Insert(0, new BreakbarStateEvent(pair.Key, pair.Key.FirstAware, BreakbarState.Active));
            }
        }
        var players = agentData.GetAgentByType(AgentItem.AgentType.Player).ToList();
        players.AddRange(agentData.GetAgentByType(AgentItem.AgentType.NonSquadPlayer));
        // master attachements
        operation.UpdateProgressWithCancellationCheck("Parsing: Processing Warrior Gadgets");
        WarriorHelper.ProcessGadgets(players, this);
        operation.UpdateProgressWithCancellationCheck("Parsing: Processing Engineer Gadgets");
        EngineerHelper.ProcessGadgets(players, this);
        operation.UpdateProgressWithCancellationCheck("Parsing: Processing Ranger Gadgets");
        RangerHelper.ProcessGadgets(players, this);
        operation.UpdateProgressWithCancellationCheck("Parsing: Processing Revenant Gadgets");
        RevenantHelper.ProcessGadgets(players, this, agentData);
        operation.UpdateProgressWithCancellationCheck("Parsing: Processing Antiquary Gadgets");
        AntiquaryHelper.ProcessGadgets(players, this, agentData);
        operation.UpdateProgressWithCancellationCheck("Parsing: Processing Racial Gadget");
        ProfHelper.ProcessRacialGadgets(players, this);
        // Custom events
        operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Buff Events");
        EIBuffParse(players, skillData, logData, evtcVersion);
        operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Damage Events");
        EIDamageParse(skillData, agentData, logData);
        operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Cast Events");
        EICastParse(players, skillData, logData, agentData);
        operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Status Events");
        EIMetaAndStatusParse(logData, agentData, evtcVersion);
    }

    private void OffsetBuffExtensionEvents(EvtcVersionEvent evtcVersion)
    {
        using var _t = new AutoTrace("Buff Extension");
        if (evtcVersion.Build <= ArcDPSBuilds.BuffExtensionBroken)
        {
            return;
        }

        foreach (var events in _buffDataByDst.Values)
        {
            var dictExtensions = events.OfType<BuffExtensionEvent>()
                .Where(x => x.BuffInstance != 0)
                .GroupBy(x => x.BuffInstance);
            if (dictExtensions.Any())
            {
                var dictApply = events.OfType<BuffApplyEvent>()
                    .Where(x => x.BuffInstance != 0)
                    .GroupBy(x => x.BuffInstance)
                    .ToDictionary(x => x.Key, x => x.GroupBy(y => y.BuffID).ToDictionary(y => y.Key, y => y.ToList()));
                var dictStacks = events.OfType<BuffStackEvent>()
                    .Where(x => x.BuffInstance != 0)
                    .GroupBy(x => x.BuffInstance)
                    .ToDictionary(x => x.Key, x => x.GroupBy(y => y.BuffID).ToDictionary(y => y.Key, y => y.ToList()));

                foreach (var extensionEventsPerID in dictExtensions)
                {
                    if (!dictApply.TryGetValue(extensionEventsPerID.Key, out var appliesPerBuffID)) { continue; }

                    foreach (var extensionEvents in extensionEventsPerID.GroupBy(y => y.BuffID))
                    {
                        if (!appliesPerBuffID.TryGetValue(extensionEvents.Key, out var applies)) { continue; }

                        BuffExtensionEvent? previousExtension = null;
                        foreach (BuffExtensionEvent extensionEvent in extensionEvents)
                        {
                            BuffApplyEvent? initialStackApplication = applies.LastOrDefault(x => x.Time <= extensionEvent.Time);
                            if (initialStackApplication == null) { continue; }

                            var sequence = new List<BuffEvent>(2) { initialStackApplication };
                            if (dictStacks.TryGetValue(extensionEvent.BuffInstance, out var stacksPerBuffID))
                            {
                                if (stacksPerBuffID.TryGetValue(extensionEvent.BuffID, out var stacks))
                                {
                                    sequence.AddRange(stacks.Where(x => x.Time >= initialStackApplication.Time && x.Time <= extensionEvent.Time));
                                }
                            }

                            if (previousExtension != null && previousExtension.Time >= initialStackApplication.Time)
                            {
                                sequence.Add(previousExtension);
                            }

                            previousExtension = extensionEvent;
                            sequence.SortByTime();
                            extensionEvent.OffsetNewDuration(sequence, evtcVersion);
                        }
                    }
                }
                events.RemoveAll(x => x is BuffExtensionEvent bee && bee.ExtendedDuration < 1);
            }
        }
    }

    internal CombatData(IReadOnlyList<CombatItem> allCombatItems, LogData logData, AgentData agentData, SkillData skillData, IReadOnlyList<Player> players, ParserController operation, IReadOnlyDictionary<uint, ExtensionHandler> extensions, EvtcVersionEvent evtcVersion, EvtcParserSettings settings)
    {
        using var _t = new AutoTrace("CombatData");
        _metaDataEvents.EvtcVersionEvent = evtcVersion;

        var combatEvents = allCombatItems.ToList();
        combatEvents.SortByTime();

        //TODO_PERF(Rennorb): find average complexity
        _skillIDs = new HashSet<long>(combatEvents.Count / 2);
        var castCombatEvents = new Dictionary<ulong, List<CombatItem>>(combatEvents.Count / 5);
        var buffEvents = new List<BuffEvent>(combatEvents.Count / 2);
        var wepSwaps = new List<WeaponSwapEvent>(combatEvents.Count / 50);
        var brkDamageData = new List<BreakbarDamageEvent>(combatEvents.Count / 25);
        var brkRecoveredData = new List<BreakbarRecoveryEvent>(combatEvents.Count / 25);
        var crowdControlData = new List<CrowdControlEvent>(combatEvents.Count / 10);
        var damageData = new List<HealthDamageEvent>(combatEvents.Count / 2);

        operation.UpdateProgressWithCancellationCheck("Parsing: Creating EI Combat Data");
        // First iteration to create necessary metadata events first
        foreach (CombatItem combatItem in combatEvents)
        {
            if (combatItem.IsEssentialMetadata)
            {
                CombatEventFactory.AddStateChangeEvent(logData.EvtcLogOffset, combatItem, agentData, skillData, _metaDataEvents, _statusEvents, _rewardEvents, wepSwaps, buffEvents, evtcVersion, settings);
            }
        }
        foreach (CombatItem combatItem in combatEvents)
        {
            bool insertToSkillIDs = false;
            if (combatItem.IsStateChange != StateChange.None)
            {
                if (combatItem.IsEssentialMetadata)
                {
                    continue;
                }
                if (combatItem.IsExtension)
                {
                    if (extensions.TryGetValue(combatItem.Pad, out var handler))
                    {
                        insertToSkillIDs = handler.IsSkillID(combatItem);
                        handler.InsertEIExtensionEvent(combatItem, agentData, skillData);
                    }
                }
                else
                {
                    insertToSkillIDs = combatItem.IsStateChange == StateChange.BuffInitial;
                    CombatEventFactory.AddStateChangeEvent(logData.EvtcLogOffset, combatItem, agentData, skillData, _metaDataEvents, _statusEvents, _rewardEvents, wepSwaps, buffEvents, evtcVersion, settings);
                }

            }
            else if (combatItem.IsActivation != Activation.None)
            {
                insertToSkillIDs = true;
                castCombatEvents.AddToList(combatItem.SrcAgent, combatItem);
            }
            else if (combatItem.IsBuffRemove != BuffRemove.None)
            {
                insertToSkillIDs = true;
                CombatEventFactory.AddBuffRemoveEvent(combatItem, buffEvents, agentData, skillData);
            }
            else
            {
                insertToSkillIDs = true;
                if (combatItem.IsBuff != 0 && combatItem.BuffDmg == 0 && combatItem.Value > 0)
                {
                    CombatEventFactory.AddBuffApplyEvent(combatItem, buffEvents, agentData, skillData, evtcVersion);
                }
                else if (combatItem.IsBuff == 0)
                {
                    CombatEventFactory.AddDirectDamageEvent(combatItem, damageData, brkDamageData, brkRecoveredData, crowdControlData, agentData, skillData);
                }
                else if (combatItem.IsBuff != 0 && combatItem.Value == 0)
                {
                    CombatEventFactory.AddIndirectDamageEvent(combatItem, damageData, agentData, skillData);
                }
            }

            if (insertToSkillIDs)
            {
                _skillIDs.Add(combatItem.SkillID);
            }
        }

        HasStackIDs = evtcVersion.Build > ArcDPSBuilds.ProperConfusionDamageSimulation && buffEvents.Any(x => x is BuffStackActiveEvent || x is BuffStackResetEvent);
        UseBuffInstanceSimulator = false;// evtcVersion.Build > ArcDPSBuilds.RemovedDurationForInfiniteDurationStacksChanged && HasStackIDs && (logData.Logic.ParseMode == EncounterLogic.FightLogic.ParseModeEnum.Instanced10 || logData.Logic.ParseMode == EncounterLogic.FightLogic.ParseModeEnum.Instanced5 || logData.Logic.ParseMode == EncounterLogic.FightLogic.ParseModeEnum.Benchmark);
        HasMovementData = _statusEvents.MovementEvents.Count > 1;
        HasBreakbarDamageData = brkDamageData.Count != 0 || brkRecoveredData.Count != 0;
        HasEffectData = _statusEvents.EffectEvents.Count != 0;
        HasMarkerData = _statusEvents.MarkerEvents.Count != 0;
        HasCrowdControlData = crowdControlData.Count != 0;
        HasSpeciesAndSkillGUIDs = evtcVersion.Build >= ArcDPSBuilds.SpeciesSkillGUIDs;
        HasMissileData = _statusEvents.MissileEvents.Count != 0;

        operation.UpdateProgressWithCancellationCheck("Parsing: Combining SkillInfo with SkillData");
        skillData.CombineWithSkillInfo(_metaDataEvents.SkillInfoEvents);
        
        operation.UpdateProgressWithCancellationCheck("Parsing: Creating Cast Events");
        List<AnimatedCastEvent> animatedCastData = CombatEventFactory.CreateCastEvents(castCombatEvents, agentData, skillData, logData);
        _weaponSwapData = wepSwaps.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
        _animatedCastData = animatedCastData.GroupBy(x => x.Caster).ToDictionary(x => x.Key, x => x.ToList());
        //TODO_PERF(Rennorb)
        _instantCastData = [];
        _instantCastDataByID = [];
        _animatedCastDataByID = animatedCastData.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
        
        operation.UpdateProgressWithCancellationCheck("Parsing: Creating Buff Events");
        _buffDataByDst = buffEvents.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        _buffDataBySrc = buffEvents.Where(x => !(x is BuffExtensionEvent)).GroupBy(x => x.By).ToDictionary(x => x.Key, x => x.ToList());
        _buffData = buffEvents.GroupBy(x => x.BuffID).ToDictionary(x => x.Key, x => x.ToList());
        OffsetBuffExtensionEvents(evtcVersion);
        // damage events
        operation.UpdateProgressWithCancellationCheck("Parsing: Creating Damage Events");
        _damageData = damageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        _damageTakenData = damageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        _damageDataByID = damageData.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
        _breakbarRecoveredData = brkRecoveredData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        _breakbarRecoveredDataByID = brkRecoveredData.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
        _breakbarDamageData = brkDamageData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        _breakbarDamageDataByID = brkDamageData.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
        _breakbarDamageTakenData = brkDamageData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        _crowControlData = crowdControlData.GroupBy(x => x.From).ToDictionary(x => x.Key, x => x.ToList());
        _crowControlDataByID = crowdControlData.GroupBy(x => x.SkillID).ToDictionary(x => x.Key, x => x.ToList());
        _crowControlTakenData = crowdControlData.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
        // buff depend events
        operation.UpdateProgressWithCancellationCheck("Parsing: Creating Buff Dependent Events");
        BuildBuffDependentContainers();
        
        operation.UpdateProgressWithCancellationCheck("Parsing: Attaching Extension Events");
        foreach (ExtensionHandler handler in extensions.Values)
        {
            handler.AttachToCombatData(this, operation, GetGW2BuildEvent().Build);
        }
        operation.UpdateProgressWithCancellationCheck("Parsing: Adjusting player specs and groups based on Enter Combat events");
        logData.Logic.UpdatePlayersSpecAndGroup(players, this, logData);
        
        operation.UpdateProgressWithCancellationCheck("Parsing: Creating Custom Events");
        EIExtraEventProcess(skillData, agentData, logData, operation, evtcVersion);

#if DEBUG
        foreach (var effectGUID in _metaDataEvents.EffectGUIDEventsByGUID.Keys)
        {
            if (!TryGetEffectEventsByGUID(effectGUID, out var effectEvents))
            {
                operation.UpdateProgressWithCancellationCheck("Parsing: Found orphan GUID: " + effectGUID.ToHex());
            }
        }
#endif
    }

    internal void TryFindSrc(ParsedEvtcLog log)
    {
        foreach (var pair in _buffExtensionData)
        {
            pair.Value.ForEach(x => x.TryFindSrc(log));
        }
    }

    private void BuildBuffDependentContainers()
    {
        _buffRemoveAllData = _buffData.ToDictionary(x => x.Key, x => x.Value.OfType<BuffRemoveAllEvent>().ToList());
        _buffRemoveAllDataByIDBySrc = _buffData.ToDictionary(
            x => x.Key, 
            x => x.Value.OfType<BuffRemoveAllEvent>()
                .GroupBy(y => y.CreditedBy)
                .ToDictionary(y => y.Key, y => y.ToList())
        );
        _buffRemoveAllDataByIDByDst = _buffData.ToDictionary(
            x => x.Key,
            x => x.Value.OfType<BuffRemoveAllEvent>()
                .GroupBy(y => y.To)
                .ToDictionary(y => y.Key, y => y.ToList())
        );
        _buffDataByIDByDst = _buffData.ToDictionary(x => x.Key, x => x.Value.GroupBy(y => y.To).ToDictionary(y => y.Key, y => y.ToList()));
        _buffApplyData = _buffData.ToDictionary(x => x.Key, x => x.Value.OfType<AbstractBuffApplyEvent>().ToList());
        _buffApplyDataByDst = _buffDataByDst.ToDictionary(x => x.Key, x => x.Value.OfType<AbstractBuffApplyEvent>().ToList());
        _buffRemoveAllDataByDst = _buffDataByDst.ToDictionary(x => x.Key, x => x.Value.OfType<BuffRemoveAllEvent>().ToList());
        _buffApplyDataByIDBySrc = _buffData.ToDictionary(
            x => x.Key,
            x => x.Value.OfType<BuffApplyEvent>()
                .GroupBy(y => y.By)
                .ToDictionary(y => y.Key, y => y.ToList())
        );
        _buffApplyDataByIDByDst = _buffData.ToDictionary(
            x => x.Key, 
            x => x.Value.OfType<AbstractBuffApplyEvent>()
                .GroupBy(y => y.To)
                .ToDictionary(y => y.Key, y => y.ToList())
        );
        _buffRemoveAllDataBySrc = _buffDataBySrc.ToDictionary(x => x.Key, x => x.Value.OfType<BuffRemoveAllEvent>().ToList());
        _buffExtensionData = _buffData.ToDictionary(x => x.Key, x => x.Value.OfType<BuffExtensionEvent>().ToList());
        //TODO_PERF(Rennorb) @find average complexity
        _buffDataByInstanceID = new(_buffData.Count / 10);
        foreach (var buffEvents in _buffData.Values)
        {
            foreach (BuffEvent abe in buffEvents)
            {
                if (!_buffDataByInstanceID.TryGetValue(abe.BuffID, out var dict))
                {
                    //TODO_PERF(Rennorb) @find average complexity
                    dict = new(10);
                    _buffDataByInstanceID[abe.BuffID] = dict;
                }

                uint buffInstance = (abe) switch {
                    AbstractBuffApplyEvent abae => abae.BuffInstance,
                    BuffStackEvent abse => abse.BuffInstance,
                    BuffRemoveSingleEvent brse => brse.BuffInstance,
                    _ => 0,
                };

                if (buffInstance != 0)
                {
                    dict.AddToList(buffInstance, abe);
                }
            }
        }
    }

}
