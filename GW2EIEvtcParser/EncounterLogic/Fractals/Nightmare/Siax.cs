using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using static GW2EIEvtcParser.ArcDPSEnums;
using System.Collections;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Siax : Nightmare
    {
        public Siax(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(VileSpit, "Vile Spit", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Spit","Vile Spit (green goo)", "Poison Spit",0),
            new PlayerDstHitMechanic(TailLashSiax, "Tail Lash", new MechanicPlotlySetting(Symbols.TriangleLeft,Colors.Yellow), "Tail","Tail Lash (half circle Knockback)", "Tail Lash",0),
            new SpawnMechanic((int)TrashID.NightmareHallucinationSiax, "Nightmare Hallucination", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Black), "Hallu","Nightmare Hallucination Spawn", "Hallucination",0),
            new PlayerDstHitMechanic(new long[] { CascadeOfTorment1, CascadeOfTorment2 }, "Cascade of Torment", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new EnemyCastStartMechanic(new long[] { CausticExplosionSiaxPhase66, CausticExplosionSiaxPhase33 }, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Yellow), "Phase","Phase Start", "Phase", 0),
            new EnemyCastEndMechanic(new long[] { CausticExplosionSiaxPhase66, CausticExplosionSiaxPhase33 }, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "Phase Fail","Phase Fail (Failed to kill Echos in time)", "Phase Fail", 0).UsingChecker((ce,log) => ce.ActualDuration >= 20649), //
            new EnemyCastStartMechanic(CausticExplosionSiaxBreakbar, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.DarkTeal), "CC","Breakbar Start", "Breakbar", 0),
            new EnemyCastEndMechanic(CausticExplosionSiaxBreakbar, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.Red), "CC Fail","Failed to CC in time", "CC Fail", 0).UsingChecker( (ce,log) => ce.ActualDuration >= 15232),
            new PlayerDstBuffApplyMechanic(FixatedNightmare, "Fixated", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Magenta), "Fixate", "Fixated by Volatile Hallucination", "Fixated",0),
            });
            Extension = "siax";
            Icon = EncounterIconSiax;
            EncounterCategoryInformation.InSubCategoryOrder = 1;
            EncounterID |= 0x000002;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplaySiax,
                            (476, 548),
                            (663, -4127, 3515, -997)/*,
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054)*/);
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            var trashIDs = new List<ArcDPSEnums.TrashID>
            {
                TrashID.VolatileHallucinationSiax,
                TrashID.NightmareHallucinationSiax
            };
            trashIDs.AddRange(base.GetTrashMobsIDs());
            return trashIDs;
        }
        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)TargetID.Siax,
                (int)TrashID.EchoOfTheUnclean,
            };
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor siax = Targets.FirstOrDefault(x => x.IsSpecies(TargetID.Siax));
            if (siax == null)
            {
                throw new MissingKeyActorsException("Siax not found");
            }
            phases[0].AddTarget(siax);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, siax, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    var ids = new List<int>
                    {
                       (int) TrashID.EchoOfTheUnclean,
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                    phase.Name = "Caustic Explosion " + (i / 2);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(siax);
                }
            }
            return phases;
        }
        
        static readonly List<(string, Point3D)> EchoLocations = new List<(string, Point3D)> {
            ("N", new Point3D(1870.630f, -2205.379f)),
            ("E", new Point3D(2500.260f, -3288.280f)),
            ("S", new Point3D(1572.040f, -3992.580f)),
            ("W", new Point3D(907.199f, -2976.850f)),
            ("NW", new Point3D(1036.980f, -2237.050f)),
            ("NE", new Point3D(2556.450f, -2628.590f)),
            ("SE", new Point3D(2293.149f, -3912.510f)),
            ("SW", new Point3D(891.370f, -3722.450f)),
        };

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, evtcVersion, fightData, agentData, combatData, extensions);
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.IsSpecies(TargetID.Siax))
                {
                    target.OverrideName("Siax the Corrupted");
                }
                else if (target.IsSpecies(TrashID.EchoOfTheUnclean))
                {
                    AddNameSuffixBasedOnInitialPosition(target, combatData, EchoLocations);
                }
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)TargetID.Siax:
                    // Siax's Breakbar
                    var causticExplosionBreakbar = casts.Where(x => x.SkillId == CausticExplosionSiaxBreakbar).ToList();
                    foreach (AbstractCastEvent c in causticExplosionBreakbar)
                    {
                        int duration = 15000;
                        int start = (int)c.Time;
                        int expectedHitTime = (int)c.Time + duration;
                        int attackEnd = (int)c.Time + duration;

                        Segment stunSegment = target.GetBuffStatus(log, Stun, c.Time, c.Time + duration).FirstOrDefault(x => x.Value > 0);
                        if (stunSegment != null)
                        {
                            attackEnd = Math.Min((int)stunSegment.Start, attackEnd); // Start of stun
                        }
                        Segment detSegment = target.GetBuffStatus(log, Determined762, c.Time, c.Time + duration).FirstOrDefault(x => x.Value > 0);
                        if (detSegment != null)
                        {
                            attackEnd = Math.Min((int)detSegment.Start, attackEnd); // Start of determinated
                        }
                        var doughnut = new DoughnutDecoration(0, 1500, (start, attackEnd), Colors.Red, 0.2, new AgentConnector(target));
                        replay.AddDecorationWithGrowing(doughnut, expectedHitTime, true);
                    }
                    // Tail Swipe
                    var tailLash = casts.Where(x => x.SkillId == TailLashSiax).ToList();
                    foreach (AbstractCastEvent c in tailLash)
                    {
                        int duration = 1500;
                        int openingAngle = 144;
                        uint radius = 600;
                        int start = (int)c.Time;
                        int end = start + duration;
                        if (replay.Rotations.Any())
                        {
                            replay.Decorations.Add(new PieDecoration(radius, openingAngle, (start, end), Colors.Orange, 0.2, new AgentConnector(target)).UsingRotationConnector(new AgentFacingConnector(target)));
                        }
                    }
                    // 66% and 33% phases
                    var causticExplosionPhases = casts.Where(x => x.SkillId == CausticExplosionSiaxPhase66 || x.SkillId == CausticExplosionSiaxPhase33).ToList();
                    foreach (AbstractCastEvent c in causticExplosionPhases)
                    {
                        int duration = 20000;
                        int start = (int)c.Time;
                        int expectedHitTime = (int)c.Time + duration;
                        int attackEnd = (int)c.Time + duration;

                        Segment detSegment = target.GetBuffStatus(log, Determined762, c.Time, c.Time + duration).FirstOrDefault(x => x.Value > 0);
                        if (detSegment != null)
                        {
                            attackEnd = Math.Min((int)detSegment.End, attackEnd); // End of determinated
                        }
                        var circle = new CircleDecoration(1500, (start, attackEnd), Colors.Red, 0.2, new AgentConnector(target));
                        replay.AddDecorationWithGrowing(circle, expectedHitTime);
                    }
                    // Poison AoE
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.VileSpitSiax, out IReadOnlyList<EffectEvent> poisonEffects))
                    {
                        int duration = 16000;
                        foreach (EffectEvent effect in poisonEffects)
                        {
                            replay.Decorations.Add(new CircleDecoration(240, ((int)effect.Time, (int)effect.Time + duration), Colors.Green, 0.2, new PositionConnector(effect.Position)));
                        }
                    }
                    // Nightmare Hallucinations Spawn Event
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.NightmareHallucinationsSpawn, out IReadOnlyList<EffectEvent> spawnEffects))
                    {
                        int duration = 3000;
                        foreach (EffectEvent effect in spawnEffects)
                        {
                            var circle = new CircleDecoration(360, (effect.Time, effect.Time + duration), Colors.Orange, 0.2, new PositionConnector(effect.Position));
                            replay.AddDecorationWithGrowing(circle, effect.Time + duration);
                        }
                    }
                    // Caustic Barrage
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.CausticBarrageIndicator, out IReadOnlyList<EffectEvent> barrageEffects))
                    {
                        int duration = 500;
                        foreach (EffectEvent effect in barrageEffects)
                        {
                            var circle = new CircleDecoration(100, (effect.Time, effect.Time + duration), Colors.Orange, 0.2, new PositionConnector(effect.Position));
                            replay.AddDecorationWithGrowing(circle, effect.Time + duration);
                        }
                    }
                    // Volatile Hallucinations Explosions
                    if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.VolatileExpulsionIndicator, out IReadOnlyList<EffectEvent> expulsionEffects))
                    {
                        int duration = 200;
                        foreach (EffectEvent effect in expulsionEffects)
                        {
                            var circle = new CircleDecoration(240, (effect.Time, effect.Time + duration), Colors.Orange, 0.2, new PositionConnector(effect.Position));
                            replay.AddDecorationWithGrowing(circle, effect.Time + duration);
                        }
                    }
                    // Cascade Of Torment
                    int cotDuration = 1000;
                    AddCascadeOfTormentDecoration(log, replay, EffectGUIDs.CascadeOfTormentRing0, cotDuration, 0, 150);
                    AddCascadeOfTormentDecoration(log, replay, EffectGUIDs.CascadeOfTormentRing1, cotDuration, 150, 250);
                    AddCascadeOfTormentDecoration(log, replay, EffectGUIDs.CascadeOfTormentRing2, cotDuration, 250, 350);
                    AddCascadeOfTormentDecoration(log, replay, EffectGUIDs.CascadeOfTormentRing3, cotDuration, 350, 450);
                    AddCascadeOfTormentDecoration(log, replay, EffectGUIDs.CascadeOfTormentRing4, cotDuration, 450, 550);
                    AddCascadeOfTormentDecoration(log, replay, EffectGUIDs.CascadeOfTormentRing5, cotDuration, 550, 650);
                    break;
                case (int)TrashID.EchoOfTheUnclean:
                    var causticExplosionEcho = casts.Where(x => x.SkillId == CausticExplosionSiaxEcho).ToList();
                    foreach (AbstractCastEvent c in causticExplosionEcho)
                    {
                        // Duration is the same as Siax's explosion but starts 2 seconds later
                        int duration = 20000;
                        int start = (int)c.Time + 18000;
                        int attackEnd = (int)c.Time + duration;
                        replay.Decorations.Add(new CircleDecoration(3000, (start, attackEnd), Colors.Orange, 0.2, new AgentConnector(target)));
                    }
                    break;
                case (int)TrashID.VolatileHallucinationSiax:
                    break;
                case (int)TrashID.NightmareHallucinationSiax:
                    break;
                default: break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            // Fixations
            IEnumerable<Segment> fixations = p.GetBuffStatus(log, FixatedNightmare, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            List<AbstractBuffEvent> fixationEvents = GetFilteredList(log.CombatData, FixatedNightmare, p, true, true);
            replay.AddOverheadIcons(fixations, p, ParserIcons.FixationPurpleOverhead);
            replay.AddTether(fixationEvents, Colors.Magenta, 0.5);
        }
    }
}
