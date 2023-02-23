using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerSrcSkillMechanic : PlayerSkillMechanic
    {

        public PlayerSrcSkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        public PlayerSrcSkillMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        protected override IReadOnlyList<AbstractHealthDamageEvent> GetDamageEvents(ParsedEvtcLog log, AbstractSingleActor actor)
        {
            return actor.GetDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd);
        }
    }
}
