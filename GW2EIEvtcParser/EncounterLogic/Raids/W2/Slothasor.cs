using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Slothasor : RaidLogic
    {
        public Slothasor(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(34479, "Tantrum", new MechanicPlotlySetting("circle-open","rgb(255,200,0)"), "Tantrum","Tantrum (Triple Circles after Ground slamming)", "Tantrum",5000),
            new PlayerBuffApplyMechanic(34387, "Volatile Poison", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Poison","Volatile Poison Application (Special Action Key)", "Poison (Action Key)",0),
            new HitOnPlayerMechanic(34481, "Volatile Poison", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Poison dmg","Stood in Volatile Poison", "Poison dmg",0),
            new HitOnPlayerMechanic(34516, "Halitosis", new MechanicPlotlySetting("triangle-right-open","rgb(255,140,0)"), "Breath","Halitosis (Flame Breath)", "Flame Breath",0),
            new HitOnPlayerMechanic(34482, "Spore Release", new MechanicPlotlySetting("pentagon","rgb(255,0,0)"), "Shake","Spore Release (Coconut Shake)", "Shake",0),
            new PlayerBuffApplyMechanic(34362, "Magic Transformation", new MechanicPlotlySetting("hexagram","rgb(0,255,255)"), "Slub","Magic Transformation (Ate Magic Mushroom)", "Slub Transform",0), 
            //new Mechanic(34496, "Nauseated", ParseEnum.BossIDS.Slothasor, new MechanicPlotlySetting("diamond-tall-open","rgb(200,140,255)"), "Slub CD",0), //can be skipped imho, identical person and timestamp as Slub Transform
            new PlayerBuffApplyMechanic(34508, "Fixated", new MechanicPlotlySetting("star","rgb(255,0,255)"), "Fixate","Fixated by Slothasor", "Fixated",0),
            new HitOnPlayerMechanic(34565, "Toxic Cloud", new MechanicPlotlySetting("pentagon-open","rgb(0,128,0)"), "Floor","Toxic Cloud (stood in green floor poison)", "Toxic Floor",0),
            new HitOnPlayerMechanic(34537, "Toxic Cloud", new MechanicPlotlySetting("pentagon-open","rgb(0,128,0)"), "Floor","Toxic Cloud (stood in green floor poison)", "Toxic Floor",0),
            new PlayerBuffApplyMechanic(791, "Fear", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Fear","Hit by fear after breakbar", "Feared",0, (ba,log) => ba.AppliedDuration == 8000),
            new EnemyBuffApplyMechanic(34467, "Narcolepsy", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","Narcolepsy (Breakbar)", "Breakbar",0),
            new EnemyBuffRemoveMechanic(34467, "Narcolepsy", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","Narcolepsy (Failed CC)", "CC Fail",0, (br,log) => br.RemovedDuration > 120000),
            new EnemyBuffRemoveMechanic(34467, "Narcolepsy", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Narcolepsy (Breakbar broken)", "CCed",0, (br,log) => br.RemovedDuration <= 120000)
            });
            Extension = "sloth";
            Icon = "https://wiki.guildwars2.com/images/e/ed/Mini_Slubling.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/PaKMZ8Z.png",
                            (1688, 2581),
                            (5822, -3491, 9549, 2205),
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210));
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Slubling1,
                ArcDPSEnums.TrashID.Slubling2,
                ArcDPSEnums.TrashID.Slubling3,
                ArcDPSEnums.TrashID.Slubling4
            };
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long fightDuration = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.FirstOrDefault(x => x.ID == (int)ArcDPSEnums.TargetID.Slothasor);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Slothasor not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            var sleepy = mainTarget.GetCastLogs(log, 0, log.FightData.FightEnd).Where(x => x.SkillId == 34515).ToList();
            long start = 0;
            int i = 1;
            foreach (AbstractCastEvent c in sleepy)
            {
                var phase = new PhaseData(start, Math.Min(c.Time, fightDuration), "Phase " + i++);
                phase.Targets.Add(mainTarget);
                start = c.EndTime;
                phases.Add(phase);
            }
            var lastPhase = new PhaseData(start, fightDuration, "Phase " + i++);
            lastPhase.Targets.Add(mainTarget);
            phases.Add(lastPhase);
            return phases;
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            IReadOnlyList<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightEnd);
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Slothasor:
                    var sleepy = cls.Where(x => x.SkillId == 34515).ToList();
                    foreach (AbstractCastEvent c in sleepy)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, ((int)c.Time, (int)c.EndTime), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    var breath = cls.Where(x => x.SkillId == 34516).ToList();
                    foreach (AbstractCastEvent c in breath)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 1000;
                        int duration = 2000;
                        int range = 600;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            int direction = Point3D.GetRotationFromFacing(facing);
                            int angle = 60;
                            replay.Decorations.Add(new PieDecoration(true, 0, range, direction, angle, (start, start + preCastTime), "rgba(255,200,0,0.1)", new AgentConnector(target)));
                            replay.Decorations.Add(new PieDecoration(true, 0, range, direction, angle, (start + preCastTime, start + preCastTime + duration), "rgba(255,200,0,0.4)", new AgentConnector(target)));
                        }
                    }
                    var tantrum = cls.Where(x => x.SkillId == 34547).ToList();
                    foreach (AbstractCastEvent c in tantrum)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        replay.Decorations.Add(new CircleDecoration(false, 0, 300, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, end, 300, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                    }
                    var shakes = cls.Where(x => x.SkillId == 34482).ToList();
                    foreach (AbstractCastEvent c in shakes)
                    {
                        int start = (int)c.Time;
                        int end = (int)c.EndTime;
                        replay.Decorations.Add(new CircleDecoration(false, 0, 700, (start, end), "rgba(255, 0, 0, 0.4)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, end, 700, (start, end), "rgba(255, 0, 0, 0.4)", new AgentConnector(target)));
                    }
                    break;
                default:
                    break;
            }

        }

        internal override void ComputePlayerCombatReplayActors(Player p, ParsedEvtcLog log, CombatReplay replay)
        {
            // Poison
            List<AbstractBuffEvent> poisonToDrop = GetFilteredList(log.CombatData, 34387, p, true);
            int toDropStart = 0;
            foreach (AbstractBuffEvent c in poisonToDrop)
            {
                if (c is BuffApplyEvent)
                {
                    toDropStart = (int)c.Time;
                }
                else
                {
                    int toDropEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(false, 0, 180, (toDropStart, toDropEnd), "rgba(255, 255, 100, 0.5)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, toDropStart + 8000, 180, (toDropStart, toDropEnd), "rgba(255, 255, 100, 0.5)", new AgentConnector(p)));
                    Point3D poisonNextPos = replay.PolledPositions.FirstOrDefault(x => x.Time >= toDropEnd);
                    Point3D poisonPrevPos = replay.PolledPositions.LastOrDefault(x => x.Time <= toDropEnd);
                    if (poisonNextPos != null || poisonPrevPos != null)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, toDropStart + 90000, 900, (toDropEnd, toDropEnd + 90000), "rgba(255, 0, 0, 0.3)", new InterpolatedPositionConnector(poisonPrevPos, poisonNextPos, toDropEnd), 180));
                    }
                }
            }
            // Transformation
            List<AbstractBuffEvent> slubTrans = GetFilteredList(log.CombatData, 34362, p, true);
            int transfoStart = 0;
            foreach (AbstractBuffEvent c in slubTrans)
            {
                if (c is BuffApplyEvent)
                {
                    transfoStart = (int)c.Time;
                }
                else
                {
                    int transfoEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 180, (transfoStart, transfoEnd), "rgba(0, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
            // fixated
            List<AbstractBuffEvent> fixatedSloth = GetFilteredList(log.CombatData, 34508, p, true);
            int fixatedSlothStart = 0;
            foreach (AbstractBuffEvent c in fixatedSloth)
            {
                if (c is BuffApplyEvent)
                {
                    fixatedSlothStart = (int)c.Time;
                }
                else
                {
                    int fixatedSlothEnd = (int)c.Time;
                    replay.Decorations.Add(new CircleDecoration(true, 0, 120, (fixatedSlothStart, fixatedSlothEnd), "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
        }
    }
}
