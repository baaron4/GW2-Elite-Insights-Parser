using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class EnemyDstHitMechanic : EnemyDstSkillMechanic
    {
        protected override bool Keep(AbstractHealthDamageEvent c, ParsedEvtcLog log)
        {
            return c.HasHit && base.Keep(c, log);
        }

        public EnemyDstHitMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public EnemyDstHitMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}
