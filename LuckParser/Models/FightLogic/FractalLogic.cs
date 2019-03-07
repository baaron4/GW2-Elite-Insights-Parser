using LuckParser.Parser;
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
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBoonApplyMechanic(37695, "Flux Bomb", new MechanicPlotlySetting("circle","rgb(150,0,255)",10), "Flux","Flux Bomb application", "Flux Bomb",0),
            new SkillOnPlayerMechanic(36393, "Flux Bomb", new MechanicPlotlySetting("circle-open","rgb(150,0,255)",10), "Flux dmg","Flux Bomb hit", "Flux Bomb dmg",0),
            new SpawnMechanic(19684, "Fractal Vindicator", new MechanicPlotlySetting("star-diamond-open","rgb(0,0,0)",10), "Vindicator","Fractal Vindicator spawned", "Vindicator spawn",0),
            });
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            // generic method for fractals
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
            phases.AddRange(GetPhasesByInvul(log, 762, mainTarget, false, true));
            phases.RemoveAll(x => x.DurationInMS < 1000);
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = "Phase " + i;
                phases[i].Targets.Add(mainTarget);
            }
            return phases;
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                TriggerID
            };
        }

        public override void SetSuccess(ParsedLog log)
        {
            // check reward
            Target mainTarget = Targets.Find(x => x.ID == TriggerID);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            HashSet<ushort> pIds = log.PlayerIDs;
            CombatItem reward = log.CombatData.GetStates(ParseEnum.StateChange.Reward).LastOrDefault();
            CombatItem lastDamageTaken = log.CombatData.GetDamageTakenData(mainTarget.InstID, mainTarget.FirstAware, mainTarget.LastAware).LastOrDefault(x => (x.Value > 0 || x.BuffDmg > 0) && pIds.Contains(x.SrcInstid));
            if (lastDamageTaken != null)
            {
                if (reward != null && lastDamageTaken.Time - reward.Time < 100)
                {
                    log.FightData.Success = true;
                    log.FightData.FightEnd = Math.Min(lastDamageTaken.Time, reward.Time);
                }
                else
                {
                    SetSuccessByDeath(log);
                    if (log.FightData.Success)
                    {
                        log.FightData.FightEnd = Math.Min(log.FightData.FightEnd, lastDamageTaken.Time);
                    }
                }
            }
        }

    }
}
