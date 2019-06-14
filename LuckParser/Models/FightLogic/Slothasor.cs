using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Slothasor : RaidLogic
    {
        public Slothasor(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(34479, "Tantrum", new MechanicPlotlySetting("circle-open","rgb(255,200,0)"), "Tantrum","Tantrum (Triple Circles after Ground slamming)", "Tantrum",5000), 
            new PlayerBoonApplyMechanic(34387, "Volatile Poison", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Poison","Volatile Poison Application (Special Action Key)", "Poison (Action Key)",0),
            new HitOnPlayerMechanic(34481, "Volatile Poison", new MechanicPlotlySetting("circle-open","rgb(255,0,0)"), "Poison dmg","Stood in Volatile Poison", "Poison dmg",0),
            new HitOnPlayerMechanic(34516, "Halitosis", new MechanicPlotlySetting("triangle-right-open","rgb(255,140,0)"), "Breath","Halitosis (Flame Breath)", "Flame Breath",0),
            new HitOnPlayerMechanic(34482, "Spore Release", new MechanicPlotlySetting("pentagon","rgb(255,0,0)"), "Shake","Spore Release (Coconut Shake)", "Shake",0),
            new PlayerBoonApplyMechanic(34362, "Magic Transformation", new MechanicPlotlySetting("hexagram","rgb(0,255,255)"), "Slub","Magic Transformation (Ate Magic Mushroom)", "Slub Transform",0), 
            //new Mechanic(34496, "Nauseated", ParseEnum.BossIDS.Slothasor, new MechanicPlotlySetting("diamond-tall-open","rgb(200,140,255)"), "Slub CD",0), //can be skipped imho, identical person and timestamp as Slub Transform
            new PlayerBoonApplyMechanic(34508, "Fixated", new MechanicPlotlySetting("star","rgb(255,0,255)"), "Fixate","Fixated by Slothasor", "Fixated",0),
            new HitOnPlayerMechanic(34565, "Toxic Cloud", new MechanicPlotlySetting("pentagon-open","rgb(0,128,0)"), "Floor","Toxic Cloud (stood in green floor poison)", "Toxic Floor",0), 
            new HitOnPlayerMechanic(34537, "Toxic Cloud", new MechanicPlotlySetting("pentagon-open","rgb(0,128,0)"), "Floor","Toxic Cloud (stood in green floor poison)", "Toxic Floor",0),
            new PlayerBoonApplyMechanic(791, "Fear", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Fear","Hit by fear after breakbar", "Feared",0, new List<BoonApplyMechanic.BoonApplyChecker>{ (ba,log) => ba.AppliedDuration == 8000 }, Mechanic.TriggerRule.AND),
            new EnemyBoonApplyMechanic(34467, "Narcolepsy", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "CC","Narcolepsy (Breakbar)", "Breakbar",0),
            new EnemyBoonRemoveMechanic(34467, "Narcolepsy", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","Narcolepsy (Failed CC)", "CC Fail",0, new List<BoonRemoveMechanic.BoonRemoveChecker>{ (br,log) => br.RemovedDuration > 120000 }, Mechanic.TriggerRule.AND),
            new EnemyBoonRemoveMechanic(34467, "Narcolepsy", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Narcolepsy (Breakbar broken)", "CCed",0, new List<BoonRemoveMechanic.BoonRemoveChecker>{ (br,log) => br.RemovedDuration <= 120000 }, Mechanic.TriggerRule.AND)
            });
            Extension = "sloth";
            IconUrl = "https://wiki.guildwars2.com/images/e/ed/Mini_Slubling.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/PaKMZ8Z.png",
                            (1688, 2581),
                            (5822, -3491, 9549, 2205),
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210));
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Slubling1,
                Slubling2,
                Slubling3,
                Slubling4
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Slothasor);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            List<AbstractCastEvent> sleepy = mainTarget.GetCastLogs(log, 0, log.FightData.FightDuration).Where(x => x.SkillId == 34515).ToList();
            long start = 0;
            int i = 1;
            foreach (AbstractCastEvent c in sleepy)
            {
                PhaseData phase = new PhaseData(start, Math.Min(c.Time, fightDuration)) {
                    Name = "Phase " + i++
                };
                phase.Targets.Add(mainTarget);
                start = c.Time + c.ActualDuration;
                phases.Add(phase);
            }
            PhaseData lastPhase = new PhaseData(start, fightDuration)
            {
                Name = "Phase " + i++
            };
            lastPhase.Targets.Add(mainTarget);
            phases.Add(lastPhase);
            phases.RemoveAll(x => x.DurationInMS <= 1000);
            return phases;
        }

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Slothasor:
                    List<AbstractCastEvent> sleepy = cls.Where(x => x.SkillId == 34515).ToList();
                    foreach (AbstractCastEvent c in sleepy)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 180, ((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
                    }
                    List<AbstractCastEvent> breath = cls.Where(x => x.SkillId == 34516).ToList();
                    foreach (AbstractCastEvent c in breath)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 1000;
                        int duration = 2000;
                        int range = 600;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start+1000);
                        if (facing != null)
                        {
                            int direction = Point3D.GetRotationFromFacing(facing);
                            int angle = 60;
                            replay.Actors.Add(new PieActor(true, 0, range, direction, angle, (start, start + preCastTime), "rgba(255,200,0,0.1)", new AgentConnector(target)));
                            replay.Actors.Add(new PieActor(true, 0, range, direction, angle, (start + preCastTime, start + preCastTime + duration), "rgba(255,200,0,0.4)", new AgentConnector(target)));
                        }
                    }
                    List<AbstractCastEvent> tantrum = cls.Where(x => x.SkillId == 34547).ToList();
                    foreach (AbstractCastEvent c in tantrum)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        replay.Actors.Add(new CircleActor(false, 0, 300, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                        replay.Actors.Add(new CircleActor(true, end, 300, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(target)));
                    }
                    List<AbstractCastEvent> shakes = cls.Where(x => x.SkillId == 34482).ToList();
                    foreach (AbstractCastEvent c in shakes)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        replay.Actors.Add(new CircleActor(false, 0, 700, (start, end), "rgba(255, 0, 0, 0.4)", new AgentConnector(target)));
                        replay.Actors.Add(new CircleActor(true, end, 700, (start, end), "rgba(255, 0, 0, 0.4)", new AgentConnector(target)));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
           
        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
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
                    replay.Actors.Add(new CircleActor(false, 0, 180, (toDropStart, toDropEnd), "rgba(255, 255, 100, 0.5)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, toDropStart + 8000, 180, (toDropStart, toDropEnd), "rgba(255, 255, 100, 0.5)", new AgentConnector(p)));
                    Point3D poisonNextPos = replay.PolledPositions.FirstOrDefault(x => x.Time >= toDropEnd);
                    Point3D poisonPrevPos = replay.PolledPositions.LastOrDefault(x => x.Time <= toDropEnd);
                    if (poisonNextPos != null || poisonPrevPos != null)
                    {
                        replay.Actors.Add(new CircleActor(true, toDropStart + 90000, 900, (toDropEnd, toDropEnd + 90000), "rgba(255, 0, 0, 0.3)", new InterpolatedPositionConnector(poisonPrevPos, poisonNextPos, toDropEnd), 180));
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
                    replay.Actors.Add(new CircleActor(true, 0, 180, (transfoStart, transfoEnd), "rgba(0, 80, 255, 0.3)", new AgentConnector(p)));
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
                    replay.Actors.Add(new CircleActor(true, 0, 120, (fixatedSlothStart, fixatedSlothEnd), "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
        }
    }
}
