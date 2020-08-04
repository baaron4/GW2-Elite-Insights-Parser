using System.Collections.Generic;
using System.IO;
using GW2EIEvtcParser.ParsedData;
using GW2EIUtils;
using static GW2EIUtils.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData
{
    public class Buff
    {
        // Boon
        public enum BuffNature { 
            Condition, 
            Boon, 
            OffensiveBuffTable, 
            DefensiveBuffTable, 
            GraphOnlyBuff, 
            Consumable, 
            Unknow 
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
        public BuffNature Nature { get; }
        public ParseHelper.Source Source { get; }
        private BuffStackType _stackType { get; }
        public BuffType Type {    
            get {
                switch (_stackType)
                {
                    case BuffStackType.Queue:
                    case BuffStackType.Regeneration:
                    case BuffStackType.Force:
                        return BuffType.Duration;
                    case BuffStackType.Stacking:
                    case BuffStackType.StackingConditionalLoss:
                        return BuffType.Intensity;
                    default:
                        return BuffType.Unknown;
                }
            }
        }

        public BuffInfoEvent BuffInfo { get; private set; }

        public ulong MaxBuild { get; } = ulong.MaxValue;
        public ulong MinBuild { get; } = ulong.MinValue;
        public int Capacity { get; private set; }
        public string Link { get; }

        /// <summary>
        /// Buff constructor
        /// </summary>
        /// <param name="name">The name of the boon</param>
        /// <param name="id">The id of the buff</param>
        /// <param name="source">Source of the buff <see cref="ParseHelper.Source"/></param>
        /// <param name="type">Stack Type of the buff<see cref="BuffStackType"/></param>
        /// <param name="capacity">Maximun amount of buff in stack</param>
        /// <param name="nature">Nature of the buff, dictates in which category the buff will appear <see cref="BuffNature"/></param>
        /// <param name="link">URL to the icon of the buff</param>
        public Buff(string name, long id, ParseHelper.Source source, BuffStackType type, int capacity, BuffNature nature, string link)
        {
            Name = name;
            ID = id;
            Source = source;
            _stackType = type;
            Capacity = capacity;
            Nature = nature;
            Link = link;
        }

        public Buff(string name, long id, ParseHelper.Source source, BuffNature nature, string link) : this(name, id, source, BuffStackType.Force, 1, nature, link)
        {
        }

        public Buff(string name, long id, ParseHelper.Source source, BuffStackType type, int capacity, BuffNature nature, string link, ulong minBuild, ulong maxBuild) : this(name, id, source, type, capacity, nature, link)
        {
            MaxBuild = maxBuild;
            MinBuild = minBuild;
        }

        public Buff(string name, long id, ParseHelper.Source source, BuffNature nature, string link, ulong minBuild, ulong maxBuild) : this(name, id, source, BuffStackType.Force, 1, nature, link, minBuild, maxBuild)
        {
        }

        public Buff(string name, long id, string link)
        {
            Name = name;
            ID = id;
            Source = ParseHelper.Source.Unknown;
            _stackType = BuffStackType.Unknown;
            Capacity = 1;
            Nature = BuffNature.Unknow;
            Link = link;
        }

        public static Buff CreateCustomConsumable(string name, long id, string link, int capacity)
        {
            return new Buff(name + " " + id, id, ParseHelper.Source.Item, capacity > 1 ? BuffStackType.Stacking : BuffStackType.Force, capacity, BuffNature.Consumable, link);
        }

        public void AttachBuffInfoEvent(BuffInfoEvent buffInfoEvent, OperationTracer operation)
        {
            if (buffInfoEvent.BuffID != ID)
            {
                return;
            }
            BuffInfo = buffInfoEvent;
            if (Capacity != buffInfoEvent.MaxStacks)
            {
                operation.UpdateProgressWithCancellationCheck("Adjusted capacity for " + Name + " from " + Capacity + " to " + buffInfoEvent.MaxStacks);
                if (buffInfoEvent.StackingType != _stackType)
                {
                    //_stackType = buffInfoEvent.StackingType; // might be unreliable due to its absence on some logs
                    operation.UpdateProgressWithCancellationCheck("Incoherent stack type for " + Name + ": is " + _stackType + " but expected " + buffInfoEvent.StackingType);
                }
                Capacity = buffInfoEvent.MaxStacks;
            }
        }
        public AbstractBuffSimulator CreateSimulator(ParsedEvtcLog log)
        {
            if (!log.CombatData.HasStackIDs)
            {
                StackingLogic logicToUse;
                switch (_stackType)
                {
                    case BuffStackType.Queue:
                        logicToUse = new QueueLogic();
                        break;
                    case BuffStackType.Regeneration:
                        logicToUse = new HealingLogic();
                        break;
                    case BuffStackType.Force:
                        logicToUse = new ForceOverrideLogic();
                        break;
                    case BuffStackType.Stacking:
                    case BuffStackType.StackingConditionalLoss:
                        logicToUse = new OverrideLogic();
                        break;
                    case BuffStackType.Unknown:
                    default:
                        throw new InvalidDataException("Buffs can not be typless");
                }
                switch (Type)
                {
                    case BuffType.Intensity: return new BuffSimulatorIntensity(Capacity, log, logicToUse);
                    case BuffType.Duration: return new BuffSimulatorDuration(Capacity, log, logicToUse);
                    case BuffType.Unknown:
                        throw new InvalidDataException("Buffs can not be stackless");
                }
            }
            switch (Type)
            {
                case BuffType.Intensity: 
                    return new BuffSimulatorIDIntensity(log);
                case BuffType.Duration: 
                    return new BuffSimulatorIDDuration(log);
                case BuffType.Unknown:
                default:
                    throw new InvalidDataException("Buffs can not be stackless");
            }
        }

        internal static BuffSourceFinder GetBuffSourceFinder(ulong version, HashSet<long> boonIds)
        {
            if (version > 99526)
            {
                return new BuffSourceFinder01102019(boonIds);
            }
            if (version > 95112)
            {
                return new BuffSourceFinder05032019(boonIds);
            }
            return new BuffSourceFinder11122018(boonIds);
        }
    }
}
