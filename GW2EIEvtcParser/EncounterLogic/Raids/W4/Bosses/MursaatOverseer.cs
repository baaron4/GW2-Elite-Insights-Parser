using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class MursaatOverseer : BastionOfThePenitent
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new MechanicGroup([
                new PlayerDstHealthDamageHitMechanic(JadeSoldierAura, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Jade", "Jade Soldier's Aura hit","Jade Aura", 0),
                new PlayerDstHealthDamageHitMechanic(JadeSoldierExplosion, new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Jade Expl", "Jade Soldier's Death Explosion","Jade Explosion", 0),
            ]),
            //new Mechanic(ClaimSAK, "Claim", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.Square,Colors.Yellow), "Claim",0), //Buff remove only
            //new Mechanic(DispelSAK, "Dispel", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Dispel",0), //Buff remove only
            //new Mechanic(ProtectSAK, "Protect", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.Circle,Colors.Teal), "Protect",0), //Buff remove only
            new PlayerDstBuffApplyMechanic(Invulnerability757, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Teal), "Protect", "Protected by the Protect Shield","Protect Shield",0).UsingChecker((ba, log) => ba.AppliedDuration == 1000),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(ProtectBuff, new MechanicPlotlySetting(Symbols.Circle,Colors.Blue), "Protect (SAK)", "Took protect","Protect (SAK)",0)
                    .UsingTimeClamper((time, log) => Math.Max(log.FightData.FightStart, time)),
                new PlayerDstBuffApplyMechanic(DispelBuff, new MechanicPlotlySetting(Symbols.Circle,Colors.Purple), "Dispel (SAK)", "Took dispel","Dispel (SAK)",0)
                    .UsingTimeClamper((time, log) => Math.Max(log.FightData.FightStart, time)),
                new PlayerDstBuffApplyMechanic(ClaimBuff, new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Claim (SAK)", "Took claim","Claim (SAK)",0)
                    .UsingTimeClamper((time, log) => Math.Max(log.FightData.FightStart, time)),
            ]),
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(MursaatOverseersShield, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Shield", "Jade Soldier Shield","Soldier Shield", 0),
                new EnemyDstBuffRemoveMechanic(MursaatOverseersShield, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Yellow), "Dispel", "Dispelled Jade Soldier Shield","Dispel", 0),
            ]),
            //new Mechanic(EnemyTile, "Enemy Tile", ParseEnum.BossIDS.MursaatOverseer, new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Yellow), "Floor","Enemy Tile damage", "Tile dmg",0) //Fixed damage (3500), not trackable
        ]);
    public MursaatOverseer(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "mo";
        Icon = EncounterIconMursaatOverseer;
        EncounterCategoryInformation.InSubCategoryOrder = 1;
        EncounterID |= 0x000002;
        ChestID = ChestID.RecreationRoomChest;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplayMursaatOverseer,
                        (889, 889),
                        (1360, 2701, 3911, 5258)/*,
                        (-27648, -9216, 27648, 12288),
                        (11774, 4480, 14078, 5376)*/);
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return
        [
            TargetID.Jade
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

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.MursaatOverseer)) ?? throw new MissingKeyActorsException("Mursaat Overseer not found");
        phases[0].AddTarget(mainTarget, log);
        if (!requirePhases)
        {
            return phases;
        }
        phases.AddRange(GetPhasesByHealthPercent(log, mainTarget, new List<double> { 75, 50, 25, 0 }));
        for (var i = 1; i < phases.Count; i++)
        {
            phases[i].AddParentPhase(phases[0]);
        }
        return phases;
    }

    internal override IEnumerable<ErrorEvent> GetCustomWarningMessages(FightData fightData, EvtcVersionEvent evtcVersion)
    {
        return base.GetCustomWarningMessages(fightData, evtcVersion)
            .Concat(GetConfusionDamageMissingMessage(evtcVersion).ToEnumerable());
    }

    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        switch (target.ID)
        {
            case (int)TargetID.Jade:
                foreach (CastEvent cast in target.GetAnimatedCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd))
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
                var shields = target.GetBuffStatus(log, MursaatOverseersShield, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                foreach (var seg in shields)
                {
                    replay.Decorations.Add(new CircleDecoration(100, seg, Colors.Yellow, 0.3, new AgentConnector(target)));
                }
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor player, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(player, log, replay);

        (long start, long end) lifespan;

        // Claim - Overhead
        var claims = player.GetBuffStatus(log, ClaimBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
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

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.MursaatOverseer)) ?? throw new MissingKeyActorsException("Mursaat Overseer not found");
        return (target.GetHealth(combatData) > 25e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }

    internal override List<CastEvent> SpecialCastEventProcess(CombatData combatData, SkillData skillData)
    {
        List<CastEvent> res = base.SpecialCastEventProcess(combatData, skillData);

        var claimApply = combatData.GetBuffData(ClaimBuff).OfType<BuffApplyEvent>();
        var dispelApply = combatData.GetBuffData(DispelBuff).OfType<BuffApplyEvent>();

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
