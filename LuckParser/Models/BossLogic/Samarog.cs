using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class Samarog : BossLogic
    {
        public Samarog()
        {
            Mode = ParseMode.Raid;
            MechanicList.AddRange(new List<Mechanic>
            {

            new Mechanic(37996, "Shockwave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(0,0,255)',", "Shkwv",0), //Shockwave from Spears, Shockwave
            new Mechanic(38168, "Prisoner Sweep", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'hexagon',color:'rgb(0,0,255)',", "Swp",0), //Prisoner Sweep (horizontal), Sweep
            new Mechanic(38305, "Bludgeon", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-down',color:'rgb(0,0,255)',", "Slm",0), //Bludgeon (vertical Slam), Slam
            new Mechanic(37868, "Fixate: Samarog", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'star',color:'rgb(255,0,255)',", "S.Fix",0), //Fixated by Samarog, Fixate: Samarog
            new Mechanic(38223, "Fixate: Guldhem", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'star-open',color:'rgb(255,200,0)',", "G.Fix",0), //Fixated by Guldhem, Fixate: Guldhem
            new Mechanic(37693, "Fixate: Rigom", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'star-open',color:'rgb(255,0,0)',", "R.Fix",0), //Fixated by Rigom, Fixate: Rigom
            new Mechanic(37966, "Big Hug", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(0,128,0)',", "BgGrn",0), // Big Green (friends mechanic), Big Green
            new Mechanic(38247, "Small Hug", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'circle-open',color:'rgb(0,128,0)',", "SmGrn",0), //Small Green (friends mechanic), Small Green
            new Mechanic(38180, "Spear Return", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-left',color:'rgb(255,0,0)',", "SprRtn",0), //Hit by Spear Return, Spear Return
            new Mechanic(38260, "Inevitable Betrayal", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(255,0,0)',", "G.Fail",0), //Inevitable Betrayal (failed Green), Failed Green
            new Mechanic(37851, "Inevitable Betrayal", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(255,0,0)',", "G.Fail",0), //Inevitable Betrayal (failed Green), Failed Green
            new Mechanic(37901, "Effigy Pulse", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-down-open',color:'rgb(255,0,0)',", "S.Pls",0), //Effigy Pulse (Stood in Spear AoE), Spear Aoe
            new Mechanic(37816, "Spear Impact", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-down',color:'rgb(255,0,0)',", "S.Spwn",0), // Spear Impact (hit by spawning Spear), Spear Spawned
            new Mechanic(38199, "Brutalize", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'diamond',color:'rgb(255,0,255)',","CC",0), //Brutalize (jumped upon by Samarog->Breakbar), Brutalize (Breakbar)
            new Mechanic(37892, "Soul Swarm", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'x-thin-open',color:'rgb(0,255,255)',","Wall",0), // Soul Swarm (stood in or beyond Spear Wall), Spear Wall
            new Mechanic(38231, "Impaling Stab", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'hourglass',color:'rgb(0,0,255)',","ShWv.Ctr",0), //Impaling Stab (hit by Spears causing Shockwave), Shockwave Center
            
            //  new Mechanic(37816, "Brutalize", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'star-square',color:'rgb(255,0,0)',", "CC Target", casted without dmg odd
            });
        }

        public override CombatReplayMap GetCombatMap()
        {
            return new CombatReplayMap("https://i.imgur.com/o2DHN29.png",
                            Tuple.Create(1221, 1171),
                             Tuple.Create(-6526, 1218, -2423, 5146),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
        }

        public override List<PhaseData> GetPhases(Boss boss, ParsedLog log, List<CastLog> castLogs)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.GetBossData().GetAwareDuration();
            List<PhaseData> phases = GetInitialPhase(log);
            // Determined check
            List<CombatItem> invulsSam = GetFilteredList(log, 762, boss.GetInstid());         
            for (int i = 0; i < invulsSam.Count; i++)
            {
                CombatItem c = invulsSam[i];
                if (c.IsBuffremove() == ParseEnum.BuffRemove.None)
                {
                    end = c.GetTime() - log.GetBossData().GetFirstAware();
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsSam.Count - 1)
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
            string[] namesSam = new [] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.SetName(namesSam[i - 1]);
                if (i == 2 || i == 4)
                {
                    List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                       ParseEnum.ThrashIDS.Rigom,
                       ParseEnum.ThrashIDS.Guldhem
                    };
                    List<AgentItem> slaves = log.GetAgentData().GetNPCAgentList().Where(x => ids.Contains(ParseEnum.GetThrashIDS(x.GetID()))).ToList();
                    foreach (AgentItem a in slaves)
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
            // TODO: facing information (shock wave)
            List<ParseEnum.ThrashIDS> ids = new List<ParseEnum.ThrashIDS>
                    {
                        ParseEnum.ThrashIDS.Rigom,
                        ParseEnum.ThrashIDS.Guldhem
                    };
            List<CombatItem> brutalize = log.GetBoonData().Where(x => x.GetSkillID() == 38226 && x.IsBuffremove() != ParseEnum.BuffRemove.Manual).ToList();
            int brutStart = 0;
            foreach (CombatItem c in brutalize)
            {
                if (c.IsBuffremove() == ParseEnum.BuffRemove.None)
                {
                    brutStart = (int)(c.GetTime() - log.GetBossData().GetFirstAware());
                }
                else
                {
                    int brutEnd = (int)(c.GetTime() - log.GetBossData().GetFirstAware());
                    replay.AddCircleActor(new CircleActor(true, 0, 120, new Tuple<int, int>(brutStart, brutEnd), "rgba(0, 180, 255, 0.3)"));
                }
            }
            return ids;
        }

        public override void GetAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            // big bomb
            List<CombatItem> bigbomb = log.GetBoonData().Where(x => x.GetSkillID() == 37966 && ((x.GetDstInstid() == p.GetInstid() && x.IsBuffremove() == ParseEnum.BuffRemove.None))).ToList();
            foreach (CombatItem c in bigbomb)
            {
                int bigStart = (int)(c.GetTime() - log.GetBossData().GetFirstAware());
                int bigEnd = bigStart + 6000;
                replay.AddCircleActor(new CircleActor(true, 0, 300, new Tuple<int, int>(bigStart, bigEnd), "rgba(150, 80, 0, 0.2)"));
                replay.AddCircleActor(new CircleActor(true, bigEnd, 300, new Tuple<int, int>(bigStart, bigEnd), "rgba(150, 80, 0, 0.2)"));
            }
            // small bomb
            List<CombatItem> smallbomb = log.GetBoonData().Where(x => x.GetSkillID() == 38247 && ((x.GetDstInstid() == p.GetInstid() && x.IsBuffremove() == ParseEnum.BuffRemove.None))).ToList();
            foreach (CombatItem c in smallbomb)
            {
                int smallStart = (int)(c.GetTime() - log.GetBossData().GetFirstAware());
                int smallEnd = smallStart + 6000;
                replay.AddCircleActor(new CircleActor(true, 0, 80, new Tuple<int, int>(smallStart, smallEnd), "rgba(80, 150, 0, 0.3)"));
            }
            // fixated
            List<CombatItem> fixatedSam = GetFilteredList(log, 37868, p.GetInstid());
            int fixatedSamStart = 0;
            foreach (CombatItem c in fixatedSam)
            {
                if (c.IsBuffremove() == ParseEnum.BuffRemove.None)
                {
                    fixatedSamStart = (int)(c.GetTime() - log.GetBossData().GetFirstAware());
                }
                else
                {
                    int fixatedSamEnd = (int)(c.GetTime() - log.GetBossData().GetFirstAware());
                    replay.AddCircleActor(new CircleActor(true, 0, 80, new Tuple<int, int>(fixatedSamStart, fixatedSamEnd), "rgba(255, 80, 255, 0.3)"));
                }
            }
        }

        public override int IsCM(List<CombatItem> clist, int health)
        {
            return (health > 30e6) ? 1 : 0;
        }

        public override string GetReplayIcon()
        {
            return "https://i.imgur.com/MPQhKfM.png";
        }
    }
}
