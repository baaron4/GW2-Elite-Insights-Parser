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

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class ValeGuardian : SpiritVale
    {
        public ValeGuardian(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(GreenGuardianUnstableMagicSpike, "Unstable Magic Spike", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Blue), "Split TP","Unstable Magic Spike (Green Guard Teleport)","Green Guard TP",500),
            new PlayerDstHitMechanic(UnstableMagicSpike, "Unstable Magic Spike", new MechanicPlotlySetting(Symbols.Circle,Colors.Blue), "Boss TP","Unstable Magic Spike (Boss Teleport)", "Boss TP",500),
            new PlayerDstHitMechanic(new long[] {DistributedMagicBlue, DistributedMagicRed, DistributedMagic, DistributedMagicGreen }, "Distributed Magic", new MechanicPlotlySetting(Symbols.Circle,Colors.DarkGreen), "Green","Distributed Magic (Stood in Green)", "Green Team",0),
            new EnemyCastStartMechanic(DistributedMagicBlue, "Distributed Magic", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.LightBlue) , "Green Cast B","Distributed Magic (Green Field appeared in Blue Sector)", "Green in Blue",0),
            new EnemyCastStartMechanic(DistributedMagicRed, "Distributed Magic", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Orange), "Green Cast R","Distributed Magic (Green Field appeared in Red Sector)", "Green in Red",0),
            new EnemyCastStartMechanic(DistributedMagicGreen, "Distributed Magic", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Green), "Green Cast G","Distributed Magic (Green Field appeared in Green Sector)", "Green in Green",0),
            new PlayerDstHitMechanic(MagicPulse, "Magic Pulse", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Seeker","Magic Pulse (Hit by Seeker)", "Seeker",0),
            new PlayerDstBuffApplyMechanic(PylonAttunementRed, "Pylon Attunement: Red", new MechanicPlotlySetting(Symbols.Square,Colors.Red), "Attune R","Pylon Attunement: Red", "Red Attuned",0),
            new PlayerDstBuffApplyMechanic(PylonAttunementBlue, "Pylon Attunement: Blue", new MechanicPlotlySetting(Symbols.Square,Colors.Blue), "Attune B","Pylon Attunement: Blue", "Blue Attuned",0),
            new PlayerDstBuffApplyMechanic(PylonAttunementGreen, "Pylon Attunement: Green", new MechanicPlotlySetting(Symbols.Square,Colors.DarkGreen), "Attune G","Pylon Attunement: Green", "Green Attuned",0),
            new EnemyDstBuffRemoveMechanic(BluePylonPower, "Blue Pylon Power", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Blue), "Invuln Strip","Blue Guard Invuln was stripped", "Blue Invuln Strip",0),
            new PlayerDstHitMechanic(UnstablePylonRed, "Unstable Pylon", new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.Red), "Floor R","Unstable Pylon (Red Floor dmg)", "Floor dmg",0),
            new PlayerDstHitMechanic(UnstablePylonBlue, "Unstable Pylon", new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.Blue), "Floor B","Unstable Pylon (Blue Floor dmg)", "Floor dmg",0),
            new PlayerDstHitMechanic(UnstablePylonGreen, "Unstable Pylon", new MechanicPlotlySetting(Symbols.HexagramOpen,Colors.DarkGreen), "Floor G","Unstable Pylon (Green Floor dmg)", "Floor dmg",0),
            new EnemyCastStartMechanic(MagicStorm, "Magic Storm", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","Magic Storm (Breakbar)","Breakbar",0),
            new EnemyCastEndMechanic(MagicStorm, "Magic Storm", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Magic Storm (Breakbar broken) ", "CCed",0).UsingChecker((c, log) => c.ActualDuration <= 8544),
            new EnemyCastEndMechanic(MagicStorm, "Magic Storm", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","Magic Storm (Breakbar failed) ", "CC Fail",0).UsingChecker((c, log) => c.ActualDuration > 8544),
            });
            Extension = "vg";
            Icon = EncounterIconValeGuardian;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayValeGuardian,
                            (889, 889),
                            (-6365, -22213, -3150, -18999)/*,
                            (-15360, -36864, 15360, 39936),
                            (3456, 11012, 4736, 14212)*/);
        }
        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(MagicAuraValeGuardian, MagicAuraValeGuardian),
                new DamageCastFinder(MagicAuraRedGuardian, MagicAuraRedGuardian),
                new DamageCastFinder(MagicAuraBlueGuardian, MagicAuraBlueGuardian),
                new DamageCastFinder(MagicAuraGreenGuardian, MagicAuraGreenGuardian),
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.ValeGuardian,
                (int)ArcDPSEnums.TrashID.RedGuardian,
                (int)ArcDPSEnums.TrashID.BlueGuardian,
                (int)ArcDPSEnums.TrashID.GreenGuardian
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.ValeGuardian));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Vale Guardian not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Invul check
            phases.AddRange(GetPhasesByInvul(log, Invulnerability757, mainTarget, true, true));
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                if (i % 2 == 0)
                {
                    phase.Name = "Split " + (i) / 2;
                    var ids = new List<int>
                    {
                       (int) ArcDPSEnums.TrashID.BlueGuardian,
                       (int) ArcDPSEnums.TrashID.GreenGuardian,
                       (int) ArcDPSEnums.TrashID.RedGuardian
                    };
                    AddTargetsToPhaseAndFit(phase, ids, log);
                }
                else
                {
                    phase.Name = "Phase " + (i + 1) / 2;
                    phase.AddTarget(mainTarget);
                }
            }
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            int curRed = 1;
            int curBlue = 1;
            int curGreen = 1;
            foreach (AbstractSingleActor target in Targets)
            {
                if (target.IsSpecies(ArcDPSEnums.TrashID.RedGuardian))
                {
                    target.OverrideName(target.Character + " " + curRed++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.BlueGuardian))
                {
                    target.OverrideName(target.Character + " " + curBlue++);
                }
                if (target.IsSpecies(ArcDPSEnums.TrashID.GreenGuardian))
                {
                    target.OverrideName(target.Character + " " + curGreen++);
                }
            }
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
               ArcDPSEnums.TrashID.Seekers
            };
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ValeGuardianDistributedMagic, out IReadOnlyList<EffectEvent> distributedMagicEvents))
            {
                int distributedMagicDuration = 6700;
                //knownEffectsIDs.Add(distributedMagicGUIDEvent.ContentID);
                foreach (EffectEvent distributedMagic in distributedMagicEvents)
                {
                    int start = (int)distributedMagic.Time;
                    int expectedEnd = start + distributedMagicDuration;
                    int end = Math.Min(expectedEnd, (int)distributedMagic.Src.LastAware);
                    var circle = new CircleDecoration(180, (start, end), "rgba(0,255,0,0.2)", new PositionConnector(distributedMagic.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(expectedEnd)) ;
                }
            }
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.ValeGuardianMagicSpike, out IReadOnlyList<EffectEvent> magicSpikeEvents))
            {
                //knownEffectsIDs.Add(magicSpikeGUIDEvent.ContentID);
                foreach (EffectEvent magicSpike in magicSpikeEvents)
                {
                    int start = (int)magicSpike.Time;
                    int end = start + 2000;
                    var circle = new CircleDecoration(90, (start, end), "rgba(0,50,255,0.2)", new PositionConnector(magicSpike.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(end));
                }
            }
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            var lifespan = ((int)replay.TimeOffsets.start, (int)replay.TimeOffsets.end);
            //var knownEffectsIDs = new HashSet<long>();
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.ValeGuardian:
                    var magicStorms = cls.Where(x => x.SkillId == MagicStorm).ToList();
                    foreach (AbstractCastEvent c in magicStorms)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        replay.AddDecorationWithGrowing(new CircleDecoration(180, (start, end), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)), start + c.ExpectedDuration);
                    }
                    if (!log.CombatData.HasEffectData)
                    {
                        int distributedMagicDuration = 6700;
                        int impactDuration = 110;
                        int arenaRadius = 1600;
                        var distributedMagicGreen = cls.Where(x => x.SkillId == DistributedMagicGreen).ToList();
                        foreach (AbstractCastEvent c in distributedMagicGreen)
                        {
                            int start = (int)c.Time;
                            int end = start + distributedMagicDuration;
                            var positionConnector = new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f));
                            var rotationConnector = new AngleConnector(151);
                            replay.Decorations.Add(new PieDecoration(arenaRadius, 120, (start, end), "rgba(0,255,0,0.1)", positionConnector).UsingGrowingEnd(start + distributedMagicDuration).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration( arenaRadius, 120, (end, end + impactDuration), "rgba(0,255,0,0.3)", positionConnector).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new CircleDecoration(180, (start, end), "rgba(0,255,0,0.2)", new PositionConnector(new Point3D(-5449.0f, -20219.0f, 0.0f))));
                        }
                        var distributedMagicBlue = cls.Where(x => x.SkillId == DistributedMagicBlue).ToList();
                        foreach (AbstractCastEvent c in distributedMagicBlue)
                        {
                            int start = (int)c.Time;
                            int end = start + distributedMagicDuration;
                            var positionConnector = new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f));
                            var rotationConnector = new AngleConnector(31);
                            replay.Decorations.Add(new PieDecoration(arenaRadius, 120, (start, end), "rgba(0,255,0,0.1)", positionConnector).UsingGrowingEnd(start + distributedMagicDuration).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(arenaRadius, 120, (end, end + impactDuration), "rgba(0,255,0,0.3)", positionConnector).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new CircleDecoration(180, (start, end), "rgba(0,255,0,0.2)", new PositionConnector(new Point3D(-4063.0f, -20195.0f, 0.0f))));
                        }
                        var distributedMagicRed = cls.Where(x => x.SkillId == DistributedMagicRed).ToList();
                        foreach (AbstractCastEvent c in distributedMagicRed)
                        {
                            int start = (int)c.Time;
                            int end = start + distributedMagicDuration;
                            var positionConnector = new PositionConnector(new Point3D(-4749.838867f, -20607.296875f, 0.0f));
                            var rotationConnector = new AngleConnector(271);
                            replay.Decorations.Add(new PieDecoration(arenaRadius, 120, (start, end), "rgba(0,255,0,0.1)", positionConnector).UsingGrowingEnd(start + distributedMagicDuration).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(arenaRadius, 120, (end, end + impactDuration), "rgba(0,255,0,0.3)", positionConnector).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new CircleDecoration(180, (start, end), "rgba(0,255,0,0.2)", new PositionConnector(new Point3D(-4735.0f, -21407.0f, 0.0f))));
                        }
                    } 
                    //CombatReplay.DebugEffects(target, log, replay, knownEffectsIDs, target.FirstAware, target.LastAware);
                    //CombatReplay.DebugUnknownEffects(log, replay, knownEffectsIDs, target.FirstAware, target.LastAware);
                    break;
                case (int)ArcDPSEnums.TrashID.BlueGuardian:
                    replay.Decorations.Add(new CircleDecoration(1500, lifespan, "rgba(0, 0, 255, 0.5)", new AgentConnector(target)).UsingFilled(false));
                    break;
                case (int)ArcDPSEnums.TrashID.GreenGuardian:
                    replay.Decorations.Add(new CircleDecoration(1500, lifespan, "rgba(0, 255, 0, 0.5)", new AgentConnector(target)).UsingFilled(false));
                    break;
                case (int)ArcDPSEnums.TrashID.RedGuardian:
                    replay.Decorations.Add(new CircleDecoration(1500, lifespan, "rgba(255, 0, 0, 0.5)", new AgentConnector(target)).UsingFilled(false));
                    break;
                case (int)ArcDPSEnums.TrashID.Seekers:
                    replay.Decorations.Add(new CircleDecoration(180, lifespan, "rgba(255, 0, 0, 0.5)", new AgentConnector(target)).UsingFilled(false));
                    break;
                default:
                    break;
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            // Attunements Overhead
            replay.AddOverheadIcons(p.GetBuffStatus(log, PylonAttunementBlue, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.SensorBlueOverhead);
            replay.AddOverheadIcons(p.GetBuffStatus(log, PylonAttunementGreen, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.SensorGreenOverhead);
            replay.AddOverheadIcons(p.GetBuffStatus(log, PylonAttunementRed, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), p, ParserIcons.SensorRedOverhead);
        }
    }
}
