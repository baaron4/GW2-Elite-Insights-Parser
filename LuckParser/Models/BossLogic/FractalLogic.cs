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
            mechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37695, "Flux Bomb", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Unknown, "symbol:'circle',color:'rgb(150,0,255)',size:10,", "FBmb",0), // Flux Bomb application, Flux Bomb
            new Mechanic(36393, "Flux Bomb", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Unknown, "symbol:'circle-open',color:'rgb(150,0,255)',size:10,", "FB.dmg",0), // Flux Bomb hit, Flux Bomb dmg
            new Mechanic(19684, "Fractal Vindicator", Mechanic.MechType.Spawn, ParseEnum.BossIDS.Unknown, "symbol:'star-diamond-open',color:'rgb(0,0,0)',size:10,", "FV.spwn",0), // Fractal Vindicator spawned, Vindicator spawn
            });
        }

        public override List<PhaseData> getPhases(Boss boss, ParsedLog log, List<CastLog> cast_logs)
        {
            // generic method for fractals
            long start = 0;
            long end = 0;
            long fight_dur = log.getBossData().getAwareDuration();
            List<PhaseData> phases = getInitialPhase(log);
            List<CombatItem> invulsBoss = getFilteredList(log,762,boss.getInstid());        
            for (int i = 0; i < invulsBoss.Count; i++)
            {
                CombatItem c = invulsBoss[i];
                if (c.isBuffremove() == ParseEnum.BuffRemove.None)
                {
                    end = c.getTime() - log.getBossData().getFirstAware();
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsBoss.Count - 1)
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
