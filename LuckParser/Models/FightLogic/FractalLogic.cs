using LuckParser.Models.DataModels;
using LuckParser.Models.ParseModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LuckParser.Models.Logic
{
    public abstract class FractalLogic : FightLogic
    {
        protected FractalLogic(ushort triggerID) : base (triggerID)
        { 
            Mode = ParseMode.Fractal;
            CanCombatReplay = true;
            MechanicList.AddRange(new List<Mechanic>
            {
            new Mechanic(37695, "Flux Bomb", Mechanic.MechType.PlayerBoon, new MechanicPlotlySetting("circle","rgb(150,0,255)",10), "FBmb","Flux Bomb application", "Flux Bomb",0),
            new Mechanic(36393, "Flux Bomb", Mechanic.MechType.SkillOnPlayer, new MechanicPlotlySetting("circle-open","rgb(150,0,255)",10), "FB.dmg","Flux Bomb hit", "Flux Bomb dmg",0),
            new Mechanic(19684, "Fractal Vindicator", Mechanic.MechType.Spawn, new MechanicPlotlySetting("star-diamond-open","rgb(0,0,0)",10), "FV.spwn","Fractal Vindicator spawned", "Vindicator spawn",0),
            });
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            // generic method for fractals
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            Target mainTarget = Targets.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            List<CombatItem> invulsTarget = GetFilteredList(log,762,mainTarget);        
            for (int i = 0; i < invulsTarget.Count; i++)
            {
                CombatItem c = invulsTarget[i];
                if (c.IsBuffRemove == ParseEnum.BuffRemove.None)
                {
                    end = log.FightData.ToFightSpace(c.Time);
                    phases.Add(new PhaseData(start, end));
                    if (i == invulsTarget.Count - 1)
                    {
                        mainTarget.AddCustomCastLog(new CastLog(end, -5, (int)(fightDuration - end), ParseEnum.Activation.None, (int)(fightDuration - end), ParseEnum.Activation.None), log);
                    }
                }
                else
                {
                    start = log.FightData.ToFightSpace(c.Time);
                    mainTarget.AddCustomCastLog(new CastLog(end, -5, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None), log);
                }
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            phases.RemoveAll(x => x.GetDuration() < 1000);
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].Targets.Add(mainTarget);
            }
            return phases;
        }

        public override void SetSuccess(ParsedLog log)
        {
            // check reward
            Target mainTarget = Targets.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            CombatItem reward = log.CombatData.GetStatesData(ParseEnum.StateChange.Reward).LastOrDefault();
            CombatItem lastDamageTaken = log.CombatData.GetDamageTakenData(mainTarget.InstID, mainTarget.FirstAware, mainTarget.LastAware).LastOrDefault(x => x.Value > 0);
            if (lastDamageTaken != null)
            {
                if (reward != null && lastDamageTaken.Time - reward.Time < 100)
                {
                    log.FightData.Success = true;
                    log.FightData.FightEnd = reward.Time;
                }
                else
                {
                    SetSuccessByDeath(log);
                }
            }
        }

    }
}
