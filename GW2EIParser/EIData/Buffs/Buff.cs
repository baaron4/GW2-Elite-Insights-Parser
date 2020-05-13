using System;
using System.Collections.Generic;
using System.IO;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Parser.ParseEnum;

namespace GW2EIParser.EIData
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

        public enum BuffStack
        {
            Duration = 0,
            Intensity = 1,
            Unknown = 2,
        }

        // Fields
        public string Name { get; }
        public long ID { get; }
        public BuffNature Nature { get; }
        public GeneralHelper.Source Source { get; }
        private BuffType _type { get; set; }
        public BuffStack Stack {    
            get {
                switch (_type)
                {
                    case BuffType.Queue:
                    case BuffType.Regeneration:
                    case BuffType.Force:
                        return BuffStack.Duration;
                    case BuffType.Stacking:
                    case BuffType.StackingConditionalLoss:
                        return BuffStack.Intensity;
                    default:
                        return BuffStack.Unknown;
                }
            }
        }
        public ulong MaxBuild { get; } = ulong.MaxValue;
        public ulong MinBuild { get; } = ulong.MinValue;
        public int Capacity { get; private set; }
        public string Link { get; }

        /// <summary>
        /// Buff constructor
        /// </summary>
        /// <param name="name">The name of the boon</param>
        /// <param name="id">The id of the boon</param>
        /// <param name="source">Source of the boon <see cref="GeneralHelper.Source"/></param>
        /// <param name="type">Type of the boon (duration or intensity) <see cref="BuffType"/></param>
        /// <param name="capacity">Maximun amount of boon in the queue (duration) or stack (intensity)</param>
        /// <param name="nature">Nature of the boon, dictates in which category the boon will appear <see cref="BuffNature"/></param>
        /// <param name="link">URL to the icon of the buff</param>
        public Buff(string name, long id, GeneralHelper.Source source, BuffType type, int capacity, BuffNature nature, string link)
        {
            Name = name;
            ID = id;
            Source = source;
            _type = type;
            Capacity = capacity;
            Nature = nature;
            Link = link;
        }

        public Buff(string name, long id, GeneralHelper.Source source, BuffNature nature, string link) : this(name, id, source, BuffType.Force, 1, nature, link)
        {
        }

        public Buff(string name, long id, GeneralHelper.Source source, BuffType type, int capacity, BuffNature nature, string link, ulong minBuild, ulong maxBuild) : this(name, id, source, type, capacity, nature, link)
        {
            MaxBuild = maxBuild;
            MinBuild = minBuild;
        }

        public Buff(string name, long id, GeneralHelper.Source source, BuffNature nature, string link, ulong minBuild, ulong maxBuild) : this(name, id, source, BuffType.Force, 1, nature, link, minBuild, maxBuild)
        {
        }

        public Buff(string name, long id, string link)
        {
            Name = name;
            ID = id;
            Source = GeneralHelper.Source.Unknown;
            _type = BuffType.Unknown;
            Capacity = 1;
            Nature = BuffNature.Unknow;
            Link = link;
        }

        public static Buff CreateCustomConsumable(string name, long id, string link, int capacity)
        {
            return new Buff(name + " " + id, id, GeneralHelper.Source.Item, BuffType.Queue, capacity, BuffNature.Consumable, link);
        }
        public AbstractBuffSimulator CreateSimulator(ParsedLog log)
        {
            if (!log.CombatData.HasStackIDs)
            {
                StackingLogic logicToUse;
                switch (_type)
                {
                    case BuffType.Queue:
                        logicToUse = new QueueLogic();
                        break;
                    case BuffType.Regeneration:
                        logicToUse = new HealingLogic();
                        break;
                    case BuffType.Force:
                        logicToUse = new ForceOverrideLogic();
                        break;
                    case BuffType.Stacking:
                    case BuffType.StackingConditionalLoss:
                        logicToUse = new OverrideLogic();
                        break;
                    case BuffType.Unknown:
                    default:
#if DEBUG
                        throw new InvalidDataException("Buffs can not be typless");
#else
                        return null;
#endif
                }
                switch (Stack)
                {
                    case BuffStack.Intensity: return new BuffSimulatorIntensity(Capacity, log, logicToUse);
                    case BuffStack.Duration: return new BuffSimulatorDuration(Capacity, log, logicToUse);
                    case BuffStack.Unknown:
#if DEBUG
                        throw new InvalidDataException("Buffs can not be stackless");
#else
                        return null;
#endif
                }
            }
            switch (Stack)
            {
                case BuffStack.Intensity: 
                    return new BuffSimulatorIDIntensity(log);
                case BuffStack.Duration: 
                    return new BuffSimulatorIDDuration(log);
                case BuffStack.Unknown:
                default:
#if DEBUG
                    throw new InvalidDataException("Buffs can not be stackless");
#else
                    return null;
#endif
            }
        }

        public void AdjustBuff(BuffInfoEvent buffInfoEvent)
        {
            if (buffInfoEvent.BuffID == ID && Capacity != buffInfoEvent.MaxStacks)
            {
#if DEBUG
                int a = 0;
                if (buffInfoEvent.StackingType != _type)
                {
                    int b = 0;
                }
#endif
                _type = buffInfoEvent.StackingType;
                Capacity = buffInfoEvent.MaxStacks;
            }
        }

        public static BuffSourceFinder GetBuffSourceFinder(ulong version, HashSet<long> boonIds)
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
