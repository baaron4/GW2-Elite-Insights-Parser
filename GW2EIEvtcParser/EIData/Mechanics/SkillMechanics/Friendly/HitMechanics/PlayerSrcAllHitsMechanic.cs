using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class PlayerSrcAllHitsMechanic : PlayerSrcHitMechanic
    {

        public PlayerSrcAllHitsMechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(0, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
        }

        protected override bool SkillInIDs(AbstractHealthDamageEvent c)
        {
            return true;
        }
    }
}
