using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models
{
    public class Samarog : RaidLogic
    {
        public Samarog(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new Mechanic(37996, "Shockwave", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(0,0,255)'", "Shkwv","Shockwave from Spears", "Shockwave",0),
            new Mechanic(38168, "Prisoner Sweep", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'hexagon',color:'rgb(0,0,255)'", "Swp","Prisoner Sweep (horizontal)", "Sweep",0),
            new Mechanic(37797, "Trampling Rush", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-right',color:'rgb(255,0,0)'", "Trmpl","Trampling Rush (hit by stampede towards home)", "Trampling Rush",0),
            new Mechanic(38305, "Bludgeon", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-down',color:'rgb(0,0,255)'", "Slm","Bludgeon (vertical Slam)", "Slam",0),
            new Mechanic(37868, "Fixate: Samarog", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'star',color:'rgb(255,0,255)'", "S.Fix","Fixated by Samarog", "Fixate: Samarog",0),
            new Mechanic(38223, "Fixate: Guldhem", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'star-open',color:'rgb(255,100,0)'", "G.Fix","Fixated by Guldhem", "Fixate: Guldhem",0),
            new Mechanic(37693, "Fixate: Rigom", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'star-open',color:'rgb(255,0,0)'", "R.Fix","Fixated by Rigom", "Fixate: Rigom",0),
            new Mechanic(37966, "Big Hug", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(0,128,0)'", "BgGrn","Big Green (friends mechanic)", "Big Green",0), 
            new Mechanic(38247, "Small Hug", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'circle-open',color:'rgb(0,128,0)'", "SmGrn","Small Green (friends mechanic)", "Small Green",0),
            new Mechanic(38180, "Spear Return", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-left',color:'rgb(255,0,0)'", "SprRtn","Hit by Spear Return", "Spear Return",0),
            new Mechanic(38260, "Inevitable Betrayal", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(255,0,0)'", "G.Fail","Inevitable Betrayal (failed Green)", "Failed Green",0),
            new Mechanic(37851, "Inevitable Betrayal", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(255,0,0)'", "G.Fail","Inevitable Betrayal (failed Green)", "Failed Green",0),
            new Mechanic(37901, "Effigy Pulse", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-down-open',color:'rgb(255,0,0)'", "S.Pls","Effigy Pulse (Stood in Spear AoE)", "Spear Aoe",0),
            new Mechanic(37816, "Spear Impact", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'triangle-down',color:'rgb(255,0,0)'", "S.Spwn","Spear Impact (hit by spawning Spear)", "Spear Spawned",0),
            new Mechanic(38199, "Brutalize", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'diamond-tall',color:'rgb(255,0,255)'","Brtlz","Brutalize (jumped upon by Samarog->Breakbar)", "Brutalize",0),
            new Mechanic(38136, "Brutalize (Jump End)", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Samarog, "symbol:'diamond-tall',color:'rgb(0,160,150)'","CC","Brutalize (Breakbar)", "Breakbar",0),
            new Mechanic(38013, "Brutalize", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'diamond-tall',color:'rgb(255,0,0)'", "CC.Fail","Brutalize (Failed CC)", "CC Fail",0,(condition => condition.DamageLog.Result == ParseEnum.Result.KillingBlow)),
            new Mechanic(38013, "Brutalize", Mechanic.MechType.EnemyCastEnd, ParseEnum.BossIDS.Samarog, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "CC.End","Ended Brutalize", "CC Ended",0,(condition => (condition.CombatItem.IsActivation == ParseEnum.Activation.CancelCancel || condition.CombatItem.IsActivation == ParseEnum.Activation.CancelFire))),
            //new Mechanic(38199, "Brutalize", Mechanic.MechType.PlayerBoonRemove, ParseEnum.BossIDS.Samarog, "symbol:'diamond-tall',color:'rgb(0,160,0)'", "CCed","Ended Brutalize (Breakbar broken)", "CCEnded",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.Manual)),
            //new Mechanic(38199, "Brutalize", Mechanic.MechType.EnemyBoonStrip, ParseEnum.BossIDS.Samarog, "symbol:'diamond-tall',color:'rgb(110,160,0)'", "CCed1","Ended Brutalize (Breakbar broken)", "CCed1",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.All)),
            new Mechanic(37892, "Soul Swarm", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Samarog, "symbol:'x-thin-open',color:'rgb(0,255,255)'","Wall","Soul Swarm (stood in or beyond Spear Wall)", "Spear Wall",0),
            new Mechanic(38231, "Impaling Stab", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'hourglass',color:'rgb(0,0,255)'","ShWv.Ctr","Impaling Stab (hit by Spears causing Shockwave)", "Shockwave Center",0),
            new Mechanic(38314, "Anguished Bolt", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'circle',color:'rgb(255,140,0)'","Stun","Anguished Bolt (AoE Stun Circle by Guldhem)", "Guldhem's Stun",0),
            
            //  new Mechanic(37816, "Brutalize", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, "symbol:'star-square',color:'rgb(255,0,0)'", "CC Target", casted without dmg odd
            });
            Extension = "sam";
            IconUrl = "https://wiki.guildwars2.com/images/f/f0/Mini_Samarog.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/o2DHN29.png",
                            Tuple.Create(1221, 1171),
                             Tuple.Create(-6526, 1218, -2423, 5146),
                            Tuple.Create(-27648, -9216, 27648, 12288),
                            Tuple.Create(11774, 4480, 14078, 5376));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Boss mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Samarog);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Determined check
            List<CombatItem> invulsSam = GetFilteredList(log, 762, log.Boss.InstID);         
            for (int i = 0; i < invulsSam.Count; i++)
            {
                CombatItem c = invulsSam[i];
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    end = c.Time - log.FightData.FightStart;
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsSam.Count - 1)
                    {
                        log.Boss.AddCustomCastLog(new CastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None), log);
                    }
                }
                else
                {
                    start = c.Time - log.FightData.FightStart;
                    phases.Add(new PhaseData(end, start));
                    log.Boss.AddCustomCastLog(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None), log);
                }
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            string[] namesSam = new [] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesSam[i - 1];
                phase.DrawArea = i == 1 || i == 3 || i == 5;
                phase.DrawStart = i == 3 || i == 5;
                phase.DrawEnd = i == 1 || i == 3;
                if (i == 2 || i == 4)
                {
                    List<ushort> ids = new List<ushort>
                    {
                       (ushort) Rigom,
                       (ushort) Guldhem
                    };
                    AddTargetsToPhase(phase, ids, log);
                } else
                {
                    phase.Targets.Add(mainTarget);
                }
            }
            return phases;
        }

        protected override List<ushort> GetFightTargetsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.BossIDS.Samarog,
                (ushort)Rigom,
                (ushort)Guldhem,
            };
        }
        

        public override void ComputeAdditionalBossData(Boss boss, ParsedLog log)
        {
            // TODO: facing information (shock wave)
            CombatReplay replay = boss.CombatReplay;
            List<CastLog> cls = boss.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (boss.ID)
            {
                case (ushort)ParseEnum.BossIDS.Samarog:
                    List<CombatItem> brutalize = log.GetBoonData(38226).Where(x => x.IsBuffRemove != ParseEnum.BuffRemove.Manual).ToList();
                    int brutStart = 0;
                    foreach (CombatItem c in brutalize)
                    {
                        if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                        {
                            brutStart = (int)(c.Time - log.FightData.FightStart);
                        }
                        else
                        {
                            int brutEnd = (int)(c.Time - log.FightData.FightStart);
                            replay.Actors.Add(new CircleActor(true, 0, 120, new Tuple<int, int>(brutStart, brutEnd), "rgba(0, 180, 255, 0.3)"));
                        }
                    }
                    break;
                case (ushort)Rigom:
                case (ushort)Guldhem:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeAdditionalPlayerData(Player p, ParsedLog log)
        {
            // big bomb
            CombatReplay replay = p.CombatReplay;
            List<CombatItem> bigbomb = log.GetBoonData(37966).Where(x => (x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None)).ToList();
            foreach (CombatItem c in bigbomb)
            {
                int bigStart = (int)(c.Time - log.FightData.FightStart);
                int bigEnd = bigStart + 6000;
                replay.Actors.Add(new CircleActor(true, 0, 300, new Tuple<int, int>(bigStart, bigEnd), "rgba(150, 80, 0, 0.2)"));
                replay.Actors.Add(new CircleActor(true, bigEnd, 300, new Tuple<int, int>(bigStart, bigEnd), "rgba(150, 80, 0, 0.2)"));
            }
            // small bomb
            List<CombatItem> smallbomb = log.GetBoonData(38247).Where(x => (x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None)).ToList();
            foreach (CombatItem c in smallbomb)
            {
                int smallStart = (int)(c.Time - log.FightData.FightStart);
                int smallEnd = smallStart + 6000;
                replay.Actors.Add(new CircleActor(true, 0, 80, new Tuple<int, int>(smallStart, smallEnd), "rgba(80, 150, 0, 0.3)"));
            }
            // fixated
            List<CombatItem> fixatedSam = GetFilteredList(log, 37868, p.InstID);
            int fixatedSamStart = 0;
            foreach (CombatItem c in fixatedSam)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    fixatedSamStart = Math.Max((int)(c.Time - log.FightData.FightStart), 0);
                }
                else
                {
                    int fixatedSamEnd = (int)(c.Time - log.FightData.FightStart);
                    replay.Actors.Add(new CircleActor(true, 0, 80, new Tuple<int, int>(fixatedSamStart, fixatedSamEnd), "rgba(255, 80, 255, 0.3)"));
                }
            }
            //fixated Ghuldem
            List<CombatItem> fixatedGuldhem = GetFilteredList(log, 38223, p.InstID);
            int fixationGuldhemStart = 0;
            Boss guldhem = null;
            foreach (CombatItem c in fixatedGuldhem)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    fixationGuldhemStart = (int)(c.Time - log.FightData.FightStart);
                    guldhem = Targets.FirstOrDefault(x => x.ID == (ushort)ParseEnum.TrashIDS.Guldhem && c.Time >= x.FirstAware && c.Time <= x.LastAware);
                }
                else
                {
                    int fixationGuldhemEnd = (int)(c.Time - log.FightData.FightStart);
                    Tuple<int, int> duration = new Tuple<int, int>(fixationGuldhemStart, fixationGuldhemEnd);
                    replay.Actors.Add(new LineActor(0, 10, guldhem, duration, "rgba(255, 100, 0, 0.3)"));
                }
            }
            //fixated Rigom
            List<CombatItem> fixatedRigom = GetFilteredList(log, 37693, p.InstID);
            int fixationRigomStart = 0;
            Boss rigom = null;
            foreach (CombatItem c in fixatedRigom)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    fixationRigomStart = (int)(c.Time - log.FightData.FightStart);
                    rigom = Targets.FirstOrDefault(x => x.ID == (ushort)ParseEnum.TrashIDS.Rigom && c.Time >= x.FirstAware && c.Time <= x.LastAware);
                }
                else
                {
                    int fixationRigomEnd = (int)(c.Time - log.FightData.FightStart);
                    Tuple<int, int> duration = new Tuple<int, int>(fixationRigomStart, fixationRigomEnd);
                    replay.Actors.Add(new LineActor(0, 10, rigom, duration, "rgba(255, 0, 0, 0.3)"));
                }
            }
        }

        public override int IsCM(ParsedLog log)
        {
            Boss target = Targets.Find(x => x.ID == (ushort)ParseEnum.BossIDS.Samarog);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.Health > 30e6) ? 1 : 0;
        }
    }
}
