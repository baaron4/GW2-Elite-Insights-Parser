using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public class Sabetha : BossLogic
    {
        public Sabetha() : base()
        {
            mode = ParseMode.Raid;
            mechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(34108, "Shell-Shocked", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Sabetha, "symbol:'circle-open',color:'rgb(0,128,0)',", "Shell-Shocked",0),
            new Mechanic(31473, "Sapper Bomb", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Sabetha, "symbol:'circle',color:'rgb(0,128,0)',", "Sapper Bomb",0),
            new Mechanic(31485, "Time Bomb", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Sabetha, "symbol:'circle-open',color:'rgb(255,140,0)',", "Time Bomb",0),//or 3? buff or hits //It's the buff, not the hit (see below)
            new Mechanic(31332, "Firestorm", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'square',color:'rgb(255,0,0)',", "Flamewall",0),
            new Mechanic(31544, "Flak Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'hexagram-open',color:'rgb(255,140,0)',", "Flak Shot",0),
            new Mechanic(31643, "Cannon Barrage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'hexagon-open',color:'rgb(255,200,0)',", "Cannon Shot",0),
            new Mechanic(31761, "Flame Blast", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'triangle-left-open',color:'rgb(255,200,0)',", "Flamethrower (Karde)",0),
            new Mechanic(31408, "Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'triangle-right',color:'rgb(255,0,255)',", "Kicked by Bandit",0)
            // Hit by Time Bomb could be implemented by checking if a person is affected by ID 31324 (1st Time Bomb) or 34152 (2nd Time Bomb, only below 50% boss HP) without being attributed a bomb (ID: 31485) 3000ms before (+-50ms). I think the actual heavy hit isn't logged because it may be percentage based. Nothing can be found in the logs.
            });
        }

        public override CombatReplayMap getCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/FwpMbYf.png",
                            Tuple.Create(2790, 2763),
                            Tuple.Create(-8587, -162, -1601, 6753),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
        }

        public override List<PhaseData> getPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            long start = 0;
            long end = 0;
            long fight_dur = log.GetBossData().getAwareDuration();
            List<PhaseData> phases = getInitialPhase(log);
            // Invul check
            List<CombatItem> invulsSab = getFilteredList(log, 757, boss.GetInstid());
            for (int i = 0; i < invulsSab.Count; i++)
            {
                CombatItem c = invulsSab[i];
                if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    end = c.getTime() - log.GetBossData().getFirstAware();
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsSab.Count - 1)
                    {
                        cast_logs.Add(new CastLog(end, -5, (int)(fight_dur - end), ParseEnum.Activation.None, (int)(fight_dur - end), ParseEnum.Activation.None));
                    }
                }
                else
                {
                    start = c.getTime() - log.GetBossData().getFirstAware();
                    phases.Add(new PhaseData(end, start));
                    cast_logs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                }
            }
            if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
            {
                phases.Add(new PhaseData(start, fight_dur));
            }
            string[] namesSab = new string[] { "Phase 1", "Kernan", "Phase 2", "Knuckles", "Phase 3", "Karde", "Phase 4" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.setName(namesSab[i - 1]);
                if (i == 2 || i == 4 || i == 6)
                {
                    List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                       ParseEnum.ThrashIDS.Kernan,
                       ParseEnum.ThrashIDS.Knuckles,
                       ParseEnum.ThrashIDS.Karde,
                    };
                    List<AgentItem> champs = log.GetAgentData().getNPCAgentList().Where(x => ids.Contains(ParseEnum.getThrashIDS(x.getID()))).ToList();
                    foreach (AgentItem a in champs)
                    {
                        long agentStart = a.getFirstAware() - log.GetBossData().getFirstAware();
                        long agentEnd = a.getLastAware() - log.GetBossData().getFirstAware();
                        if (phase.inInterval(agentStart))
                        {
                            phase.addRedirection(a);
                        }
                    }
                    phase.overrideStart(log.GetBossData().getFirstAware());
                }
            }
            return phases;
        }

        public override List<ParseEnum.ThrashIDS> getAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
        {
            // TODO:facing information (flame wall)
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Kernan,
                        ParseEnum.ThrashIDS.Knuckles,
                        ParseEnum.ThrashIDS.Karde,
                        ParseEnum.ThrashIDS.BanditSapper,
                        ParseEnum.ThrashIDS.BanditThug,
                        ParseEnum.ThrashIDS.BanditArsonist
                    };
            return ids;
        }

        public override void getAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            // timed bombs
            List<CombatItem> timedBombs = log.GetBoonData().Where(x => x.getSkillID() == 31485 && (x.getDstInstid() == p.GetInstid() && x.isBuffremove() == ParseEnum.BuffRemove.None)).ToList();
            foreach (CombatItem c in timedBombs)
            {
                int start = (int)(c.getTime() - log.GetBossData().getFirstAware());
                int end = start + 3000;
                replay.addCircleActor(new CircleActor(false, 0, 280, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)"));
                replay.addCircleActor(new CircleActor(true, end, 280, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)"));
            }
            // Sapper bombs
            List<CombatItem> sapperBombs = getFilteredList(log, 31473, p.GetInstid());
            int sapperStart = 0;
            int sapperEnd = 0;
            foreach (CombatItem c in sapperBombs)
            {
                if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    sapperStart = (int)(c.getTime() - log.GetBossData().getFirstAware());
                }
                else
                {
                    sapperEnd = (int)(c.getTime() - log.GetBossData().getFirstAware()); replay.addCircleActor(new CircleActor(false, 0, 180, new Tuple<int, int>(sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)"));
                    replay.addCircleActor(new CircleActor(true, sapperStart + 5000, 180, new Tuple<int, int>(sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)"));
                }
            }
        }

        public override string getReplayIcon()
        {
            return "https://i.imgur.com/UqbFp9S.png";
        }
    }
}
