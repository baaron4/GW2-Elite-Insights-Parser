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
        public int Size { get; }
        public string Symbol { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol">Symbol to use, see https://plot.ly/javascript/reference/#scatter-marker-symbol </param>
        /// <param name="color">The color of the symbol</param>
        /// <param name="size">Size, in pixel, of the symbol, defaults to 15</param>
        public MechanicPlotlySetting(string symbol, string color, int size = 15)
        {
            Color = color;
            Symbol = symbol;
            Size = size;
        }

    }

    public abstract class Mechanic
    {
        public delegate bool CheckTriggerCondition(CombatItem conditionItem);
        private readonly CheckTriggerCondition _triggerCondition;

        protected bool Keep(CombatItem c)
        {
            if (_triggerCondition == null)
            {
                return true;
            }
            return _triggerCondition(c);
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

        protected Mechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown, CheckTriggerCondition condition) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown, condition)
        {
        }

        protected Mechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown, CheckTriggerCondition condition)
        {
            InGameName = inGameName;
            SkillId = skillId;
            PlotlySetting = plotlySetting;
            ShortName = shortName;
            FullName = fullName;
            Description = description;
            InternalCooldown = internalCoolDown;
            ShowOnTable = true;
            _triggerCondition = condition;
        }

        public abstract void CheckMechanic(ParsedLog log, Dictionary<ushort, DummyActor> regroupedMobs);

    }
}
