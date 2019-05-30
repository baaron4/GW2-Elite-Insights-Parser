using LuckParser.Parser;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.Logic
{
    public class Golem : FightLogic
    {
        public Golem(ushort id, AgentData agentData) : base(id, agentData)
        {
            Mode = ParseMode.Golem;  
            switch (id)
            {
                case 16202:
                    Extension = "MassiveGolem";
                    IconUrl = "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                    break;
                case 16177:
                    Extension = "AvgGolem";
                    IconUrl = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    break;
                case 19676:
                    Extension = "LGolem";
                    IconUrl = "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                    break;
                case 19645:
                    Extension = "MedGolem";
                    IconUrl = "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                    break;
                case 16199:
                    Extension = "StdGolem";
                    IconUrl = "https://wiki.guildwars2.com/images/8/8f/Mini_Professor_Mew.png";
                    break;
            }
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            Target target = Targets.Find(x => x.ID == TriggerID);
            foreach (CombatItem c in combatData)
            {
                // redirect all attacks to the main golem
                if (c.DstAgent == 0 && c.DstInstid == 0 && c.IsStateChange == ParseEnum.StateChange.Normal && c.IFF == ParseEnum.IFF.Foe && c.IsActivation == ParseEnum.Activation.None && c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    c.OverrideDstValues(target.Agent, target.InstID);
                }
            }
            CombatItem pov = combatData.FirstOrDefault(x => x.IsStateChange == ParseEnum.StateChange.PointOfView);
            if (pov != null)
            {
                // to make sure that the logging starts when the PoV starts attacking (in case there is a slave with them)
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcAgent == pov.SrcAgent && x.IsStateChange == ParseEnum.StateChange.EnterCombat);
                if (enterCombat != null)
                {
                    fightData.OverrideStart(enterCombat.LogTime);
                }
            }
        }

        public override void CheckSuccess(ParsedEvtcContainer evtcContainer)
        {
            Target mainTarget = Targets.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            AbstractDamageEvent lastDamageTaken = evtcContainer.CombatData.GetDamageTakenData(mainTarget.AgentItem).LastOrDefault(x => x.Damage > 0);
            long fightEnd = evtcContainer.FightData.FightEnd;
            bool success = false;
            if (lastDamageTaken != null)
            {
                fightEnd = lastDamageTaken.Time;
            }
            if (mainTarget.HealthOverTime.Count > 0)
            {
                success = mainTarget.HealthOverTime.Last().hp < 200;
            }
            evtcContainer.FightData.SetSuccess(success, fightEnd);
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                TriggerID
            };
        }
    }
}
