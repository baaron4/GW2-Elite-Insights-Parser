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

        public override List<PhaseData> getPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            // generic method for fractals
            long start = 0;
            long end = 0;
            long fight_dur = log.getBossData().getAwareDuration();
            List<PhaseData> phases = getInitialPhase(log);
            List<CombatItem> invulsBoss = log.getBoonData().Where(x => x.getSkillID() == 762 && boss.getInstid() == x.getDstInstid()).ToList();
            List<CombatItem> invulsBossFiltered = new List<CombatItem>();
            foreach (CombatItem c in invulsBoss)
            {
                if (invulsBossFiltered.Count > 0)
                {
                    CombatItem last = invulsBossFiltered.Last();
                    if (last.getTime() != c.getTime())
                    {
                        invulsBossFiltered.Add(c);
                    }
                }
                else
                {
                    invulsBossFiltered.Add(c);
                }
            }
            for (int i = 0; i < invulsBossFiltered.Count; i++)
            {
                CombatItem c = invulsBossFiltered[i];
                if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    end = c.getTime() - log.getBossData().getFirstAware();
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsBossFiltered.Count - 1)
                    {
                        cast_logs.Add(new CastLog(end, -5, (int)(fight_dur - end), ParseEnum.Activation.None, (int)(fight_dur - end), ParseEnum.Activation.None));
                    }
                }
                else
                {
                    start = c.getTime() - log.getBossData().getFirstAware();
                    cast_logs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                }
            }
            if (fight_dur - start > 5000 && start >= phases.Last().getEnd())
            {
                phases.Add(new PhaseData(start, fight_dur));
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].setName("Phase " + i);
            }
            return phases;
        }
    }
}
