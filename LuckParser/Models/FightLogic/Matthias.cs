using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class Matthias : RaidLogic
    {
        public Matthias(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new SkillOnPlayerMechanic(34380, "Oppressive Gaze", new MechanicPlotlySetting("hexagram","rgb(255,0,0)"), "Hadouken","Oppressive Gaze (Hadouken projectile)", "Hadouken",0),//human
            new SkillOnPlayerMechanic(34371, "Oppressive Gaze", new MechanicPlotlySetting("hexagram","rgb(255,0,0)"), "Hadouken","Oppressive Gaze (Hadouken projectile)", "Hadouken",0),//abom
            new SkillOnPlayerMechanic(34480, "Blood Shards", new MechanicPlotlySetting("diamond-wide-open","rgb(255,0,255)"), "Shoot Shards","Blood Shard projectiles during bubble", "Rapid Fire",0),// //human
            new SkillOnPlayerMechanic(34440, "Blood Shards", new MechanicPlotlySetting("diamond-wide-open","rgb(255,0,255)"), "Shoot Shards","Blood Shard projectiles during bubble", "Rapid Fire", 0),// //abom
            new SkillOnPlayerMechanic(34404, "Shards of Rage", new MechanicPlotlySetting("star-diamond","rgb(255,0,0)"), "Jump Shards","Shards of Rage (Jump)", "Jump Shards",1000),//human
            new SkillOnPlayerMechanic(34411, "Shards of Rage", new MechanicPlotlySetting("star-diamond","rgb(255,0,0)"), "Jump Shards","Shards of Rage (Jump)", "Jump Shards",1000),//abom
            new SkillOnPlayerMechanic(34466, "Fiery Vortex", new MechanicPlotlySetting("triangle-down-open","rgb(255,200,0)"), "Tornado","Fiery Vortex (Tornado)", "Tornado",250),
            new SkillOnPlayerMechanic(34543, "Thunder", new MechanicPlotlySetting("triangle-up-open","rgb(0,255,255)"), "Storm","Thunder Storm hit (air phase)", "Storm cloud",0),
            new PlayerBoonApplyMechanic(34450, "Unstable Blood Magic", new MechanicPlotlySetting("diamond","rgb(255,0,0)"), "Well","Unstable Blood Magic application", "Well",0),
            new SkillOnPlayerMechanic(34336, "Well of the Profane", new MechanicPlotlySetting("diamond-open","rgb(255,0,0)"), "Well dmg","Unstable Blood Magic AoE hit", "Stood in Well",0),
            new PlayerBoonApplyMechanic(34416, "Corruption", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Corruption","Corruption Application", "Corruption",0),
            new SkillOnPlayerMechanic(34473, "Corruption", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Corr. dmg","Hit by Corruption AoE", "Corruption dmg",0),
            new PlayerBoonApplyMechanic(34442, "Sacrifice", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "Sacrifice","Sacrifice (Breakbar)", "Sacrifice",0),
            new PlayerBoonRemoveMechanic(34442, "Sacrifice", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CC.End","Sacrifice (Breakbar) ended", "Sacrifice End",0, new List<BoonRemoveMechanic.BoonRemoveChecker>{(br,log) => br.RemovedDuration > 25}, Mechanic.TriggerRule.AND),
            new PlayerBoonRemoveMechanic(34442, "Sacrificed", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC.Fail","Sacrifice time ran out", "Sacrificed",0, new List<BoonRemoveMechanic.BoonRemoveChecker>{ (br,log) => br.RemovedDuration <= 25 }, Mechanic.TriggerRule.AND),
            new PlayerBoonRemoveMechanic(34367, "Unbalanced", new MechanicPlotlySetting("square","rgb(200,140,255)"), "KD","Unbalanced (triggered Storm phase Debuff)", "Knockdown",0, new List<BoonRemoveMechanic.BoonRemoveChecker>{ (br,log) => br.RemovedDuration > 0, (br, log) => 
            {
                AbstractActor actor = log.FindActor(br.To);
                Dictionary<long,BoonsGraphModel> bgms = actor.GetBoonGraphs(log);
                // stability check
                if (bgms.TryGetValue(1122, out var bgm))
                {
                    return !bgm.IsPresent(br.Time, 10);
                }
                else
                {
                    return true;
                }
            } }, Mechanic.TriggerRule.AND),
            //new Mechanic(34367, "Unbalanced", Mechanic.MechType.PlayerOnPlayer, ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting("square","rgb(0,140,0)"), "KD","Unbalanced (triggered Storm phase Debuff) only on successful interrupt", "Knockdown (interrupt)",0,(condition => condition.getCombatItem().Result == ParseEnum.Result.Interrupt)),
            //new Mechanic(34367, "Unbalanced", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting("square","rgb(0,140,0)"), "KD","Unbalanced (triggered Storm phase Debuff) only on successful interrupt", "Knockdown (interrupt)",0,(condition => condition.getDLog().GetResult() == ParseEnum.Result.Interrupt)),
            //new Mechanic(34422, "Blood Fueled", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting("square","rgb(255,0,0)"), "Ate Reflects(good)",0),//human //Applied at the same time as Backflip Shards since it is the buff applied by them, can be omitted imho
            //new Mechanic(34428, "Blood Fueled", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting("square","rgb(255,0,0)"), "Ate Reflects(good)",0),//abom
            new EnemyBoonApplyMechanic(34376, "Blood Shield", new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Bubble","Blood Shield (protective bubble)", "Bubble",0),//human
            new EnemyBoonApplyMechanic(34518, "Blood Shield", new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Bubble","Blood Shield (protective bubble)", "Bubble",0),//abom
            new PlayerBoonApplyMechanic(34511, "Zealous Benediction", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Bombs","Zealous Benediction (Expanding bombs)","Bomb",0),
            new PlayerBoonApplyMechanic(26766, "Icy Patch", new MechanicPlotlySetting("circle-open","rgb(0,0,255)"), "Icy KD","Knockdown by Icy Patch", "Icy Patch KD",0, new List<BoonApplyMechanic.BoonApplyChecker>{ (br,log) => br.AppliedDuration == 10000 }, Mechanic.TriggerRule.AND),
            new SkillOnPlayerMechanic(34413, "Surrender", new MechanicPlotlySetting("circle-open","rgb(0,0,0)"), "Spirit","Surrender (hit by walking Spirit)", "Spirit hit",0)
            });
            Extension = "matt";
            IconUrl = "https://wiki.guildwars2.com/images/5/5d/Mini_Matthias_Abomination.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/3X0YveK.png",
                            (880, 880),
                            (-7248, 4585, -4625, 7207),
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210));
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Matthias);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Special buff cast check
            AbstractBuffEvent heatWave = log.CombatData.GetBoonData(34526).FirstOrDefault();
            if (heatWave != null)
            {
                phases.Add(new PhaseData(0, heatWave.Time - 1));
                AbstractDamageEvent downPour = log.CombatData.GetDamageData(mainTarget.AgentItem).Find(x => x.SkillId == 34554);
                if (downPour != null)
                {
                    phases.Add(new PhaseData(heatWave.Time, downPour.Time - 1));
                    List<AbstractCastEvent> castLogs = mainTarget.GetCastLogs(log, 0, log.FightData.FightDuration);
                    AbstractCastEvent abo = castLogs.Find(x => x.SkillId == 34427);
                    if (abo != null)
                    {
                        phases.Add(new PhaseData(downPour.Time, abo.Time - 1));
                        AbstractBuffEvent invulRemove = log.CombatData.GetBoonDataByDst(mainTarget.AgentItem).FirstOrDefault(x => x.Time >= abo.Time && x.Time <= abo.Time + 10000 && x.BuffID == 757 && !(x is BuffApplyEvent));
                        if (invulRemove != null)
                        {
                            phases.Add(new PhaseData(invulRemove.Time, fightDuration));
                        }
                    }
                    else
                    {
                        phases.Add(new PhaseData(downPour.Time, fightDuration));
                    }
                }
                else
                {
                    phases.Add(new PhaseData(heatWave.Time, fightDuration));
                }
            }
            else
            {
                phases.Add(new PhaseData(0, fightDuration));
            }
            string[] namesMat = new [] { "Ice Phase", "Fire Phase", "Storm Phase", "Abomination Phase" };
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = namesMat[i - 1];
                phases[i].DrawStart = i > 1;
                phases[i].Targets.Add(mainTarget);
            }
            return phases;
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>
            {
                Storm,
                Spirit,
                Spirit2,
                IcePatch,
                Tornado
            };
        }

        public override void ComputeMobCombatReplayActors(Mob mob, ParsedLog log, CombatReplay replay)
        {
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch(mob.ID)
            {
                case (ushort)Storm:
                    replay.Actors.Add(new CircleActor(false, 0, 260, (start, end), "rgba(0, 80, 255, 0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)Spirit:
                case (ushort)Spirit2:
                    replay.Actors.Add(new CircleActor(true, 0, 180, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)IcePatch:
                    replay.Actors.Add(new CircleActor(true, 0, 200, (start, end), "rgba(0, 0, 255, 0.5)", new AgentConnector(mob)));
                    break;
                case (ushort)Tornado:
                    replay.Actors.Add(new CircleActor(true, 0, 90, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(mob)));
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
        }

        public override void ComputeTargetCombatReplayActors(Target target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.TargetIDS.Matthias:
                    List<AbstractCastEvent> humanShield = cls.Where(x => x.SkillId == 34468).ToList();
                    List<int> humanShieldRemoval = log.CombatData.GetBoonData(34518).Where(x => x is BuffRemoveAllEvent).Select(x => (int)x.Time).Distinct().ToList();
                    for (var i = 0; i < humanShield.Count; i++)
                    {
                        var shield = humanShield[i];
                        if (i < humanShieldRemoval.Count)
                        {
                            int removal = humanShieldRemoval[i];
                            replay.Actors.Add(new CircleActor(true, 0, 250, ((int)shield.Time, removal), "rgba(255, 0, 255, 0.5)", new AgentConnector(target)));
                        }
                        else
                        {
                            replay.Actors.Add(new CircleActor(true, 0, 250, ((int)shield.Time, (int)log.FightData.FightDuration), "rgba(255, 0, 255, 0.5)", new AgentConnector(target)));
                        }
                    }
                    List<AbstractCastEvent> aboShield = cls.Where(x => x.SkillId == 34510).ToList();
                    List<int> aboShieldRemoval = log.CombatData.GetBoonData(34376).Where(x => x is BuffRemoveAllEvent).Select(x => (int)x.Time).Distinct().ToList();
                    for (var i = 0; i < aboShield.Count; i++)
                    {
                        var shield = aboShield[i];
                        if (i < aboShieldRemoval.Count)
                        {
                            int removal = aboShieldRemoval[i];
                            replay.Actors.Add(new CircleActor(true, 0, 250, ((int)shield.Time, removal), "rgba(255, 0, 255, 0.5)", new AgentConnector(target)));
                        }
                        else
                        {
                            replay.Actors.Add(new CircleActor(true, 0, 250, ((int)shield.Time, (int)log.FightData.FightDuration), "rgba(255, 0, 255, 0.5)", new AgentConnector(target)));
                        }
                    }
                    List<AbstractCastEvent> rageShards = cls.Where(x => x.SkillId == 34404 || x.SkillId == 34411).ToList();
                    foreach (AbstractCastEvent c in rageShards)
                    {
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        replay.Actors.Add(new CircleActor(false, 0, 300, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        replay.Actors.Add(new CircleActor(true, end, 300, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    }
                    List<AbstractCastEvent> hadouken = cls.Where(x => x.SkillId == 34371 || x.SkillId == 34380).ToList();
                    foreach (AbstractCastEvent c in hadouken)
                    {
                        int start = (int)c.Time;
                        int preCastTime = 1000;
                        int duration = 750;
                        int width = 4000; int height = 130;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start+1000);
                        if (facing != null)
                        {
                            int direction = (int)(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI);
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, width, height, direction, width / 2, (start, start + preCastTime), "rgba(255, 0, 0, 0.1)", new AgentConnector(target)));
                            replay.Actors.Add(new RotatedRectangleActor(true, 0, width, height, direction, width / 2, (start + preCastTime, start + preCastTime + duration), "rgba(255, 0, 0, 0.7)", new AgentConnector(target)));
                        }
                    }
                        break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");
            }
            
        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            // Corruption
            List<AbstractBuffEvent> corruptedMatthias = GetFilteredList(log.CombatData, 34416, p, true);
            corruptedMatthias.AddRange(GetFilteredList(log.CombatData, 34473, p, true));
            int corruptedMatthiasStart = 0;
            foreach (AbstractBuffEvent c in corruptedMatthias)
            {
                if (c is BuffApplyEvent)
                {
                    corruptedMatthiasStart = (int)c.Time;
                }
                else
                {
                    int corruptedMatthiasEnd = (int)c.Time;
                    replay.Actors.Add(new CircleActor(true, 0, 180, (corruptedMatthiasStart, corruptedMatthiasEnd), "rgba(255, 150, 0, 0.5)", new AgentConnector(p)));
                    Point3D wellNextPosition = replay.Positions.FirstOrDefault(x => x.Time >= corruptedMatthiasEnd);
                    Point3D wellPrevPosition = replay.Positions.LastOrDefault(x => x.Time <= corruptedMatthiasEnd);
                    if (wellNextPosition != null || wellPrevPosition != null)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 180, (corruptedMatthiasEnd, corruptedMatthiasEnd + 100000), "rgba(0, 0, 0, 0.3)", new InterpolatedPositionConnector(wellPrevPosition, wellNextPosition, corruptedMatthiasEnd)));
                        replay.Actors.Add(new CircleActor(true, corruptedMatthiasEnd + 100000, 180, (corruptedMatthiasEnd, corruptedMatthiasEnd + 100000), "rgba(0, 0, 0, 0.3)", new InterpolatedPositionConnector(wellPrevPosition, wellNextPosition, corruptedMatthiasEnd)));
                    }
                }
            }
            // Well of profane
            List<AbstractBuffEvent> wellMatthias = GetFilteredList(log.CombatData, 34450, p, true);
            int wellMatthiasStart = 0;
            foreach (AbstractBuffEvent c in wellMatthias)
            {
                if (c is BuffApplyEvent)
                {
                    wellMatthiasStart = (int)c.Time;
                }
                else
                {
                    int wellMatthiasEnd = (int)c.Time;
                    replay.Actors.Add(new CircleActor(false, 0, 120, (wellMatthiasStart, wellMatthiasEnd), "rgba(150, 255, 80, 0.5)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, wellMatthiasStart + 9000, 120, (wellMatthiasStart, wellMatthiasEnd), "rgba(150, 255, 80, 0.5)", new AgentConnector(p)));
                    Point3D wellNextPosition = replay.Positions.FirstOrDefault(x => x.Time >= wellMatthiasEnd);
                    Point3D wellPrevPosition = replay.Positions.LastOrDefault(x => x.Time <= wellMatthiasEnd);
                    if (wellNextPosition != null || wellPrevPosition != null)
                    {
                        replay.Actors.Add(new CircleActor(true, 0, 300, (wellMatthiasEnd, wellMatthiasEnd + 90000), "rgba(255, 0, 50, 0.5)", new InterpolatedPositionConnector(wellPrevPosition, wellNextPosition, wellMatthiasEnd)));
                    }
                }
            }
            // Sacrifice
            List<AbstractBuffEvent> sacrificeMatthias = GetFilteredList(log.CombatData, 34442, p, true);
            int sacrificeMatthiasStart = 0;
            foreach (AbstractBuffEvent c in sacrificeMatthias)
            {
                if (c is BuffApplyEvent)
                {
                    sacrificeMatthiasStart = (int)c.Time;
                }
                else
                {
                    int sacrificeMatthiasEnd = (int)c.Time;
                    replay.Actors.Add(new CircleActor(true, 0, 120, (sacrificeMatthiasStart, sacrificeMatthiasEnd), "rgba(0, 150, 250, 0.2)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleActor(true, sacrificeMatthiasStart + 10000, 120, (sacrificeMatthiasStart, sacrificeMatthiasEnd), "rgba(0, 150, 250, 0.35)", new AgentConnector(p)));
                }
            }
            // Bombs
            List<AbstractBuffEvent> zealousBenediction = log.CombatData.GetBoonData(34511).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent c in zealousBenediction)
            {
                int zealousStart = (int)c.Time ;
                int zealousEnd = zealousStart + 5000;
                replay.Actors.Add(new CircleActor(true, 0, 180, (zealousStart, zealousEnd), "rgba(200, 150, 0, 0.2)", new AgentConnector(p)));
                replay.Actors.Add(new CircleActor(true, zealousEnd, 180, (zealousStart, zealousEnd), "rgba(200, 150, 0, 0.4)", new AgentConnector(p)));
            }
        }
       
    }
}
