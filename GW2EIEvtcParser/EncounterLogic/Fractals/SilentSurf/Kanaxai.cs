using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.Extensions;
using GW2EIEvtcParser.ParsedData;
using GW2EIEvtcParser.ParserHelpers;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Kanaxai : SilentSurf
    {
        public Kanaxai(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new PlayerDstHitMechanic(RendingStormSkill, "Rending Storm", new MechanicPlotlySetting(Symbols.CircleXOpen, Colors.Red), "RendStm.H", "Hit by Rending Storm (Axe AoE)", "Rending Storm Hit", 0),
                new PlayerDstHitMechanic(new long [] { HarrowshotDeath, HarrowshotExposure, HarrowshotFear, HarrowshotLethargy, HarrowshotTorment }, "Harrowshot", new MechanicPlotlySetting(Symbols.Circle, Colors.Orange), "Harrowshot.H", "Harrowshot (Lost all boons)", "Harrowshot (Boonstrip)", 0),
                new PlayerDstBuffApplyMechanic(ExtremeVulnerability, "Extreme Vulnerability", new MechanicPlotlySetting(Symbols.X, Colors.DarkRed), "ExtVuln.A", "Applied Extreme Vulnerability", "Extreme Vulnerability Application", 150),
                new PlayerDstBuffApplyMechanic(ExposedPlayer, "Exposed", new MechanicPlotlySetting(Symbols.TriangleRight, Colors.Pink), "Expo.A", "Applied Exposed", "Exposed Application (Player)", 0),
                new PlayerDstBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting(Symbols.TriangleUp, Colors.Yellow), "Fear.A", "Fear Applied", "Fear Application", 150),
                new PlayerDstBuffApplyMechanic(Phantasmagoria, "Phantasmagoria", new MechanicPlotlySetting(Symbols.Diamond, Colors.Pink), "Phant.A", "Phantasmagoria Applied (Aspect visible on Island)", "Phantasmagoria Application", 150),
                new EnemyDstBuffApplyMechanic(Exposed31589, "Exposed", new MechanicPlotlySetting(Symbols.TriangleLeft, Colors.Pink), "Expo.A", "Applied Exposed to Kanaxai", "Exposed Application (Kanaxai)", 150),
                new PlayerDstBuffRemoveMechanic(ExtremeVulnerability, "Dread Visage", new MechanicPlotlySetting(Symbols.Bowtie, Colors.DarkRed), "Eye.D", "Died to Dread Visage (Eye)", "Dread Visage Death", 150)
                    .UsingChecker((remove, log) =>
                    {
                        // 5s extreme vulnerability from dread visage
                        const int duration = 5000;
                        // find last apply
                        BuffApplyEvent apply = log.CombatData.GetBuffData(ExtremeVulnerability)
                            .OfType<BuffApplyEvent>()
                            .Where(e => e.Time <= remove.Time && e.To == remove.To)
                            .MaxBy(e => e.Time);
                        // check for removed duration, applied duration & death within 1s after
                        return remove.RemovedDuration > ServerDelayConstant
                            && Math.Abs(apply.AppliedDuration - duration) < ServerDelayConstant
                            && log.CombatData.GetDeadEvents(remove.To).Any(dead =>
                            {
                                long diff = dead.Time - remove.Time;
                                return diff > -ServerDelayConstant && diff <= 1000;
                            });
                    }),
                new PlayerDstBuffRemoveMechanic(ExtremeVulnerability, "Frightening Speed", new MechanicPlotlySetting(Symbols.Circle, Colors.DarkRed), "Numbers.D", "Died to Frightening Speed (Numbers)", "Frightening Speed Death", 150)
                    .UsingChecker((remove, log) =>
                    {
                        // 60s extreme vulnerability from frightening speed
                        const int duration = 60000;
                        // find last apply
                        BuffApplyEvent apply = log.CombatData.GetBuffData(ExtremeVulnerability)
                            .OfType<BuffApplyEvent>()
                            .Where(e => e.Time <= remove.Time && e.To == remove.To)
                            .MaxBy(e => e.Time);
                        // check for removed duration, applied duration & death within 1s after
                        return remove.RemovedDuration > ServerDelayConstant
                            && Math.Abs(apply.AppliedDuration - duration) < ServerDelayConstant
                            && log.CombatData.GetDeadEvents(remove.To).Any(dead =>
                            {
                                long diff = dead.Time - remove.Time;
                                return diff > -ServerDelayConstant && diff <= 1000;
                            });
                    }),
                new PlayerDstBuffApplyMechanic(new long [] { RendingStormAxeTargetBuff1, RendingStormAxeTargetBuff2 }, "Rending Storm Target", new MechanicPlotlySetting(Symbols.CircleX, Colors.LightPurple), "RendStm.T", "Targetted by Rending Storm (Axe Throw)", "Rending Storm Target", 150),
            });
            Extension = "kanaxai";
            Icon = EncounterIconKanaxai;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplayKanaxai,
                           (334, 370),
                           (-6195, -295, -799, 5685));
        }

        protected override HashSet<int> GetUniqueNPCIDs()
        {
            return new HashSet<int>
            {
                (int)ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM,
            };
        }

        protected override List<int> GetTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM,
                (int)ArcDPSEnums.TrashID.AspectOfTorment,
                (int)ArcDPSEnums.TrashID.AspectOfLethargy,
                (int)ArcDPSEnums.TrashID.AspectOfExposure,
                (int)ArcDPSEnums.TrashID.AspectOfDeath,
                (int)ArcDPSEnums.TrashID.AspectOfFear,
            };
        }

        internal override FightData.EncounterMode GetEncounterMode(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return FightData.EncounterMode.CMNoName;
        }

        internal override void EIEvtcParse(ulong gw2Build, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            base.EIEvtcParse(gw2Build, fightData, agentData, combatData, extensions);
            var aspectCounts = new Dictionary<int, int>();
            foreach (AbstractSingleActor actor in Targets)
            {
                switch (actor.ID)
                {
                    case (int)ArcDPSEnums.TrashID.AspectOfTorment:
                    case (int)ArcDPSEnums.TrashID.AspectOfLethargy:
                    case (int)ArcDPSEnums.TrashID.AspectOfExposure:
                    case (int)ArcDPSEnums.TrashID.AspectOfDeath:
                    case (int)ArcDPSEnums.TrashID.AspectOfFear:
                        if (aspectCounts.TryGetValue(actor.ID, out int count))
                        {
                            actor.OverrideName(actor.Character + " " + count);
                            aspectCounts[actor.ID] = count + 1;
                        }
                        else
                        {
                            actor.OverrideName(actor.Character + " 1");
                            aspectCounts[actor.ID] = 2;
                        }
                        break;
                }
            }
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM));
            if (kanaxai == null)
            {
                throw new MissingKeyActorsException("Kanaxai not found");
            }
            phases[0].AddTarget(kanaxai);
            if (!requirePhases)
            {
                return phases;
            }
            // Phases
            List<PhaseData> encounterPhases = GetPhasesByInvul(log, DeterminedToDestroy, kanaxai, true, true);

            var worldCleaverPhaseStarts = log.CombatData.GetBuffData(DeterminedToDestroy).OfType<BuffApplyEvent
                >().Where(x => x.To == kanaxai.AgentItem).Select(x => x.Time).ToList();
            int worldCleaverCount = 0;
            int repeatedCount = 0;
            var isRepeatedWorldCleaverPhase = new List<bool>();
            for (int i = 0; i < encounterPhases.Count; i++)
            {
                PhaseData curPhase = encounterPhases[i];
                if (worldCleaverPhaseStarts.Any(x => curPhase.Start == x))
                {
                    var baseName = "World Cleaver ";
                    long midPhase = (curPhase.Start + curPhase.End) / 2;
                    if (kanaxai.GetCurrentHealthPercent(log, midPhase) > 50)
                    {
                        if (repeatedCount == 0)
                        {
                            isRepeatedWorldCleaverPhase.Add(false);
                            curPhase.Name = baseName + (++worldCleaverCount);
                        }
                        else
                        {
                            isRepeatedWorldCleaverPhase.Add(true);
                            curPhase.Name = baseName + (worldCleaverCount) + " Repeated " + repeatedCount;
                        }
                        repeatedCount++;
                    }
                    else if (kanaxai.GetCurrentHealthPercent(log, midPhase) > 25)
                    {
                        if (worldCleaverCount == 1)
                        {
                            repeatedCount = 0;
                        }
                        if (repeatedCount == 0)
                        {
                            isRepeatedWorldCleaverPhase.Add(false);
                            curPhase.Name = baseName + (++worldCleaverCount);
                        }
                        else
                        {
                            isRepeatedWorldCleaverPhase.Add(true);
                            curPhase.Name = baseName + (worldCleaverCount) + " Repeated " + repeatedCount;
                        }
                        repeatedCount++;
                    }
                    else
                    {
                        // No hp update events, buggy log
                        return phases;
                    }
                    foreach (AbstractSingleActor aspect in Targets)
                    {
                        switch (aspect.ID)
                        {
                            case (int)ArcDPSEnums.TrashID.AspectOfTorment:
                            case (int)ArcDPSEnums.TrashID.AspectOfLethargy:
                            case (int)ArcDPSEnums.TrashID.AspectOfExposure:
                            case (int)ArcDPSEnums.TrashID.AspectOfDeath:
                            case (int)ArcDPSEnums.TrashID.AspectOfFear:
                                if (log.CombatData.GetBuffRemoveAllData(Determined762).Any(x => x.To == aspect.AgentItem && x.Time >= curPhase.Start && x.Time <= curPhase.End))
                                {
                                    curPhase.AddTarget(aspect);
                                }
                                break;
                        }
                    }
                    curPhase.AddTarget(kanaxai);
                }
                else
                {
                    isRepeatedWorldCleaverPhase.Add(false);
                }
            }
            // Handle main phases after world cleave phases as we need to know if it is a repeated phase
            int phaseCount = 0;
            for (int i = 0; i < encounterPhases.Count; i++)
            {
                PhaseData curPhase = encounterPhases[i];
                if (!worldCleaverPhaseStarts.Any(x => curPhase.Start == x))
                {
                    var baseName = "Phase ";
                    if (i < isRepeatedWorldCleaverPhase.Count - 1)
                    {
                        if (isRepeatedWorldCleaverPhase[i + 1])
                        {
                            curPhase.Name = baseName + (phaseCount) + " Repeated " + (++repeatedCount);
                        }
                        else
                        {
                            curPhase.Name = baseName + (++phaseCount);
                            repeatedCount = 0;
                        }
                    }
                    else
                    {
                        curPhase.Name = baseName + (++phaseCount);
                    }
                    curPhase.AddTarget(kanaxai);
                }
            }
            phases.AddRange(encounterPhases);

            return phases;
        }

        internal override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, IReadOnlyCollection<AgentItem> playerAgents)
        {
            AbstractSingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM));
            if (kanaxai == null)
            {
                throw new MissingKeyActorsException("Kanaxai not found");
            }
            BuffApplyEvent invul762Gain = combatData.GetBuffData(Determined762).OfType<BuffApplyEvent>().Where(x => x.To == kanaxai.AgentItem).FirstOrDefault();
            if (invul762Gain != null)
            {
                fightData.SetSuccess(true, invul762Gain.Time);
            }
        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer player, ParsedEvtcLog log, CombatReplay replay)
        {
            long maxEnd = log.FightData.FightEnd;

            // Orange Tether from Aspect to player
            IEnumerable<AbstractBuffEvent> tethers = log.CombatData.GetBuffData(AspectTetherBuff).Where(x => x.To == player.AgentItem);
            IEnumerable<BuffApplyEvent> tetherApplies = tethers.OfType<BuffApplyEvent>();
            IEnumerable<BuffRemoveAllEvent> tetherRemoves = tethers.OfType<BuffRemoveAllEvent>();
            AgentItem tetherAspect = _unknownAgent;
            foreach (BuffApplyEvent apply in tetherApplies)
            {
                tetherAspect = apply.By == _unknownAgent ? tetherAspect : apply.By;
                int start = (int)apply.Time;
                BuffApplyEvent replace = tetherApplies.FirstOrDefault(x => x.Time >= apply.Time && x.By != tetherAspect);
                BuffRemoveAllEvent remove = tetherRemoves.FirstOrDefault(x => x.Time >= apply.Time);
                long end = Math.Min(replace?.Time ?? maxEnd, remove?.Time ?? maxEnd);
                replay.Decorations.Add(new LineDecoration((start, (int)end), "rgba(255, 200, 0, 0.5)", new AgentConnector(tetherAspect), new AgentConnector(player)));
            }

            // Blue tether from Aspect to player, appears when the player gains Phantasmagoria
            // Custom decoration not visible in game
            List<AbstractBuffEvent> phantasmagorias = GetFilteredList(log.CombatData, Phantasmagoria, player, true, true);
            replay.AddTether(phantasmagorias, "rgba(0, 100, 255, 0.5)");

            // Rending Storm - Axe AoE attached to players - There are 2 buffs for the targetting
            IEnumerable<Segment> axes = player.GetBuffStatus(log, new long[] { RendingStormAxeTargetBuff1, RendingStormAxeTargetBuff2 }, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            foreach (Segment segment in axes)
            {
                replay.AddDecorationWithGrowing(new CircleDecoration(180, segment, "rgba(200, 120, 0, 0.2)", new AgentConnector(player)), segment.End);
            }

            // Frightening Speed - Numbers spread AoEs
            IEnumerable<Segment> spreads = player.GetBuffStatus(log, KanaxaiSpreadOrangeAoEBuff, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0);
            foreach (Segment spreadSegment in spreads)
            {
                replay.Decorations.Add(new CircleDecoration(380, spreadSegment, "rgba(200, 120, 0, 0.2)", new AgentConnector(player)));
            }

            // Target Order Overhead
            replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder1, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder1Overhead);
            replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder2, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder2Overhead);
            replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder3, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder3Overhead);
            replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder4, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder4Overhead);
            replay.AddOverheadIcons(player.GetBuffStatus(log, TargetOrder5, log.FightData.LogStart, log.FightData.LogEnd).Where(x => x.Value > 0), player, ParserIcons.TargetOrder5Overhead);
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> casts = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);

            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM:
                    // World Cleaver
                    var worldCleaver = casts.Where(x => x.SkillId == WorldCleaver).ToList();
                    foreach (AbstractCastEvent c in worldCleaver)
                    {
                        int duration = 26320;
                        int start = (int)c.Time;
                        IEnumerable<AbstractHealthDamageEvent> hits = log.CombatData.GetDamageData(WorldCleaver).Where(x => x.Time > c.Time);
                        if (hits.Any())
                        {
                            AddWorldCleaverDecoration(target, replay, start, (int)hits.FirstOrDefault(x => x.Time > c.Time).Time, start + duration);
                        }
                        else
                        {
                            AddWorldCleaverDecoration(target, replay, start, start + duration, start + duration);
                        }
                    }
                    // Dread Visage
                    var dreadVisage = casts.Where(x => x.SkillId == DreadVisageKanaxaiSkill || x.SkillId == DreadVisageKanaxaiSkillIsland).ToList();
                    foreach (AbstractCastEvent c in dreadVisage)
                    {
                        int castDuration = 5400;
                        int expectedEndCastTime = (int)c.Time + castDuration;
                        Segment quickness = target.GetBuffStatus(log, Quickness, c.Time, expectedEndCastTime).Where(x => x.Value == 1).FirstOrDefault();
                        if (quickness != null)
                        {
                            long quicknessTimeDuringCast = Math.Min(expectedEndCastTime, quickness.End) - Math.Max((int)c.Time, quickness.Start);
                            double actualDuration = castDuration - quicknessTimeDuringCast + (quicknessTimeDuringCast * 0.66);
                            replay.AddOverheadIcon(new Segment((int)c.Time, (int)c.Time + (int)Math.Ceiling(actualDuration), 1), target, ParserIcons.EyeOverhead, 30);
                        }
                        else
                        {
                            replay.AddOverheadIcon(new Segment((int)c.Time, expectedEndCastTime, 1), target, ParserIcons.EyeOverhead, 30);
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.AspectOfTorment:
                case (int)ArcDPSEnums.TrashID.AspectOfLethargy:
                case (int)ArcDPSEnums.TrashID.AspectOfExposure:
                case (int)ArcDPSEnums.TrashID.AspectOfDeath:
                case (int)ArcDPSEnums.TrashID.AspectOfFear:
                    // Tether casts performed by Aspects
                    IEnumerable<AnimatedCastEvent> tetherCasts = log.CombatData.GetAnimatedCastData(target.AgentItem).Where(x => x.SkillId == AspectTetherSkill);
                    foreach (AnimatedCastEvent cast in tetherCasts)
                    {
                        int start = (int)cast.Time;
                        int end = (int)cast.ExpectedEndTime; // actual end is often much later, just use expected end for short highlight
                        replay.Decorations.Add(new CircleDecoration(180, 20, (start, end), "rgba(0, 100, 255, 0.5)", new AgentConnector(target)).UsingFilled(false));
                    }
                    // Dread Visage
                    var dreadVisageAspects = casts.Where(x => x.SkillId == DreadVisageAspectSkill).ToList();
                    // Check if the log contains Sugar Rush
                    bool hasSugarRush = log.CombatData.GetBuffData(MistlockInstabilitySugarRush).Any(x => x.To.IsPlayer);

                    foreach (AbstractCastEvent c in dreadVisageAspects)
                    {
                        int castDuration = 5400;
                        int expectedEndCastTime = (int)c.Time + castDuration;
                        
                        Segment quickness = target.GetBuffStatus(log, Quickness, c.Time, expectedEndCastTime).Where(x => x.Value == 1).FirstOrDefault();

                        // If the aspect has Sugar Rush AND Quickness
                        if (hasSugarRush && quickness != null)
                        {
                            long quicknessTimeDuringCast = Math.Min(expectedEndCastTime, quickness.End) - Math.Max((int)c.Time, quickness.Start);
                            double castTimeWithSugarRush = castDuration * 0.8;
                            double actualFinalDuration = castTimeWithSugarRush - quicknessTimeDuringCast + (quicknessTimeDuringCast * 0.66 / 0.8);
                            replay.AddOverheadIcon(new Segment((int)c.Time, (int)c.Time + (int)Math.Ceiling(actualFinalDuration), 1), target, ParserIcons.EyeOverhead, 30);
                        }

                        // If the aspect has Sugar rush AND NOT Quickness
                        if (hasSugarRush && quickness == null)
                        {
                            replay.AddOverheadIcon(new Segment((int)c.Time, (int)Math.Ceiling((int)c.Time + castDuration * 0.8), 1), target, ParserIcons.EyeOverhead, 30);
                        }

                        // If the aspect DOESN'T have Sugar rush but HAS Quickness
                        if (!hasSugarRush && quickness != null)
                        {
                            long quicknessTimeDuringCast = Math.Min(expectedEndCastTime, quickness.End) - Math.Max((int)c.Time, quickness.Start);
                            double actualDuration = castDuration - quicknessTimeDuringCast + (quicknessTimeDuringCast * 0.66);
                            replay.AddOverheadIcon(new Segment((int)c.Time, (int)c.Time + (int)Math.Ceiling(actualDuration), 1), target, ParserIcons.EyeOverhead, 30);
                        }

                        // If the aspect DOESN'T have Sugar Rush and Quickness
                        if (!hasSugarRush && quickness == null)
                        {
                            replay.AddOverheadIcon(new Segment((int)c.Time, expectedEndCastTime, 1), target, ParserIcons.EyeOverhead, 30);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        internal override void ComputeEnvironmentCombatReplayDecorations(ParsedEvtcLog log)
        {
            base.ComputeEnvironmentCombatReplayDecorations(log);

            // Frightening Speed - Red AoE
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.FrighteningSpeedRedAoE, out IReadOnlyList<EffectEvent> frighteningSpeedRedAoEs))
            {
                foreach (EffectEvent aoe in frighteningSpeedRedAoEs)
                {
                    int duration = 1500;
                    int start = (int)aoe.Time;
                    int effectEnd = start + duration;
                    var circle = new CircleDecoration( 380, (start, effectEnd), "rgba(255, 0, 0, 0.2)", new PositionConnector(aoe.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.Copy().UsingFilled(false));
                }
            }

            // Rending Storm - Red Axe AoE
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.AxeGroundAoE, out IReadOnlyList<EffectEvent> axeAoEs))
            {
                // Get World Cleaver casts
                AbstractSingleActor kanaxai = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.KanaxaiScytheOfHouseAurkusCM));
                IReadOnlyList<AbstractCastEvent> casts = kanaxai.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
                
                // Get Axe AoE Buffs
                var axes = new List<AbstractBuffEvent>();
                axes.AddRange(log.CombatData.GetBuffData(RendingStormAxeTargetBuff1));
                axes.AddRange(log.CombatData.GetBuffData(RendingStormAxeTargetBuff2));
                var orderedAxes = axes.OfType<BuffRemoveAllEvent>().OrderBy(x => x.Time).ToList();

                foreach (EffectEvent aoe in axeAoEs)
                {
                    // Find the first cast time event present after the AoE effect time
                    AbstractCastEvent cast = casts.Where(x => x.SkillId == WorldCleaver).FirstOrDefault(x => x.Time > aoe.Time);
                    long worldCleaverTime = cast != null ? cast.Time : 0;

                    // Find the first BuffRemoveAllEvent after the AoE effect Time or next World Cleaver cast time
                    // World Cleaver is the time-limit of when the AoEs reset, in third phase we use FightEnd
                    if (worldCleaverTime != 0)
                    {
                        AbstractBuffEvent axeBuffRemoval = orderedAxes.FirstOrDefault(buff => buff.Time > aoe.Time && buff.Time < worldCleaverTime);
                        AddAxeAoeDecoration(aoe, axeBuffRemoval, worldCleaverTime);
                    }
                    else
                    {
                        AbstractBuffEvent axeBuffRemoval = orderedAxes.FirstOrDefault(buff => buff.Time > aoe.Time);
                        AddAxeAoeDecoration(aoe, axeBuffRemoval, log.FightData.FightEnd);
                    }
                }
            }

            // Harrowshot - Boonstrip AoE
            if (log.CombatData.TryGetEffectEventsByGUID(EffectGUIDs.HarrowshotAoE, out IReadOnlyList<EffectEvent> harrowshots))
            {
                foreach (EffectEvent harrowshot in harrowshots)
                {
                    int duration = 3000;
                    int start = (int)harrowshot.Time;
                    int end = (int)harrowshot.Time + duration;
                    var circle = new CircleDecoration(280, (start, end), "rgba(255, 120, 0, 0.2)", new PositionConnector(harrowshot.Position));
                    EnvironmentDecorations.Add(circle);
                    EnvironmentDecorations.Add(circle.Copy().UsingGrowingEnd(end));
                }
            }
        }

        /// <summary>
        /// Adds the Axe AoE decoration.<br></br>
        /// If the next orange AoE <see cref="BuffRemoveAllEvent"/> on players is after <see cref="WorldCleaver"/> cast time or not present,<br></br>
        /// utilise the <see cref="WorldCleaver"/> cast time or <see cref="FightData.LogEnd"/>.
        /// </summary>
        /// <param name="aoe">Effect of the AoE.</param>
        /// <param name="axeBuffRemoval">Buff removal of the orange AoE.</param>
        /// <param name="time">Last time possible.</param>
        private void AddAxeAoeDecoration(EffectEvent aoe, AbstractBuffEvent axeBuffRemoval, long time)
        {
            int duration;
            if (axeBuffRemoval != null)
            {
                duration = (int)(axeBuffRemoval.Time - aoe.Time);
            }
            else
            {
                duration = (int)(time - aoe.Time);
            }
            int start = (int)aoe.Time;
            int effectEnd = start + duration;
            var circle = new CircleDecoration(180, (start, effectEnd), "rgba(255, 0, 0, 0.2)", new PositionConnector(aoe.Position));
            EnvironmentDecorations.Add(circle);
            EnvironmentDecorations.Add(circle.Copy().UsingFilled(false));
        }

        /// <summary>
        /// Adds the World Cleaver decoration.
        /// </summary>
        /// <param name="target">Kanaxai.</param>
        /// <param name="replay">Combat Replay.</param>
        /// <param name="start">Start of the cast.</param>
        /// <param name="end">End of the cast.</param>
        /// <param name="growing">Duration of the channel.</param>
        private static void AddWorldCleaverDecoration(NPC target, CombatReplay replay, int start, int end, int growing)
        {
            replay.AddDecorationWithGrowing(new CircleDecoration(1100, (start, end), "rgba(255, 55, 0, 0.2)", new AgentConnector(target)), growing);
        }
    }
}
