using System.Numerics;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EIData.Mechanic.MechanicSeverity;
using static GW2EIEvtcParser.LogLogic.LogLogicPhaseUtils;
using static GW2EIEvtcParser.LogLogic.LogLogicUtils;
using static GW2EIEvtcParser.MechanicIDs;
using static GW2EIEvtcParser.ParserHelpers.LogImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.LogLogic;

internal class MursaatOverseer : BastionOfThePenitent
{
    internal readonly MechanicGroup Mechanics = new([
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(JadeSoldierAura, Mech_JadeSoldierAura, new (Symbols.CircleOpen,Colors.Red), new("Jade", "Jade Soldier's Aura hit","Jade Aura"), Sev2),
                new PlayerDstHealthDamageHitMechanic(JadeSoldierExplosion, Mech_JadeSoldierExplosion, new (Symbols.Circle,Colors.Red), new("Jade Expl", "Jade Soldier's Death Explosion","Jade Explosion"), Sev0),
            ]),
            //new Mechanic(ClaimSAK, "Claim", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new (Symbols.Square,Colors.Yellow), new("Claim",0), //Buff remove only
            //new Mechanic(DispelSAK, "Dispel", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new (Symbols.Circle,Colors.Yellow), new("Dispel",0), //Buff remove only
            //new Mechanic(ProtectSAK, "Protect", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new (Symbols.Circle,Colors.Teal), new("Protect",0), //Buff remove only
            new PlayerDstBuffApplyMechanic(Invulnerability757, Mech_SAKProtected, new (Symbols.CircleOpen,Colors.Teal), new("Protect", "Protected by the Protect Shield","Protect Shield"), Sev0)
                .UsingChecker((ba, log) => ba.AppliedDuration == 1000),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(ProtectBuff, Mech_ProtectSAK, new (Symbols.Circle,Colors.Blue), new("Protect (SAK)", "Took protect","Protect (SAK)"), Sev0)
                    .UsingTimeClamper((time, log, encounterPhase) => Math.Max(encounterPhase.Start, time)),
                new PlayerDstBuffApplyMechanic(DispelBuff, Mech_DispelSAK, new (Symbols.Circle,Colors.Purple), new("Dispel (SAK)", "Took dispel","Dispel (SAK)"), Sev0)
                    .UsingTimeClamper((time, log, encounterPhase) => Math.Max(encounterPhase.Start, time)),
                new PlayerDstBuffApplyMechanic(ClaimBuff, Mech_ClaimSAK, new (Symbols.Circle,Colors.Yellow), new("Claim (SAK)", "Took claim","Claim (SAK)"), Sev0)
                    .UsingTimeClamper((time, log, encounterPhase) => Math.Max(encounterPhase.Start, time)),
            ]),
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(MursaatOverseersShield, Mech_MursaatOverseersShieldApply, new (Symbols.CircleOpen,Colors.Yellow), new("Shield", "Jade Soldier Shield","Soldier Shield"), Sev2),
                new PlayerSrcBuffRemoveFromMechanic(MursaatOverseersShield, Mech_MursaatOverseersShieldRemove, new (Symbols.SquareOpen,Colors.Yellow), new("Dispel", "Dispelled Jade Soldier Shield","Dispel"), Sev1),
                new EnemyDstBuffRemoveMechanic(MursaatOverseersShield, Mech_MursaatOverseersShieldLost, new (Symbols.CircleCrossOpen,Colors.Yellow), new("Shield.L", "Jade Soldier Shield Lost","Soldier Shield Lost"), Sev1),
            ]),
            //new Mechanic(EnemyTile, "Enemy Tile", ParseEnum.BossIDS.MursaatOverseer, new (Symbols.SquareOpen,Colors.Yellow), new("Floor","Enemy Tile damage", "Tile dmg",0) //Fixed damage (3500), not trackable
        ]);
    public MursaatOverseer(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "mo";
        Icon = EncounterIconMursaatOverseer;
        LogCategoryInformation.InSubCategoryOrder = 1;
        LogID |= 0x000002;
        ChestID = ChestID.RecreationRoomChest;
    }

    internal override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log, CombatReplayDecorationContainer arenaDecorations, CombatReplayMap? parentMap = null)
    {
        var crMap = new CombatReplayMap(
                        (889, 889),
                        (1360, 2711, 3911, 5248));
        AddArenaDecorationsPerEncounter(log, arenaDecorations, LogID, CombatReplayMursaatOverseer, crMap, parentMap);
        return crMap;
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.Jade,
            TargetID.MursaatOverseerSpikes,
            TargetID.MursaatOverseerClaimArea,
        ];
    }
    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(PunishementAura, PunishementAura),
            new EffectCastFinder(ProtectSAK, EffectGUIDs.MursaarOverseerProtectBubble),
        ];
    }

    internal static IReadOnlyList<SubPhasePhaseData> ComputePhases(ParsedEvtcLog log, SingleActor mursaatOverseer, EncounterPhaseData encounterPhase, bool requirePhases)
    {
        if (!requirePhases)
        {
            return [];
        }
        var phases = new List<SubPhasePhaseData>(4);
        phases.AddRange(GetPhasesByHealthPercent(log, mursaatOverseer, new List<double> { 75, 50, 25, 0 }, encounterPhase.Start, encounterPhase.End));
        foreach (var phase in phases)
        {
            phase.AddParentPhase(encounterPhase);
        }
        return phases;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.MursaatOverseer)) ?? throw new MissingKeyActorsException("Mursaat Overseer not found");
        phases[0].AddTarget(mainTarget, log);
        phases.AddRange(ComputePhases(log, mainTarget, (EncounterPhaseData)phases[0], requirePhases));
        return phases;
    }

    internal override IEnumerable<ErrorEvent> GetCustomWarningMessages(LogData logData, AgentData agentData, CombatData combatData, EvtcVersionEvent evtcVersion)
    {
        return base.GetCustomWarningMessages(logData, agentData, combatData, evtcVersion)
            .Concat(GetConfusionDamageMissingMessage(evtcVersion).ToEnumerable());
    }

    private static Vector3 ArenaCenter = new(2636.6294f, 3983.2795f, -4180.5854f);

    internal static void IdentifyMursaatCheckboards(ulong gw2Build, EvtcVersionEvent evtcVersion, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        var gadgetAnims = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.GadgetAnimation).Select(x => new GadgetAnimationEvent(x, agentData)).ToList();
        var gadgetAnimDict = gadgetAnims.GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());
        var positionsDict = combatData.Where(x => x.IsStateChange == ArcDPSEnums.StateChange.Position).Select(x => new PositionEvent(x, agentData)).GroupBy(x => x.Src).ToDictionary(x => x.Key, x => x.ToList());

        var areasToken = new Token("areas");
        var spikes = new HashSet<AgentItem>(gadgetAnims.Where(x => x.AnimationToken == areasToken).Select(x => x.Src).Where(x =>
        {
            if (positionsDict.TryGetValue(x, out var positions))
            {
                return positions.Any(x => (x.Point2D - ArenaCenter.XY()).LengthSquared() < 1890000);
            }
            return false;
        }));
        foreach (var spike in spikes)
        {
            spike.OverrideID(TargetID.MursaatOverseerSpikes, agentData);
        }

        var offToken = new Token("off");
        // Type check as it could conflict with spikes
        var claimAreas = new HashSet<AgentItem>(gadgetAnims.Where(x => x.AnimationToken == offToken && !x.Src.IsSpecies(TargetID.MursaatOverseerSpikes)).Select(x => x.Src).Where(x =>
        {
            if (positionsDict.TryGetValue(x, out var positions))
            {
                return positions.Any(x => (x.Point2D - ArenaCenter.XY()).LengthSquared() < 1890000);
            }
            return false;
        }));
        foreach (var claimArea in claimAreas)
        {
            claimArea.OverrideID(TargetID.MursaatOverseerClaimArea, agentData);
        }
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, LogData logData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        IdentifyMursaatCheckboards(gw2Build, evtcVersion, agentData, combatData, extensions);
        base.EIEvtcParse(gw2Build, evtcVersion, logData, agentData, combatData, extensions);
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeNPCCombatReplayActors(target, log, replay);
        }
        switch (target.ID)
        {
            case (int)TargetID.Jade:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log))
                {
                    switch (cast.SkillID)
                    {
                        case JadeSoldierExplosion:
                            long start = cast.Time;
                            long precast = 1350;
                            long duration = 100;
                            uint radius = 1200;
                            replay.Decorations.Add(new CircleDecoration(radius, (start, start + precast + duration), Colors.Red, 0.05, new AgentConnector(target)));
                            replay.Decorations.Add(new CircleDecoration(radius, (start + precast, start + precast + duration), Colors.Red, 0.25, new AgentConnector(target)));
                            break;
                        default:
                            break;
                    }
                }

                // Jade Scout Shield
                var shields = target.GetBuffStatus(log, MursaatOverseersShield).Where(x => x.Value > 0);
                foreach (var seg in shields)
                {
                    replay.Decorations.Add(new CircleDecoration(100, seg, Colors.Yellow, 0.3, new AgentConnector(target)));
                }
                break;
            case (int)TargetID.MursaatOverseerSpikes:
                var spikeAnims = log.CombatData.GetGadgetAnimationData(target.AgentItem);
                var spikePrepare = new Token("areas");
                var spikeUp = new Token("impacts");
                var spikeConnector = new AgentConnector(target);
                foreach (var spikeAnim in spikeAnims)
                {
                    if (spikeAnim.AnimationToken == spikePrepare)
                    {
                        var rectangle = new RectangleDecoration(630, 630, (spikeAnim.Time, spikeAnim.Next?.Time ?? spikeAnim.Time + 5000), Colors.Red, 0.2, spikeConnector).UsingFilled(true);
                        replay.Decorations.AddWithGrowing(rectangle, rectangle.Lifespan.end);
                    }
                    else if (spikeAnim.AnimationToken == spikeUp)
                    {
                        var rectangle = new RectangleDecoration(630, 630, (spikeAnim.Time, spikeAnim.Next?.Time ?? spikeAnim.Time + 1000), Colors.Red, 0.5, spikeConnector).UsingFilled(true);
                        replay.Decorations.Add(rectangle);
                    }
                }
                break;
            case (int)TargetID.MursaatOverseerClaimArea:
                var claimAnims = log.CombatData.GetGadgetAnimationData(target.AgentItem);
                if (claimAnims.Count > 0)
                {
                    var first = claimAnims[0];
                    var onToken = new Token("on");
                    var claimConnector = new AgentConnector(target);
                    if (first.AnimationToken != onToken && replay.Positions.Any(x => x.XYZ.X - ArenaCenter.X < 0))
                    {
                        // Tile was on claimed state, limit if to the tiles to the left of MO as all tiles get an "off" event at encounter end, regardless of initial state
                        var rectangle = new RectangleDecoration(630, 630, (target.FirstAware, first.Time), Colors.LightOrange, 0.15, claimConnector).UsingFilled(true);
                        replay.Decorations.Add(rectangle);
                    }
                    foreach (var claimAnim in claimAnims)
                    {
                        // Tile was claimed
                        if (claimAnim.AnimationToken == onToken)
                        {
                            var rectangle = new RectangleDecoration(630, 630, (claimAnim.Time, claimAnim.LoopEnd), Colors.LightOrange, 0.15, claimConnector).UsingFilled(true);
                            replay.Decorations.Add(rectangle);
                        }
                    }
                }
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputePlayerCombatReplayActors(player, log, replay);
        }

        (long start, long end) lifespan;

        // Claim - Overhead
        var claims = player.GetBuffStatus(log, ClaimBuff).Where(x => x.Value > 0);
        replay.Decorations.AddOverheadIcons(claims, player, ParserIcons.FixationPurpleOverhead);

        // Protect - Bubble
        if (log.CombatData.TryGetEffectEventsBySrcWithGUID(player.AgentItem, EffectGUIDs.MursaarOverseerProtectBubble, out var protects))
        {
            foreach (EffectEvent effect in protects)
            {
                lifespan = effect.ComputeLifespan(log, 5000);
                var circle = new CircleDecoration(180, lifespan, Colors.LightBlue, 0.1, new PositionConnector(effect.Position));
                replay.Decorations.AddWithBorder(circle, Colors.Blue, 0.2);
            }
        }

        // Dispel - Projectile
        var dispels = log.CombatData.GetMissileEventsBySrcBySkillID(player.AgentItem, DispelSAK);
        replay.Decorations.AddNonHomingMissiles(log, dispels, Colors.Yellow, 0.3, 25);
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log, CombatReplayDecorationContainer environmentDecorations)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log, environmentDecorations);
        }
    }
    internal override void SetInstanceBuffs(ParsedEvtcLog log, List<InstanceBuff> instanceBuffs)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.SetInstanceBuffs(log, instanceBuffs);
        }
    }
    internal override void ComputeAchievementEligibilityEvents(ParsedEvtcLog log, Player p, List<AchievementEligibilityEvent> achievementEligibilityEvents)
    {
        if (!log.LogData.IgnoreBaseCallsForCRAndInstanceBuffs)
        {
            base.ComputeAchievementEligibilityEvents(log, p, achievementEligibilityEvents);
        }
    }

    internal override LogData.Mode GetLogMode(CombatData combatData, AgentData agentData, LogData logData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.MursaatOverseer)) ?? throw new MissingKeyActorsException("Mursaat Overseer not found");
        return (target.GetHealth(combatData) > 25e6) ? LogData.Mode.CM : LogData.Mode.Normal;
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, AgentData agentData, SkillData skillData, Dictionary<long, List<AnimatedCastEvent>> animatedCastDataByID)
    {
        List<CastEvent> res = base.SpecialCastEventProcess(combatData, agentData, skillData, animatedCastDataByID);

        var claimApply = combatData.GetBuffApplyData(ClaimBuff).OfType<BuffApplyEvent>();
        var dispelApply = combatData.GetBuffApplyData(DispelBuff).OfType<BuffApplyEvent>();

        SkillItem claimSkill = skillData.Get(ClaimSAK);
        SkillItem dispelSkill = skillData.Get(DispelSAK);

        if (combatData.TryGetEffectEventsByGUID(EffectGUIDs.MursaarOverseerClaimMarker, out var claims))
        {
            skillData.NotAccurate.Add(ClaimSAK);
            foreach (EffectEvent effect in claims)
            {
                BuffApplyEvent? src = claimApply.LastOrDefault(x => x.Time <= effect.Time);
                if (src != null)
                {
                    res.Add(new InstantCastEvent(effect.Time, claimSkill, src.To));
                }
            }
        }

        if (combatData.TryGetEffectEventsByGUID(EffectGUIDs.MursaarOverseerDispelProjectile, out var dispels))
        {
            skillData.NotAccurate.Add(DispelSAK);
            foreach (EffectEvent effect in dispels)
            {
                BuffApplyEvent? src = dispelApply.LastOrDefault(x => x.Time <= effect.Time);
                if (src != null)
                {
                    res.Add(new InstantCastEvent(effect.Time, dispelSkill, src.To));
                }
            }
        }

        return res;
    }
}
