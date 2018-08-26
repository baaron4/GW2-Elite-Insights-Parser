using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models
{
    public abstract class FractalLogic : BossLogic
    {
        protected FractalLogic()
        { 
            Mode = ParseMode.Fractal;
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37695, "Flux Bomb", Mechanic.MechType.PlayerBoon, ParseEnum.BossIDS.Unknown, "symbol:'circle',color:'rgb(150,0,255)',size:10,", "FBmb",0), // Flux Bomb application, Flux Bomb
            new Mechanic(36393, "Flux Bomb", Mechanic.MechType.SkillOnPlayer, ParseEnum.BossIDS.Unknown, "symbol:'circle-open',color:'rgb(150,0,255)',size:10,", "FB.dmg",0), // Flux Bomb hit, Flux Bomb dmg
            new Mechanic(19684, "Fractal Vindicator", Mechanic.MechType.Spawn, ParseEnum.BossIDS.Unknown, "symbol:'star-diamond-open',color:'rgb(0,0,0)',size:10,", "FV.spwn",0), // Fractal Vindicator spawned, Vindicator spawn
            });
        }

        public override List<PhaseData> GetPhases(Boss boss, ParsedLog log, List<CastLog> castLogs)
        {
            // generic method for fractals
            long start = 0;
            long end = 0;
            long fightDuration = log.GetBossData().GetAwareDuration();
            List<PhaseData> phases = GetInitialPhase(log);
            List<CombatItem> invulsBoss = GetFilteredList(log,762,boss.GetInstid());        
            for (int i = 0; i < invulsBoss.Count; i++)
            {
                CombatItem c = invulsBoss[i];
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    end = c.Time - log.GetBossData().GetFirstAware();
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsBoss.Count - 1)
                    {
                        castLogs.Add(new CastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None));
                    }
                }
                else
                {
                    start = c.Time - log.GetBossData().GetFirstAware();
                    castLogs.Add(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None));
                }
            }
            if (fightDuration - start > 5000 && start >= phases.Last().GetEnd())
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].SetName("Phase " + i);
            }
            return phases;
        }
    }
}
