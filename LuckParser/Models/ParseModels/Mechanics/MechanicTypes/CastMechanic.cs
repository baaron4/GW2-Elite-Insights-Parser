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

        protected bool EndCast { get; }

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

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, bool end, List<CastChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, end, conditions, rule)
        {
        }

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, bool end, List<CastChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, rule)
        {
            EndCast = end;
            _triggerConditions = conditions;
        }

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, bool end) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, end)
        {
        }

        public CastMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, bool end) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            EndCast = end;
        }
    }
}