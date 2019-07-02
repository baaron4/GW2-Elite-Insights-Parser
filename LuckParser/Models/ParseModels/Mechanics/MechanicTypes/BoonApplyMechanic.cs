using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{

    public abstract class BoonApplyMechanic : Mechanic
    {
        public delegate bool BoonApplyChecker(BuffApplyEvent ba, ParsedLog log);

        private readonly List<BoonApplyChecker> _triggerConditions = new List<BoonApplyChecker>();

        protected bool Keep(BuffApplyEvent c, ParsedLog log)
        {
            if (_triggerConditions.Count > 0)
            {
                foreach (BoonApplyChecker checker in _triggerConditions)
                {
                    bool res = checker(c, log);
                    if (!res)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public BoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<BoonApplyChecker> conditions) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions)
        {
        }

        public BoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<BoonApplyChecker> conditions) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerConditions = conditions;
        }

        public BoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public BoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}