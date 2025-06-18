using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.ParserHelpers.EncounterImages;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.SpeciesIDs;

namespace GW2EIEvtcParser.EncounterLogic;

internal class Samarog : BastionOfThePenitent
{
    internal readonly MechanicGroup Mechanics = new MechanicGroup([
            new PlayerDstHitMechanic(SamarogShockwave, new MechanicPlotlySetting(Symbols.Circle,Colors.Blue), "Schk.Wv", "Shockwave from Spears","Shockwave", 0)
                .UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerDstHitMechanic(PrisonerSweep, new MechanicPlotlySetting(Symbols.Hexagon,Colors.Blue), "Swp", "Prisoner Sweep (horizontal)","Sweep", 0)
                .UsingChecker((de, log) => !de.To.HasBuff(log, Stability, de.Time - ParserHelper.ServerDelayConstant)),
            new PlayerDstHitMechanic(TramplingRush, new MechanicPlotlySetting(Symbols.TriangleRight,Colors.Red), "Trpl", "Trampling Rush (hit by stampede towards home)","Trampling Rush", 0),
            new PlayerDstHitMechanic(Bludgeon , new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Blue), "Slam", "Bludgeon (vertical Slam)","Slam", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(FixatedSamarog, new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "S.Fix", "Fixated by Samarog","Fixate: Samarog", 0),
                new PlayerDstBuffApplyMechanic(FixatedGuldhem, new MechanicPlotlySetting(Symbols.StarOpen,Colors.Orange), "G.Fix", "Fixated by Guldhem","Fixate: Guldhem", 0),
                new PlayerDstBuffApplyMechanic(FixatedRigom, new MechanicPlotlySetting(Symbols.StarOpen,Colors.Red), "R.Fix", "Fixated by Rigom","Fixate: Rigom", 0),
            ]),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(InevitableBetrayalBig, new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "B.Gr", "Big Green (friends mechanic)","Big Green", 0),
                new PlayerDstBuffApplyMechanic(InevitableBetrayalSmall, new MechanicPlotlySetting(Symbols.CircleOpen,Colors.DarkGreen), "S.Gr", "Small Green (friends mechanic)","Small Green", 0),
            ]),
            new MechanicGroup([
                new EnemyDstBuffApplyMechanic(StrengthenedBondGuldhem, new MechanicPlotlySetting(Symbols.TriangleNE,Colors.Orange), "G.Str", "Strengthened Bond: Guldhem","Strengthened: Guldhem", 0),
                new EnemyDstBuffApplyMechanic(StrengthenedBondRigom, new MechanicPlotlySetting(Symbols.TriangleNE,Colors.Red), "R.Str", "Strengthened Bond: Rigom","Strengthened: Rigom", 0),
            ]),
            new MechanicGroup([
                new PlayerDstHitMechanic(SpearReturn, new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Red), "S.Rt", "Hit by Spear Return","Spear Return", 0),
                new PlayerDstHitMechanic(EffigyPulse, new MechanicPlotlySetting(Symbols.TriangleDownOpen,Colors.Red), "S.Pls", "Effigy Pulse (Stood in Spear AoE)","Spear Aoe", 0),
                new PlayerDstHitMechanic(SpearImpact, new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Red), "S.Spwn", "Spear Impact (hit by spawning Spear)","Spear Spawned", 0),
            ]),
            new PlayerDstHitMechanic([InevitableBetrayalFailSmall, InevitableBetrayalFailBig], new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Gr.Fl", "Inevitable Betrayal (failed Green)","Failed Green", 0),
            new MechanicGroup([
                new PlayerDstBuffApplyMechanic(BrutalizeBuff, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Magenta), "Brtlzd","Brutalized (jumped upon by Samarog->Breakbar)","Brutalized", 0),
                new EnemyCastEndMechanic(BrutalizeCast, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","Brutalize (Breakbar)","Breakbar", 0),
                new PlayerDstSkillMechanic(BrutalizeKill, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail", "Brutalize (Failed CC)","CC Fail", 0)
                    .UsingChecker((de, log) => de.HasKilled),
                new EnemyDstBuffRemoveMechanic(FanaticalResilience, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CC End", "Ended Brutalize","CC Ended", 0),
            ]),
            //new PlayerBoonRemoveMechanic(BrutalizeEffect, "Brutalize", ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Ended Brutalize (Breakbar broken)", "CCEnded",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.Manual)),
            //new Mechanic(BrutalizeEffect, "Brutalize", Mechanic.MechType.EnemyBoonStrip, ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting(Symbols.DiamondTall,"rgb(110,160,0)"), "CCed1","Ended Brutalize (Breakbar broken)", "CCed1",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.All)),
            new PlayerDstBuffApplyMechanic(SoulSwarm, new MechanicPlotlySetting(Symbols.XThinOpen,Colors.Teal), "Wall","Soul Swarm (stood in or beyond Spear Wall)","Spear Wall", 0),
            new PlayerDstHitMechanic(ImpalingStab, new MechanicPlotlySetting(Symbols.Hourglass,Colors.Blue), "Shck.Wv Ctr","Impaling Stab (hit by Spears causing Shockwave)","Shockwave Center", 0),
            new PlayerDstHitMechanic(AnguishedBolt, new MechanicPlotlySetting(Symbols.Circle,Colors.LightOrange), "Stun","Anguished Bolt (AoE Stun Circle by Guldhem)","Guldhem's Stun", 0),
        
            //  new Mechanic(SpearImpact, "Brutalize", ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting(Symbols.StarSquare,Color.Red), "CC Target", casted without dmg odd
        ]);
    public Samarog(int triggerID) : base(triggerID)
    {
        MechanicList.Add(Mechanics);
        Extension = "sam";
        Icon = EncounterIconSamarog;
        EncounterCategoryInformation.InSubCategoryOrder = 2;
        EncounterID |= 0x000003;
        ChestID = ChestID.SamarogChest;
    }

    protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
    {
        return new CombatReplayMap(CombatReplaySamarog,
                        (1000, 959),
                        (-6526, 1218, -2423, 5146)/*,
                        (-27648, -9216, 27648, 12288),
                        (11774, 4480, 14078, 5376)*/);
    }

    internal override List<InstantCastFinder> GetInstantCastFinders()
    {
        return
        [
            new DamageCastFinder(BrutalAura, BrutalAura),
        ];
    }

    internal override List<HealthDamageEvent> SpecialDamageEventProcess(CombatData combatData, SkillData skillData)
    {
        var fanaticalAppliedAgents = combatData.GetBuffApplyData(FanaticalResilience).Select(x => x.To).Distinct();
        foreach (var fanaticalAppliedTo in fanaticalAppliedAgents)
        {
            IReadOnlyList<HealthDamageEvent> damageTaken = combatData.GetDamageTakenData(fanaticalAppliedTo);
            var fanaticalResilienceTimes = GetFilteredList(combatData, FanaticalResilience, fanaticalAppliedTo, true, false).Select(x => x.Time).ToList();
            var fanaticalResilienceSegments = new List<Segment>();
            for (int i = 0; i < fanaticalResilienceTimes.Count; i += 2)
            {
                long start = fanaticalResilienceTimes[i];
                long end = long.MaxValue;
                if (i + 1 < fanaticalResilienceTimes.Count)
                {
                    end = fanaticalResilienceTimes[i + 1];
                }
                fanaticalResilienceSegments.Add(new Segment(start, end, 1));
            }
            foreach (HealthDamageEvent healthDamageEvent in damageTaken)
            {
                // Can't have been absorbed if not 0 damages
                if (healthDamageEvent.HasHit && healthDamageEvent.HealthDamage == 0 && fanaticalResilienceSegments.Any(x => healthDamageEvent.Time >= x.Start && healthDamageEvent.Time <= x.End))
                {
                    healthDamageEvent.MakeIntoAbsorbed();
                }
            }
        }
        return [];
    }

    internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
    {
        List<PhaseData> phases = GetInitialPhase(log);
        SingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Samarog)) ?? throw new MissingKeyActorsException("Samarog not found");
        phases[0].AddTarget(mainTarget, log);
        phases[0].AddTargets(Targets.Where(x => x.IsSpecies(TargetID.Guldhem) || x.IsSpecies(TargetID.Rigom)), log, PhaseData.TargetPriority.Blocking);
        if (!requirePhases)
        {
            return phases;
        }
        // Determined check
        phases.AddRange(GetPhasesByInvul(log, Determined762, mainTarget, true, true));
        for (int i = 1; i < phases.Count; i++)
        {
            PhaseData phase = phases[i];
            phase.AddParentPhase(phases[0]);
            if (i % 2 == 0)
            {
                phase.Name = "Split " + i / 2;
                var ids = new List<TargetID>
                {
                    TargetID.Rigom,
                    TargetID.Guldhem
                };
                AddTargetsToPhaseAndFit(phase, ids, log);
            }
            else
            {
                phase.Name = "Phase " + (i + 1) / 2;
                phase.AddTarget(mainTarget, log);
            }
        }
        return phases;
    }

    internal override void EIEvtcParse(ulong gw2Build, EvtcVersionEvent evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, ExtensionHandler> extensions)
    {
        // With lingering agents, last aware of the spears are properly set
        if (evtcVersion.Build >= ArcDPSBuilds.LingeringAgents)
        {
            var spearAgents = combatData.Where(x => MaxHealthUpdateEvent.GetMaxHealth(x) == 104580 && x.IsStateChange == StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && x.HitboxWidth == 100 && x.HitboxHeight == 300);
            foreach (AgentItem spear in spearAgents)
            {
                spear.OverrideType(AgentItem.AgentType.NPC, agentData);
                spear.OverrideID(TargetID.SpearAggressionRevulsion, agentData);
            }
        }

        base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
    }

    internal override IReadOnlyList<TargetID>  GetTargetsIDs()
    {
        return
        [
            TargetID.Samarog,
            TargetID.Rigom,
            TargetID.Guldhem,
        ];
    }
    internal override Dictionary<TargetID, int> GetTargetsSortIDs()
    {
        return new Dictionary<TargetID, int>()
        {
            {TargetID.Samarog, 0 },
            {TargetID.Rigom, 1 },
            {TargetID.Guldhem, 1 },
        };
    }

    internal override IReadOnlyList<TargetID> GetTrashMobsIDs()
    {
        return [
            TargetID.SpearAggressionRevulsion
        ];
    }


    internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
    {
        // TODO: facing information (shock wave)
        switch (target.ID)
        {
            case (int)TargetID.Samarog:
                var brutalize = target.GetBuffStatus(log, FanaticalResilience, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
                foreach (Segment seg in brutalize)
                {
                    replay.Decorations.Add(new OverheadProgressBarDecoration(ParserHelper.CombatReplayOverheadProgressBarMajorSizeInPixel, (seg.Start, seg.End), Colors.Red, 0.6, Colors.Black, 0.2, [(seg.Start, 0), (seg.Start + 15000, 100)], new AgentConnector(target))
                        .UsingRotationConnector(new AngleConnector(180)));
                }
                break;
            case (int)TargetID.Rigom:
            case (int)TargetID.Guldhem:
                break;
            case (int)TargetID.SpearAggressionRevulsion:
                var spearLifespan = new Segment(target.FirstAware, target.LastAware, 1);
                replay.Decorations.Add(new CircleDecoration(240, spearLifespan, Colors.Orange, 0.1, new AgentConnector(target)));
                if (log.CombatData.GetBuffDataByIDByDst(SpearOfAggressionBuff, target.AgentItem).Any())
                {
                    replay.Decorations.AddOverheadIcon(spearLifespan, target, BuffImages.Taunt, 15);
                }
                else
                {
                    replay.Decorations.AddOverheadIcon(spearLifespan, target, BuffImages.Fear, 15);
                }
                break;
            default:
                break;
        }
    }

    internal override void ComputePlayerCombatReplayActors(PlayerActor p, ParsedEvtcLog log, CombatReplay replay)
    {
        base.ComputePlayerCombatReplayActors(p, log, replay);
        // big bomb
        var bigbomb = log.CombatData.GetBuffDataByIDByDst(InevitableBetrayalBig, p.AgentItem).Where(x => x is BuffApplyEvent);
        foreach (BuffEvent c in bigbomb)
        {
            int bigStart = (int)c.Time;
            int bigEnd = bigStart + 6000;
            var circle = new CircleDecoration(300, (bigStart, bigEnd), "rgba(150, 80, 0, 0.2)", new AgentConnector(p));
            replay.Decorations.AddWithGrowing(circle, bigEnd);
        }
        // small bomb
        var smallbomb = log.CombatData.GetBuffDataByIDByDst(InevitableBetrayalSmall, p.AgentItem).Where(x => x is BuffApplyEvent);
        foreach (BuffEvent c in smallbomb)
        {
            int smallStart = (int)c.Time;
            int smallEnd = smallStart + 6000;
            replay.Decorations.Add(new CircleDecoration(80, (smallStart, smallEnd), "rgba(80, 150, 0, 0.3)", new AgentConnector(p)));
        }
        // fixated Samarog
        var fixatedSam = p.GetBuffStatus(log, FixatedSamarog, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        foreach (Segment seg in fixatedSam)
        {
            replay.Decorations.AddOverheadIcon(seg, p, ParserIcons.FixationPurpleOverhead);
        }
        var fixatedSamarog = GetFilteredList(log.CombatData, FixatedSamarog, p, true, true);
        replay.Decorations.AddTether(fixatedSamarog, "rgba(255, 80, 255, 0.3)");
        //fixated Guldhem
        var fixatedGuldhem = p.GetBuffStatus(log, FixatedGuldhem, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        foreach (Segment seg in fixatedGuldhem)
        {
            long mid = (seg.Start + seg.End) / 2;
            SingleActor? guldhem = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Guldhem) && mid >= x.FirstAware && mid <= x.LastAware);
            if (guldhem != null)
            {
                replay.Decorations.Add(new LineDecoration(seg, Colors.Orange, 0.3, new AgentConnector(p), new AgentConnector(guldhem)));
            }
        }
        //fixated Rigom
        var fixatedRigom = p.GetBuffStatus(log, FixatedRigom, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0);
        foreach (Segment seg in fixatedRigom)
        {
            long mid = (seg.Start + seg.End) / 2;
            SingleActor? rigom = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Rigom) && mid >= x.FirstAware && mid <= x.LastAware);
            if (rigom != null)
            {
                replay.Decorations.Add(new LineDecoration(seg, Colors.Red, 0.3, new AgentConnector(p), new AgentConnector(rigom)));
            }
        }
    }

    internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
    {
        SingleActor target = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Samarog)) ?? throw new MissingKeyActorsException("Samarog not found");
        return (target.GetHealth(combatData) > 30e6) ? FightData.EncounterMode.CM : FightData.EncounterMode.Normal;
    }
}
