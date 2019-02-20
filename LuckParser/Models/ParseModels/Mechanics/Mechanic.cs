using LuckParser.Controllers;
using LuckParser.Parser;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    /// <summary>
    /// Plot description of the mechanic
    /// </summary>
    public class MechanicPlotlySetting
    {
        public string Color { get; }
        public int Size { get; } = 15;
        public string Symbol { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol">Symbol to use, see https://plot.ly/javascript/reference/#scatter-marker-symbol </param>
        /// <param name="color">The color of the symbol</param>
        public MechanicPlotlySetting(string symbol, string color)
        {
            Color = color;
            Symbol = symbol;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol">Symbol to use, see https://plot.ly/javascript/reference/#scatter-marker-symbol </param>
        /// <param name="color">The color of the symbol</param>
        /// <param name="size">Size, in pixel, of the symbol, defaults to 15</param>
        public MechanicPlotlySetting(string symbol, string color, int size)
        {
            Color = color;
            Symbol = symbol;
            Size = size;
        }

    }

    public abstract class Mechanic
    {
        public enum TriggerRule { OR, AND};
        private readonly List<MechanicChecker> _triggerConditions = new List<MechanicChecker>();
        private readonly TriggerRule _triggerRule = TriggerRule.AND;

        protected bool Keep(CombatItem c, ParsedLog log)
        {
            if (_triggerConditions.Count > 0)
            {
                foreach (MechanicChecker checker in _triggerConditions)
                {
                    bool res = checker.Keep(c, log);
                    if (_triggerRule == TriggerRule.AND && !res)
                    {
                        return false;
                    }
                    else if (_triggerRule == TriggerRule.OR && res)
                    {
                        return true;
                    }
                }
            }
            return true;
        }

        public long SkillId { get; }

        public int InternalCooldown { get; }
        public MechanicPlotlySetting PlotlySetting { get; }
        public string Description { get; }
        public string InGameName { get; }
        public string ShortName { get; }
        public string FullName { get; }
        public bool IsEnemyMechanic { get; protected set; }
        public bool ShowOnTable { get; protected set; }

        protected Mechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, conditions, rule)
        {
        }

        protected Mechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, List<MechanicChecker> conditions, TriggerRule rule)
        {
            InGameName = inGameName;
            SkillId = skillId;
            PlotlySetting = plotlySetting;
            ShortName = shortName;
            FullName = fullName;
            Description = description;
            InternalCooldown = internalCoolDown;
            ShowOnTable = true;
            _triggerConditions.AddRange(conditions);
            _triggerRule = rule;
        }

        protected Mechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        protected Mechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown)
        {
            InGameName = inGameName;
            SkillId = skillId;
            PlotlySetting = plotlySetting;
            ShortName = shortName;
            FullName = fullName;
            Description = description;
            InternalCooldown = internalCoolDown;
            ShowOnTable = true;
        }

        public abstract void CheckMechanic(ParsedLog log, Dictionary<ushort, DummyActor> regroupedMobs);

    }
}
