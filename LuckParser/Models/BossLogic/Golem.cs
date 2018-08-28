using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public class Golem : BossLogic
    {
        public Golem()
        {
            Mode = ParseMode.Golem;          
        }

        public override List<PhaseData> GetPhases(Boss boss, ParsedLog log, List<CastLog> castLogs)
        {
            List<PhaseData> phases = GetInitialPhase(log);          
            return phases;
        }

        public override void SetSuccess(CombatData combatData, LogData logData, BossData bossData)
        {
            CombatItem pov = combatData.FirstOrDefault(x => x.IsStateChange == ParseEnum.StateChange.PointOfView);
            if (pov != null)
            {
                // to make sure that the logging starts when the PoV starts attacking (in case there is a slave with them)
                CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcAgent == pov.SrcAgent && x.IsStateChange == ParseEnum.StateChange.EnterCombat);
                if (enterCombat != null)
                {
                    bossData.SetLastAware(enterCombat.Time);
                }
            }
            CombatItem combatExit = combatData.LastOrDefault(x => x.IsStateChange == ParseEnum.StateChange.ExitCombat);
            if (combatExit != null)
            {
                bossData.SetLastAware(combatExit.Time);
            }
            if (bossData.GetHealthOverTime().Count > 0)
            {
                logData.SetBossKill(bossData.GetHealthOverTime().Last().Y < 200);
            }
        }
    }
}
