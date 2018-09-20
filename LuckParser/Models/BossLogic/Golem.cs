using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class Golem : BossLogic
    {
        public Golem(ushort id)
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

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);          
            return phases;
        }

        public override void SetSuccess(ParsedLog log)
        {
            CombatItem pov = log.CombatData.FirstOrDefault(x => x.IsStateChange == ParseEnum.StateChange.PointOfView);
            if (pov != null)
            {
                // to make sure that the logging starts when the PoV starts attacking (in case there is a slave with them)
                CombatItem enterCombat = log.CombatData.FirstOrDefault(x => x.SrcAgent == pov.SrcAgent && x.IsStateChange == ParseEnum.StateChange.EnterCombat);
                if (enterCombat != null)
                {
                    log.FightData.FightStart = enterCombat.Time;
                }
            }
            CombatItem lastDamageTaken = log.CombatData.GetDamageTakenData(log.Boss.InstID).LastOrDefault(x => x.Value > 0 || x.BuffDmg > 0);
            if (lastDamageTaken != null)
            {
                log.FightData.FightEnd = lastDamageTaken.Time;
            }
            if (log.Boss.HealthOverTime.Count > 0)
            {
                log.LogData.Success = log.Boss.HealthOverTime.Last().Y < 200;
            }
        }
    }
}
