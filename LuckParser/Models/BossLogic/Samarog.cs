using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class Samarog : RaidLogic
    {
        public Samarog()
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new Mechanic(37996, "Shockwave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(0,0,255)',", "Shkwv","Shockwave from Spears", "Shockwave",0),
            new Mechanic(38168, "Prisoner Sweep", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'hexagon',color:'rgb(0,0,255)',", "Swp","Prisoner Sweep (horizontal)", "Sweep",0),
            new Mechanic(38305, "Bludgeon", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-down',color:'rgb(0,0,255)',", "Slm","Bludgeon (vertical Slam)", "Slam",0),
            new Mechanic(37868, "Fixate: Samarog", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'star',color:'rgb(255,0,255)',", "S.Fix","Fixated by Samarog", "Fixate: Samarog",0),
            new Mechanic(38223, "Fixate: Guldhem", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'star-open',color:'rgb(255,200,0)',", "G.Fix","Fixated by Guldhem", "Fixate: Guldhem",0),
            new Mechanic(37693, "Fixate: Rigom", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'star-open',color:'rgb(255,0,0)',", "R.Fix","Fixated by Rigom", "Fixate: Rigom",0),
            new Mechanic(37966, "Big Hug", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(0,128,0)',", "BgGrn","Big Green (friends mechanic)", "Big Green",0), 
            new Mechanic(38247, "Small Hug", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'circle-open',color:'rgb(0,128,0)',", "SmGrn","Small Green (friends mechanic)", "Small Green",0),
            new Mechanic(38180, "Spear Return", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-left',color:'rgb(255,0,0)',", "SprRtn","Hit by Spear Return", "Spear Return",0),
            new Mechanic(38260, "Inevitable Betrayal", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(255,0,0)',", "G.Fail","Inevitable Betrayal (failed Green)", "Failed Green",0),
            new Mechanic(37851, "Inevitable Betrayal", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(255,0,0)',", "G.Fail","Inevitable Betrayal (failed Green)", "Failed Green",0),
            new Mechanic(37901, "Effigy Pulse", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-down-open',color:'rgb(255,0,0)',", "S.Pls","Effigy Pulse (Stood in Spear AoE)", "Spear Aoe",0),
            new Mechanic(37816, "Spear Impact", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-down',color:'rgb(255,0,0)',", "S.Spwn","Spear Impact (hit by spawning Spear)", "Spear Spawned",0), 
            new Mechanic(38199, "Brutalize", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'diamond',color:'rgb(255,0,255)',","CC","Brutalize (jumped upon by Samarog->Breakbar)", "Brutalize (Breakbar)",0),
            new Mechanic(37892, "Soul Swarm", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'x-thin-open',color:'rgb(0,255,255)',","Wall","Soul Swarm (stood in or beyond Spear Wall)", "Spear Wall",0),
            new Mechanic(38231, "Impaling Stab", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'hourglass',color:'rgb(0,0,255)',","ShWv.Ctr","Impaling Stab (hit by Spears causing Shockwave)", "Shockwave Center",0),
            new Mechanic(38314, "Anguished Bolt", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(255,140,0)',","Stun","Anguished Bolt (AoE Stun Circle by Guldhem)", "Guldhem's Stun",0),
            
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
            long fightDuration = log.GetFightData().FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            // Determined check
            List<CombatItem> invulsSam = GetFilteredList(log, 762, boss.InstID);         
            for (int i = 0; i < invulsSam.Count; i++)
            {
                CombatItem c = invulsSam[i];
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    end = c.Time - log.GetFightData().FightStart;
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsSam.Count - 1)
                    {
                        castLogs.Add(new CastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None));
                    }
                }
                else
                {
                    start = c.Time - log.GetFightData().FightStart;
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
                    List<AgentItem> slaves = log.GetAgentData().GetNPCAgentList().Where(x => ids.Contains(ParseEnum.GetThrashIDS(x.ID))).ToList();
                    foreach (AgentItem a in slaves)
                    {
                        long agentStart = a.FirstAware - log.GetFightData().FightStart;
                        if (phase.InInterval(agentStart))
                        {
                            phase.AddRedirection(a);
                        }
                    }
                    phase.OverrideStart(log.GetFightData().FightStart);
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
            List<CombatItem> brutalize = log.GetBoonData(38226).Where(x => x.IsBuffRemove != ParseEnum.BuffRemove.Manual).ToList();
            int brutStart = 0;
            foreach (CombatItem c in brutalize)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    brutStart = (int)(c.Time - log.GetFightData().FightStart);
                }
                else
                {
                    int brutEnd = (int)(c.Time - log.GetFightData().FightStart);
                    replay.AddCircleActor(new CircleActor(true, 0, 120, new Tuple<int, int>(brutStart, brutEnd), "rgba(0, 180, 255, 0.3)"));
                }
            }
            return ids;
        }

        public override void GetAdditionalPlayerData(CombatReplay replay, Player p, ParsedLog log)
        {
            // big bomb
            List<CombatItem> bigbomb = log.GetBoonData(37966).Where(x => (x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None)).ToList();
            foreach (CombatItem c in bigbomb)
            {
                int bigStart = (int)(c.Time - log.GetFightData().FightStart);
                int bigEnd = bigStart + 6000;
                replay.AddCircleActor(new CircleActor(true, 0, 300, new Tuple<int, int>(bigStart, bigEnd), "rgba(150, 80, 0, 0.2)"));
                replay.AddCircleActor(new CircleActor(true, bigEnd, 300, new Tuple<int, int>(bigStart, bigEnd), "rgba(150, 80, 0, 0.2)"));
            }
            // small bomb
            List<CombatItem> smallbomb = log.GetBoonData(38247).Where(x => (x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None)).ToList();
            foreach (CombatItem c in smallbomb)
            {
                int smallStart = (int)(c.Time - log.GetFightData().FightStart);
                int smallEnd = smallStart + 6000;
                replay.AddCircleActor(new CircleActor(true, 0, 80, new Tuple<int, int>(smallStart, smallEnd), "rgba(80, 150, 0, 0.3)"));
            }
            // fixated
            List<CombatItem> fixatedSam = GetFilteredList(log, 37868, p.InstID);
            int fixatedSamStart = 0;
            foreach (CombatItem c in fixatedSam)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    fixatedSamStart = (int)(c.Time - log.GetFightData().FightStart);
                }
                else
                {
                    int fixatedSamEnd = (int)(c.Time - log.GetFightData().FightStart);
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
