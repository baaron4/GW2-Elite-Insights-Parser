using System.Collections.Generic;
using System.IO;
using GW2EIEvtcParser.EIData.Buffs;
using GW2EIEvtcParser.EIData.BuffSimulators;
using GW2EIEvtcParser.EIData.BuffSourceFinders;
using GW2EIEvtcParser.Interfaces;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.ParserHelper;
using static GW2EIEvtcParser.SkillIDs;

namespace GW2EIEvtcParser.EIData
{
    public class Buff : IVersionable
    {

        // Boon
        public enum BuffClassification
        {
            Condition,
            Boon,
            Offensive,
            Defensive,
            Support,
            Debuff,
            Gear,
            Other,
            Enhancement,
            Nourishment,
            OtherConsumable,
            Hidden,
            Unknown
        };

        public enum BuffType
        {
            Duration = 0,
            Intensity = 1,
            Unknown = 2,
        }

        // Fields
        public string Name { get; }
        public long ID { get; }
        public BuffClassification Classification { get; }
        public ParserHelper.Source Source { get; }
        public BuffStackType StackType { get; }
        public BuffType Type
        {
            get
            {
                switch (StackType)
                {
                    case BuffStackType.Queue:
                    case BuffStackType.Regeneration:
                    case BuffStackType.Force:
                        return BuffType.Duration;
                    case BuffStackType.Stacking:
                    case BuffStackType.StackingTargetUniqueSrc:
                    case BuffStackType.StackingConditionalLoss:
                        return BuffType.Intensity;
                    default:
                        return BuffType.Unknown;
                }
            }
        }

        private ulong _maxBuild { get; set; } = GW2Builds.EndOfLife;
        private ulong _minBuild { get; set; } = GW2Builds.StartOfLife;
        public int Capacity { get; }
        public string Link { get; }

        /// <summary>
        /// Buff constructor
        /// </summary>
        /// <param name="name">The name of the boon</param>
        /// <param name="id">The id of the buff</param>
        /// <param name="source">Source of the buff <see cref="ParserHelper.Source"/></param>
        /// <param name="type">Stack Type of the buff<see cref="BuffStackType"/></param>
        /// <param name="capacity">Maximun amount of buff in stack</param>
        /// <param name="nature">Nature of the buff, dictates in which category the buff will appear <see cref="BuffClassification"/></param>
        /// <param name="link">URL to the icon of the buff</param>
        internal Buff(string name, long id, ParserHelper.Source source, BuffStackType type, int capacity, BuffClassification nature, string link)
        {
            Name = name;
            ID = id;
            Source = source;
            StackType = type;
            Capacity = capacity;
            Classification = nature;
            Link = link;
        }

        internal Buff(string name, long id, ParserHelper.Source source, BuffClassification nature, string link) : this(name, id, source, BuffStackType.Force, 1, nature, link)
        {
        }

        internal Buff WithBuilds(ulong minBuild, ulong maxBuild = GW2Builds.EndOfLife)
        {
            _minBuild = minBuild;
            _maxBuild = maxBuild;
            return this;
        }

        public Buff(string name, long id, string link)
        {
            Name = name;
            ID = id;
            Source = Source.Unknown;
            StackType = BuffStackType.Unknown;
            Capacity = 1;
            Classification = BuffClassification.Unknown;
            Link = link;
        }

        internal static Buff CreateCustomBuff(string name, long id, string link, int capacity, BuffClassification classification)
        {
            return new Buff(name + " " + id, id, Source.Item, capacity > 1 ? BuffStackType.Stacking : BuffStackType.Force, capacity, classification, link);
        }

        internal void VerifyBuffInfoEvent(BuffInfoEvent buffInfoEvent, ParserController operation)
        {
            if (buffInfoEvent.BuffID != ID)
            {
                return;
            }
            if (Capacity != buffInfoEvent.MaxStacks)
            {
                operation.UpdateProgressWithCancellationCheck("Adjusted capacity for " + Name + " from " + Capacity + " to " + buffInfoEvent.MaxStacks);
            }
            if (buffInfoEvent.StackingType != StackType && buffInfoEvent.StackingType != BuffStackType.Unknown)
            {
                operation.UpdateProgressWithCancellationCheck("Incoherent stack type for " + Name + ": is " + StackType + " but expected " + buffInfoEvent.StackingType);
            }
        }

        internal AbstractBuffSimulator CreateSimulator(ParsedEvtcLog log, bool forceNoId)
        {
            BuffInfoEvent buffInfoEvent = log.CombatData.GetBuffInfoEvent(ID);
            int capacity = Capacity;
            if (buffInfoEvent != null && buffInfoEvent.MaxStacks != capacity)
            {
                capacity = buffInfoEvent.MaxStacks;
            }
            if (!log.CombatData.UseBuffInstanceSimulator || forceNoId)
            {

                switch (Type)
                {
                    case BuffType.Intensity:
                        return new BuffSimulatorIntensity(log, this, capacity);
                    case BuffType.Duration:
                        return new BuffSimulatorDuration(log, this, capacity);
                    case BuffType.Unknown:
                    default:
                        throw new InvalidDataException("Buffs can not be stackless");
                }
            }
            switch (Type)
            {
                case BuffType.Intensity:
                    return new BuffSimulatorIDIntensity(log, this, capacity);
                case BuffType.Duration:
                    return new BuffSimulatorIDDuration(log, this);
                case BuffType.Unknown:
                default:
                    throw new InvalidDataException("Buffs can not be stackless");
            }
        }

        internal static BuffSourceFinder GetBuffSourceFinder(CombatData combatData, HashSet<long> boonIds)
        {
            ulong gw2Build = combatData.GetBuildEvent().Build;
            if (gw2Build >= GW2Builds.October2022BalanceHotFix)
            {
                return new BuffSourceFinder20221018(boonIds);
            }
            if (gw2Build >= GW2Builds.EODBeta2)
            {
                return new BuffSourceFinder20210921(boonIds);
            }
            if (gw2Build >= GW2Builds.May2021Balance)
            {
                return new BuffSourceFinder20210511(boonIds);
            }
            if (gw2Build >= GW2Builds.October2019Balance)
            {
                return new BuffSourceFinder20191001(boonIds);
            }
            if (gw2Build >= GW2Builds.March2019Balance)
            {
                return new BuffSourceFinder20190305(boonIds);
            }
            return new BuffSourceFinder20181211(boonIds);
        }

        public bool Available(CombatData combatData)
        {
            ulong gw2Build = combatData.GetBuildEvent().Build;
            return gw2Build < _maxBuild && gw2Build >= _minBuild;
        }
    }
}
