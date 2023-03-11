using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;
using System.Collections;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class MAMA : Nightmare
    {
        public MAMA(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(new long[]{ Blastwave1, Blastwave2 }, "Blastwave", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "KB","Blastwave (Spinning Knockback)", "KB Spin",0),
            new PlayerDstHitMechanic(TantrumMAMA, "Tantrum", new MechanicPlotlySetting(Symbols.StarDiamondOpen,Colors.Green), "Tantrum","Tantrum (Double hit or Slams)", "Dual Spin/Slams",700),
            new PlayerDstHitMechanic(Leap, "Leap", new MechanicPlotlySetting(Symbols.TriangleDown,Colors.Red), "Jump","Leap (<33% only)", "Leap",0),
            new PlayerDstHitMechanic(ShootGreenBalls, "Shoot", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Brown), "Shoot","Toxic Shoot (Green Bullets)", "Toxic Shoot",0),
            new PlayerDstHitMechanic(ExplosiveImpact, "Explosive Impact", new MechanicPlotlySetting(Symbols.Circle,Colors.Yellow), "Knight Jump","Explosive Impact (Knight Jump)", "Knight Jump",0),
            new PlayerDstHitMechanic(SweepingStrikes, "Sweeping Strikes", new MechanicPlotlySetting(Symbols.AsteriskOpen,Colors.Red), "Sweep","Swings (Many rapid front spins)", "Sweeping Strikes",200),
            new PlayerDstHitMechanic(NightmareMiasmaMAMA, "Nightmare Miasma", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Magenta), "Goo","Nightmare Miasma (Poison Puddle)", "Poison Goo",700),
            new PlayerDstHitMechanic(new long[] { GrenadeBarrage, GrenadeBarrage2 }, "Grenade Barrage", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Barrage","Grenade Barrage (Red Bullets with AoEs)", "Ball Barrage",0),
            new PlayerDstHitMechanic(new long[] { ShootRedBalls, ShootRedBalls2 }, "Red Ball Shot", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Ball","Shoot (Direct Red Bullets)", "Bullet",0),
            new PlayerDstHitMechanic(Extraction, "Extraction", new MechanicPlotlySetting(Symbols.Bowtie,Colors.LightOrange), "Pull","Extraction (Knight Pull Circle)", "Knight Pull",0),
            new PlayerDstHitMechanic(new long[] { HomingGrenades, HomingGrenades2 }, "Homing Grenades", new MechanicPlotlySetting(Symbols.StarTriangleDownOpen,Colors.Red), "Grenades","Homing Grenades", "Homing Grenades",0),
            new PlayerDstHitMechanic(new long[] { CascadeOfTorment1, CascadeOfTorment2 }, "Cascade of Torment", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightOrange), "Rings","Cascade of Torment (Alternating Rings)", "Rings", 0),
            new PlayerDstHitMechanic(KnightsGaze, "Knight's Daze", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.LightPurple), "Daze","Knight's Daze", "Daze", 0),
            new PlayerDstSkillMechanic(new long[] { NightmareDevastation1, NightmareDevastation3, NightmareDevastation4 }, "Nightmare Devastation", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Blue), "Bubble", "Nightmare Devastation (not stood in Arkk's Shield)", "Bubble", 0),
            });
            Extension = "mama";
            Icon = EncounterIconMAMA;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayMAMA,
                            (664, 407),
                            (1653, 4555, 5733, 7195)/*,
                            (-6144, -6144, 9216, 9216),
                            (11804, 4414, 12444, 5054)*/);
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mama = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.MAMA));
            if (mama == null)
            {
                throw new MissingKeyActorsException("MAMA not found");
            }
            phases[0].AddTarget(mama);
            if (!requirePhases)
            {
                return phases;
            }
            phases.AddRange(GetPhasesByInvul(log, Determined762, mama, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    var ids = new List<int>
                    {
                       (int) ArcDPSEnums.TrashID.GreenKnight,
                       (int) ArcDPSEnums.TrashID.RedKnight,
                       (int) ArcDPSEnums.TrashID.BlueKnight,
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                    if (phase.Targets.Count > 0)
                    {
                        AbstractSingleActor phaseTar = phase.Targets[0];
                        phase.Name = PhaseNames.TryGetValue(phaseTar.ID, out string phaseName) ? phaseName : "Unknown";
                    }
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(mama);
                }
            }
            return phases;
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.MAMA,
                (int)ArcDPSEnums.TrashID.GreenKnight,
                (int)ArcDPSEnums.TrashID.RedKnight,
                (int)ArcDPSEnums.TrashID.BlueKnight
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.TwistedHorror
            };
        }

        private readonly IReadOnlyDictionary<int, string> PhaseNames = new Dictionary<int, string>()
        {
            { (int)ArcDPSEnums.TrashID.GreenKnight, "Green Knight" },
            { (int)ArcDPSEnums.TrashID.RedKnight, "Red Knight" },
            { (int)ArcDPSEnums.TrashID.BlueKnight, "Blue Knight" }
        };

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.MAMA:
                    // AoE Knockback
                    var blastwave = casts.Where(x => x.SkillId == Blastwave1 || x.SkillId == Blastwave2).ToList();
                    foreach (AbstractCastEvent c in blastwave)
                    {
                        int hitTime = (int)c.ExpectedEndTime;
                        int endTime = Math.Min((int)c.GetInterruptedByStunTime(log), hitTime);

                        replay.Decorations.Add(new CircleDecoration(true, hitTime, 550, ((int)c.Time, endTime), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(false, 0, 550, ((int)c.Time, endTime), "rgba(255, 0, 0, 2)", new AgentConnector(target)));
                    }

                    // Leap with shockwaves
                    var leap = casts.Where(x => x.SkillId == Leap).ToList();
                    foreach (AbstractCastEvent c in leap)
                    {
                        int attackStart = (int)c.Time;
                        int hitTime = (int)c.ExpectedEndTime;
                        // Find position at the end of the leap time
                        Point3D targetPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= hitTime + 1000);
                        if (targetPosition == null)
                        {
                            continue;
                        }
                        int attackEnd = Math.Min((int)c.GetInterruptedByStunTime(log), hitTime);
                        int impactRadius = (int)target.HitboxWidth / 2 + 100;
                        replay.Decorations.Add(new CircleDecoration(true, 0, impactRadius, (attackStart, attackEnd), "rgba(255, 120, 0, 0.2)", new PositionConnector(targetPosition)));
                        replay.Decorations.Add(new CircleDecoration(true, hitTime, impactRadius, (attackStart, attackEnd), "rgba(255, 120, 0, 0.2)", new PositionConnector(targetPosition)));
                        // 3 rounds of decorations for the 3 waves
                        if (c.Status != AbstractCastEvent.AnimationStatus.Interrupted && attackEnd >= hitTime)
                        {
                            int shockwaveRadius = 1300;
                            int duration = 2680;
                            for (int i = 0; i < 3; i++)
                            {
                                int shockWaveStart = hitTime + i * 120;
                                replay.Decorations.Add(new CircleDecoration(false, shockWaveStart + duration, shockwaveRadius, (shockWaveStart, shockWaveStart + duration), "rgba(255, 200, 0, 0.3)", new PositionConnector(targetPosition)));
                            }
                        }
                    }

                    // Nightmare Miasma AoE
                    EffectGUIDEvent miasma = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.NightmareMiasmaIndicator);
                    if (miasma != null)
                    {
                        var miasmaEffects = log.CombatData.GetEffectEventsByEffectID(miasma.ContentID).ToList();
                        foreach (EffectEvent miasmaEffect in miasmaEffects)
                        {
                            int durationFirstAoe = 300;
                            int durationSecondAoe = 2000;
                            int growingFirstAoe = (int)miasmaEffect.Time + durationFirstAoe;
                            int growingSecondAoe = (int)miasmaEffect.Time + durationSecondAoe;
                            int startFirstAoe = (int)miasmaEffect.Time;
                            int startSecondAoe = (int)miasmaEffect.Time + durationFirstAoe;
                            int endFirstAndSecondAoe = startFirstAoe + durationFirstAoe + durationSecondAoe;
                            int safeTime = endFirstAndSecondAoe + 1000;
                            int dangerTime = 77000;

                            replay.Decorations.Add(new CircleDecoration(true, growingFirstAoe, 540, (startFirstAoe, endFirstAndSecondAoe), "rgba(250, 120, 0, 0.1)", new PositionConnector(miasmaEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, growingSecondAoe, 540, (startSecondAoe, endFirstAndSecondAoe), "rgba(250, 120, 0, 0.1)", new PositionConnector(miasmaEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, safeTime, 540, (endFirstAndSecondAoe, safeTime), "rgba(83, 30, 25, 0.1)", new PositionConnector(miasmaEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 540, (endFirstAndSecondAoe, safeTime), "rgba(83, 30, 25, 0.1)", new PositionConnector(miasmaEffect.Position)));
                            replay.Decorations.Add(new CircleDecoration(true, 0, 540, (safeTime, endFirstAndSecondAoe + dangerTime), "rgba(83, 30, 25, 0.2)", new PositionConnector(miasmaEffect.Position)));
                        }
                    }

                    // Arkk's Shield
                    EffectGUIDEvent shield = log.CombatData.GetEffectGUIDEvent(EffectGUIDs.ArkkShieldIndicator);
                    if (shield != null)
                    {
                        var shieldEffects = log.CombatData.GetEffectEventsByEffectID(shield.ContentID).ToList();
                        foreach (EffectEvent shieldEffect in shieldEffects)
                        {
                            int duration = 6200;
                            int start = (int)shieldEffect.Time;
                            int effectEnd = start + duration;
                            replay.Decorations.Add(new CircleDecoration(true, 0, 300, (start, effectEnd), "rgba(0, 0, 255, 0.4)", new PositionConnector(shieldEffect.Position)));
                            replay.Decorations.Add(new DoughnutDecoration(true, -effectEnd, 300, 5000, (start, effectEnd), "rgba(255, 0, 0, 0.2)", new PositionConnector(shieldEffect.Position)));
                            replay.Decorations.Add(new DoughnutDecoration(true, 0, 300, 5000, (start, effectEnd), "rgba(255, 0, 0, 0.2)", new PositionConnector(shieldEffect.Position)));
                        }
                    }

                    break;
                case (int)ArcDPSEnums.TrashID.BlueKnight:
                case (int)ArcDPSEnums.TrashID.RedKnight:
                case (int)ArcDPSEnums.TrashID.GreenKnight:
                    // Knockback AoE
                    var explosiveImpact = casts.Where(x => x.SkillId == ExplosiveImpact).ToList();
                    foreach (AbstractCastEvent c in explosiveImpact)
                    {
                        int duration = 3000;
                        int hitTime = (int)c.ExpectedEndTime;
                        int attackEnd = Math.Min((int)c.GetInterruptedByStunTime(log), hitTime);
                        int windUpDuration = (int)c.Time - (attackEnd - duration);
                        int windUpStart = (int)c.Time - windUpDuration;

                        // Wind Up - Knight in air
                        replay.Decorations.Add(new CircleDecoration(true, -(int)c.Time, 600, (windUpStart, (int)c.Time), "rgba(250, 120, 0, 0.1)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, 600, (windUpStart, (int)c.Time), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        // Hit - Knight falling
                        replay.Decorations.Add(new CircleDecoration(true, hitTime, 600, ((int)c.Time, attackEnd), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, 600, ((int)c.Time, attackEnd), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }

                    // Pull AoE
                    var extraction = casts.Where(x => x.SkillId == Extraction).ToList();
                    foreach (AbstractCastEvent c in extraction)
                    {
                        int hitTime = (int)c.ExpectedEndTime;
                        int attackEnd = Math.Min((int)c.GetInterruptedByStunTime(log), hitTime);

                        replay.Decorations.Add(new DoughnutDecoration(true, hitTime, 300, 2000, ((int)c.Time, attackEnd), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                        replay.Decorations.Add(new DoughnutDecoration(true, 0, 300, 2000, ((int)c.Time, attackEnd), "rgba(250, 120, 0, 0.2)", new AgentConnector(target)));
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
