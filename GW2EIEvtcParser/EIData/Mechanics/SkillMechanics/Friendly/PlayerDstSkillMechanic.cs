using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerDstSkillMechanic : PlayerSkillMechanic
    {

        public PlayerDstSkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public PlayerDstSkillMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }


        protected override IReadOnlyList<AbstractHealthDamageEvent> GetDamageEvents(ParsedEvtcLog log, AbstractSingleActor actor)
        {
            return actor.GetDamageTakenEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd);
        }
    }
}
