using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class KeepConstruct : BossLogic
    {
        public KeepConstruct() : base()
        {
            mode = ParseMode.Raid;
            mechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(34912, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.KeepConstruct, "symbol:'star',color:'rgb(255,0,255)',", "Fixate",0),
            new Mechanic(34925, "Fixate", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.KeepConstruct, "symbol:'star',color:'rgb(255,0,255)',", "Fixate",0),
            new Mechanic(35077, "Hail of Fury", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.KeepConstruct, "symbol:'circle-open',color:'rgb(255,0,0)',", "Debris",0),
            new Mechanic(35096, "Compromised", Mechanic.MechType.EnemyBoon, ParseEnum.BossIDS.KeepConstruct, "symbol:'hexagon',color:'rgb(0,0,255)',", "Compromised",0),
            new Mechanic(16227, "Insidious Projection", Mechanic.MechType.Spawn, ParseEnum.BossIDS.KeepConstruct, "symbol:'bowtie',color:'rgb(255,0,0)',", "Merge",0),//Spawn check; How should this be handled? Species ID is 16227 if that helps. Could check combat events with state_change 'spawn' if it is an insidious projection?
            new Mechanic(35137, "Phantasmal Blades", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.KeepConstruct, "symbol:'hexagram-open',color:'rgb(255,0,255)',", "Phantasmal Blades",0),
            new Mechanic(35064, "Phantasmal Blades", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.KeepConstruct, "symbol:'hexagram-open',color:'rgb(255,0,255)',", "Phantasmal Blades",0),
            new Mechanic(35086, "Tower Drop", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.KeepConstruct, "symbol:'circle',color:'rgb(255,140,0)',", "Tower Drop",0),
            new Mechanic(35103, "Xera's Fury", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.KeepConstruct, "symbol:'circle',color:'rgb(200,140,0)',", "Bombs",0),
            new Mechanic(16261, "Core Hit", Mechanic.MechType.HitOnEnemy, ParseEnum.BossIDS.KeepConstruct, "symbol:'hexagram',color:'rgb(240,140,0)',", "Core Hit",1000)
            //hit orb: May need different format since the Skill ID is irrelevant but any combat event with dst_agent of the species ID 16261 (Construct Core) should be shown. Tracking either via the different Construct Core adresses or their instance_id? (every phase it's a new one)
            });
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/tBAFCEf.png",
                            Tuple.Create(1099, 1114),
                            Tuple.Create(-5467, 8069, -2282, 11297),
                            Tuple.Create(-12288, -27648, 12288, 27648),
                            Tuple.Create(1920, 12160, 2944, 14464));
        }

        public override List<PhaseData> getPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            long start = 0;
            long end = 0;
            long fight_dur = log.GetBossData().getAwareDuration();
            List<PhaseData> phases = getInitialPhase(log);
            // Main phases
            List<CastLog> clsKC = cast_logs.Where(x => x.getID() == 35048).ToList();
            foreach (CastLog cl in clsKC)
            {
                end = cl.getTime();
                phases.Add(new PhaseData(start, end));
                start = end + cl.getActDur();
            }
            if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
            {
                phases.Add(new PhaseData(start, fight_dur));
                start = fight_dur;
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].setName("Phase " + i);
            }
            // add burn phases
            int offset = phases.Count;
            List<CombatItem> orbItems = log.GetBoonData().Where(x => x.getDstInstid() == boss.GetInstid() && x.getSkillID() == 35096).ToList();
            // Get number of orbs and filter the list
            List<CombatItem> orbItemsFiltered = new List<CombatItem>();
            Dictionary<long, int> orbs = new Dictionary<long, int>();
            foreach (CombatItem c in orbItems)
            {
                long time = c.getTime() - log.GetBossData().getFirstAware();
                if (!orbs.ContainsKey(time))
                {
                    orbs[time] = 0;
                }
                if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    orbs[time] = orbs[time] + 1;
                }
                if (orbItemsFiltered.Count > 0)
                {
                    CombatItem last = orbItemsFiltered.Last();
                    if (last.getTime() != c.getTime())
                    {
                        orbItemsFiltered.Add(c);
                    }
                }
                else
                {
                    orbItemsFiltered.Add(c);
                }

            }
            foreach (CombatItem c in orbItemsFiltered)
            {
                if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    start = c.getTime() - log.GetBossData().getFirstAware();
                }
                else
                {
                    end = c.getTime() - log.GetBossData().getFirstAware();
                    phases.Add(new PhaseData(start, end));
                }
            }
            if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
            {
                phases.Add(new PhaseData(start, fight_dur));
                start = fight_dur;
            }
            for (int i = offset; i < phases.Count; i++)
            {
                phases[i].setName("Burn " + (i - offset + 1) + " (" + orbs[phases[i].getStart()] + " orbs)");
            }
            phases.Sort((x, y) => (x.getStart() < y.getStart()) ? -1 : 1);
            return phases;
        }

        public override List<ParseEnum.ThrashIDS> getAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO: needs arc circles for blades
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Core,
                        ParseEnum.ThrashIDS.Jessica,
                        ParseEnum.ThrashIDS.Olson,
                        ParseEnum.ThrashIDS.Engul,
                        ParseEnum.ThrashIDS.Faerla,
                        ParseEnum.ThrashIDS.Caulle,
                        ParseEnum.ThrashIDS.Henley,
                        ParseEnum.ThrashIDS.Galletta,
                        ParseEnum.ThrashIDS.Ianim,
                        ParseEnum.ThrashIDS.GreenPhantasm,
                        ParseEnum.ThrashIDS.InsidiousProjection,
                        ParseEnum.ThrashIDS.UnstableLeyRift,
                        ParseEnum.ThrashIDS.RadiantPhantasm,
                        ParseEnum.ThrashIDS.CrimsonPhantasm,
                        ParseEnum.ThrashIDS.RetrieverProjection
                    };
            List<CastLog> magicCharge = cls.Where(x => x.getID() == 35048).ToList();
            List<CastLog> magicExplose = cls.Where(x => x.getID() == 34894).ToList();
            for (var i = 0; i < magicCharge.Count; i++)
            {
                CastLog charge = magicCharge[i];
                if (i < magicExplose.Count)
                {
                    CastLog fire = magicExplose[i];
                    int start = (int)charge.getTime();
                    int end = (int)fire.getTime() + fire.getActDur();
                    replay.addCircleActor(new CircleActor(false, 0, 300, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
                    replay.addCircleActor(new CircleActor(true, end, 300, new Tuple<int, int>(start, end), "rgba(255, 0, 0, 0.5)"));
                }
            }
            List<CastLog> towerDrop = cls.Where(x => x.getID() == 35086).ToList();
            foreach (CastLog c in towerDrop)
            {
                int start = (int)c.getTime();
                int end = start + c.getActDur();
                Point3D pos = replay.getPositions().FirstOrDefault(x => x.time > end);
                if (pos != null)
                {
                    replay.addCircleActor(new CircleActor(false, 0, 400, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", pos));
                    replay.addCircleActor(new CircleActor(true, end, 400, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)", pos));
                }
            }
            return ids;
        }

        public override void getAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            // Bombs
            List<CombatItem> xeraFury = getFilteredList(log, 35103, p.GetInstid());
            int xeraFuryStart = 0;
            int xeraFuryEnd = 0;
            foreach (CombatItem c in xeraFury)
            {
                if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    xeraFuryStart = (int)(c.getTime() - log.GetBossData().getFirstAware());
                }
                else
                {
                    xeraFuryEnd = (int)(c.getTime() - log.GetBossData().getFirstAware());
                    replay.addCircleActor(new CircleActor(true, 0, 550, new Tuple<int, int>(xeraFuryStart, xeraFuryEnd), "rgba(200, 150, 0, 0.2)"));
                    replay.addCircleActor(new CircleActor(true, xeraFuryEnd, 550, new Tuple<int, int>(xeraFuryStart, xeraFuryEnd), "rgba(200, 150, 0, 0.4)"));
                }

            }
        }

        public override string getReplayIcon()
        {
            return "https://i.imgur.com/Kq0kL07.png";
        }
    }
}
