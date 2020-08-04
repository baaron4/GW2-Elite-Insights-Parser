using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
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

        public long SkillId { get; }

        public int InternalCooldown { get; }
        public MechanicPlotlySetting PlotlySetting { get; }
        public string Description { get; }
        private readonly string _inGameName;
        public string ShortName { get; }
        public string FullName { get; }
        public bool IsEnemyMechanic { get; protected set; }
        public bool ShowOnTable { get; protected set; }


        /// <summary>
        /// Simplified constructor without special checks where only short name is given. FullName and Description are equal to ShortName
        /// </summary>
        /// <param name="skillId">id of the mechanic</param>
        /// <param name="inGameName">official name of the mechanic</param>
        /// <param name="plotlySetting">html plot settings <seealso cref="MechanicPlotlySetting"/></param>
        /// <param name="shortName">name of the mechanic</param>
        /// <param name="internalCoolDown">grace period, in ms, during which getting hit by the mechanic does not count</param>
        protected Mechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, int internalCoolDown) : this(skillId, inGameName, plotlySetting, shortName, shortName, shortName, internalCoolDown)
        {
        }

        /// <summary>
        /// Full constructor without special checks
        /// </summary>
        /// <param name="skillId">id of the mechanic</param>
        /// <param name="inGameName">official name of the mechanic</param>
        /// <param name="plotlySetting">html plot settings <seealso cref="MechanicPlotlySetting"/></param>
        /// <param name="shortName">shortened name of the mechanic</param>
        /// <param name="description">description of the mechanic</param>
        /// <param name="fullName">full name of the mechanic</param>
        /// <param name="internalCoolDown">grace period, in ms, during which getting hit by the mechanic does not count</param>
        protected Mechanic(long skillId, string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown)
        {
            _inGameName = inGameName;
            SkillId = skillId;
            PlotlySetting = plotlySetting;
            ShortName = shortName;
            FullName = fullName;
            Description = description;
            InternalCooldown = internalCoolDown;
            ShowOnTable = true;
        }

        public abstract void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs);

    }
}
