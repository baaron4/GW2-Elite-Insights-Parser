using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Samarog : RaidLogic
    {
        public Samarog(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new Mechanic(37996, "Shockwave", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(0,0,255)"), "Shockwave","Shockwave from Spears", "Shockwave",0),
            new Mechanic(38168, "Prisoner Sweep", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("hexagon","rgb(0,0,255)"), "Sweep","Prisoner Sweep (horizontal)", "Sweep",0),
            new Mechanic(37797, "Trampling Rush", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-right","rgb(255,0,0)"), "Trample","Trampling Rush (hit by stampede towards home)", "Trampling Rush",0),
            new Mechanic(38305, "Bludgeon", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-down","rgb(0,0,255)"), "Slam","Bludgeon (vertical Slam)", "Slam",0),
            new Mechanic(37868, "Fixate: Samarog", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("star","rgb(255,0,255)"), "Sam Fix","Fixated by Samarog", "Fixate: Samarog",0),
            new Mechanic(38223, "Fixate: Guldhem", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("star-open","rgb(255,100,0)"), "Ghuld Fix","Fixated by Guldhem", "Fixate: Guldhem",0),
            new Mechanic(37693, "Fixate: Rigom", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("star-open","rgb(255,0,0)"), "Rigom Fix","Fixated by Rigom", "Fixate: Rigom",0),
            new Mechanic(37966, "Big Hug", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("circle","rgb(0,128,0)"), "Big Green","Big Green (friends mechanic)", "Big Green",0), 
            new Mechanic(38247, "Small Hug", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("circle-open","rgb(0,128,0)"), "Small Green","Small Green (friends mechanic)", "Small Green",0),
            new Mechanic(38180, "Spear Return", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-left","rgb(255,0,0)"), "Spear Return","Hit by Spear Return", "Spear Return",0),
            new Mechanic(38260, "Inevitable Betrayal", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Green Fail","Inevitable Betrayal (failed Green)", "Failed Green",0),
            new Mechanic(37851, "Inevitable Betrayal", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Green Fail","Inevitable Betrayal (failed Green)", "Failed Green",0),
            new Mechanic(37901, "Effigy Pulse", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-down-open","rgb(255,0,0)"), "Spear Pulse","Effigy Pulse (Stood in Spear AoE)", "Spear Aoe",0),
            new Mechanic(37816, "Spear Impact", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("triangle-down","rgb(255,0,0)"), "Spear Spawn","Spear Impact (hit by spawning Spear)", "Spear Spawned",0),
            new Mechanic(38199, "Brutalize", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("diamond-tall","rgb(255,0,255)"),"Brutalize","Brutalize (jumped upon by Samarog->Breakbar)", "Brutalize",0),
            new Mechanic(38136, "Brutalize (Jump End)", Mechanic.MechType.EnemyCastEnd, new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"),"CC","Brutalize (Breakbar)", "Breakbar",0),
            new Mechanic(38013, "Brutalize", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","Brutalize (Failed CC)", "CC Fail",0,(condition => condition.DamageLog.Result == ParseEnum.Result.KillingBlow)),
            new Mechanic(38013, "Brutalize", Mechanic.MechType.EnemyCastEnd, new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CC End","Ended Brutalize", "CC Ended",0,(condition => (condition.CombatItem.IsActivation == ParseEnum.Activation.CancelCancel || condition.CombatItem.IsActivation == ParseEnum.Activation.CancelFire))),
            //new Mechanic(38199, "Brutalize", Mechanic.MechType.PlayerBoonRemove, ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Ended Brutalize (Breakbar broken)", "CCEnded",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.Manual)),
            //new Mechanic(38199, "Brutalize", Mechanic.MechType.EnemyBoonStrip, ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting("diamond-tall","rgb(110,160,0)"), "CCed1","Ended Brutalize (Breakbar broken)", "CCed1",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.All)),
            new Mechanic(37892, "Soul Swarm", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("x-thin-open","rgb(0,255,255)"),"Wall","Soul Swarm (stood in or beyond Spear Wall)", "Spear Wall",0),
            new Mechanic(38231, "Impaling Stab", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("hourglass","rgb(0,0,255)"),"ShockWv Ctr","Impaling Stab (hit by Spears causing Shockwave)", "Shockwave Center",0),
            new Mechanic(38314, "Anguished Bolt", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle","rgb(255,140,0)"),"Stun","Anguished Bolt (AoE Stun Circle by Guldhem)", "Guldhem's Stun",0),
            
            //  new Mechanic(37816, "Brutalize", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting("star-square","rgb(255,0,0)"), "CC Target", casted without dmg odd
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
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Samarog);
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
            phases.AddRange(GetPhasesByInvul(log, 762, mainTarget, true, true));
            string[] namesSam = new [] { "Phase 1", "Split 1", "Phase 2", "Split 2", "Phase 3" };
            for (int i = 1; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];
                phase.Name = namesSam[i - 1];
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
                (ushort)ParseEnum.TargetIDS.Samarog,
                (ushort)Rigom,
                (ushort)Guldhem,
            };
        }
        

        public override void ComputeAdditionalTargetData(Target target, ParsedLog log)
        {
            // TODO: facing information (shock wave)
            CombatReplay replay = target.CombatReplay;
            List<Point3D> facings = replay.Rotations;
            if (facings.Any())
            {
                replay.Actors.Add(new FacingActor(new Tuple<int, int>((int)replay.TimeOffsets.Item1, (int)replay.TimeOffsets.Item2), new AgentConnector(target), facings));
            }
            List<CastLog> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Samarog:
                    List<CombatItem> brutalize = log.GetBoonData(38226).Where(x => x.IsBuffRemove != ParseEnum.BuffRemove.Manual).ToList();
                    int brutStart = 0;
                    foreach (CombatItem c in brutalize)
                    {
                        if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                        {
                            brutStart = (int)(log.FightData.ToFightSpace(c.Time));
                        }
                        else
                        {
                            int brutEnd = (int)(log.FightData.ToFightSpace(c.Time));
                            replay.Actors.Add(new CircleActor(true, 0, 120, new Tuple<int, int>(brutStart, brutEnd), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
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
                int bigStart = (int)(log.FightData.ToFightSpace(c.Time));
                int bigEnd = bigStart + 6000;
                replay.Actors.Add(new CircleActor(true, 0, 300, new Tuple<int, int>(bigStart, bigEnd), "rgba(150, 80, 0, 0.2)", new AgentConnector(p)));
                replay.Actors.Add(new CircleActor(true, bigEnd, 300, new Tuple<int, int>(bigStart, bigEnd), "rgba(150, 80, 0, 0.2)", new AgentConnector(p)));
            }
            // small bomb
            List<CombatItem> smallbomb = log.GetBoonData(38247).Where(x => (x.DstInstid == p.InstID && x.IsBuffRemove == ParseEnum.BuffRemove.None)).ToList();
            foreach (CombatItem c in smallbomb)
            {
                int smallStart = (int)(log.FightData.ToFightSpace(c.Time));
                int smallEnd = smallStart + 6000;
                replay.Actors.Add(new CircleActor(true, 0, 80, new Tuple<int, int>(smallStart, smallEnd), "rgba(80, 150, 0, 0.3)", new AgentConnector(p)));
            }
            // fixated
            List<CombatItem> fixatedSam = GetFilteredList(log, 37868, p, true);
            int fixatedSamStart = 0;
            foreach (CombatItem c in fixatedSam)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    fixatedSamStart = Math.Max((int)(log.FightData.ToFightSpace(c.Time)), 0);
                }
                else
                {
                    int fixatedSamEnd = (int)(log.FightData.ToFightSpace(c.Time));
                    replay.Actors.Add(new CircleActor(true, 0, 80, new Tuple<int, int>(fixatedSamStart, fixatedSamEnd), "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
            //fixated Ghuldem
            List<CombatItem> fixatedGuldhem = GetFilteredList(log, 38223, p, true);
            int fixationGuldhemStart = 0;
            Target guldhem = null;
            foreach (CombatItem c in fixatedGuldhem)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    fixationGuldhemStart = (int)(log.FightData.ToFightSpace(c.Time));
                    guldhem = Targets.FirstOrDefault(x => x.ID == (ushort)ParseEnum.TrashIDS.Guldhem && c.Time >= x.FirstAware && c.Time <= x.LastAware);
                }
                else
                {
                    int fixationGuldhemEnd = (int)(log.FightData.ToFightSpace(c.Time));
                    Tuple<int, int> duration = new Tuple<int, int>(fixationGuldhemStart, fixationGuldhemEnd);
                    if (guldhem != null)
                    {
                        replay.Actors.Add(new LineActor(0, duration, "rgba(255, 100, 0, 0.3)", new AgentConnector(p), new AgentConnector(guldhem)));
                    }
                }
            }
            //fixated Rigom
            List<CombatItem> fixatedRigom = GetFilteredList(log, 37693, p, true);
            int fixationRigomStart = 0;
            Target rigom = null;
            foreach (CombatItem c in fixatedRigom)
            {
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    fixationRigomStart = (int)(log.FightData.ToFightSpace(c.Time));
                    rigom = Targets.FirstOrDefault(x => x.ID == (ushort)ParseEnum.TrashIDS.Rigom && c.Time >= x.FirstAware && c.Time <= x.LastAware);
                }
                else
                {
                    int fixationRigomEnd = (int)(log.FightData.ToFightSpace(c.Time));
                    Tuple<int, int> duration = new Tuple<int, int>(fixationRigomStart, fixationRigomEnd);
                    if (rigom != null)
                    {
                        replay.Actors.Add(new LineActor(0, duration, "rgba(255, 0, 0, 0.3)", new AgentConnector(p), new AgentConnector(rigom)));
                    }
                }
            }
        }

        public override int IsCM(ParsedLog log)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Samarog);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            OverrideMaxHealths(log);
            return (target.Health > 30e6) ? 1 : 0;
        }
    }
}
