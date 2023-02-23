using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal class EnemySrcSkillMechanic : SkillMechanic
    {

        public EnemySrcSkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
            IsEnemyMechanic = true;
        }

        public EnemySrcSkillMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, condition)
        {
            IsEnemyMechanic = true;
        }

        protected override IReadOnlyList<AbstractHealthDamageEvent> GetDamageEvents(ParsedEvtcLog log, AbstractSingleActor actor)
        {
            return actor.GetDamageEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd);
        }

        internal override void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            EnemyChecker(log, mechanicLogs, regroupedMobs);
        }
    }
}
