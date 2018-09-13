using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class Sabetha : RaidLogic
    {
        public Sabetha()
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(34108, "Shell-Shocked", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Sabetha, "symbol:'circle-open',color:'rgb(0,128,0)',", "Lnchd","Shell-Shocked (launched up to cannons)", "Shell-Shocked",0),
            new Mechanic(31473, "Sapper Bomb", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Sabetha, "symbol:'circle',color:'rgb(0,128,0)',", "SBmb","Got a Sapper Bomb", "Sapper Bomb",0),
            new Mechanic(31485, "Time Bomb", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Sabetha, "symbol:'circle-open',color:'rgb(255,0,0)',", "TBmb","Got a Timed Bomb (Expanding circle)", "Timed Bomb",0),
            new Mechanic(31332, "Firestorm", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'square',color:'rgb(255,0,0)',", "Flmwll","Firestorm (killed by Flamewall)", "Flamewall",0),
            new Mechanic(31544, "Flak Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'hexagram-open',color:'rgb(255,140,0)',", "Flak","Flak Shot (Fire Patches)", "Flak Shot",0),
            new Mechanic(31643, "Cannon Barrage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'circle',color:'rgb(255,200,0)',", "Cannon","Cannon Barrage (stood in AoE)", "Cannon Shot",0),
            new Mechanic(31761, "Flame Blast", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'triangle-left-open',color:'rgb(255,200,0)',", "Flmthrwr","Flame Blast (Karde's Flamethrower)", "Flamethrower (Karde)",0),
            new Mechanic(31408, "Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'triangle-right',color:'rgb(255,0,255)',", "Kick","Kicked by Bandit", "Bandit Kick",0) 
            // Hit by Time Bomb could be implemented by checking if a person is affected by ID 31324 (1st Time Bomb) or 34152 (2nd Time Bomb, only below 50% boss HP) without being attributed a bomb (ID: 31485) 3000ms before (+-50ms). I think the actual heavy hit isn't logged because it may be percentage based. Nothing can be found in the logs.
            });
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/FwpMbYf.png",
                            Tuple.Create(2790, 2763),
                            Tuple.Create(-8587, -162, -1601, 6753),
                            Tuple.Create(-15360, -36864, 15360, 39936),
                            Tuple.Create(3456, 11012, 4736, 14212));
        }

        public override List<PhaseData> GetPhases(Boss boss, ParsedLog log, List<CastLog> castLogs)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            // Invul check
            List<CombatItem> invulsSab = GetFilteredList(log, 757, boss.InstID);
            for (int i = 0; i < invulsSab.Count; i++)
            {
                CombatItem c = invulsSab[i];
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    end = c.Time - log.FightData.FightStart;
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsSab.Count - 1)
                    {
                        castLogs.Add(new CastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None));
                    }
                }
                else
                {
                    start = c.Time - log.FightData.FightStart;
                    phases.Add(new PhaseData(end, start));
                    castLogs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                }
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            string[] namesSab = new [] { "Phase 1", "Kernan", "Phase 2", "Knuckles", "Phase 3", "Karde", "Phase 4" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesSab[i - 1];
                if (i == 2 || i == 4 || i == 6)
                {
                    List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                       ParseEnum.ThrashIDS.Kernan,
                       ParseEnum.ThrashIDS.Knuckles,
                       ParseEnum.ThrashIDS.Karde,
                    };
                    List<AgentItem> champs = log.AgentData.NPCAgentList.Where(x => ids.Contains(ParseEnum.GetThrashIDS(x.ID))).ToList();
                    foreach (AgentItem a in champs)
                    {
                        long agentStart = a.FirstAware - log.FightData.FightStart;
                        if (phase.InInterval(agentStart))
                        {
                            phase.Redirection.Add(a);
                        }
                    }
                    phase.OverrideStart(log.FightData.FightStart);
                }
            }
            return phases;
        }

        public override List<ParseEnum.ThrashIDS> GetAdditionalData(CombatReplay replay, List<CastLog> cls, ParsedLog log)
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

        public override void GetAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            // timed bombs
            List<CombatItem> timedBombs = log.GetBoonData(31485).Where(x => x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None).ToList();
            foreach (CombatItem c in timedBombs)
            {
                int start = (int)(c.Time - log.FightData.FightStart);
                int end = start + 3000;
                replay.CircleActors.Add(new CircleActor(false, 0, 280, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)"));
                replay.CircleActors.Add(new CircleActor(true, end, 280, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)"));
            }
            // Sapper bombs
            List<CombatItem> sapperBombs = GetFilteredList(log, 31473, p.InstID);
            int sapperStart = 0;
            foreach (CombatItem c in sapperBombs)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    sapperStart = (int)(c.Time - log.FightData.FightStart);
                }
                else
                {
                    int sapperEnd = (int)(c.Time - log.FightData.FightStart); replay.CircleActors.Add(new CircleActor(false, 0, 180, new Tuple<int, int>(sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)"));
                    replay.CircleActors.Add(new CircleActor(true, sapperStart + 5000, 180, new Tuple<int, int>(sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)"));
                }
            }
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/UqbFp9S.png";
        }
    }
}
