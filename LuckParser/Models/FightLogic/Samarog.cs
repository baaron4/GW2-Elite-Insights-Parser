using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Samarog : RaidLogic
    {
        public Samarog(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new SkillOnPlayerMechanic(37996, "Shockwave", new MechanicPlotlySetting("circle","rgb(0,0,255)"), "Shockwave","Shockwave from Spears", "Shockwave",0),
            new SkillOnPlayerMechanic(38168, "Prisoner Sweep", new MechanicPlotlySetting("hexagon","rgb(0,0,255)"), "Sweep","Prisoner Sweep (horizontal)", "Sweep",0),
            new SkillOnPlayerMechanic(37797, "Trampling Rush", new MechanicPlotlySetting("triangle-right","rgb(255,0,0)"), "Trample","Trampling Rush (hit by stampede towards home)", "Trampling Rush",0),
            new SkillOnPlayerMechanic(38305, "Bludgeon", new MechanicPlotlySetting("triangle-down","rgb(0,0,255)"), "Slam","Bludgeon (vertical Slam)", "Slam",0),
            new PlayerBoonApplyMechanic(37868, "Fixate: Samarog", new MechanicPlotlySetting("star","rgb(255,0,255)"), "Sam Fix","Fixated by Samarog", "Fixate: Samarog",0),
            new PlayerBoonApplyMechanic(38223, "Fixate: Guldhem", new MechanicPlotlySetting("star-open","rgb(255,100,0)"), "Ghuld Fix","Fixated by Guldhem", "Fixate: Guldhem",0),
            new PlayerBoonApplyMechanic(37693, "Fixate: Rigom", new MechanicPlotlySetting("star-open","rgb(255,0,0)"), "Rigom Fix","Fixated by Rigom", "Fixate: Rigom",0),
            new PlayerBoonApplyMechanic(37966, "Big Hug", new MechanicPlotlySetting("circle","rgb(0,128,0)"), "Big Green","Big Green (friends mechanic)", "Big Green",0), 
            new PlayerBoonApplyMechanic(38247, "Small Hug", new MechanicPlotlySetting("circle-open","rgb(0,128,0)"), "Small Green","Small Green (friends mechanic)", "Small Green",0),
            new SkillOnPlayerMechanic(38180, "Spear Return", new MechanicPlotlySetting("triangle-left","rgb(255,0,0)"), "Spear Return","Hit by Spear Return", "Spear Return",0),
            new SkillOnPlayerMechanic(38260, "Inevitable Betrayal", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Green Fail","Inevitable Betrayal (failed Green)", "Failed Green",0),
            new SkillOnPlayerMechanic(37851, "Inevitable Betrayal", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Green Fail","Inevitable Betrayal (failed Green)", "Failed Green",0),
            new SkillOnPlayerMechanic(37901, "Effigy Pulse", new MechanicPlotlySetting("triangle-down-open","rgb(255,0,0)"), "Spear Pulse","Effigy Pulse (Stood in Spear AoE)", "Spear Aoe",0),
            new SkillOnPlayerMechanic(37816, "Spear Impact", new MechanicPlotlySetting("triangle-down","rgb(255,0,0)"), "Spear Spawn","Spear Impact (hit by spawning Spear)", "Spear Spawned",0),
            new PlayerBoonApplyMechanic(38199, "Brutalize", new MechanicPlotlySetting("diamond-tall","rgb(255,0,255)"),"Brutalize","Brutalize (jumped upon by Samarog->Breakbar)", "Brutalize",0),
            new EnemyCastMechanic(38136, "Brutalize (Jump End)", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"),"CC","Brutalize (Breakbar)", "Breakbar",0),
            new SkillOnPlayerMechanic(38013, "Brutalize", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC Fail","Brutalize (Failed CC)", "CC Fail",0, new List<MechanicChecker>{ new CombatItemResultChecker(ParseEnum.PhysicalResult.KillingBlow) }, Mechanic.TriggerRule.AND),
            new EnemyCastMechanic(38013, "Brutalize", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CC End","Ended Brutalize", "CC Ended",0),
            //new PlayerBoonRemoveMechanic(38199, "Brutalize", ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CCed","Ended Brutalize (Breakbar broken)", "CCEnded",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.Manual)),
            //new Mechanic(38199, "Brutalize", Mechanic.MechType.EnemyBoonStrip, ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting("diamond-tall","rgb(110,160,0)"), "CCed1","Ended Brutalize (Breakbar broken)", "CCed1",0),//(condition => condition.getCombatItem().IsBuffRemove == ParseEnum.BuffRemove.All)),
            new PlayerBoonApplyMechanic(37892, "Soul Swarm", new MechanicPlotlySetting("x-thin-open","rgb(0,255,255)"),"Wall","Soul Swarm (stood in or beyond Spear Wall)", "Spear Wall",0),
            new SkillOnPlayerMechanic(38231, "Impaling Stab", new MechanicPlotlySetting("hourglass","rgb(0,0,255)"),"ShockWv Ctr","Impaling Stab (hit by Spears causing Shockwave)", "Shockwave Center",0),
            new SkillOnPlayerMechanic(38314, "Anguished Bolt", new MechanicPlotlySetting("circle","rgb(255,140,0)"),"Stun","Anguished Bolt (AoE Stun Circle by Guldhem)", "Guldhem's Stun",0),
            
            //  new Mechanic(37816, "Brutalize", ParseEnum.BossIDS.Samarog, new MechanicPlotlySetting("star-square","rgb(255,0,0)"), "CC Target", casted without dmg odd
            });
            Extension = "sam";
            IconUrl = "https://wiki.guildwars2.com/images/f/f0/Mini_Samarog.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/o2DHN29.png",
                            (1221, 1171),
                            (-6526, 1218, -2423, 5146),
                            (-27648, -9216, 27648, 12288),
                            (11774, 4480, 14078, 5376));
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
        

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            // TODO: facing information (shock wave)
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Samarog:
                    List<AbstractBuffEvent> brutalize = GetFilteredList(log.CombatData, 38226, target, true);
                    int brutStart = 0;
                    foreach (AbstractBuffEvent c in brutalize)
                    {
                        if (c is BuffApplyEvent)
                        {
                            brutStart = (int)c.Time;
                        }
                        else
                        {
                            int brutEnd = (int)c.Time;
                            replay.Actors.Add(new CircleActor(true, 0, 120, (brutStart, brutEnd), "rgba(0, 180, 255, 0.3)", new AgentConnector(target)));
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

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            // big bomb
            List<AbstractBuffEvent> bigbomb = log.CombatData.GetBoonData(37966).Where(x => (x.To == p.AgentItem && x is BuffApplyEvent)).ToList();
            foreach (AbstractBuffEvent c in bigbomb)
            {
                int bigStart = (int)c.Time;
                int bigEnd = bigStart + 6000;
                replay.Actors.Add(new CircleActor(true, 0, 300, (bigStart, bigEnd), "rgba(150, 80, 0, 0.2)", new AgentConnector(p)));
                replay.Actors.Add(new CircleActor(true, bigEnd, 300, (bigStart, bigEnd), "rgba(150, 80, 0, 0.2)", new AgentConnector(p)));
            }
            // small bomb
            List<AbstractBuffEvent> smallbomb = log.CombatData.GetBoonData(38247).Where(x => (x.To == p.AgentItem && x is BuffApplyEvent)).ToList();
            foreach (AbstractBuffEvent c in smallbomb)
            {
                int smallStart = (int)c.Time;
                int smallEnd = smallStart + 6000;
                replay.Actors.Add(new CircleActor(true, 0, 80, (smallStart, smallEnd), "rgba(80, 150, 0, 0.3)", new AgentConnector(p)));
            }
            // fixated
            List<AbstractBuffEvent> fixatedSam = GetFilteredList(log.CombatData, 37868, p, true);
            int fixatedSamStart = 0;
            foreach (AbstractBuffEvent c in fixatedSam)
            {
                if (c is BuffApplyEvent)
                {
                    fixatedSamStart = Math.Max((int)c.Time, 0);
                }
                else
                {
                    int fixatedSamEnd = (int)c.Time;
                    replay.Actors.Add(new CircleActor(true, 0, 80, (fixatedSamStart, fixatedSamEnd), "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
            //fixated Ghuldem
            List<AbstractBuffEvent> fixatedGuldhem = GetFilteredList(log.CombatData, 38223, p, true);
            int fixationGuldhemStart = 0;
            Target guldhem = null;
            foreach (AbstractBuffEvent c in fixatedGuldhem)
            {
                if (c is BuffApplyEvent)
                {
                    fixationGuldhemStart = (int)c.Time;
                    long logTime = log.FightData.ToLogSpace(c.Time);
                    guldhem = Targets.FirstOrDefault(x => x.ID == (ushort)Guldhem && logTime >= x.FirstAwareLogTime && logTime <= x.LastAwareLogTime);
                }
                else
                {
                    int fixationGuldhemEnd = (int)c.Time;
                    if (guldhem != null)
                    {
                        replay.Actors.Add(new LineActor(0, (fixationGuldhemStart, fixationGuldhemEnd), "rgba(255, 100, 0, 0.3)", new AgentConnector(p), new AgentConnector(guldhem)));
                    }
                }
            }
            //fixated Rigom
            List<AbstractBuffEvent> fixatedRigom = GetFilteredList(log.CombatData, 37693, p, true);
            int fixationRigomStart = 0;
            Target rigom = null;
            foreach (AbstractBuffEvent c in fixatedRigom)
            {
                if (c is BuffApplyEvent)
                {
                    fixationRigomStart = (int)c.Time;
                    long logTime = log.FightData.ToLogSpace(c.Time);
                    rigom = Targets.FirstOrDefault(x => x.ID == (ushort)Rigom && logTime >= x.FirstAwareLogTime && logTime <= x.LastAwareLogTime);
                }
                else
                {
                    int fixationRigomEnd = (int)c.Time;
                    if (rigom != null)
                    {
                        replay.Actors.Add(new LineActor(0, (fixationRigomStart, fixationRigomEnd), "rgba(255, 0, 0, 0.3)", new AgentConnector(p), new AgentConnector(rigom)));
                    }
                }
            }
        }

        public override int IsCM(ParsedEvtcContainer evtcContainer)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Samarog);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            OverrideMaxHealths(evtcContainer);
            return (target.Health > 30e6) ? 1 : 0;
        }
    }
}
