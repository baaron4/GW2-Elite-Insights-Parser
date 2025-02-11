using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal abstract class BuffSimulator : AbstractBuffSimulator
{
    protected readonly List<BuffStackItem> BuffStack;
    private readonly StackingLogic _logic;

    private readonly int _capacity;

    private static readonly QueueLogic _queueLogic = new();
    private static readonly HealingLogic _healingLogic = new();
    private static readonly ForceOverrideLogic _forceOverrideLogic = new();
    private static readonly OverrideLogic _overrideLogic = new();
    //private static readonly CappedDurationLogic _cappedDurationLogic = new CappedDurationLogic();

    // Constructor
    protected BuffSimulator(ParsedEvtcLog log, Buff buff, BuffStackItemPool pool, int capacity) : base(log, buff, pool)
    {
        pool.InitializeBuffStackItemPool();
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
            case BuffStackType.StackingUniquePerSrc:
            case BuffStackType.StackingConditionalLoss:
                _logic = _overrideLogic;
                break;
            case BuffStackType.Unknown:
            default:
                throw new InvalidDataException("Buffs can not be typless");
        }
        BuffStack = new List<BuffStackItem>((int)Math.Max(Math.Min(_capacity * 1.2, 300), 4));
    }

    protected bool IsFull => _logic.IsFull(BuffStack, _capacity);

    protected override void AfterSimulate()
    {
        Pool.ReleaseBuffStackItems(BuffStack);
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
            if (!_logic.FindLowestValue(Log, Pool, toAdd, BuffStack, WasteSimulationResult, overridenDuration, overridenStackID))
            {
                OverstackSimulationResult.Add(new BuffSimulationItemOverstack(toAdd.Src, toAdd.Duration, toAdd.Start));
            }
        }
        if (addedActive)
        {
            _logic.Activate(BuffStack, toAdd);
        }
    }
    protected override void UpdateSimulator(BuffEvent buffEvent)
    {
        buffEvent.UpdateSimulator(this, true);
    }

    public override void Add(long duration, AgentItem src, long start, uint stackID, bool addedActive, long overridenDuration, uint overridenStackID)
    {
        var toAdd = Pool.GetBuffStackItem(start, duration, src, stackID);
        Add(toAdd, addedActive, overridenDuration, overridenStackID);
    }

    protected void Add(long duration, AgentItem src, AgentItem seedSrc, long time, bool addedActive, bool isExtension, uint stackID)
    {
        var toAdd = Pool.GetBuffStackItem(time, duration, src, seedSrc, isExtension, stackID);
        Add(toAdd, addedActive, 0, 0);
    }

    public override void Remove(AgentItem by, long removedDuration, int removedStacks, long time, BuffRemove removeType, uint stackID)
    {
        switch (removeType)
        {
            case BuffRemove.All:
                foreach (BuffStackItem stackItem in BuffStack)
                {
                    WasteSimulationResult.Add(new BuffSimulationItemWasted(stackItem.Src, stackItem.Duration, time));
                    if (stackItem.Extensions.Count != 0)
                    {
                        foreach ((AgentItem src, long value) in stackItem.Extensions)
                        {
                            WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                        }
                    }
                }
                Pool.ReleaseBuffStackItems(BuffStack);
                BuffStack.Clear();
                break;
            case BuffRemove.Single:
                for (int i = 0; i < BuffStack.Count; i++)
                {
                    BuffStackItem stackItem = BuffStack[i];
                    if (Math.Abs(removedDuration - stackItem.TotalDuration) < ParserHelper.BuffSimulatorDelayConstant)
                    {
                        WasteSimulationResult.Add(new BuffSimulationItemWasted(stackItem.Src, stackItem.Duration, time));
                        if (stackItem.Extensions.Count != 0)
                        {
                            foreach ((AgentItem src, long value) in stackItem.Extensions)
                            {
                                WasteSimulationResult.Add(new BuffSimulationItemWasted(src, value, time));
                            }
                        }
                        Pool.ReleaseBuffStackItem(stackItem);
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
