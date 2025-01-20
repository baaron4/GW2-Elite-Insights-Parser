using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Escort : StrongholdOfTheFaithful
{
    private static readonly Vector3 SiegeChestPosition = new(-3815.47f, 16688.5f, -5322.35f);
    private bool _hasPreEvent = false;
    public Escort(int triggerID) : base(triggerID)
    {
        MechanicList.AddRange(new List<Mechanic>
        {
            new PlayerDstSkillMechanic(DetonateMineEscort, "Detonate", new MechanicPlotlySetting(Symbols.CircleCross, Colors.Red), "Mine.H", "Hit by Mine Detonation", "Mine Detonation Hit", 150).UsingChecker((de, log) => de.CreditedFrom.IsSpecies(ArcDPSEnums.TrashID.Mine)),
            new PlayerDstSkillMechanic(GlennaBombHit, "Bomb Explosion", new MechanicPlotlySetting(Symbols.Hexagon, Colors.LightGrey), "Bomb.H", "Hit by Glenna's Bomb", "Glenna's Bomb Hit", 0),
            new PlayerDstHitMechanic(FireMortarEscortHit, "Fire Mortar", new MechanicPlotlySetting(Symbols.Hourglass, Colors.DarkPurple), "Shrd.H", "Hit by Mortar Fire (Bloodstone Turrets)", "Mortar Fire Hit", 0),
            new PlayerDstBuffApplyMechanic(RadiantAttunementPhantasm, "Radiant Attunement", new MechanicPlotlySetting(Symbols.Diamond, Colors.White), "Rad.A", "Radiant Attunement Application", "Radiant Attunement Application", 150),
            new PlayerDstBuffApplyMechanic(CrimsonAttunementPhantasm, "Crimson Attunement", new MechanicPlotlySetting(Symbols.Diamond, Colors.Red), "Crim.A", "Crimson Attunement Application", "Crimson Attunement Application", 150),
            new PlayerSrcEffectMechanic(EffectGUIDs.EscortOverHere, "Over Here!", new MechanicPlotlySetting(Symbols.Star, Colors.White), "OverHere.C", "Used Over Here! (Special Action Key)", "Over Here! Cast", 0),
            new EnemyDstBuffApplyMechanic(Invulnerability757, "Invulnerability", new MechanicPlotlySetting(Symbols.DiamondOpen, Colors.LightBlue), "Inv.A", "Invulnerability Applied", "Invulnerability Applied", 150),
            new EnemyCastStartMechanic(TeleportDisplacementField, "Teleport Displacement Field", new MechanicPlotlySetting(Symbols.Square, Colors.LightPurple), "Tel.C", "Teleport Cast", "Teleport Cast", 150),
        }
        );
        ChestID = ArcDPSEnums.ChestID.SiegeChest;
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
        mainPhase.AddTarget(mcLeod);
        phases.Add(mainPhase);
        //
        phases.AddRange(GetPhasesByInvul(log, Invulnerability757, mcLeod, true, true, mcLeodStart, mcLeodEnd));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            if (i % 2 == 0)
            {
                phase.Name = "McLeod Split " + (i) / 2;
                SingleActor? whiteMcLeod = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TrashID.RadiantMcLeod) && x.LastAware > phase.Start);
                SingleActor? redMcLeod = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TrashID.CrimsonMcLeod) && x.LastAware > phase.Start);
                phase.AddTarget(whiteMcLeod);
                phase.AddTarget(redMcLeod);
                phase.OverrideTimes(log);
            }
            else
            {
                phase.Name = "McLeod Phase " + (i + 1) / 2;
                phase.AddTarget(mcLeod);
            }
        }
        //
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mcLeod = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.McLeodTheSilent) ?? throw new MissingKeyActorsException("McLeod not found");
        phases[0].AddTarget(mcLeod);
        if (!requirePhases)
        {
            return phases;
        }
        var wargs = Targets.Where(x => x.IsSpecies(ArcDPSEnums.TrashID.WargBloodhound));
        if (_hasPreEvent)
        {
            var preEventWargs = wargs.Where(x => x.FirstAware <= mcLeod.LastAware);
            var preEventPhase = new PhaseData(log.FightData.FightStart, mcLeod.FirstAware)
            {
                Name = "Escort",
            };
            preEventPhase.AddTargets(preEventWargs);
            preEventPhase.AddTarget(Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.DummyTarget));
            phases.Add(preEventPhase);
        }
        phases.AddRange(GetMcLeodPhases(mcLeod, log));
        var mcLeodWargs = wargs.Where(x => x.FirstAware >= mcLeod.FirstAware && x.FirstAware <= mcLeod.LastAware);
        if (mcLeodWargs.Any())
        {
            var phase = new PhaseData(log.FightData.FightStart, log.FightData.FightEnd)
            {
                Name = "McLeod Wargs",
                CanBeSubPhase = false,
            };
            phase.AddTargets(mcLeodWargs);
            phase.OverrideTimes(log);
            phases.Add(phase);
        }

        return phases;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        if (!agentData.TryGetFirstAgentItem(ArcDPSEnums.TargetID.McLeodTheSilent, out var mcLeod))
        {
            throw new MissingKeyActorsException("McLeod not found");
        }
        FindChestGadget(ChestID, agentData, combatData, SiegeChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100));
        //
        var mineAgents = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 1494 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 100 && x.HitboxHeight == 300);
        foreach (AgentItem mine in mineAgents)
        {
            mine.OverrideID(ArcDPSEnums.TrashID.Mine, agentData);
            mine.OverrideType(AgentItem.AgentType.NPC, agentData);
        }
        var duplicateGlennaPosition = new Vector3(-4326.979f, 13687.298f, -5561.857f);
        foreach (var glenna in agentData.GetNPCsByID(ArcDPSEnums.TrashID.Glenna))
        {
            var positions = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Position && x.SrcMatchesAgent(glenna)).Take(5).Select(x => new PositionEvent(x, agentData).GetParametricPoint3D());
            if (positions.Any(x => (duplicateGlennaPosition.XY() - x.XYZ.XY()).LengthSquared() < 10))
            {
                glenna.OverrideID(ArcDPSEnums.IgnoredSpecies, agentData);
            }
        }
        // to keep the pre event as we need targets
        if (_hasPreEvent && !agentData.GetNPCsByID(ArcDPSEnums.TrashID.WargBloodhound).Any(x => x.FirstAware < mcLeod.FirstAware))
        {
            agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Escort", Spec.NPC, ArcDPSEnums.TargetID.DummyTarget, true);
        }
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        int curCrimson = 1;
        int curRadiant = 1;
        int curWarg = 1;
        foreach (SingleActor target in Targets)
        {
            if (target.IsSpecies(ArcDPSEnums.TrashID.WargBloodhound))
            {
                target.OverrideName(target.Character + " " + curWarg++);
            }
            if (target.IsSpecies(ArcDPSEnums.TrashID.CrimsonMcLeod))
            {
                target.OverrideName("Crimson " + target.Character + " " + curCrimson++);
            }
            if (target.IsSpecies(ArcDPSEnums.TrashID.RadiantMcLeod))
            {
                target.OverrideName("Radiant " + target.Character + " " + curRadiant++);
            }
        }
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        if (!agentData.TryGetFirstAgentItem(ArcDPSEnums.TargetID.McLeodTheSilent, out var mcLeod))
        {
            throw new MissingKeyActorsException("McLeod not found");
        }
        long startToUse = GetGenericFightOffset(fightData);
        CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == ArcDPSEnums.StateChange.LogNPCUpdate);
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
                startToUse = GetEnterCombatTime(fightData, agentData, combatData, logStartNPCUpdate.Time, (int)ArcDPSEnums.TargetID.McLeodTheSilent, logStartNPCUpdate.DstAgent);
            }
        }
        return startToUse;
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        if (_hasPreEvent)
        {
            if (!agentData.TryGetFirstAgentItem(ArcDPSEnums.TrashID.Glenna, out var glenna))
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

    protected override ReadOnlySpan<int> GetUniqueNPCIDs()
    {
        return
        [
            (int)ArcDPSEnums.TargetID.McLeodTheSilent,
            (int)ArcDPSEnums.TrashID.Glenna
        ];
    }
    protected override ReadOnlySpan<int> GetTargetsIDs()
    {
        return
        [
            (int)ArcDPSEnums.TargetID.McLeodTheSilent,
            (int)ArcDPSEnums.TrashID.RadiantMcLeod,
            (int)ArcDPSEnums.TrashID.CrimsonMcLeod,
            (int)ArcDPSEnums.TrashID.WargBloodhound,
            (int)ArcDPSEnums.TargetID.DummyTarget,
        ];
    }

    protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
    {
        return [
            ArcDPSEnums.TrashID.MushroomCharger,
            ArcDPSEnums.TrashID.MushroomKing,
            ArcDPSEnums.TrashID.MushroomSpikeThrower,
            ArcDPSEnums.TrashID.WhiteMantleBattleMage1Escort,
            ArcDPSEnums.TrashID.WhiteMantleBattleMage2Escort,
            ArcDPSEnums.TrashID.WhiteMantleBattleCultist1,
            ArcDPSEnums.TrashID.WhiteMantleBattleCultist2,
            ArcDPSEnums.TrashID.WhiteMantleBattleKnight1,
            ArcDPSEnums.TrashID.WhiteMantleBattleKnight2,
            ArcDPSEnums.TrashID.WhiteMantleBattleCleric1,
            ArcDPSEnums.TrashID.WhiteMantleBattleCleric2,
            ArcDPSEnums.TrashID.WhiteMantleBattleSeeker1,
            ArcDPSEnums.TrashID.WhiteMantleBattleSeeker2,
            ArcDPSEnums.TrashID.Mine,
        ];
    }

    protected override ReadOnlySpan<int> GetFriendlyNPCIDs()
    {
        return
        [
            (int)ArcDPSEnums.TrashID.Glenna
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
            case (int)ArcDPSEnums.TargetID.McLeodTheSilent:
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
        ];
    }
}

