using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{

    public abstract class SkillMechanic : Mechanic
    {
        public delegate bool SkillChecker(AbstractDamageEvent d, ParsedLog log);

        private readonly List<SkillChecker> _triggerConditions = new List<SkillChecker>();

        protected virtual bool Keep(AbstractDamageEvent c, ParsedLog log)
        {
            if (_triggerConditions.Count > 0)
            {
                foreach (SkillChecker checker in _triggerConditions)
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

        public SkillMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<SkillChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions, rule)
        {
        }

        public SkillMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<SkillChecker> conditions, TriggerRule rule) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, rule)
        {
            _triggerConditions = conditions;
        }

        public SkillMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public SkillMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}