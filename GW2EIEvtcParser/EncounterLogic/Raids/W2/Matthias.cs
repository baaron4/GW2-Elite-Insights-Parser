using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal class Matthias : RaidLogic
    {
        public Matthias(int triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {

            new HitOnPlayerMechanic(34380, "Oppressive Gaze", new MechanicPlotlySetting("hexagram","rgb(255,0,0)"), "Hadouken","Oppressive Gaze (Hadouken projectile)", "Hadouken",0),//human
            new HitOnPlayerMechanic(34371, "Oppressive Gaze", new MechanicPlotlySetting("hexagram","rgb(255,0,0)"), "Hadouken","Oppressive Gaze (Hadouken projectile)", "Hadouken",0),//abom
            new HitOnPlayerMechanic(34480, "Blood Shards", new MechanicPlotlySetting("diamond-wide-open","rgb(255,0,255)"), "Shoot Shards","Blood Shard projectiles during bubble", "Rapid Fire",0),// //human
            new HitOnPlayerMechanic(34440, "Blood Shards", new MechanicPlotlySetting("diamond-wide-open","rgb(255,0,255)"), "Shoot Shards","Blood Shard projectiles during bubble", "Rapid Fire", 0),// //abom
            new HitOnPlayerMechanic(34404, "Shards of Rage", new MechanicPlotlySetting("star-diamond","rgb(255,0,0)"), "Jump Shards","Shards of Rage (Jump)", "Jump Shards",1000),//human
            new HitOnPlayerMechanic(34411, "Shards of Rage", new MechanicPlotlySetting("star-diamond","rgb(255,0,0)"), "Jump Shards","Shards of Rage (Jump)", "Jump Shards",1000),//abom
            new HitOnPlayerMechanic(34466, "Fiery Vortex", new MechanicPlotlySetting("triangle-down-open","rgb(255,200,0)"), "Tornado","Fiery Vortex (Tornado)", "Tornado",250),
            new HitOnPlayerMechanic(34543, "Thunder", new MechanicPlotlySetting("triangle-up-open","rgb(0,255,255)"), "Storm","Thunder Storm hit (air phase)", "Storm cloud",0),
            new PlayerBuffApplyMechanic(34450, "Unstable Blood Magic", new MechanicPlotlySetting("diamond","rgb(255,0,0)"), "Well","Unstable Blood Magic application", "Well",0),
            new HitOnPlayerMechanic(34336, "Well of the Profane", new MechanicPlotlySetting("diamond-open","rgb(255,0,0)"), "Well dmg","Unstable Blood Magic AoE hit", "Stood in Well",0),
            new PlayerBuffApplyMechanic(34416, "Corruption", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Corruption","Corruption Application", "Corruption",0),
            new HitOnPlayerMechanic(34473, "Corruption", new MechanicPlotlySetting("circle-open","rgb(255,140,0)"), "Corr. dmg","Hit by Corruption AoE", "Corruption dmg",0),
            new PlayerBuffApplyMechanic(34442, "Sacrifice", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "Sacrifice","Sacrifice (Breakbar)", "Sacrifice",0),
            new PlayerBuffRemoveMechanic(34442, "Sacrifice", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "CC.End","Sacrifice (Breakbar) ended", "Sacrifice End",0, (br,log) => br.RemovedDuration > 25 && !log.CombatData.GetDeadEvents(br.To).Any(x => Math.Abs(br.Time - x.Time) < ParserHelper.ServerDelayConstant)),
            new PlayerBuffRemoveMechanic(34442, "Sacrificed", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "CC.Fail","Sacrifice time ran out", "Sacrificed",0, (br,log) => br.RemovedDuration <= 25 || log.CombatData.GetDeadEvents(br.To).Any(x => Math.Abs(br.Time - x.Time) < ParserHelper.ServerDelayConstant)),
            new PlayerBuffRemoveMechanic(34367, "Unbalanced", new MechanicPlotlySetting("square","rgb(200,140,255)"), "KD","Unbalanced (triggered Storm phase Debuff)", "Knockdown",0, (br,log) => br.RemovedDuration > 0 && !br.To.HasBuff(log, 1122, br.Time)),
            //new Mechanic(34367, "Unbalanced", Mechanic.MechType.PlayerOnPlayer, ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting("square","rgb(0,140,0)"), "KD","Unbalanced (triggered Storm phase Debuff) only on successful interrupt", "Knockdown (interrupt)",0,(condition => condition.getCombatItem().Result == ParseEnum.Result.Interrupt)),
            //new Mechanic(34367, "Unbalanced", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting("square","rgb(0,140,0)"), "KD","Unbalanced (triggered Storm phase Debuff) only on successful interrupt", "Knockdown (interrupt)",0,(condition => condition.getDLog().GetResult() == ParseEnum.Result.Interrupt)),
            //new Mechanic(34422, "Blood Fueled", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting("square","rgb(255,0,0)"), "Ate Reflects(good)",0),//human //Applied at the same time as Backflip Shards since it is the buff applied by them, can be omitted imho
            //new Mechanic(34428, "Blood Fueled", ParseEnum.BossIDS.Matthias, new MechanicPlotlySetting("square","rgb(255,0,0)"), "Ate Reflects(good)",0),//abom
            new EnemyBuffApplyMechanic(34376, "Blood Shield", new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Bubble","Blood Shield (protective bubble)", "Bubble",100),//human
            new EnemyBuffApplyMechanic(34518, "Blood Shield", new MechanicPlotlySetting("octagon","rgb(255,0,0)"), "Bubble","Blood Shield (protective bubble)", "Bubble",100),//abom
            new PlayerBuffApplyMechanic(34511, "Zealous Benediction", new MechanicPlotlySetting("circle","rgb(255,200,0)"), "Bombs","Zealous Benediction (Expanding bombs)","Bomb",0),
            new PlayerBuffApplyMechanic(26766, "Icy Patch", new MechanicPlotlySetting("circle-open","rgb(0,0,255)"), "Icy KD","Knockdown by Icy Patch", "Icy Patch KD",0, (br,log) => br.AppliedDuration == 10000 && !br.To.HasBuff(log, 1122, br.Time)),
            new HitOnPlayerMechanic(34413, "Surrender", new MechanicPlotlySetting("circle-open","rgb(0,0,0)"), "Spirit","Surrender (hit by walking Spirit)", "Spirit hit",0)
            });
            Extension = "matt";
            Icon = "https://wiki.guildwars2.com/images/5/5d/Mini_Matthias_Abomination.png";
        }

        protected override CombatReplayMap GetCombatMapInternal(ParsedEvtcLog log)
        {
            return new CombatReplayMap("https://i.imgur.com/3X0YveK.png",
                            (880, 880),
                            (-7248, 4585, -4625, 7207),
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210));
        }

        internal override List<PhaseData> GetPhases(ParsedEvtcLog log, bool requirePhases)
        {
            long fightDuration = log.FightData.FightEnd;
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = Targets.Find(x => x.ID == (int)ArcDPSEnums.TargetID.Matthias);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Matthias not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Special buff cast check
            AbstractBuffEvent heatWave = log.CombatData.GetBuffData(34526).FirstOrDefault();
            if (heatWave != null)
            {
                phases.Add(new PhaseData(0, heatWave.Time - 1));
                AbstractDamageEvent downPour = log.CombatData.GetDamageData(mainTarget.AgentItem).Find(x => x.SkillId == 34554);
                if (downPour != null)
                {
                    phases.Add(new PhaseData(heatWave.Time, downPour.Time - 1));
                    List<AbstractCastEvent> castLogs = mainTarget.GetCastLogs(log, 0, log.FightData.FightEnd);
                    AbstractCastEvent abo = castLogs.Find(x => x.SkillId == 34427);
                    if (abo != null)
                    {
                        phases.Add(new PhaseData(downPour.Time, abo.Time - 1));
                        AbstractBuffEvent invulRemove = log.CombatData.GetBuffData(mainTarget.AgentItem).FirstOrDefault(x => x.Time >= abo.Time && x.Time <= abo.Time + 10000 && x.BuffID == 757 && !(x is BuffApplyEvent));
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
            string[] namesMat = new[] { "Ice Phase", "Fire Phase", "Storm Phase", "Abomination Phase" };
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = namesMat[i - 1];
                phases[i].DrawStart = i > 1;
                phases[i].Targets.Add(mainTarget);
            }
            return phases;
        }

        internal override void EIEvtcParse(FightData fightData, AgentData agentData, List<CombatItem> combatData, List<Player> playerList)
        {
            // has breakbar state into
            if (combatData.Any(x => x.IsStateChange == ArcDPSEnums.StateChange.BreakbarState))
            {
                long sacrificeID = 34442;
                var sacrificeList = combatData.Where(x => x.SkillID == sacrificeID && ((x.IsBuffRemove == ArcDPSEnums.BuffRemove.All && x.IsBuff != 0) || (x.IsBuff != 0 && x.BuffDmg == 0 && x.Value > 0 && x.IsStateChange == ArcDPSEnums.StateChange.None && x.IsActivation == ArcDPSEnums.Activation.None && x.IsBuffRemove == ArcDPSEnums.BuffRemove.None))).ToList();
                var sacrificeStartList = sacrificeList.Where(x => x.IsBuffRemove == ArcDPSEnums.BuffRemove.None).ToList();
                var sacrificeEndList = sacrificeList.Where(x => x.IsBuffRemove == ArcDPSEnums.BuffRemove.All).ToList();
                var copies = new List<CombatItem>();
                for (int i = 0; i < sacrificeStartList.Count; i++)
                {
                    //
                    long sacrificeStartTime = sacrificeStartList[i].Time;
                    long sacrificeEndTime = i < sacrificeEndList.Count ? sacrificeEndList[i].Time : fightData.FightEnd;
                    //
                    Player sacrifice = playerList.FirstOrDefault(x => x.AgentItem == agentData.GetAgent(sacrificeStartList[i].DstAgent));
                    if (sacrifice == null)
                    {
                        continue;
                    }
                    AgentItem sacrificeCrystal = agentData.AddCustomAgent(sacrificeStartTime, sacrificeEndTime + 100, AgentItem.AgentType.NPC, "Sacrificed " + (i + 1) + " " + sacrifice.Character, sacrifice.Prof, (int)ArcDPSEnums.TrashID.MatthiasSacrificeCrystal);
                    foreach (CombatItem cbt in combatData)
                    {
                        if (!sacrificeCrystal.InAwareTimes(cbt.Time))
                        {
                            continue;
                        }
                        bool skip = !((cbt.IsStateChange.DstIsAgent() && cbt.DstAgent == sacrifice.Agent) || (cbt.IsStateChange.SrcIsAgent() && cbt.SrcAgent == sacrifice.Agent));
                        if (skip)
                        {
                            continue;
                        }
                        bool isDamageEvent = cbt.IsStateChange == ArcDPSEnums.StateChange.None && cbt.IsActivation == ArcDPSEnums.Activation.None && cbt.IsBuffRemove == ArcDPSEnums.BuffRemove.None && ((cbt.IsBuff != 0 && cbt.Value == 0) || (cbt.IsBuff == 0));
                        // redirect damage events
                        if (isDamageEvent)
                        {
                            // only redirect incoming damage
                            if (cbt.DstAgent == sacrifice.Agent)
                            {
                                cbt.OverrideDstAgent(sacrificeCrystal.Agent);
                            }
                        }
                        // copy the rest
                        else
                        {
                            var copy = new CombatItem(cbt);
                            if (cbt.IsStateChange.DstIsAgent() && cbt.DstAgent == sacrifice.Agent)
                            {
                                cbt.OverrideDstAgent(sacrificeCrystal.Agent);
                            }
                            if (cbt.IsStateChange.SrcIsAgent() && cbt.SrcAgent == sacrifice.Agent)
                            {
                                cbt.OverrideSrcAgent(sacrificeCrystal.Agent);
                            }
                            copies.Add(copy);
                        }
                    }
                }
                if (copies.Any())
                {
                    combatData.AddRange(copies);
                    combatData.Sort((x, y) => x.Time.CompareTo(y.Time));
                }
            }       
            ComputeFightTargets(agentData, combatData);
            Targets.ForEach(x =>
            {
                if (x.ID == (int)ArcDPSEnums.TrashID.MatthiasSacrificeCrystal)
                {
                    x.SetManualHealth(100000);
                }
            });
        }

        protected override List<int> GetFightTargetsIDs()
        {
            return new List<int>
            {
                (int)ArcDPSEnums.TargetID.Matthias,
                (int)ArcDPSEnums.TrashID.MatthiasSacrificeCrystal
            };
        }

        protected override List<ArcDPSEnums.TrashID> GetTrashMobsIDS()
        {
            return new List<ArcDPSEnums.TrashID>
            {
                ArcDPSEnums.TrashID.Storm,
                ArcDPSEnums.TrashID.Spirit,
                ArcDPSEnums.TrashID.Spirit2,
                ArcDPSEnums.TrashID.IcePatch,
                ArcDPSEnums.TrashID.Tornado
            };
        }

        internal override void ComputeNPCCombatReplayActors(NPC target, ParsedEvtcLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightEnd);
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (target.ID)
            {
                case (int)ArcDPSEnums.TargetID.Matthias:
                    var humanShield = cls.Where(x => x.SkillId == 34468).ToList();
                    var humanShieldRemoval = log.CombatData.GetBuffRemoveAllData(34518).Select(x => (int)x.Time).Distinct().ToList();
                    for (int i = 0; i < humanShield.Count; i++)
                    {
                        AbstractCastEvent shield = humanShield[i];
                        if (i < humanShieldRemoval.Count)
                        {
                            int removal = humanShieldRemoval[i];
                            replay.Decorations.Add(new CircleDecoration(true, 0, 250, ((int)shield.Time, removal), "rgba(255, 0, 255, 0.5)", new AgentConnector(target)));
                        }
                        else
                        {
                            replay.Decorations.Add(new CircleDecoration(true, 0, 250, ((int)shield.Time, (int)log.FightData.FightEnd), "rgba(255, 0, 255, 0.5)", new AgentConnector(target)));
                        }
                    }
                    var aboShield = cls.Where(x => x.SkillId == 34510).ToList();
                    var aboShieldRemoval = log.CombatData.GetBuffRemoveAllData(34376).Select(x => (int)x.Time).Distinct().ToList();
                    for (int i = 0; i < aboShield.Count; i++)
                    {
                        AbstractCastEvent shield = aboShield[i];
                        if (i < aboShieldRemoval.Count)
                        {
                            int removal = aboShieldRemoval[i];
                            replay.Decorations.Add(new CircleDecoration(true, 0, 250, ((int)shield.Time, removal), "rgba(255, 0, 255, 0.5)", new AgentConnector(target)));
                        }
                        else
                        {
                            replay.Decorations.Add(new CircleDecoration(true, 0, 250, ((int)shield.Time, (int)log.FightData.FightEnd), "rgba(255, 0, 255, 0.5)", new AgentConnector(target)));
                        }
                    }
                    var rageShards = cls.Where(x => x.SkillId == 34404 || x.SkillId == 34411).ToList();
                    foreach (AbstractCastEvent c in rageShards)
                    {
                        start = (int)c.Time;
                        end = (int)c.EndTime;
                        replay.Decorations.Add(new CircleDecoration(false, 0, 300, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                        replay.Decorations.Add(new CircleDecoration(true, end, 300, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    }
                    var hadouken = cls.Where(x => x.SkillId == 34371 || x.SkillId == 34380).ToList();
                    foreach (AbstractCastEvent c in hadouken)
                    {
                        start = (int)c.Time;
                        int preCastTime = 1000;
                        int duration = 750;
                        int width = 4000; int height = 130;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start + 1000);
                        if (facing != null)
                        {
                            int direction = (int)(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI);
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, direction, width / 2, (start, start + preCastTime), "rgba(255, 0, 0, 0.1)", new AgentConnector(target)));
                            replay.Decorations.Add(new RotatedRectangleDecoration(true, 0, width, height, direction, width / 2, (start + preCastTime, start + preCastTime + duration), "rgba(255, 0, 0, 0.7)", new AgentConnector(target)));
                        }
                    }
                    break;
                case (int)ArcDPSEnums.TrashID.Storm:
                    replay.Decorations.Add(new CircleDecoration(false, 0, 260, (start, end), "rgba(0, 80, 255, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.Spirit:
                case (int)ArcDPSEnums.TrashID.Spirit2:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.IcePatch:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 200, (start, end), "rgba(0, 0, 255, 0.5)", new AgentConnector(target)));
                    break;
                case (int)ArcDPSEnums.TrashID.Tornado:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 90, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(target)));
                    break;
                default:
                    break;
            }

        }

        internal override void ComputePlayerCombatReplayActors(Player p, ParsedEvtcLog log, CombatReplay replay)
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
                    replay.Decorations.Add(new CircleDecoration(true, 0, 180, (corruptedMatthiasStart, corruptedMatthiasEnd), "rgba(255, 150, 0, 0.5)", new AgentConnector(p)));
                    Point3D wellNextPosition = replay.PolledPositions.FirstOrDefault(x => x.Time >= corruptedMatthiasEnd);
                    Point3D wellPrevPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= corruptedMatthiasEnd);
                    if (wellNextPosition != null || wellPrevPosition != null)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 180, (corruptedMatthiasEnd, corruptedMatthiasEnd + 100000), "rgba(0, 0, 0, 0.3)", new InterpolatedPositionConnector(wellPrevPosition, wellNextPosition, corruptedMatthiasEnd)));
                        replay.Decorations.Add(new CircleDecoration(true, corruptedMatthiasEnd + 100000, 180, (corruptedMatthiasEnd, corruptedMatthiasEnd + 100000), "rgba(0, 0, 0, 0.3)", new InterpolatedPositionConnector(wellPrevPosition, wellNextPosition, corruptedMatthiasEnd)));
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
                    replay.Decorations.Add(new CircleDecoration(false, 0, 120, (wellMatthiasStart, wellMatthiasEnd), "rgba(150, 255, 80, 0.5)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, wellMatthiasStart + 9000, 120, (wellMatthiasStart, wellMatthiasEnd), "rgba(150, 255, 80, 0.5)", new AgentConnector(p)));
                    Point3D wellNextPosition = replay.PolledPositions.FirstOrDefault(x => x.Time >= wellMatthiasEnd);
                    Point3D wellPrevPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= wellMatthiasEnd);
                    if (wellNextPosition != null || wellPrevPosition != null)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, 300, (wellMatthiasEnd, wellMatthiasEnd + 90000), "rgba(255, 0, 50, 0.5)", new InterpolatedPositionConnector(wellPrevPosition, wellNextPosition, wellMatthiasEnd)));
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
                    replay.Decorations.Add(new CircleDecoration(true, 0, 120, (sacrificeMatthiasStart, sacrificeMatthiasEnd), "rgba(0, 150, 250, 0.2)", new AgentConnector(p)));
                    replay.Decorations.Add(new CircleDecoration(true, sacrificeMatthiasStart + 10000, 120, (sacrificeMatthiasStart, sacrificeMatthiasEnd), "rgba(0, 150, 250, 0.35)", new AgentConnector(p)));
                }
            }
            // Bombs
            var zealousBenediction = log.CombatData.GetBuffData(34511).Where(x => x.To == p.AgentItem && x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent c in zealousBenediction)
            {
                int zealousStart = (int)c.Time;
                int zealousEnd = zealousStart + 5000;
                replay.Decorations.Add(new CircleDecoration(true, 0, 180, (zealousStart, zealousEnd), "rgba(200, 150, 0, 0.2)", new AgentConnector(p)));
                replay.Decorations.Add(new CircleDecoration(true, zealousEnd, 180, (zealousStart, zealousEnd), "rgba(200, 150, 0, 0.4)", new AgentConnector(p)));
            }
        }

    }
}
