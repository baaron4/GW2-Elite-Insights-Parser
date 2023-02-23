using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData
{

    internal abstract class SkillMechanic : IDBasedMechanic
    {
        public delegate bool SkillChecker(AbstractHealthDamageEvent d, ParsedEvtcLog log);

        private readonly SkillChecker _triggerCondition = null;

        protected virtual bool Keep(AbstractHealthDamageEvent c, ParsedEvtcLog log)
        {
            return SkillInIDs(c) && (_triggerCondition == null || _triggerCondition(c, log));
        }

        protected virtual bool SkillInIDs(AbstractHealthDamageEvent c)
        {
            return MechanicIDs.Contains(c.SkillId);
        }

        public SkillMechanic(long mechanicID, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicID, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }

        public SkillMechanic(long[] mechanicIDs, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, SkillChecker condition = null) : base(mechanicIDs, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }

        protected abstract IReadOnlyList<AbstractHealthDamageEvent> GetDamageEvents(ParsedEvtcLog log, AbstractSingleActor actor);

        protected void PlayerChecker(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs)
        {
            foreach (Player p in log.PlayerList)
            {
                IReadOnlyList<AbstractHealthDamageEvent> damageEvents = p.GetDamageTakenEvents(null, log, log.FightData.FightStart, log.FightData.FightEnd);
                foreach (AbstractHealthDamageEvent c in damageEvents)
                {
                    if (Keep(c, log))
                    {
                        mechanicLogs[this].Add(new MechanicEvent(c.Time, this, p));
                    }
                }
            }
        }

        protected void EnemyChecker(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs)
        {
            foreach (AbstractSingleActor actor in log.FightData.Logic.Hostiles)
            {
                foreach (AbstractHealthDamageEvent c in GetDamageEvents(log, actor))
                {
                    AbstractSingleActor amp = null;
                    if (Keep(c, log))
                    {
                        amp = EnemyMechanicHelper.FindActor(log, actor.AgentItem, regroupedMobs);
                    }
                    if (amp != null)
                    {
                        mechanicLogs[this].Add(new MechanicEvent(c.Time, this, amp));
                    }
                }
            }
            
        }

    }
}
