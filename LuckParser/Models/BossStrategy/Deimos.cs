using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Deimos : BossStrategy
    {
        public Deimos() : base()
        {
            mode = ParseMode.Raid;
            mechanicList = new List<Mechanic>
            {
                new Mechanic(37716, "Rapid Decay", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Deimos, "symbol:'circle',color:'rgb(0,0,0)',", "Black Oil",0),
            //new Mechanic(37844, "Off Balance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Deimos, "symbol:'cross',color:'rgb(255,0,255)',", "Failed Teleport Break",0), Cast by the drunkard Saul, would be logical to be the forced random teleport but not sure when it's successful or not
            //new Mechanic(37846, "Off Balance", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Deimos, "symbol:'cross',color:'rgb(255,0,255)',", "Teleport Break",0), Seems to be the ID for the breakbar/cast start. If value is above 2200 (and the is_activation==5), the break apparently failed.
            //new Mechanic(38272, "Boon Thief", Mechanic.MechType.SkillOnEnemy(?), ParseEnum.BossIDS.Deimos, "symbol:'x',color:'rgb(255,0,255)',", "Failed Boon Thief Break",0), Seems to only be successful if the value is above 4400 (the duration of the breakbar). Also, has no dst_agent so new category might be necessary?
            new Mechanic(38208, "Annihilate", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Deimos, "symbol:'hexagon',color:'rgb(255,200,0)',", "Boss Smash",0),
            new Mechanic(37929, "Annihilate", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Deimos, "symbol:'hexagon',color:'rgb(255,200,0)',", "Chain Smash",0),
            new Mechanic(37980, "Demonic Shock Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Deimos, "symbol:'circle',color:'rgb(255,0,0)',", "10% Smash",0),
            new Mechanic(37982, "Demonic Shock Wave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Deimos, "symbol:'circle',color:'rgb(255,0,0)',", "10% Smash",0),
            new Mechanic(37733, "Tear Instability", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Deimos, "symbol:'diamond',color:'rgb(0,128,0)',", "Tear",0),
            new Mechanic(37613, "Mind Crush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Deimos, "symbol:'square',color:'rgb(0,0,255)',", "Mind Crush",0), // Not properly detected everytime, sometimes people receive mind crush hit but no weak minded afterwards (partly due to green circle immunity). Suggestion: Only mark as hit if it deals non-zero damage (value>=0).
            new Mechanic(38187, "Weak Minded", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Deimos, "symbol:'square-open',color:'rgb(200,140,255)',", "Weak Minded",0), //
            new Mechanic(37730, "Chosen by Eye of Janthir", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Deimos, "symbol:'circle',color:'rgb(0,255,0)',", "Chosen",0),
            new Mechanic(38169, "Teleported", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Deimos, "symbol:'circle-open',color:'rgb(0,128,0)',", "Teleported",0),
            new Mechanic(38169, "Unnatural Signet", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.Deimos, "symbol:'square-open',color:'rgb(0,255,255)',", "+100% Dmg Buff",0)
            //mlist.Add("Chosen by Eye of Janthir");
            //mlist.Add("");//tp from drunkard
            //mlist.Add("");//bon currupt from thief
            //mlist.Add("Teleport");//to demonic realm
            };
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/TskIM9i.png",
                            Tuple.Create(4267, 5770),
                            Tuple.Create(-9542, 1932, -7078, 5275),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
        }

        public override List<PhaseData> getPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            long start = 0;
            long end = 0;
            long fight_dur = log.getBossData().getAwareDuration();
            List<PhaseData> phases = getInitialPhase(log);
            // Determined + additional data on inst change
            CombatItem invulDei = log.getBoonData().Find(x => x.getSkillID() == 762 && x.isBuffremove() == ParseEnum.BuffRemove.None && x.getDstInstid() == boss.getInstid());
            if (invulDei != null)
            {
                end = invulDei.getTime() - log.getBossData().getFirstAware();
                phases.Add(new PhaseData(start, end));
                start = (boss.getPhaseData().Count == 1 ? boss.getPhaseData()[0] - log.getBossData().getFirstAware() : fight_dur);
                cast_logs.Add(new CastLog(end, -6, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
            }
            if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
            {
                phases.Add(new PhaseData(start, fight_dur));
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].setName("Phase " + i);
            }
            int offsetDei = phases.Count;
            CombatItem teleport = log.getCombatList().FirstOrDefault(x => x.getSkillID() == 38169);
            int splits = 0;
            while (teleport != null && splits < 3)
            {
                start = teleport.getTime() - log.getBossData().getFirstAware();
                CombatItem teleportBack = log.getCombatList().FirstOrDefault(x => x.getSkillID() == 38169 && x.getTime() - log.getBossData().getFirstAware() > start + 10000);
                if (teleportBack != null)
                {
                    end = teleportBack.getTime() - log.getBossData().getFirstAware();
                }
                else
                {
                    end = fight_dur;
                }
                phases.Add(new PhaseData(start, end));
                splits++;
                teleport = log.getCombatList().FirstOrDefault(x => x.getSkillID() == 38169 && x.getTime() - log.getBossData().getFirstAware() > end + 10000);
            }

            string[] namesDeiSplit = new string[] { "Thief", "Gambler", "Drunkard" };
            for (int i = offsetDei; i < phases.Count; i++)
            {
                phases[i].setName(namesDeiSplit[i - offsetDei]);
            }
            phases.Sort((x, y) => (x.getStart() < y.getStart()) ? -1 : 1);
            return phases;
        }

        public override List<ParseEnum.ThrashIDS> getAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: facing information (slam)
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Saul,
                        ParseEnum.ThrashIDS.Thief,
                        ParseEnum.ThrashIDS.Drunkard,
                        ParseEnum.ThrashIDS.Gambler,
                        ParseEnum.ThrashIDS.GamblerClones
                    };
            List<CastLog> mindCrush = cls.Where(x => x.getID() == 37613).ToList();
            foreach (CastLog c in mindCrush)
            {
                int start = (int)c.getTime();
                int end = start + 5000;
                replay.addCircleActor(new FollowingCircle(true, end, 180, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
                replay.addCircleActor(new FollowingCircle(false, 0, 180, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
                if (!log.getBossData().getCM())
                {
                    replay.addCircleActor(new ImmobileCircle(true, 0, 180, new Tuple<int, int>(start, end), "rgba(0, 0, 255, 0.3)", new Point3D(-8421.818f, 3091.72949f, -9.818082e8f, 216)));
                }
            }
            return ids;
        }

        public override void getAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            // teleport zone
            List<CombatItem> tpDeimos = getFilteredList(log, 37730, p.getInstid());
            int tpStart = 0;
            int tpEnd = 0;
            foreach (CombatItem c in tpDeimos)
            {
                if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    tpStart = (int)(c.getTime() - log.getBossData().getFirstAware());
                }
                else
                {
                    tpEnd = (int)(c.getTime() - log.getBossData().getFirstAware());
                    replay.addCircleActor(new FollowingCircle(true, 0, 180, new Tuple<int, int>(tpStart, tpEnd), "rgba(0, 150, 0, 0.3)"));
                    replay.addCircleActor(new FollowingCircle(true, tpEnd, 180, new Tuple<int, int>(tpStart, tpEnd), "rgba(0, 150, 0, 0.3)"));
                }
            }
        }

        public override string getReplayIcon()
        {
            return "https://i.imgur.com/mWfxBaO.png";
        }
    }
}
