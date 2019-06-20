using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;
using System.Drawing;

namespace LuckParser.Models.Logic
{
    public class Sabir : RaidLogic
    {
        public Sabir(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new SkillOnPlayerMechanic(56202, "Dire Drafts", new MechanicPlotlySetting("circle","rgb(255,120,0)"), "B.Tornado", "Hit by big tornado", "Big Tornado Hit", 500, new List<SkillMechanic.SkillChecker>{(de, log) => de.HasDowned || de.HasKilled }, Mechanic.TriggerRule.AND),
                new SkillOnPlayerMechanic(56643, "Unbridled Tempest", new MechanicPlotlySetting("circle","rgb(255,0,150)"), "Shockwave", "Hit by Shockwave", "Shockwave Hit", 0, new List<SkillMechanic.SkillChecker>{(de, log) => de.HasDowned || de.HasKilled }, Mechanic.TriggerRule.AND),
                new SkillOnPlayerMechanic(56372, "Fury of the Storm", new MechanicPlotlySetting("circle","rgb(200,150,0)"), "Arena AoE", "Hit by Arena wide AoE", "Arena AoE hit", 0, new List<SkillMechanic.SkillChecker>{(de, log) => de.HasDowned || de.HasKilled }, Mechanic.TriggerRule.AND),
                new SkillOnPlayerMechanic(56403, "Electrical Repulsion", new MechanicPlotlySetting("circle","rgb(255,0,150)"), "Pushed", "Pushed by rotating breakbar", "Pushed", 0, new List<SkillMechanic.SkillChecker>{(de, log) => !de.To.HasBuff(log, 1122, de.Time) }, Mechanic.TriggerRule.AND), // Not 100% sure about this one
                new EnemyCastStartMechanic(56349, "Regenerative Breakbar", new MechanicPlotlySetting("cross","rgb(255,150,0)"), "Reg.Breakbar","Regenerating Breakbar", "Regenerative Breakbar", 0),
                new EnemyBoonRemoveMechanic(56100, "Regenerative Breakbar Broken", new MechanicPlotlySetting("cross-open","rgb(255,150,0)"), "Reg.Breakbar Brkn", "Regenerative Breakbar Broken", "Regenerative Breakbar Broken", 2000),
                new EnemyBoonApplyMechanic(56172, "Rotating Breakbar", new MechanicPlotlySetting("square","rgb(255,150,0)"), "Rot.Breakbar","Rotating Breakbar", "Rotating Breakbar", 0),
                new EnemyBoonRemoveMechanic(56172, "Rotating Breakbar Broken", new MechanicPlotlySetting("square-open","rgb(255,150,0)"), "Rot.Breakbar Brkn","Rotating Breakbar Broken", "Rotating Breakbar Broken", 0),
            });
            // rotating cc 56403
            // interesting stuff 56372 (big AoE?)
            Extension = "sabir";
            IconUrl = "https://wiki.guildwars2.com/images/f/fc/Mini_Air_Djinn.png";
        }

        protected override List<ParseEnum.TrashIDS> GetTrashMobsIDS()
        {
            return new List<ParseEnum.TrashIDS>()
            {
                ParalyzingWisp,
                VoltaicWisp,
                SmallKillerTornado,
                SmallJumpyTornado,
                BigKillerTornado
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Sabir);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            List<AbstractCastEvent> cls = mainTarget.GetCastLogs(log, 0, log.FightData.FightDuration);
            List<AbstractCastEvent> wallopingWinds = cls.Where(x => x.SkillId == 56094).ToList();
            long start = 0, end = 0;
            for (int i = 0; i < wallopingWinds.Count; i++)
            {
                AbstractCastEvent wW = wallopingWinds[i];
                end = wW.Time;
                PhaseData phase = new PhaseData(start, end)
                {
                    Name = "Phase " + (i + 1)
                };
                phase.Targets.Add(mainTarget);
                phases.Add(phase);
                AbstractCastEvent nextAttack = cls.FirstOrDefault(x => x.Time >= end + wW.ActualDuration && (x.SkillId == 56620 || x.SkillId == 56629 || x.SkillId == 56307));
                if (nextAttack == null)
                {
                    break;
                }
                start = nextAttack.Time;
                if (i == wallopingWinds.Count - 1)
                {
                    phase = new PhaseData(start, log.FightData.FightDuration)
                    {
                        Name = "Phase " + (i + 2)
                    };
                    phase.Targets.Add(mainTarget);
                    phases.Add(phase);
                }
            }

            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/zs9yPuG.png",
                            (4365, 3972),
                            (-14122, 142, - 9199, 4640),
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970));
        }

        public override void ComputeMobCombatReplayActors(Mob mob, ParsedLog log, CombatReplay replay)
        {
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (mob.ID)
            {
                case (ushort)BigKillerTornado:
                    replay.Actors.Add(new CircleActor(true, 0, 420, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(mob)));
                    break;
                case (ushort)SmallKillerTornado:
                    replay.Actors.Add(new CircleActor(true, 0, 120, (start, end), "rgba(255, 150, 0, 0.4)", new AgentConnector(mob)));
                    break;
                case (ushort)SmallJumpyTornado:
                case (ushort)ParalyzingWisp:
                case (ushort)VoltaicWisp:
                    break;
                default:
                    throw new InvalidOperationException("Unknown ID in ComputeAdditionalData");

            }
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Sabir);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) > 32e6) ? 1 : 0;
        }
    }
}
