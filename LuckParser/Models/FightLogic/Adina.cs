using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;
using System.Drawing;

namespace LuckParser.Models.Logic
{
    public class Adina : RaidLogic
    {
        public Adina(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new PlayerBoonApplyMechanic(56040, "Blinding Radiance", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "R.Blind", "Blindess applied if looking at Adina", "Blinding Radiance", 0),
                new DamageOnPlayerMechanic(56648, " Boulder Barrage", new MechanicPlotlySetting("square","rgb(255,0,0)"), "Boulder", "Hit by boulder thrown during pillars", "Boulder Barrage", 0),
            });
            Extension = "adina";
            IconUrl = "https://wiki.guildwars2.com/images/d/d2/Guild_emblem_004.png";
        }

        public override void ComputeFightTargets(AgentData agentData, CombatData combatData)
        {
            base.ComputeFightTargets(agentData, combatData);
            foreach (AgentItem hands in agentData.GetAgentByType(AgentItem.AgentType.Gadget))
            {
                if (combatData.GetAttackTargetEvents(hands).Count > 0)
                {
                    Targets.Add(new Target(hands));
                }
            }
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Adina);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            List<AbstractCastEvent> quantumQuakes = mainTarget.GetCastLogs(log, 0, log.FightData.FightDuration).Where(x => x.SkillId == 56035).ToList();
            List<AbstractBuffEvent> invuls = GetFilteredList(log.CombatData, 762, mainTarget, true);
            long start = 0, end = 0;
            for (int i = 0; i < invuls.Count; i++)
            {
                AbstractBuffEvent be = invuls[i];
                if (be is BuffApplyEvent)
                {
                    start = be.Time;
                    if (i == invuls.Count - 1)
                    {
                        phases.Add(new PhaseData(start, log.FightData.FightDuration)
                        {
                            Name = "Split " + (i / 2 + 1)
                        });
                    }
                }
                else
                {
                    end = be.Time;
                    phases.Add(new PhaseData(start, end)
                    {
                        Name = "Split " + (i / 2 + 1)
                    });
                }
            }
            List<PhaseData> mainPhases = new List<PhaseData>();
            start = 0;
            end = 0;
            for (int i = 1; i < phases.Count; i++)
            {
                AbstractCastEvent qQ = quantumQuakes[i - 1];
                end = qQ.Time;
                mainPhases.Add(new PhaseData(start, end)
                {
                    Name = "Phase " + i
                });
                PhaseData split = phases[i];
                foreach (Target hand in Targets.Where(x => x.AgentItem != mainTarget.AgentItem))
                {
                    List<AgentItem> ats = log.CombatData.GetAttackTargetEvents(hand.AgentItem).Select(x => x.AttackTarget).ToList();
                    foreach (AgentItem at in ats)
                    {
                        if (log.CombatData.GetTargetableEvents(at).Count(x => split.InInterval(x.Time) && x.Targetable) > 0)
                        {
                            split.Targets.Add(hand);
                        }
                    }
                }
                start = split.End;
                if (i == phases.Count - 1 && start != log.FightData.FightDuration)
                {
                    mainPhases.Add(new PhaseData(start, log.FightData.FightDuration)
                    {
                        Name = "Phase " + (i + 1)
                    });
                }
            }
            foreach(PhaseData phase in mainPhases)
            {
                phase.Targets.Add(mainTarget);
            }
            phases.AddRange(mainPhases);
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            return phases;
        }


        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("",
                            (800, 800),
                            (-21504, -21504, 24576, 24576),
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970));
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            Target target = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.Adina);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) > 23e6) ? 1 : 0;
        }
    }
}
