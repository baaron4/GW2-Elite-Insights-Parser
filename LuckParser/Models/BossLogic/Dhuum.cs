using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Dhuum : BossLogic
    {
        public Dhuum() : base()
        {
            mode = ParseMode.Raid;
            mechanicList = new List<Mechanic>
            {
            new Mechanic(48172, "Hateful Ephemera", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Dhuum, "symbol:'square',color:'rgb(255,140,0)',", "Golem Dmg",0),//Buff or dmg? //dmg
            new Mechanic(48121, "Arcing Affliction", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Dhuum, "symbol:'circle-open',color:'rgb(255,0,0)',", "Bomb DMG",0),//Buff or dmg? //dmg
            new Mechanic(47646, "Arcing Affliction", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Dhuum, "symbol:'circle',color:'rgb(255,0,0)',", "Bomn",0),//Buff or dmg? //Buff
            //new Mechanic(47476, "Residual Affliction", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Dhuum, "symbol:'star-diamond',color:'rgb(255,200,0)',", "Bomb",0), //not needed, imho, applied at the same time as Arcing Affliction
            new Mechanic(47335, "Soul Shackle", Mechanic.MechType.PlayerOnPlayer, ParseEnum.BossIDS.Dhuum, "symbol:'diamond',color:'rgb(0,255,255)',", "Shackle",0),//4 calls probably this one //correct, this ID is used for application and removal.
            new Mechanic(47164, "Soul Shackle", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Dhuum, "symbol:'diamond-open',color:'rgb(0,255,255)',", "Shackle Dmg",0),//Value is only non-zero after 4 ticks. First 4 seconds don't deal damage. Need to exclude "value==0" events with this ID?
            new Mechanic(47561, "Slash", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Dhuum, "symbol:'triangle',color:'rgb(0,128,0)',", "Cone",0),
            new Mechanic(48752, "Cull", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Dhuum, "symbol:'asterisk-open',color:'rgb(0,255,255)',", "Crack",0),
            new Mechanic(48760, "Putrid Bomb", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Dhuum, "symbol:'circle',color:'rgb(0,128,0)',", "Marks",0),
            new Mechanic(48398, "Cataclysmic Cycle", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Dhuum, "symbol:'circle-open',color:'rgb(255,140,0)',", "Suck Dmg",0),
            new Mechanic(48176, "Death Mark", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Dhuum, "symbol:'hexagon',color:'rgb(255,140,0)',", "Dip Aoe",0),
            new Mechanic(48210, "Greater Death Mark", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Dhuum, "symbol:'circle',color:'rgb(255,140,0)',", "Knockback Dmg",0),
          //  new Mechanic(48281, "Mortal Coil", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Dhuum, "symbol:'circle',color:'rgb(0,128,0)',", "Green Orbs",
            new Mechanic(46950, "Fractured Spirit", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Dhuum, "symbol:'square',color:'rgb(0,255,0)',", "Orbs CD",0),
            };
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/CLTwWBJ.png",
                            Tuple.Create(3763, 3383),
                            Tuple.Create(13524, -1334, 18039, 2735),
                            Tuple.Create(-21504, -12288, 24576, 12288),
                            Tuple.Create(19072, 15484, 20992, 16508));
        }

        public override List<PhaseData> getPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            long start = 0;
            long end = 0;
            long fight_dur = log.getBossData().getAwareDuration();
            List<PhaseData> phases = getInitialPhase(log);
            // Sometimes the preevent is not in the evtc
            List<CastLog> dhuumCast = boss.getCastLogs(log, 0, 20000);
            if (dhuumCast.Count > 0)
            {
                CastLog shield = cast_logs.Find(x => x.getID() == 47396);
                if (shield != null)
                {
                    end = shield.getTime();
                    phases.Add(new PhaseData(start, end));
                    start = shield.getTime() + shield.getActDur();
                    if (start < fight_dur - 5000)
                    {
                        phases.Add(new PhaseData(start, fight_dur));
                    }
                }
                if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                {
                    phases.Add(new PhaseData(start, fight_dur));
                }
                string[] namesDh = new string[] { "Main Fight", "Ritual" };
                for (int i = 1; i < phases.Count; i++)
                {
                    phases[i].setName(namesDh[i - 1]);
                }
            }
            else
            {
                CombatItem invulDhuum = log.getBoonData().Find(x => x.getSkillID() == 762 && x.isBuffremove() != ParseEnum.BuffRemove.None && x.getSrcInstid() == boss.getInstid() && x.getTime() > 115000 + log.getBossData().getFirstAware());
                if (invulDhuum != null)
                {
                    end = invulDhuum.getTime() - log.getBossData().getFirstAware();
                    phases.Add(new PhaseData(start, end));
                    start = end + 1;
                    CastLog shield = cast_logs.Find(x => x.getID() == 47396);
                    if (shield != null)
                    {
                        end = shield.getTime();
                        phases.Add(new PhaseData(start, end));
                        start = shield.getTime() + shield.getActDur();
                        if (start < fight_dur - 5000)
                        {
                            phases.Add(new PhaseData(start, fight_dur));
                        }
                    }
                }
                if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
                {
                    phases.Add(new PhaseData(start, fight_dur));
                }
                string[] namesDh = new string[] { "Roleplay", "Main Fight", "Ritual" };
                for (int i = 1; i < phases.Count; i++)
                {
                    phases[i].setName(namesDh[i - 1]);
                }
            }
            return phases;
        }

        public override List<ParseEnum.ThrashIDS> getAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: facing information (pull thingy)
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Echo,
                        ParseEnum.ThrashIDS.Enforcer,
                        ParseEnum.ThrashIDS.Messenger
                    };
            List<CastLog> deathmark = cls.Where(x => x.getID() == 48176).ToList();
            CastLog majorSplit = cls.Find(x => x.getID() == 47396);
            foreach (CastLog c in deathmark)
            {
                int start = (int)c.getTime();
                int cast_end = start + c.getActDur();
                int zone_end = cast_end + 120000;
                if (majorSplit != null)
                {
                    cast_end = Math.Min(cast_end, (int)majorSplit.getTime());
                    zone_end = Math.Min(zone_end, (int)majorSplit.getTime());
                }
                Point3D pos = replay.getPositions().FirstOrDefault(x => x.time > cast_end);
                if (pos != null)
                {
                    replay.addCircleActor(new CircleActor(true, cast_end, 450, new Tuple<int, int>(start, cast_end), "rgba(200, 255, 100, 0.5)", pos));
                    replay.addCircleActor(new CircleActor(false, 0, 450, new Tuple<int, int>(start, cast_end), "rgba(200, 255, 100, 0.5)", pos));
                    replay.addCircleActor(new CircleActor(true, 0, 450, new Tuple<int, int>(cast_end, zone_end), "rgba(200, 255, 100, 0.5)", pos));
                }
            }
            List<CastLog> cataCycle = cls.Where(x => x.getID() == 48398).ToList();
            foreach (CastLog c in cataCycle)
            {
                int start = (int)c.getTime();
                int end = start + c.getActDur();
                replay.addCircleActor(new CircleActor(true, end, 300, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.7)"));
                replay.addCircleActor(new CircleActor(true, 0, 300, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)"));
            }
            if (majorSplit != null)
            {
                int start = (int)majorSplit.getTime();
                int end = (int)log.getBossData().getAwareDuration();
                replay.addCircleActor(new CircleActor(true, 0, 320, new Tuple<int, int>(start, end), "rgba(0, 180, 255, 0.2)"));
            }
            return ids;
        }

        public override void getAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            // spirit transform
            List<CombatItem> spiritTransform = log.getBoonData().Where(x => x.getDstInstid() == p.getInstid() && x.getSkillID() == 46950 && x.isBuffremove() == ParseEnum.BuffRemove.None).ToList();
            foreach (CombatItem c in spiritTransform)
            {
                int duration = 15000;
                int start = (int)(c.getTime() - log.getBossData().getFirstAware());
                if (log.getBossData().getHealthOverTime().FirstOrDefault(x => x.X > start).Y < 1050)
                {
                    duration = 30000;
                }
                CombatItem removedBuff = log.getBoonData().FirstOrDefault(x => x.getSrcInstid() == p.getInstid() && x.getSkillID() == 48281 && x.isBuffremove() == ParseEnum.BuffRemove.All && x.getTime() > c.getTime() && x.getTime() < c.getTime() + duration);
                int end = start + duration;
                if (removedBuff != null)
                {
                    end = (int)(removedBuff.getTime() - log.getBossData().getFirstAware());
                }
                replay.addCircleActor(new CircleActor(true, 0, 100, new Tuple<int, int>(start, end), "rgba(0, 50, 200, 0.3)"));
                replay.addCircleActor(new CircleActor(true, start + duration, 100, new Tuple<int, int>(start, end), "rgba(0, 50, 200, 0.5)"));
            }
            // bomb
            List<CombatItem> bombDhuum = getFilteredList(log, 47646, p.getInstid());
            int bombDhuumStart = 0;
            int bombDhuumEnd = 0;
            foreach (CombatItem c in bombDhuum)
            {
                if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    bombDhuumStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                }
                else
                {
                    bombDhuumEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                    replay.addCircleActor(new CircleActor(true, 0, 100, new Tuple<int, int>(bombDhuumStart, bombDhuumEnd), "rgba(80, 180, 0, 0.3)"));
                    replay.addCircleActor(new CircleActor(true, bombDhuumStart + 13000, 100, new Tuple<int, int>(bombDhuumStart, bombDhuumEnd), "rgba(80, 180, 0, 0.5)"));
                }
            }
        }

        public override int isCM(List<CombatItem> clist, int health)
        {
            return (health > 35e6) ? 1 : 0;
        }

        public override string getReplayIcon()
        {
            return "https://i.imgur.com/RKaDon5.png";
        }
    }
}
