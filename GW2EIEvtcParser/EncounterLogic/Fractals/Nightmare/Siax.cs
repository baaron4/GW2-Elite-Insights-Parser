using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

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
            new SpawnMechanic((int)ArcDPSEnums.TrashID.NightmareHallucinationSiax, "Nightmare Hallucination", new MechanicPlotlySetting(Symbols.StarOpen,Colors.Black), "Hallu","Nightmare Hallucination Spawn", "Hallucination",0),
            new PlayerDstHitMechanic(new long[] { CascadeOfTorment1, CascadeOfTorment2 }, "Cascade of Torment", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new EnemyCastStartMechanic(new long[] { CausticExplosionSiaxPhase66, CausticExplosionSiaxPhase33 }, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Yellow), "Phase","Phase Start", "Phase", 0),
            new EnemyCastEndMechanic(new long[] { CausticExplosionSiaxPhase66, CausticExplosionSiaxPhase33 }, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "Phase Fail","Phase Fail (Failed to kill Echos in time)", "Phase Fail", 0, (ce,log) => ce.ActualDuration >= 20649), //
            new EnemyCastStartMechanic(CausticExplosionSiaxBreakbar, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.DarkTeal), "CC","Breakbar Start", "Breakbar", 0),
            new EnemyCastEndMechanic(CausticExplosionSiaxBreakbar, "Caustic Explosion", new MechanicPlotlySetting(Symbols.DiamondWide,Colors.Red), "CC Fail","Failed to CC in time", "CC Fail", 0, (ce,log) => ce.ActualDuration >= 15232),
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
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.VolatileHallucinationSiax,
                ArcDPSEnums.TrashID.NightmareHallucinationSiax
            };
        }
        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Siax,
                (int)ArcDPSEnums.TrashID.EchoOfTheUnclean,
            };
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor siax = Targets.FirstOrDefault(x => x.IsSpecy(ArcDPSEnums.TargetID.Siax));
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
                       (int) ArcDPSEnums.TrashID.EchoOfTheUnclean,
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

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Siax:
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

                        replay.Decorations.Add(new DoughnutDecoration(true, -expectedHitTime, 0, 1500, (start, attackEnd), "rgba(255, 0, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new DoughnutDecoration(true, 0, 0, 1500, (start, attackEnd), "rgba(255, 0, 0, 0.2)", new AgentConnector(target)));
                    }
                    // Tail Swipe
                    var tailLash = casts.Where(x => x.SkillId == TailLashSiax).ToList();
                    foreach (AbstractCastEvent c in tailLash)
                    {
                        int duration = 1500;
                        int openingAngle = 144;
                        int radius = 600;
                        replay.Decorations.Add(new FacingPieDecoration(((int)c.Time, (int)c.Time + duration), new AgentConnector(target), replay.PolledRotations, radius, openingAngle, "rgba(250, 120, 0, 0.2)"));
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

                        replay.Decorations.Add(new CircleDecoration(true, expectedHitTime, 1500, (start, attackEnd), "rgba(255, 0, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, 1500, (start, attackEnd), "rgba(255, 0, 0, 0.2)", new AgentConnector(target)));
                    }
                    // Poison AoE
                    EffectGUIDEvent poisonField = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.VileSpitSiax);
                    if (poisonField != null)
                    {
                        var poisonEffects = log.CombatData.GetEffectEventsByEffectID(poisonField.ContentID).ToList();
                        int duration = 16000;
                        foreach (EffectEvent effect in poisonEffects)
                        {
                            replay.Decorations.Add(new CircleDecoration(true, 0, 240, ((int)effect.Time, (int)effect.Time + duration), "rgba(0, 255, 0, 0.2)", new PositionConnector(effect.Position)));
                        }
                    }
                    // Nightmare Hallucinations Spawn Event
                    EffectGUIDEvent spawnField = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.NightmareHallucinationsSpawn);
                    if (spawnField != null)
                    {
                        var spawnEffects = log.CombatData.GetEffectEventsByEffectID(spawnField.ContentID).ToList();
                        int duration = 3000;
                        foreach (EffectEvent effect in spawnEffects)
                        {
                            replay.Decorations.Add(new CircleDecoration(true, (int)effect.Time + duration, 360, ((int)effect.Time, (int)effect.Time + duration), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 360, ((int)effect.Time, (int)effect.Time + duration), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                        }
                    }
                    // Caustic Barrage
                    EffectGUIDEvent causticBarrage = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.CausticBarrageIndicator);
                    if (causticBarrage != null)
                    {
                        var barrageEffects = log.CombatData.GetEffectEventsByEffectID(causticBarrage.ContentID).ToList();
                        int duration = 500;
                        foreach (EffectEvent effect in barrageEffects)
                        {
                            replay.Decorations.Add(new CircleDecoration(true, (int)effect.Time + duration, 100, ((int)effect.Time, (int)effect.Time + duration), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 100, ((int)effect.Time, (int)effect.Time + duration), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                        }
                    }
                    // Volatile Hallucinations Explosions
                    EffectGUIDEvent volatileExpulsion = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.VolatileExpulsionIndicator);
                    if (volatileExpulsion != null)
                    {
                        var expulsionEffects = log.CombatData.GetEffectEventsByEffectID(volatileExpulsion.ContentID).ToList();
                        int duration = 200;
                        foreach (EffectEvent effect in expulsionEffects)
                        {
                            replay.Decorations.Add(new CircleDecoration(true, (int)effect.Time + duration, 240, ((int)effect.Time, (int)effect.Time + duration), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 240, ((int)effect.Time, (int)effect.Time + duration), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                        }
                    }
                    // Cascade Of Torment
                    EffectGUIDEvent cot0 = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.CascadeOfTormentRing0);
                    EffectGUIDEvent cot1 = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.CascadeOfTormentRing1);
                    EffectGUIDEvent cot2 = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.CascadeOfTormentRing2);
                    EffectGUIDEvent cot3 = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.CascadeOfTormentRing3);
                    EffectGUIDEvent cot4 = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.CascadeOfTormentRing4);
                    EffectGUIDEvent cot5 = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.CascadeOfTormentRing5);
                    int cotDuration = 1000;
                    // Ring 0
                    if (cot0 != null)
                    {
                        var expulsionEffects = log.CombatData.GetEffectEventsByEffectID(cot0.ContentID).ToList();
                        foreach (EffectEvent effect in expulsionEffects)
                        {
                            int endTime = (int)effect.Time + cotDuration;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 150, ((int)effect.Time, endTime), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 150, (endTime, endTime + 150), "rgba(250, 120, 0, 0.4)", new PositionConnector(effect.Position)));
                        }
                    }
                    // Ring 1
                    if (cot1 != null)
                    {
                        var expulsionEffects = log.CombatData.GetEffectEventsByEffectID(cot1.ContentID).ToList();
                        foreach (EffectEvent effect in expulsionEffects)
                        {
                            int endTime = (int)effect.Time + cotDuration;
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 150, 250, ((int)effect.Time, endTime), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 150, 250, (endTime, endTime + 150), "rgba(250, 120, 0, 0.4", new PositionConnector(effect.Position)));
                        }
                    }
                    // Ring 2
                    if (cot2 != null)
                    {
                        var expulsionEffects = log.CombatData.GetEffectEventsByEffectID(cot2.ContentID).ToList();
                        foreach (EffectEvent effect in expulsionEffects)
                        {
                            int endTime = (int)effect.Time + cotDuration;
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 250, 350, ((int)effect.Time, endTime), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 250, 350, (endTime, endTime + 150), "rgba(250, 120, 0, 0.4)", new PositionConnector(effect.Position)));
                        }
                    }
                    // Ring 3
                    if (cot3 != null)
                    {
                        var expulsionEffects = log.CombatData.GetEffectEventsByEffectID(cot3.ContentID).ToList();
                        foreach (EffectEvent effect in expulsionEffects)
                        {
                            int endTime = (int)effect.Time + cotDuration;
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 350, 450, ((int)effect.Time, endTime), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 350, 450, (endTime, endTime + 150), "rgba(250, 120, 0, 0.4)", new PositionConnector(effect.Position)));
                        }
                    }
                    // Ring 4
                    if (cot4 != null)
                    {
                        var expulsionEffects = log.CombatData.GetEffectEventsByEffectID(cot4.ContentID).ToList();
                        foreach (EffectEvent effect in expulsionEffects)
                        {
                            int endTime = (int)effect.Time + cotDuration;
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 450, 550, ((int)effect.Time, endTime), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 450, 550, (endTime, endTime + 150), "rgba(250, 120, 0, 0.4)", new PositionConnector(effect.Position)));
                        }
                    }
                    // Ring 5
                    if (cot5 != null)
                    {
                        var expulsionEffects = log.CombatData.GetEffectEventsByEffectID(cot5.ContentID).ToList();
                        foreach (EffectEvent effect in expulsionEffects)
                        {
                            int endTime = (int)effect.Time + cotDuration;
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 550, 650, ((int)effect.Time, endTime), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 550, 650, (endTime, endTime + 150), "rgba(250, 120, 0, 0.4)", new PositionConnector(effect.Position)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.EchoOfTheUnclean:
                    var causticExplosionEcho = casts.Where(x => x.SkillId == CausticExplosionSiaxEcho).ToList();
                    foreach (AbstractCastEvent c in causticExplosionEcho)
                    {
                        // Duration is the same as Siax's explosion but starts 2 seconds later
                        int duration = 20000;
                        int start = (int)c.Time + 18000;
                        int attackEnd = (int)c.Time + duration;
                        replay.Decorations.Add(new CircleDecoration(true, 0, 3000, (start, attackEnd), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.VolatileHallucinationSiax:
                    break;
                case (int)ArcDPSEnums.TrashID.NightmareHallucinationSiax:
                    break;
                default: break;
            }
        }
    }
}
