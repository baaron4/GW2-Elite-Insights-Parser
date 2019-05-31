using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{

    public abstract class CastMechanic : Mechanic
    {
        public delegate bool CastChecker(AbstractCastEvent ce, ParsedLog log);

        private readonly List<CastChecker> _triggerConditions = new List<CastChecker>();

        protected bool Keep(AbstractCastEvent c, ParsedLog log)
        {
            if (_triggerConditions.Count > 0)
            {
                foreach (CastChecker checker in _triggerConditions)
                {
                    bool res = checker(c, log);
                    if (Rule == TriggerRule.AND && !res)
                    {
                        return false;
                    }
                    else if (Rule == TriggerRule.OR && res)
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<CastChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions, rule)
        {
        }

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<CastChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, rule)
        {
        }

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}