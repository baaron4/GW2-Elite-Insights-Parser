using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Escort : StrongholdOfTheFaithful
{
    private bool _hasPreEvent = false;
    public Escort(int triggerID) : base(triggerID)
    {
        MechanicList.Add(new MechanicGroup([          
            new PlayerDstSkillMechanic(DetonateMineEscort, new MechanicPlotlySetting(Symbols.CircleCross, Colors.Red), "Mine.H", "Hit by Mine Detonation", "Mine Detonation Hit", 150).UsingChecker((de, log) => de.CreditedFrom.IsSpecies(TargetID.Mine)),
            new PlayerDstSkillMechanic(GlennaBombHit, new MechanicPlotlySetting(Symbols.Hexagon, Colors.LightGrey), "Bomb.H", "Hit by Glenna's Bomb", "Glenna's Bomb Hit", 0),
            new PlayerDstHitMechanic(FireMortarEscortHit, new MechanicPlotlySetting(Symbols.Hourglass, Colors.DarkPurple), "Shrd.H", "Hit by Mortar Fire (Bloodstone Turrets)", "Mortar Fire Hit", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(RadiantAttunementPhantasm, new MechanicPlotlySetting(Symbols.Diamond, Colors.White), "Rad.A", "Radiant Attunement Application", "Radiant Attunement Application", 150),
                new PlayerDstBuffApplyMechanic(CrimsonAttunementPhantasm, new MechanicPlotlySetting(Symbols.Diamond, Colors.Red), "Crim.A", "Crimson Attunement Application", "Crimson Attunement Application", 150),
            ]),
            new PlayerSrcEffectMechanic(EffectGUIDs.EscortOverHere, new MechanicPlotlySetting(Symbols.Star, Colors.White), "OverHere.C", "Used Over Here! (Special Action Key)", "Over Here! Cast", 0),
            new EnemyDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.LightBlue), "Inv.A", "Invulnerability Applied", "Invulnerability Applied", 150),
            new EnemyCastStartMechanic(TeleportDisplacementField, new MechanicPlotlySetting(Symbols.Square, Colors.LightPurple), "Tel.C", "Teleport Cast", "Teleport Cast", 150),
        ])
        );
        ChestID = ChestID.SiegeChest;
        Extension = "escort";
        Icon = EncounterIconEscort;
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        GenericFallBackMethod = FallBackMethod.ChestGadget;
        EncounterID |= 0x000001;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayEscort,
                        (1080, 676),
                        (-6081.86, 13624.72, 8956.86, 23099.28));
    }

    internal override string GetLogicName(CombatData combatData, AgentData agentData)
    {
        return "Siege the Stronghold";
    }

    private IReadOnlyList<PhaseData> GetMcLeodPhases(SingleActor mcLeod, ParsedEvtcLog log)
    {
        var phases = new List<PhaseData>();
        //
        DeadEvent? mcLeodDeath = log.CombatData.GetDeadEvents(mcLeod.AgentItem).LastOrDefault();
        long mcLeodStart = Math.Max(mcLeod.FirstAware, log.FightData.FightStart);
        long mcLeodEnd = Math.Min(mcLeodDeath != null ? mcLeodDeath.Time : mcLeod.LastAware, log.FightData.FightEnd);
        var mainPhase = new PhaseData(mcLeodStart, mcLeodEnd)
        {
            Name = "McLeod The Silent"
        };
        mainPhase.AddTarget(mcLeod, log);
        phases.Add(mainPhase);
        //
        phases.AddRange(GetPhasesByInvul(log, Invulnerability757, mcLeod, true, true, mcLeodStart, mcLeodEnd));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(mainPhase);
            if (i % 2 == 0)
            {
                phase.Name = "McLeod Split " + (i) / 2;
                SingleActor? whiteMcLeod = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.RadiantMcLeod) && x.LastAware > phase.Start);
                SingleActor? redMcLeod = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CrimsonMcLeod) && x.LastAware > phase.Start);
                phase.AddTarget(whiteMcLeod, log);
                phase.AddTarget(redMcLeod, log);
                phase.OverrideTimes(log);
            }
            else
            {
                phase.Name = "McLeod Phase " + (i + 1) / 2;
                phase.AddTarget(mcLeod, log);
            }
        }
        //
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mcLeod = Targets.FirstOrDefault(x => x.ID == (int)TargetID.McLeodTheSilent) ?? throw new MissingKeyActorsException("McLeod not found");
        phases[0].AddTarget(mcLeod, log);
        if (!requirePhases)
        {
            return phases;
        }
        var wargs = Targets.Where(x => x.IsSpecies(TargetID.WargBloodhound));
        PhaseData? preEventPhase = null;
        if (_hasPreEvent)
        {
            var preEventWargs = wargs.Where(x => x.FirstAware <= mcLeod.LastAware);
            preEventPhase = new PhaseData(log.FightData.FightStart, mcLeod.FirstAware)
            {
                Name = "Escort",
            };
            preEventPhase.AddTargets(preEventWargs, log);
            preEventPhase.AddParentPhase(phases[0]);
            preEventPhase.AddTarget(Targets.FirstOrDefault(x => x.ID == (int)TargetID.DummyTarget), log);
            phases.Add(preEventPhase);
        }
        var mcLeodPhases = GetMcLeodPhases(mcLeod, log);
        foreach (var mcLeodPhase in mcLeodPhases)
        {
            mcLeodPhase.AddParentPhase(phases[0]);
        }
        phases.AddRange(mcLeodPhases);
        var mcLeodWargs = wargs.Where(x => x.FirstAware >= mcLeod.FirstAware && x.FirstAware <= mcLeod.LastAware);
        if (mcLeodWargs.Any())
        {
            var phase = new PhaseData(log.FightData.FightStart, log.FightData.FightEnd, "McLeod Wargs");
            phase.AddTargets(mcLeodWargs, log);
            phase.AddParentPhase(preEventPhase);
            phase.OverrideTimes(log);
            phases.Add(phase);
        }

        return phases;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.McLeodTheSilent, out var mcLeod))
        {
            throw new MissingKeyActorsException("McLeod not found");
        }
        FindChestGadget(ChestID, agentData, combatData, SiegeChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100));
        //
        var mineAgents = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 1494 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 100 && x.HitboxHeight == 300);
        foreach (AgentItem mine in mineAgents)
        {
            mine.OverrideID(TargetID.Mine, agentData);
            mine.OverrideType(AgentItem.AgentType.NPC, agentData);
        }
        var duplicateGlennaPosition = new Vector3(-4326.979f, 13687.298f, -5561.857f);
        foreach (var glenna in agentData.GetNPCsByID(TargetID.Glenna))
        {
            var positions = combatData.Where(x => x.IsStateChange == StateChange.Position && x.SrcMatchesAgent(glenna)).Take(5).Select(x => new PositionEvent(x, agentData).GetParametricPoint3D());
            if (positions.Any(x => (duplicateGlennaPosition.XY() - x.XYZ.XY()).LengthSquared() < 10))
            {
                glenna.OverrideID(IgnoredSpecies, agentData);
            }
        }
        // to keep the pre event as we need targets
        if (_hasPreEvent && !agentData.GetNPCsByID(TargetID.WargBloodhound).Any(x => x.FirstAware < mcLeod.FirstAware))
        {
            agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Escort", Spec.NPC, TargetID.DummyTarget, true);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        NumericallyRenameSpecies(Targets.Where(x => x.IsAnySpecies([TargetID.WargBloodhound, TargetID.CrimsonMcLeod, TargetID.RadiantMcLeod])));
        foreach (SingleActor target in Targets)
        {
            if (target.IsSpecies(TargetID.CrimsonMcLeod))
            {
                target.OverrideName("Crimson " + target.Character);
            }
            if (target.IsSpecies(TargetID.RadiantMcLeod))
            {
                target.OverrideName("Radiant " + target.Character);
            }
        }
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        if (!agentData.TryGetFirstAgentItem(TargetID.McLeodTheSilent, out var mcLeod))
        {
            throw new MissingKeyActorsException("McLeod not found");
        }
        long startToUse = GetGenericFightOffset(fightData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
        if (logStartNPCUpdate != null)
        {
            if (mcLeod.FirstAware - fightData.LogStart > MinimumInCombatDuration)
            {
                _hasPreEvent = true;
                // Is this reliable?
                /*CombatItem achievementTrackApply = combatData.Where(x => (x.SkillID == AchievementEligibilityMineControl || x.SkillID == AchievementEligibilityFastSiege) && x.IsBuffApply()).FirstOrDefault();
                if (achievementTrackApply != null)
                {
                    startToUse = achievementTrackApply.Time;
                }*/
            }
            else
            {
                startToUse = GetEnterCombatTime(fightData, agentData, combatData, logStartNPCUpdate.Time, (int)TargetID.McLeodTheSilent, logStartNPCUpdate.DstAgent);
            }
        }
        return startToUse;
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        if (_hasPreEvent)
        {
            if (!agentData.TryGetFirstAgentItem(TargetID.Glenna, out var glenna))
            {
                throw new MissingKeyActorsException("Glenna not found");
            }
            if (combatData.HasMovementData)
            {
                var glennaInitialPosition = new Vector2(9092.697f, 21477.2969f/*, -2946.81885f*/);
                if (!combatData.GetMovementData(glenna).Any(x => x is PositionEvent pe && pe.Time < glenna.FirstAware + MinimumInCombatDuration && (pe.GetPointXY() - glennaInitialPosition).Length() < 100))
                {
                    return FightData.EncounterStartStatus.Late;
                }
            }
            return FightData.EncounterStartStatus.Normal;
        }
        else if (combatData.GetLogNPCUpdateEvents().Any())
        {
            return FightData.EncounterStartStatus.NoPreEvent;
        }
        else
        {
            return FightData.EncounterStartStatus.Normal;
        }
    }
    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.McLeodTheSilent,
            TargetID.RadiantMcLeod,
            TargetID.CrimsonMcLeod,
            TargetID.WargBloodhound,
            TargetID.DummyTarget,
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return [
            TargetID.MushroomCharger,
            TargetID.MushroomKing,
            TargetID.MushroomSpikeThrower,
            TargetID.WhiteMantleBattleMage1Escort,
            TargetID.WhiteMantleBattleMage2Escort,
            TargetID.WhiteMantleBattleCultist1,
            TargetID.WhiteMantleBattleCultist2,
            TargetID.WhiteMantleBattleKnight1,
            TargetID.WhiteMantleBattleKnight2,
            TargetID.WhiteMantleBattleCleric1,
            TargetID.WhiteMantleBattleCleric2,
            TargetID.WhiteMantleBattleSeeker1,
            TargetID.WhiteMantleBattleSeeker2,
            TargetID.Mine,
        ];
    }

    internal override IReadOnlyList<TargetID>  GetFriendlyNPCIDs()
    {
        return
        [
            TargetID.Glenna
        ];
    }

    protected override void SetInstanceBuffs(ParsedEvtcLog log)
    {
        base.SetInstanceBuffs(log);

        if (log.FightData.Success)
        {
            if (log.CombatData.GetBuffData(AchievementEligibilityLoveIsBunny).Any()) { InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityLoveIsBunny)); }
            if (log.CombatData.GetBuffData(AchievementEligibilityFastSiege).Any()) { InstanceBuffs.MaybeAdd(GetOnPlayerCustomInstanceBuff(log, AchievementEligibilityFastSiege)); }
        }
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)TargetID.McLeodTheSilent:
                replay.AddHideByBuff(target, log, Invulnerability757);
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        // Attunements Overhead
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, CrimsonAttunementPhantasm, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.CrimsonAttunementOverhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, RadiantAttunementPhantasm, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.RadiantAttunementOverhead);
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new EffectCastFinder(OverHere, EffectGUIDs.EscortOverHere),
            new DamageCastFinder(TeleportDisplacementField, TeleportDisplacementField).UsingICD(50),
        ];
    }
}

