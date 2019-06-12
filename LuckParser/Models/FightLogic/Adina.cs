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
        public Adina(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            });
            Extension = "adina";
            IconUrl = "";
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
            // fail check
            int quantumToInvulCounts = 2 * quantumQuakes.Count;
            if (quantumToInvulCounts != invuls.Count && quantumToInvulCounts != invuls.Count + 1)
            {
                return phases;
            }
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
            return null;
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return 0;
        }
    }
}
