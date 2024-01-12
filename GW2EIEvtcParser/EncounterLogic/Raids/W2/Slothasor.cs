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
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicPhaseUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterLogicTimeUtils;
using static GW2EIEvtcParser.EncounterLogic.EncounterImages;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Slothasor : SalvationPass
    {
        public Slothasor(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerDstHitMechanic(TantrumDamage, "Tantrum", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Yellow), "Tantrum","Tantrum (Triple Circles after Ground slamming)", "Tantrum",5000),
            new PlayerDstBuffApplyMechanic(VolatilePoisonBuff, "Volatile Poison", new MechanicPlotlySetting(Symbols.Circle,Colors.Red), "Poison","Volatile Poison Application (Special Action Key)", "Poison (Action Key)",0),
            new PlayerDstHitMechanic(VolatilePoisonSkill, "Volatile Poison", new MechanicPlotlySetting(Symbols.CircleOpen,Colors.Red), "Poison dmg","Stood in Volatile Poison", "Poison dmg",0),
            new PlayerDstHitMechanic(Halitosis, "Halitosis", new MechanicPlotlySetting(Symbols.TriangleRightOpen,Colors.LightOrange), "Breath","Halitosis (Flame Breath)", "Flame Breath",0),
            new PlayerDstHitMechanic(SporeRelease, "Spore Release", new MechanicPlotlySetting(Symbols.Pentagon,Colors.Red), "Shake","Spore Release (Coconut Shake)", "Shake",0),
            new PlayerDstBuffApplyMechanic(MagicTransformation, "Magic Transformation", new MechanicPlotlySetting(Symbols.Hexagram,Colors.Teal), "Slub","Magic Transformation (Ate Magic Mushroom)", "Slub Transform",0), 
            //new Mechanic(Nauseated, "Nauseated", ParseEnum.BossIDS.Slothasor, new MechanicPlotlySetting("diamond-tall-open",Colors.LightPurple), "Slub CD",0), //can be skipped imho, identical person and timestamp as Slub Transform
            new PlayerDstBuffApplyMechanic(FixatedSlothasor, "Fixated", new MechanicPlotlySetting(Symbols.Star,Colors.Magenta), "Fixate","Fixated by Slothasor", "Fixated",0),
            new PlayerDstHitMechanic(new long[] { ToxicCloud1, ToxicCloud2 }, "Toxic Cloud", new MechanicPlotlySetting(Symbols.PentagonOpen,Colors.DarkGreen), "Floor","Toxic Cloud (stood in green floor poison)", "Toxic Floor",0),
            new PlayerDstBuffApplyMechanic(Fear, "Fear", new MechanicPlotlySetting(Symbols.SquareOpen,Colors.Red), "Fear","Hit by fear after breakbar", "Feared",0).UsingChecker((ba,log) => ba.AppliedDuration == 8000),
            new EnemyDstBuffApplyMechanic(NarcolepsyBuff, "Narcolepsy", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkTeal), "CC","Narcolepsy (Breakbar)", "Breakbar",0),
            new EnemyDstBuffRemoveMechanic(NarcolepsyBuff, "Narcolepsy", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.Red), "CC Fail","Narcolepsy (Failed CC)", "CC Fail",0).UsingChecker((br,log) => br.RemovedDuration > 120000),
            new EnemyDstBuffRemoveMechanic(NarcolepsyBuff, "Narcolepsy", new MechanicPlotlySetting(Symbols.DiamondTall,Colors.DarkGreen), "CCed","Narcolepsy (Breakbar broken)", "CCed",0).UsingChecker( (br,log) => br.RemovedDuration <= 120000),
            new PlayerDstBuffApplyMechanic(SlipperySlubling, "Slippery Slubling", new MechanicPlotlySetting(Symbols.Star,Colors.Yellow), "Slppr.Slb","Slippery Slubling", "Slippery Slubling",0),
            });
            Extension = "sloth";
            Icon = EncounterIconSlothasor;
            EncounterCategoryInformation.InSubCategoryOrder = 0;
            EncounterID |= 0x000001;
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap(CombatReplaySlothasor,
                            (654, 1000),
                            (5822, -3491, 9549, 2205)/*,
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210)*/);
        }

        protected override void SetInstanceBuffs(ParsedEvtcLog log)
        {
            base.SetInstanceBuffs(log);

            if (log.FightData.Success && log.CombatData.GetBuffData(SlipperySlubling).Any())
            {
                InstanceBuffs.AddRange(GetOnPlayerCustomInstanceBuff(log, SlipperySlubling));
            }
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDs()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Slubling1,
                ArcDPSEnums.TrashID.Slubling2,
                ArcDPSEnums.TrashID.Slubling3,
                ArcDPSEnums.TrashID.Slubling4,
                ArcDPSEnums.TrashID.PoisonMushroom
            };
        }

        internal override List<InstantCastFinder> GetInstantCastFinders()
        {
            return new List<InstantCastFinder>()
            {
                new DamageCastFinder(VolatileAura, VolatileAura),
                new BuffLossCastFinder(PurgeSlothasor, VolatilePoisonBuff),
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long fightEnd = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            AbstractSingleActor mainTarget = Targets.FirstOrDefault(x => x.IsSpecies(ArcDPSEnums.TargetID.Slothasor));
            if (mainTarget == null)
            {
                throw new MissingKeyActorsException("Slothasor not found");
            }
            phases[0].AddTarget(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            var sleepy = mainTarget.GetCastEvents(log, log.FightData.FightStart, fightEnd).Where(x => x.SkillId == NarcolepsySkill).ToList();
            long start = 0;
            int i = 1;
            foreach (AbstractCastEvent c in sleepy)
            {
                var phase = new PhaseData(start, Math.Min(c.Time, fightEnd), "Phase " + i++);
                phase.AddTarget(mainTarget);
                start = c.EndTime;
                phases.Add(phase);
            }
            var lastPhase = new PhaseData(start, fightEnd, "Phase " + i++);
            lastPhase.AddTarget(mainTarget);
            phases.Add(lastPhase);
            return phases;
        }

        internal override void EIEvtcParse(ulong gw2Build, int evtcVersion, FightData fightData, AgentData agentData, List<CombatItem> combatData, IReadOnlyDictionary<uint, AbstractExtensionHandler> extensions)
        {
            // Mushrooms
            var mushroomAgents = combatData.Where(x => x.DstAgent == 14940 && x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate).Select(x => agentData.GetAgent(x.SrcAgent, x.Time)).Where(x => x.Type == AgentItem.AgentType.Gadget && (x.HitboxWidth == 146 || x.HitboxWidth == 210) && x.HitboxHeight == 300).ToList();
            if (mushroomAgents.Any())
            {
                int idToKeep = mushroomAgents.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.Count()).MaxBy(x => x.Value).Key;
                foreach (AgentItem mushroom in mushroomAgents)
                {
                    if (!mushroom.IsSpecies(idToKeep))
                    {
                        continue;
                    }
                    var copyEventsFrom = new List<AgentItem>() { mushroom };
                    var hpUpdates = combatData.Where(x => x.SrcMatchesAgent(mushroom) && x.IsStateChange == ArcDPSEnums.StateChange.HealthUpdate).ToList();
                    var aliveUpdates = hpUpdates.Where(x => x.DstAgent == 10000).ToList();
                    var deadUpdates = hpUpdates.Where(x => x.DstAgent == 0).ToList();
                    long lastDeadTime = long.MinValue;
                    var posFacingHP = combatData.Where(x => x.SrcMatchesAgent(mushroom) && (x.IsStateChange == ArcDPSEnums.StateChange.Position || x.IsStateChange == ArcDPSEnums.StateChange.Rotation || x.IsStateChange == ArcDPSEnums.StateChange.MaxHealthUpdate)).ToList();
                    foreach (CombatItem aliveEvent in aliveUpdates)
                    {
                        CombatItem deadEvent = deadUpdates.FirstOrDefault(x => x.Time > lastDeadTime && x.Time > aliveEvent.Time);
                        if (deadEvent == null)
                        {
                            lastDeadTime = Math.Min(fightData.LogEnd, mushroom.LastAware);
                        } 
                        else
                        {
                            lastDeadTime = deadEvent.Time;
                        }
                        AgentItem aliveMushroom = agentData.AddCustomNPCAgent(aliveEvent.Time, lastDeadTime, mushroom.Name, mushroom.Spec, ArcDPSEnums.TrashID.PoisonMushroom, false, mushroom.Toughness, mushroom.Healing, mushroom.Condition, mushroom.Concentration, mushroom.HitboxWidth, mushroom.HitboxHeight);
                        RedirectEventsAndCopyPreviousStates(combatData, extensions, agentData, mushroom, copyEventsFrom, aliveMushroom, false);
                        copyEventsFrom.Add(aliveMushroom);
                    }
                }
                agentData.Refresh();
            }
            ComputeFightTargets(agentData, combatData, extensions);
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastEvents(log, log.FightData.FightStart, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Slothasor:
                    var sleepy = cls.Where(x => x.SkillId == NarcolepsySkill).ToList();
                    foreach (AbstractCastEvent c in sleepy)
                    {
                        replay.Decorations.Add(new CircleDecoration(180, ((int)c.Time, (int)c.EndTime), Colors.LightBlue, 0.3, new AgentConnector(target)));
                    }
                    var breath = cls.Where(x => x.SkillId == Halitosis).ToList();
                    foreach (AbstractCastEvent c in breath)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 1000;
                        int duration = 2000;
                        uint range = 600;
                        Point3D facing = target.GetCurrentRotation(log, start + 1000);
                        if (facing != null)
                        {
                            var connector = new AgentConnector(target);
                            var rotationConnector = new AngleConnector(facing);
                            var openingAngle = 60;
                            replay.Decorations.Add(new PieDecoration(range, openingAngle, (start, start + preCastTime), Colors.Orange, 0.1, connector).UsingRotationConnector(rotationConnector));
                            replay.Decorations.Add(new PieDecoration(range, openingAngle, (start + preCastTime, start + preCastTime + duration), Colors.Orange, 0.4, connector).UsingRotationConnector(rotationConnector));
                        }
                    }
                    var tantrum = cls.Where(x => x.SkillId == TantrumSkill).ToList();
                    foreach (AbstractCastEvent c in tantrum)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        var circle = new CircleDecoration(300, (start, end), Colors.LightOrange, 0.4, new AgentConnector(target));
                        replay.AddDecorationWithFilledWithGrowing(circle.UsingFilled(false), true, end);
                    }
                    var shakes = cls.Where(x => x.SkillId == SporeRelease).ToList();
                    foreach (AbstractCastEvent c in shakes)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        var circle = new CircleDecoration( 700, (start, end), Colors.Red, 0.4, new AgentConnector(target));
                        replay.AddDecorationWithFilledWithGrowing(circle.UsingFilled(false), true, end);
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.PoisonMushroom:
                    break;
                default:
                    break;
            }

        }

        internal override void ComputePlayerCombatReplayActors(AbstractPlayer p, ParsedEvtcLog log, CombatReplay replay)
        {
            base.ComputePlayerCombatReplayActors(p, log, replay);
            // Poison
            var poisonToDrop = p.GetBuffStatus(log, VolatilePoisonBuff, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in poisonToDrop)
            {
                int toDropStart = (int)seg.Start;
                int toDropEnd = (int)seg.End;
                var circle = new CircleDecoration( 180, seg, "rgba(255, 255, 100, 0.5)", new AgentConnector(p));
                replay.AddDecorationWithFilledWithGrowing(circle.UsingFilled(false), true, toDropStart + 8000);
                Point3D position = p.GetCurrentInterpolatedPosition(log, toDropEnd);
                if (position != null)
                {
                    replay.Decorations.Add(new CircleDecoration(900, 180, (toDropEnd, toDropEnd + 90000), Colors.Red, 0.3, new PositionConnector(position)).UsingGrowingEnd(toDropStart + 90000));
                }
                replay.AddOverheadIcon(seg, p, ParserIcons.VolatilePoisonOverhead);
            }
            // Transformation
            var slubTrans = p.GetBuffStatus(log, MagicTransformation, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in slubTrans)
            {
                replay.Decorations.Add(new CircleDecoration(180, seg, "rgba(0, 80, 255, 0.3)", new AgentConnector(p)));
            }
            // Fixated
            var fixatedSloth = p.GetBuffStatus(log, FixatedSlothasor, log.FightData.FightStart, log.FightData.FightEnd).Where(x => x.Value > 0).ToList();
            foreach (Segment seg in fixatedSloth)
            {
                replay.Decorations.Add(new CircleDecoration(120, seg, "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                replay.AddOverheadIcon(seg, p, ParserIcons.FixationPurpleOverhead);
            }
        }
    }
}
