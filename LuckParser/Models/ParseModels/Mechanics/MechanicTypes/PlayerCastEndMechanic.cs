using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    
    public class PlayerCastEndMechanic : PlayerCastStartMechanic
    {
        protected override long GetTime(AbstractCastEvent evt)
        {
            return evt.Time + evt.ActualDuration;
        }

        public PlayerCastEndMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<CastChecker> conditions) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions)
        {
        }

        public PlayerCastEndMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown)
        {
        }

        public PlayerCastEndMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        public PlayerCastEndMechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<CastChecker> conditions) : base(skillId, inGameName, plotlySetting, shortName, description, fullName, internalCoolDown, conditions)
        {
        }
    }
}
