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

        private readonly BoonApplyChecker _triggerCondition = null;

        protected bool Keep(BuffApplyEvent c, ParsedLog log)
        {
            if (_triggerCondition != null)
            {
                return _triggerCondition(c, log);
            }
            return true;
        }

        public BoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, BoonApplyChecker condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        public BoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, BoonApplyChecker condition) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerCondition = condition;
        }

        public BoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public BoonApplyMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}