using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser.Models.Logic
{
    public class WvWFight : FightLogic
    {
        public WvWFight(ushort triggerID, AgentData agentData) : base(triggerID, agentData)
        {
            Extension = "wvw";
            Mode = ParseMode.WvW;
            IconUrl = "https://wiki.guildwars2.com/images/3/35/WvW_Rank_up.png";
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>();
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == (ushort)ParseEnum.TargetIDS.WorldVersusWorld);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            /*phases.Add(new PhaseData(phases[0].Start + 1, phases[0].End)
            {
                Name = "Detailed Full Fight"
            });
            foreach (Target tar in Targets)
            {
                if (tar != mainTarget)
                {
                    phases[1].Targets.Add(tar);
                }
            }*/
            return phases;
        }

        public override void CheckSuccess(ParsedEvtcContainer evtcContainer)
        {
            evtcContainer.FightData.SetSuccess(true, evtcContainer.FightData.FightEnd);
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            AgentItem dummyAgent = agentData.AddCustomAgent(combatData.First().Time, combatData.Last().Time, AgentItem.AgentType.NPC, "WorldVsWorld", "", TriggerID);
            Targets.Add(new Target(dummyAgent));
            Targets[0].Health = 1;
        }
    }
}
