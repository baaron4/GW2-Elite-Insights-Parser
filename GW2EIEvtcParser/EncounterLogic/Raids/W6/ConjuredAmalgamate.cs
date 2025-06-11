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
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class ConjuredAmalgamate : MythwrightGambit
{
    private readonly bool _cn;
    public ConjuredAmalgamate(int triggerID) : base((int)TargetID.ConjuredAmalgamate)
    {
        MechanicList.Add(new MechanicGroup([
            new MechanicGroup([
                new PlayerDstHitMechanic(Pulverize, new MechanicPlotlySetting(Symbols.Square,Colors.LightOrange), "Arm Slam", "Pulverize (Arm Slam)","Arm Slam", 0)
                    .UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
                new PlayerDstHitMechanic(Pulverize, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightOrange), "Stab.Slam", "Pulverize (Arm Slam) while affected by stability","Stabilized Arm Slam", 0)
                    .UsingChecker((de, log) => de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(JunkAbsorption, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Purple), "Balls", "Junk Absorption (Purple Balls during collect)","Purple Balls", 0),
                new PlayerDstHitMechanic([JunkFall1, JunkFall2], new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Pink), "Junk", "Junk Fall (Falling Debris)","Junk Fall", 0),
                new PlayerDstHitMechanic(JunkTorrent, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Wall", "Junk Torrent (Moving Wall)","Junk Torrent (Wall)", 0)
                    .UsingChecker((de,log) => de.HealthDamage > 0),
            ]),
            new PlayerDstHitMechanic(RupturedGround, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Teal), "Ground", "Ruptured Ground (Relics after Junk Wall)","Ruptured Ground", 0).UsingChecker((de,log) => de.HealthDamage > 0),
            new PlayerDstHitMechanic(Tremor, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Tremor", "Tremor (Field adjacent to Arm Slam)","Near Arm Slam", 0).UsingChecker((de,log) => de.HealthDamage > 0),
            new MechanicGroup([
                new PlayerCastStartMechanic(ConjuredSlashSAK, new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Sword.Cst", "Conjured Slash (Special action sword)","Sword Cast", 0),
                new PlayerCastStartMechanic(ConjuredProtectionSAK, new MechanicPlotlySetting(Symbols.Square,Colors.Green), "Shield.Cst", "Conjured Protection (Special action shield)","Shield Cast", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(GreatswordPower, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "Sword.C", "Collected Sword","Sword Collect", 50),
                new PlayerDstBuffApplyMechanic(ConjuredShield, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Green), "Shield.C", "Collected Shield","Shield Collect", 50),
            ]),
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(AugmentedPower, new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Red), "Augmented Power", "Augmented Power","Augmented Power", 50),
                new EnemyDstBuffApplyMechanic(ShieldedCA, new MechanicPlotlySetting(Symbols.BowtieOpen,Colors.Green), "Shielded", "Shielded","Shielded", 50),
            ]),
        ]));
        _cn = triggerID != (int)TargetID.ConjuredAmalgamate;
        Extension = "ca";
        GenericFallBackMethod = FallBackMethod.ChestGadget;
        Icon = EncounterIconConjuredAmalgamate;
        EncounterCategoryInformation.InSubCategoryOrder = 0;
        EncounterID |= 0x000001;
        ChestID = ChestID.CAChest;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayConjuredAmalgamate,
                        (544, 1000),
                        (-5064, -15030, -2864, -10830)/*,
                        (-21504, -21504, 24576, 24576),
                        (13440, 14336, 15360, 16256)*/);
    }

    internal override long GetFightOffset(EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData)
    {
        // time starts at first smash
        var effectIDToGUIDs = combatData.Where(x => x.IsStateChange == StateChange.IDToGUID);
        if (effectIDToGUIDs.Any())
        {
            CombatItem? armSmashGUID = effectIDToGUIDs.FirstOrDefault(x => new GUID(x.SrcAgent, x.DstAgent) == EffectGUIDs.CAArmSmash);
            if (armSmashGUID != null)
            {
                CombatItem? firstArmSmash = combatData.FirstOrDefault(x => x.IsEffect && x.SkillID == armSmashGUID.SkillID);
                if (firstArmSmash != null)
                {
                    CombatItem? logStartNPCUpdate = combatData.FirstOrDefault(x => x.IsStateChange == StateChange.LogNPCUpdate);
                    if (logStartNPCUpdate != null)
                    {
                        // we couldn't have hit CA before the initial smash
                        return firstArmSmash.Time > GetPostLogStartNPCUpdateDamageEventTime(fightData, agentData, combatData, logStartNPCUpdate.Time, agentData.GetGadgetsByID(_cn ? TargetID.ConjuredAmalgamate_CHINA : TargetID.ConjuredAmalgamate).FirstOrDefault()) ? logStartNPCUpdate.Time : firstArmSmash.Time;
                    }
                    else
                    {
                        // Before new logging, log would start when everyone in combat + boss in combat or enters combat
                        // as such the first smash can only happen within the first few seconds of the start
                        return firstArmSmash.Time - fightData.LogStart > 6000 ? GetGenericFightOffset(fightData) : firstArmSmash.Time;
                    }
                }
            }
        }
        return GetGenericFightOffset(fightData);
    }

    internal override FightData.EncounterStartStatus GetEncounterStartStatus(CombatData combatData, AgentData agentData, FightData fightData)
    {
        // Can be improved
        if (TargetHPPercentUnderThreshold(TargetID.ConjuredAmalgamate, fightData.FightStart, combatData, Targets, 90))
        {
            return FightData.EncounterStartStatus.Late;
        }
        return FightData.EncounterStartStatus.Normal;
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.ConjuredAmalgamate,
            TargetID.CARightArm,
            TargetID.CALeftArm
        ];
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.ConjuredGreatsword,
            TargetID.ConjuredShield
        ];
    }

    internal override IReadOnlyList<TargetID>  GetFriendlyNPCIDs()
    {
        return
        [
            TargetID.ConjuredPlayerSword
        ];
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // make those into npcs
        IReadOnlyList<AgentItem> cas = agentData.GetGadgetsByID(_cn ? TargetID.ConjuredAmalgamate_CHINA : TargetID.ConjuredAmalgamate);
        if (!cas.Any())
        {
            throw new MissingKeyActorsException("Conjured Amalgamate not found");
        }
        IReadOnlyList<AgentItem> leftArms = agentData.GetGadgetsByID(_cn ? TargetID.CALeftArm_CHINA : TargetID.CALeftArm);
        IReadOnlyList<AgentItem> rightArms = agentData.GetGadgetsByID(_cn ? TargetID.CARightArm_CHINA : TargetID.CARightArm);
        foreach (AgentItem ca in cas)
        {
            ca.OverrideType(AgentItem.AgentType.NPC, agentData);
            ca.OverrideID(TargetID.ConjuredAmalgamate, agentData);
        }
        foreach (AgentItem leftArm in leftArms)
        {
            leftArm.OverrideType(AgentItem.AgentType.NPC, agentData);
            leftArm.OverrideID(TargetID.CALeftArm, agentData);
        }
        foreach (AgentItem rightArm in rightArms)
        {
            rightArm.OverrideType(AgentItem.AgentType.NPC, agentData);
            rightArm.OverrideID(TargetID.CARightArm, agentData);
        }
        FindChestGadget(ChestID, agentData, combatData, CAChestPosition, (agentItem) => agentItem.HitboxHeight == 0 || (agentItem.HitboxHeight == 1200 && agentItem.HitboxWidth == 100));
        AgentItem sword = agentData.AddCustomNPCAgent(fightData.FightStart, fightData.FightEnd, "Conjured Sword\0:Conjured Sword\051", ParserHelper.Spec.NPC, TargetID.ConjuredPlayerSword, true);
        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
        foreach (CombatItem c in combatData)
        {
            if (c.IsDamage(extensions) && c.SkillID == ConjuredSlashPlayer)
            {
                c.OverrideSrcAgent(sword);
            }
        }
    }
    /*internal override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<long, List<AbstractBuffEvent>> buffsById, SkillData skillData)
    {
        var res = new List<AbstractBuffEvent>();
        // Greatsword Power
        AdjustTimeRefreshBuff(buffsByDst, buffsById, 52667);
        // Conjured Shield
        AdjustTimeRefreshBuff(buffsByDst, buffsById, 52754);
        return res;
    }*/

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)TargetID.ConjuredAmalgamate:
                var shieldCA = target.GetBuffStatus(log, ShieldedCA, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                uint CAShieldRadius = 500;
                foreach (Segment seg in shieldCA)
                {
                    replay.Decorations.Add(new CircleDecoration(CAShieldRadius, seg, "rgba(0, 150, 255, 0.3)", new AgentConnector(target)));
                }
                break;
            case (int)TargetID.CALeftArm:
            case (int)TargetID.CARightArm:
                break;
            case (int)TargetID.ConjuredGreatsword:
                break;
            case (int)TargetID.ConjuredShield:
                var shieldShield = target.GetBuffStatus(log, ShieldedCA, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                uint ShieldShieldRadius = 100;
                foreach (Segment seg in shieldShield)
                {
                    replay.Decorations.Add(new CircleDecoration(ShieldShieldRadius, seg, "rgba(0, 150, 255, 0.3)", new AgentConnector(target)));
                }
                break;
            default:
                break;
        }
    }

    internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
    {
        base.CheckSuccess(combatData, agentData, fightData, playerAgents);
        if (!fightData.Success)
        {
            SingleActor? target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.ConjuredAmalgamate));
            SingleActor? leftArm = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CALeftArm));
            SingleActor? rightArm = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CARightArm));
            if (target == null)
            {
                throw new MissingKeyActorsException("Conjured Amalgamate not found");
            }
            AgentItem? zommoros = agentData.GetNPCsByID(TargetID.ChillZommoros).LastOrDefault();
            if (zommoros == null)
            {
                return;
            }
            SpawnEvent? npcSpawn = combatData.GetSpawnEvents(zommoros).LastOrDefault();
            HealthDamageEvent? lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && !x.ToFriendly && playerAgents.Contains(x.From.GetFinalMaster()));
            if (lastDamageTaken == null)
            {
                return;
            }
            if (rightArm != null)
            {
                HealthDamageEvent? lastDamageTakenArm = combatData.GetDamageTakenData(rightArm.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                if (lastDamageTakenArm != null)
                {
                    lastDamageTaken = lastDamageTaken.Time > lastDamageTakenArm.Time ? lastDamageTaken : lastDamageTakenArm;
                }
            }
            if (leftArm != null)
            {
                HealthDamageEvent? lastDamageTakenArm = combatData.GetDamageTakenData(leftArm.AgentItem).LastOrDefault(x => (x.HealthDamage > 0) && playerAgents.Contains(x.From.GetFinalMaster()));
                if (lastDamageTakenArm != null)
                {
                    lastDamageTaken = lastDamageTaken.Time > lastDamageTakenArm.Time ? lastDamageTaken : lastDamageTakenArm;
                }
            }
            if (npcSpawn != null)
            {
                fightData.SetSuccess(true, lastDamageTaken.Time);
            }
        }
    }

    private static List<long> GetTargetableTimes(ParsedEvtcLog log, SingleActor? target)
    {
        if (target == null)
        {
            return [];
        }
        var attackTargetsAgents = log.CombatData.GetAttackTargetEvents(target.AgentItem);
        var attackTargets = new HashSet<AgentItem>();
        foreach (AttackTargetEvent c in attackTargetsAgents) // 3rd one is weird
        {
            attackTargets.Add(c.AttackTarget);
            if (attackTargets.Count == 2)
            {
                break;
            }
        }
        var targetables = new List<long>();
        foreach (AgentItem attackTarget in attackTargets)
        {
            IReadOnlyList<TargetableEvent> aux = log.CombatData.GetTargetableEvents(attackTarget);
            targetables.AddRange(aux.Where(x => x.Targetable).Select(x => x.Time));
        }
        return targetables;
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor ca = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.ConjuredAmalgamate)) ?? throw new MissingKeyActorsException("Conjured Amalgamate not found");
        SingleActor? leftArm = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CALeftArm));
        SingleActor? rightArm = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.CARightArm));
        phases[0].AddTarget(ca, log);
        phases[0].AddTarget(leftArm, log, PhaseData.TargetPriority.Blocking);
        phases[0].AddTarget(rightArm, log, PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        phases.AddRange(GetPhasesByInvul(log, CAInvul, ca, true, false));
        int burnPhase = 0, armPhase = 0;
        for (int i = 1; i < phases.Count; i++)
        {
            string name;
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 1)
            {
                name = "Arm Phase " + (++armPhase);
            }
            else
            {
                name = "Burn Phase " + (++burnPhase);
                phase.AddTarget(ca, log);
            }
            phase.Name = name;
        }
        int leftArmPhase = 0, rightArmPhase = 0, bothArmPhase = 0;
        if (leftArm != null || rightArm != null)
        {
            List<long> targetablesL = GetTargetableTimes(log, leftArm);
            List<long> targetablesR = GetTargetableTimes(log, rightArm);
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (!phase.Name.Contains("Arm"))
                {
                    continue;
                }
                var leftExists = targetablesL.Exists(x => phase.InInterval(x));
                var rightExists = targetablesR.Exists(x => phase.InInterval(x));
                if (leftExists && rightExists)
                {
                    phase.Name = "Both Arms Phase " + (++bothArmPhase);
                    phase.AddTarget(leftArm, log);
                    phase.AddTarget(rightArm, log);
                }
                else if (leftExists)
                {
                    phase.Name = "Left Arm Phase " + (++leftArmPhase);
                    phase.AddTarget(leftArm, log);
                }
                else if (rightExists)
                {
                    phase.Name = "Right Arm Phase " + (++rightArmPhase);
                    phase.AddTarget(rightArm, log);
                }
            }
        }
        return phases;
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        // Conjured Protection - Shield AoE
        var casts = p.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
        var shieldCast = casts.Where(x => x.SkillId == ConjuredProtectionSAK);
        foreach (CastEvent c in shieldCast)
        {
            int duration = 10000;
            uint radius = 300;
            (long start, long end) lifespan = (c.Time, c.Time + duration);
            if (p.TryGetCurrentInterpolatedPosition(log, lifespan.start, out var position))
            {
                var circle = new CircleDecoration(radius, lifespan, Colors.Magenta, 0.2, new PositionConnector(position));
                replay.Decorations.AddWithBorder(circle);
            }
        }
        // Shields and Greatswords Overheads
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, ConjuredShield, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0), p, ParserIcons.ConjuredShieldEmptyOverhead);
        replay.Decorations.AddOverheadIcons(p.GetBuffStatus(log, GreatswordPower, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0), p, ParserIcons.GreatswordPowerEmptyOverhead);
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.ConjuredAmalgamate)) ?? throw new MissingKeyActorsException("Conjured Amalgamate not found");
        return combatData.GetBuffData(LockedOn).Count > 0 ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

    internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
    {
        base.ComputeEnvironmentCombatReplayDecorations(log);

        (long start, long end) lifespan;

        // Junk Absorption - Collecting Phase
        var purpleOrbs = log.CombatData.GetMissileEventsBySkillID(JunkAbsorption);
        var swordOrbs = log.CombatData.GetMissileEventsBySkillID(JunkAbsorptionSword);
        var shieldOrbs = log.CombatData.GetMissileEventsBySkillID(JunkAbsorptionShield);

        // Purple Orbs
        EnvironmentDecorations.AddNonHomingMissiles(log, purpleOrbs, Colors.Purple, 0.5, 50);

        // Sword Orbs
        foreach (MissileEvent missileEvent in swordOrbs)
        {
            lifespan = (missileEvent.Time, missileEvent.RemoveEvent?.Time ?? log.FightData.FightEnd);
            for (int i = 0; i < missileEvent.LaunchEvents.Count; i++)
            {
                var launch = missileEvent.LaunchEvents[i];
                lifespan = (launch.Time, i != missileEvent.LaunchEvents.Count - 1 ? missileEvent.LaunchEvents[i + 1].Time : lifespan.end);
                var connector = new InterpolationConnector([new ParametricPoint3D(launch.LaunchPosition, lifespan.start), launch.GetFinalPosition(lifespan)], Connector.InterpolationMethod.Linear);
                EnvironmentDecorations.Add(new CircleDecoration(50, lifespan, Colors.Emerald, 0.5, connector));
                EnvironmentDecorations.Add(new RectangleDecoration(25, 200, lifespan, Colors.White, 0.3, connector));
            }
        }

        // Shield Orbs
        foreach (MissileEvent missileEvent in shieldOrbs)
        {
            lifespan = (missileEvent.Time, missileEvent.RemoveEvent?.Time ?? log.FightData.FightEnd);
            for (int i = 0; i < missileEvent.LaunchEvents.Count; i++)
            {
                var launch = missileEvent.LaunchEvents[i];
                lifespan = (launch.Time, i != missileEvent.LaunchEvents.Count - 1 ? missileEvent.LaunchEvents[i + 1].Time : lifespan.end);
                var connector = new InterpolationConnector([new ParametricPoint3D(launch.LaunchPosition, lifespan.start),launch.GetFinalPosition(lifespan)],Connector.InterpolationMethod.Linear);
                EnvironmentDecorations.Add(new CircleDecoration(50, lifespan, Colors.Emerald, 0.5, connector));
                EnvironmentDecorations.Add(new DoughnutDecoration(100, 125, lifespan, Colors.White, 0.3, connector));
            }
        }
    }
}
