using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Slothasor : RaidLogic
    {
        public Slothasor(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(34479, "Tantrum", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Slothasor, "symbol:'circle-open',color:'rgb(255,200,0)'", "Tntrm","Tantrum (Triple Circles after Ground slamming)", "Tantrum",5000), 
            new Mechanic(34387, "Volatile Poison", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Slothasor, "symbol:'circle',color:'rgb(255,0,0)'", "Psn","Volatile Poison Application (Special Action Key)", "Poison (Action Key)",0),
            new Mechanic(34481, "Volatile Poison", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Slothasor, "symbol:'circle-open',color:'rgb(255,0,0)'", "P.dmg","Stood in Volatile Poison", "Poison dmg",0),
            new Mechanic(34516, "Halitosis", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Slothasor, "symbol:'triangle-right-open',color:'rgb(255,140,0)'", "FlmBrth","Halitosis (Flame Breath)", "Flame Breath",0),
            new Mechanic(34482, "Spore Release", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Slothasor, "symbol:'pentagon',color:'rgb(255,0,0)'", "Shake","Spore Release (Coconut Shake)", "Shake",0),
            new Mechanic(34362, "Magic Transformation", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Slothasor, "symbol:'hexagram',color:'rgb(0,255,255)'", "Slub","Magic Transformation (Ate Magic Mushroom)", "Slub Transform",0), 
            //new Mechanic(34496, "Nauseated", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Slothasor, "symbol:'diamond-tall-open',color:'rgb(200,140,255)'", "Slub CD",0), //can be skipped imho, identical person and timestamp as Slub Transform
            new Mechanic(34508, "Fixated", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Slothasor, "symbol:'star',color:'rgb(255,0,255)'", "Fix","Fixated by Slothasor", "Fixated",0),
            new Mechanic(34565, "Toxic Cloud", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Slothasor, "symbol:'pentagon-open',color:'rgb(0,128,0)'", "Floor","Toxic Cloud (stood in green floor poison)", "Toxic Floor",0), 
            new Mechanic(34537, "Toxic Cloud", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Slothasor, "symbol:'pentagon-open',color:'rgb(0,128,0)'", "Floor","Toxic Cloud (stood in green floor poison)", "Toxic Floor",0),
            new Mechanic(791, "Fear", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Slothasor, "symbol:'square-open',color:'rgb(255,0,0)'", "Fear","Hit by fear after breakbar", "Feared",0,(condition=> condition.CombatItem.Value == 8000)),
            new Mechanic(34467, "Narcolepsy", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Slothasor, "symbol:'diamond-tall',color:'rgb(0,160,150)'", "CC","Narcolepsy (Breakbar)", "Breakbar",0),
            new Mechanic(34467, "Narcolepsy", Mechanic.MechType.EnemyBoonStrip, ParseEnum.BossIDS.Slothasor, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "CC.Fail","Narcolepsy (Failed CC)", "CC Fail",0,(condition => condition.CombatItem.Value > 120000)),
            new Mechanic(34467, "Narcolepsy", Mechanic.MechType.EnemyBoonStrip, ParseEnum.BossIDS.Slothasor, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "CCed","Narcolepsy (Breakbar broken)", "CCed",0,(condition => condition.CombatItem.Value <= 120000))
            });
            Extension = "sloth";
            IconUrl = "https://wiki.guildwars2.com/images/e/ed/Mini_Slubling.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/PaKMZ8Z.png",
                            Tuple.Create(1688, 2581),
                            Tuple.Create(5822, -3491, 9549, 2205),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(2688, 11906, 3712, 14210));
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


        public override void ComputeAdditionalThrashMobData(Mob mob, ParsedLog log)
        {
            switch (mob.ID)
            {
                case (ushort)Slubling1:
                case (ushort)Slubling2:
                case (ushort)Slubling3:
                case (ushort)Slubling4:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            // TODO:facing information (breath)
            CombatReplay replay = boss.CombatReplay;
            List<CastLog> cls = boss.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.Slothasor:
                    List<CastLog> sleepy = cls.Where(x => x.SkillId == 34515).ToList();
                    foreach (CastLog c in sleepy)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 180, new Tuple<int, int>((int)c.Time, (int)c.Time + c.ActualDuration), "rgba(0, 180, 255, 0.3)", new AgentConnector(boss)));
                    }
                    List<CastLog> breath = cls.Where(x => x.SkillId == 34515).ToList();
                    foreach (CastLog c in breath)
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
                            replay.Actors.Add(new PieActor(true, 0, range, direction, angle, new Tuple<int, int>(start, start + preCastTime), "rgba(255,200,0,0.1)", new AgentConnector(boss)));
                            replay.Actors.Add(new PieActor(true, 0, range, direction, angle, new Tuple<int, int>(start + preCastTime, start + preCastTime + duration), "rgba(255,200,0,0.4)", new AgentConnector(boss)));
                        }
                    }
                    List<CastLog> tantrum = cls.Where(x => x.SkillId == 34547).ToList();
                    foreach (CastLog c in tantrum)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        replay.Actors.Add(new CircleActor(false, 0, 300, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(boss)));
                        replay.Actors.Add(new CircleActor(true, end, 300, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(boss)));
                    }
                    List<CastLog> shakes = cls.Where(x => x.SkillId == 34482).ToList();
                    foreach (CastLog c in shakes)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        replay.Actors.Add(new CircleActor(false, 0, 700, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.4)", new AgentConnector(boss)));
                        replay.Actors.Add(new CircleActor(true, end, 700, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.4)", new AgentConnector(boss)));
                    }
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
           
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            // Poison
            CombatReplay replay = p.CombatReplay;
            List<CombatItem> poisonToDrop = GetFilteredList(log, 34387, p);
            int toDropStart = 0;
            foreach (CombatItem c in poisonToDrop)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    toDropStart = (int)(c.Time - log.FightData.FightStart);
                }
                else
                {
                    int toDropEnd = (int)(c.Time - log.FightData.FightStart); replay.Actors.Add(new CircleActor(false, 0, 180, new Tuple<int, int>(toDropStart, toDropEnd), "rgba(255, 255, 100, 0.5)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, toDropStart + 8000, 180, new Tuple<int, int>(toDropStart, toDropEnd), "rgba(255, 255, 100, 0.5)", new AgentConnector(p)));
                    Point3D poisonNextPos = replay.Positions.FirstOrDefault(x => x.Time >= toDropEnd);
                    Point3D poisonPrevPos = replay.Positions.LastOrDefault(x => x.Time <= toDropEnd);
                    if (poisonNextPos != null || poisonPrevPos != null)
                    {
                        replay.Actors.Add(new CircleActor(true, toDropStart + 90000, 900, new Tuple<int, int>(toDropEnd, toDropEnd + 90000), "rgba(255, 0, 0, 0.3)", new InterpolatedPositionConnector(poisonPrevPos, poisonNextPos, toDropEnd), 180));
                    }
                }
            }
            // Transformation
            List<CombatItem> slubTrans = GetFilteredList(log, 34362, p);
            int transfoStart = 0;
            foreach (CombatItem c in slubTrans)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    transfoStart = (int)(c.Time - log.FightData.FightStart);
                }
                else
                {
                    int transfoEnd = (int)(c.Time - log.FightData.FightStart);
                    replay.Actors.Add(new CircleActor(true, 0, 180, new Tuple<int, int>(transfoStart, transfoEnd), "rgba(0, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
            // fixated
            List<CombatItem> fixatedSloth = GetFilteredList(log, 34508, p);
            int fixatedSlothStart = 0;
            foreach (CombatItem c in fixatedSloth)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    fixatedSlothStart = (int)(c.Time - log.FightData.FightStart);
                }
                else
                {
                    int fixatedSlothEnd = (int)(c.Time - log.FightData.FightStart);
                    replay.Actors.Add(new CircleActor(true, 0, 120, new Tuple<int, int>(fixatedSlothStart, fixatedSlothEnd), "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
        }
    }
}
