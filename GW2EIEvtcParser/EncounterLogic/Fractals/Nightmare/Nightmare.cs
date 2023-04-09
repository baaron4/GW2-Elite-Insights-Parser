
using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.Exceptions;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.SkillIDs;
using static GW2EIEvtcParser.EncounterLogic.EncounterCategory;
using GW2EIEvtcParser.EIData;

namespace GW2EIEvtcParser.EncounterLogic
{
    internal abstract class Nightmare : FractalLogic
    {
        public Nightmare(int triggerID) : base(triggerID)
        {
            EncounterCategoryInformation.SubCategory = SubFightCategory.Nightmare;
            EncounterID |= EncounterIDs.FractalMasks.NightmareMask;
        }

        protected static void AddCascadeOfTormentDecoration(ParsedEvtcLog log, CombatReplay replay, EffectGUIDEvent cascadeOfTormentEffect, int cotDuration, int innerRadius, int outerRadius)
        {
            if (cascadeOfTormentEffect != null)
            {
                var expulsionEffects = log.CombatData.GetEffectEventsByEffectID(cascadeOfTormentEffect.ContentID).ToList();
                foreach (EffectEvent effect in expulsionEffects)
                {
                    int endTime = (int)effect.Time + cotDuration;
                    if (innerRadius == 0)
                    {
                        replay.Decorations.Add(new CircleDecoration(true, 0, outerRadius, ((int)effect.Time, endTime), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                        replay.Decorations.Add(new CircleDecoration(true, 0, outerRadius, (endTime, endTime + 150), "rgba(250, 120, 0, 0.4)", new PositionConnector(effect.Position)));
                    }
                    else
                    {
                        replay.Decorations.Add(new DoughnutDecoration(true, 0, innerRadius, outerRadius, ((int)effect.Time, endTime), "rgba(250, 120, 0, 0.2)", new PositionConnector(effect.Position)));
                        replay.Decorations.Add(new DoughnutDecoration(true, 0, innerRadius, outerRadius, (endTime, endTime + 150), "rgba(250, 120, 0, 0.4)", new PositionConnector(effect.Position)));
                    }
                }
            }
        }

        internal override long GetFightOffset(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            return GetFightOffsetByFirstInvulFilter(fightData, agentData, combatData, GenericTriggerID, Determined762);
        }
    }
}
