using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.InstantCastFinder;

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
        internal MechanicPlotlySetting(Symbol symbol, Color color)
        {
            Color = color.ToString(false);
            Symbol = symbol.Str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symbol">Symbol to use, see https://plot.ly/javascript/reference/#scatter-marker-symbol </param>
        /// <param name="color">The color of the symbol</param>
        /// <param name="size">Size, in pixel, of the symbol, defaults to 15</param>
        internal MechanicPlotlySetting(Symbol symbol, Color color, int size)
        {
            Color = color.ToString(false);
            Symbol = symbol.Str;
            Size = size;
        }

    }

    public abstract class Mechanic
    {
        public int InternalCooldown { get; }
        public MechanicPlotlySetting PlotlySetting { get; }
        public string Description { get; }
        private readonly string _inGameName;
        public string ShortName { get; }
        public string FullName { get; }
        public bool IsEnemyMechanic { get; protected set; }
        public bool ShowOnTable { get; private set; }

        public bool IsAchievementEligibility { get; private set; }

        public delegate bool Keeper(ParsedEvtcLog log);
        private List<Keeper> _enableConditions { get; }
        private ulong _maxBuild { get; set; } = GW2Builds.EndOfLife;
        private ulong _minBuild { get; set; } = GW2Builds.StartOfLife;

        /// <summary>
        /// Full constructor without special checks
        /// </summary>
        /// <param name="inGameName">official name of the mechanic</param>
        /// <param name="plotlySetting">html plot settings <seealso cref="MechanicPlotlySetting"/></param>
        /// <param name="shortName">shortened name of the mechanic</param>
        /// <param name="description">description of the mechanic</param>
        /// <param name="fullName">full name of the mechanic</param>
        /// <param name="internalCoolDown">grace period, in ms, during which getting hit by the mechanic does not count</param>
        protected Mechanic(string inGameName, MechanicPlotlySetting plotlySetting, string shortName, string description, string fullName, int internalCoolDown)
        {
            _inGameName = inGameName;
            PlotlySetting = plotlySetting;
            ShortName = shortName;
            FullName = fullName;
            Description = description;
            InternalCooldown = internalCoolDown;
            ShowOnTable = true;
            _enableConditions = new List<Keeper>();
        }

        internal abstract void CheckMechanic(ParsedEvtcLog log, Dictionary<Mechanic, List<MechanicEvent>> mechanicLogs, Dictionary<int, AbstractSingleActor> regroupedMobs);

        internal Mechanic UsingShowOnTable(bool showOnTable)
        {
            ShowOnTable = showOnTable;
            return this;
        }

        private static bool EligibilityKeeper(ParsedEvtcLog log)
        {
            return log.FightData.Success;
        }

        /// <summary>
        /// Eligibility mechanics will only be computed in successful logs
        /// </summary>
        /// <param name="isAchievementEligibility"></param>
        /// <returns></returns>
        internal Mechanic UsingAchievementEligibility(bool isAchievementEligibility)
        {
            IsAchievementEligibility = isAchievementEligibility;
            if (isAchievementEligibility)
            {
                _enableConditions.Add(EligibilityKeeper);
            } 
            else
            {
                _enableConditions.Remove(EligibilityKeeper);
            }
            return this;
        }

        internal Mechanic UsingEnable(Keeper keeper)
        {
            _enableConditions.Add(keeper);
            return this;
        }

        internal Mechanic UsingDisableWithEffectData()
        {
            return UsingEnable(log => !log.CombatData.HasEffectData);
        }

        internal Mechanic WithBuilds(ulong minBuild, ulong maxBuild = GW2Builds.EndOfLife)
        {
            _maxBuild = maxBuild;
            _minBuild = minBuild;
            return this;
        }

        internal bool Available(ParsedEvtcLog log)
        {
            if (!_enableConditions.All(checker => checker(log)))
            {
                return false;
            }
            ulong gw2Build = log.CombatData.GetBuildEvent().Build;
            return gw2Build < _maxBuild && gw2Build >= _minBuild;
        }

        internal bool KeepIfEmpty(ParsedEvtcLog log)
        {
            return IsAchievementEligibility && log.FightData.Success;
        }

    }
}
