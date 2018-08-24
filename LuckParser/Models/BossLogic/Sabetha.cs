using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class Sabetha : BossLogic
    {
        public Sabetha()
        {
            Mode = ParseMode.Raid;
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(34108, "Shell-Shocked", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Sabetha, "symbol:'circle-open',color:'rgb(0,128,0)',", "Lnchd",0), //Shell-Shocked (launched up to cannons), Shell-Shocked
            new Mechanic(31473, "Sapper Bomb", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Sabetha, "symbol:'circle',color:'rgb(0,128,0)',", "SBmb",0), //Got a Sapper Bomb, Sapper Bomb
            new Mechanic(31485, "Time Bomb", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Sabetha, "symbol:'circle-open',color:'rgb(255,0,0)',", "TBmb",0),//Got a Timed Bomb (Expanding circle), Timed Bomb//or 3? buff or hits //It's the buff, not the hit (see below)
            new Mechanic(31332, "Firestorm", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'square',color:'rgb(255,0,0)',", "Flmwll",0), //Firestorm (killed by Flamewall), Flamewall
            new Mechanic(31544, "Flak Shot", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'hexagram-open',color:'rgb(255,140,0)',", "Flak",0), //Flak Shot (Fire Patches), Flak Shot
            new Mechanic(31643, "Cannon Barrage", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'circle',color:'rgb(255,200,0)',", "Cannon",0), //Cannon Barrage (stood in AoE), Cannon Shot
            new Mechanic(31761, "Flame Blast", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'triangle-left-open',color:'rgb(255,200,0)',", "Flmthrwr",0), //Flame Blast (Karde's Flamethrower), Flamethrower (Karde)
            new Mechanic(31408, "Kick", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Sabetha, "symbol:'triangle-right',color:'rgb(255,0,255)',", "Kick",0) //Kicked by Bandit, Bandit Kick
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
            long fightDuration = log.GetBossData().GetAwareDuration();
            List<PhaseData> phases = GetInitialPhase(log);
            // Invul check
            List<CombatItem> invulsSab = GetFilteredList(log, 757, boss.GetInstid());
            for (int i = 0; i < invulsSab.Count; i++)
            {
                CombatItem c = invulsSab[i];
                if (c.IsBuffremove() == ParseEnum.BuffRemove.None)
                {
                    end = c.GetTime() - log.GetBossData().GetFirstAware();
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsSab.Count - 1)
                    {
                        castLogs.Add(new CastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None));
                    }
                }
                else
                {
                    start = c.GetTime() - log.GetBossData().GetFirstAware();
                    phases.Add(new PhaseData(end, start));
                    castLogs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                }
            }
            if (fightDuration - start > 5000 && start >= phases.Last().GetEnd())
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            string[] namesSab = new [] { "Phase 1", "Kernan", "Phase 2", "Knuckles", "Phase 3", "Karde", "Phase 4" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.SetName(namesSab[i - 1]);
                if (i == 2 || i == 4 || i == 6)
                {
                    List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                       ParseEnum.ThrashIDS.Kernan,
                       ParseEnum.ThrashIDS.Knuckles,
                       ParseEnum.ThrashIDS.Karde,
                    };
                    List<AgentItem> champs = log.GetAgentData().GetNPCAgentList().Where(x => ids.Contains(ParseEnum.GetThrashIDS(x.GetID()))).ToList();
                    foreach (AgentItem a in champs)
                    {
                        long agentStart = a.GetFirstAware() - log.GetBossData().GetFirstAware();
                        if (phase.InInterval(agentStart))
                        {
                            phase.AddRedirection(a);
                        }
                    }
                    phase.OverrideStart(log.GetBossData().GetFirstAware());
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
            List<CombatItem> timedBombs = log.GetBoonData().Where(x => x.GetSkillID() == 31485 && (x.GetDstInstid() == p.GetInstid() && x.IsBuffremove() == ParseEnum.BuffRemove.None)).ToList();
            foreach (CombatItem c in timedBombs)
            {
                int start = (int)(c.GetTime() - log.GetBossData().GetFirstAware());
                int end = start + 3000;
                replay.AddCircleActor(new CircleActor(false, 0, 280, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)"));
                replay.AddCircleActor(new CircleActor(true, end, 280, new Tuple<int, int>(start, end), "rgba(255, 150, 0, 0.5)"));
            }
            // Sapper bombs
            List<CombatItem> sapperBombs = GetFilteredList(log, 31473, p.GetInstid());
            int sapperStart = 0;
            foreach (CombatItem c in sapperBombs)
            {
                if (c.IsBuffremove() == ParseEnum.BuffRemove.None)
                {
                    sapperStart = (int)(c.GetTime() - log.GetBossData().GetFirstAware());
                }
                else
                {
                    int sapperEnd = (int)(c.GetTime() - log.GetBossData().GetFirstAware()); replay.AddCircleActor(new CircleActor(false, 0, 180, new Tuple<int, int>(sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)"));
                    replay.AddCircleActor(new CircleActor(true, sapperStart + 5000, 180, new Tuple<int, int>(sapperStart, sapperEnd), "rgba(200, 255, 100, 0.5)"));
                }
            }
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/UqbFp9S.png";
        }
    }
}
