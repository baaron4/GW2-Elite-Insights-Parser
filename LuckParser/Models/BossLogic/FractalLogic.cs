using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models
{
    public abstract class FractalLogic : BossLogic
    {
        public FractalLogic() : base()
        { 
            mode = ParseMode.Fractal;
        }

        public override List<PhaseData> GetPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            // generic method for fractals
            long start = 0;
            long end = 0;
            long fight_dur = log.GetBossData().GetAwareDuration();
            List<PhaseData> phases = GetInitialPhase(log);
            List<CombatItem> invulsBoss = GetFilteredList(log,762,boss.GetInstid());        
            for (int i = 0; i < invulsBoss.Count; i++)
            {
                CombatItem c = invulsBoss[i];
                if (c.IsBuffremove() == ParseEnum.BuffRemove.None)
                {
                    end = c.GetTime() - log.GetBossData().GetFirstAware();
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsBoss.Count - 1)
                    {
                        cast_logs.Add(new CastLog(end, -5, (int)(fight_dur - end), ParseEnum.Activation.None, (int)(fight_dur - end), ParseEnum.Activation.None));
                    }
                }
                else
                {
                    start = c.GetTime() - log.GetBossData().GetFirstAware();
                    cast_logs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                }
            }
            if (fight_dur - start > 5000 && start >= phases.Last().GetEnd())
            {
                phases.Add(new PhaseData(start, fight_dur));
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].SetName("Phase " + i);
            }
            return phases;
        }
    }
}
