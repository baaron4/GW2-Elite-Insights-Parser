using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData.BuffSimulators
{
    internal abstract class BuffSimulator : AbstractBuffSimulator
    {
        protected List<BuffStackItem> BuffStack { get; set; } = new List<BuffStackItem>();
        private StackingLogic _logic { get; }

        private readonly int _capacity;

        private static readonly QueueLogic _queueLogic = new QueueLogic();
        private static readonly HealingLogic _healingLogic = new HealingLogic();
        private static readonly ForceOverrideLogic _forceOverrideLogic = new ForceOverrideLogic();
        private static readonly OverrideLogic _overrideLogic = new OverrideLogic();
        //private static readonly CappedDurationLogic _cappedDurationLogic = new CappedDurationLogic();

        // Constructor
        protected BuffSimulator(ParsedEvtcLog log, Buff buff, int capacity) : base(log, buff)
        {
            _capacity = capacity;
            switch (buff.StackType)
            {
                case BuffStackType.Queue:
                    _logic = _queueLogic;
                    break;
                case BuffStackType.Regeneration:
                    _logic = _healingLogic;
                    break;
                case BuffStackType.Force:
                    _logic = _forceOverrideLogic;
                    break;
                case BuffStackType.Stacking:
                case BuffStackType.StackingSomething:
                case BuffStackType.StackingConditionalLoss:
                    _logic = _overrideLogic;
                    break;
                case BuffStackType.Unknown:
                default:
                    throw new InvalidDataException("Buffs can not be typless");
            }
        }

        protected bool IsFull => _logic.IsFull(BuffStack, _capacity);

        protected override void Clear()
        {
            BuffStack.Clear();
        }

        private void Add(BuffStackItem toAdd, bool addedActive, long overridenDuration, uint overridenStackID)
        {
            // Find empty slot
            if (!IsFull)
            {
                _logic.Add(Log, BuffStack, toAdd);
            }
            // Replace lowest value
            else
            {
                if (!_logic.FindLowestValue(Log, toAdd, BuffStack, WasteSimulationResult, overridenDuration, overridenStackID))
                {
                    OverstackSimulationResult.Add(new BuffSimulationItemOverstack(toAdd.Src, toAdd.Duration, toAdd.Start));
                }
            }
            if (addedActive)
            {
                _logic.Activate(BuffStack, toAdd);
            }
        }

        public override void Add(long duration, AgentItem src, long start, uint stackID, bool addedActive, long overridenDuration, uint overridenStackID)
        {
            var toAdd = new BuffStackItem(start, duration, src, stackID);
            Add(toAdd, addedActive, overridenDuration, overridenStackID);
        }

        protected void Add(long duration, AgentItem src, AgentItem seedSrc, long time, bool addedActive, bool isExtension, uint stackID)
        {
            var toAdd = new BuffStackItem(time, duration, src, seedSrc, isExtension, stackID);
            Add(toAdd, addedActive, 0, 0);
        }

        public override void Remove(AgentItem by, long removedDuration, int removedStacks, long time, ArcDPSEnums.BuffRemove removeType, uint stackID)
        {
            switch (removeType)
            {
                case BuffRemove.All:
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
                case BuffRemove.Single:
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
                            return;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Activate(uint id)
        {
            _logic.Activate(BuffStack, id);
        }
        public override void Reset(uint id, long toDuration)
        {
            _logic.Reset(BuffStack, id, toDuration);
        }
    }
}
