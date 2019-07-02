using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{

    public abstract class BoonRemoveMechanic : Mechanic
    {
        public delegate bool BoonRemoveChecker(BuffRemoveManualEvent rme, ParsedLog log);

        private readonly List<BoonRemoveChecker> _triggerConditions = new List<BoonRemoveChecker>();

        protected bool Keep(BuffRemoveManualEvent c, ParsedLog log)
        {
            if (_triggerConditions.Count > 0)
            {
                foreach (BoonRemoveChecker checker in _triggerConditions)
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

        public BoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<BoonRemoveChecker> conditions) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions)
        {
        }

        public BoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<BoonRemoveChecker> conditions) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
            _triggerConditions = conditions;
        }

        public BoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public BoonRemoveMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }
    }
}