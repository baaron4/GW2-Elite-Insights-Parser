using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class BuffSimulator : AbstractBuffSimulator
    {
        protected List<BuffStackItem> BuffStack { get; set; } = new List<BuffStackItem>();
        protected StackingLogic Logic { get; }

        private static readonly QueueLogic _queueLogic = new QueueLogic();
        private static readonly HealingLogic _healingLogic = new HealingLogic();
        private static readonly ForceOverrideLogic _forceOverrideLogic = new ForceOverrideLogic();
        private static readonly OverrideLogic _overrideLogic = new OverrideLogic();

        // Constructor
        protected BuffSimulator(ParsedEvtcLog log, Buff buff) : base(log, buff)
        {
            switch (buff.StackType)
            {
                case BuffStackType.Queue:
                    Logic = _queueLogic;
                    break;
                case BuffStackType.Regeneration:
                    Logic = _healingLogic;
                    break;
                case BuffStackType.Force:
                    Logic = _forceOverrideLogic;
                    break;
                case BuffStackType.Stacking:
                case BuffStackType.StackingConditionalLoss:
                    Logic = _overrideLogic;
                    break;
                case BuffStackType.Unknown:
                default:
                    throw new InvalidDataException("Buffs can not be typless");
            }
        }

        protected bool IsFull => Buff.Capacity == BuffStack.Count;

        protected override void Clear()
        {
            BuffStack.Clear();
        }

        public override void Add(long duration, AgentItem src, long start, uint stackID, bool addedActive, uint overstackDuration)
        {
            var toAdd = new BuffStackItem(start, duration, src);
            // Find empty slot
            if (!IsFull)
            {
                Logic.Add(Log, BuffStack, toAdd);
            }
            // Replace lowest value
            else
            {
                bool found = Logic.StackEffect(Log, toAdd, BuffStack, WasteSimulationResult);
                if (!found)
                {
                    OverstackSimulationResult.Add(new BuffSimulationItemOverstack(src, duration, start));
                }
            }
        }

        protected void Add(long duration, AgentItem src, AgentItem seedSrc, long time, bool atFirst, bool isExtension)
        {
            var toAdd = new BuffStackItem(time, duration, src, seedSrc, isExtension);
            // Find empty slot
            if (!IsFull)
            {
                if (atFirst)
                {
                    BuffStack.Insert(0, toAdd);
                }
                else
                {
                    Logic.Add(Log, BuffStack, toAdd);
                }
            }
            // Replace lowest value
            else
            {
                bool found = Logic.StackEffect(Log, toAdd, BuffStack, WasteSimulationResult);
                if (!found)
                {
                    OverstackSimulationResult.Add(new BuffSimulationItemOverstack(src, duration, time));
                }
            }
        }

        public override void Remove(AgentItem by, long removedDuration, int removedStacks, long time, ArcDPSEnums.BuffRemove removeType, uint stackID)
        {
            switch (removeType)
            {
                case ArcDPSEnums.BuffRemove.All:
                    foreach (BuffStackItem stackItem in BuffStack)
                    {
                        WasteSimulationResult.Add(new BuffSimulationItemWasted(stackItem.Src, stackItem.Duration, time));
                        if (stackItem.Extensions.Any())
                        {
                            foreach ((AgentItem src, long value) in stackItem.Extensions)
                            {
                                WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                            }
                        }
                    }
                    BuffStack.Clear();
                    break;
                case ArcDPSEnums.BuffRemove.Single:
                    for (int i = 0; i < BuffStack.Count; i++)
                    {
                        BuffStackItem stackItem = BuffStack[i];
                        if (Math.Abs(removedDuration - stackItem.TotalDuration) < ParserHelper.BuffSimulatorDelayConstant)
                        {
                            WasteSimulationResult.Add(new BuffSimulationItemWasted(stackItem.Src, stackItem.Duration, time));
                            if (stackItem.Extensions.Any())
                            {
                                foreach ((AgentItem src, long value) in stackItem.Extensions)
                                {
                                    WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                                }
                            }
                            BuffStack.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Activate(uint id)
        {

        }
        public override void Reset(uint id, long toDuration)
        {

        }
    }
}
