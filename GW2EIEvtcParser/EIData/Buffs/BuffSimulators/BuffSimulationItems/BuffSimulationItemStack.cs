using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal abstract class BuffSimulationItemStack : BuffSimulationItem
{
    protected readonly BuffSimulationItemBase[] Stacks;
    private AgentItem[]? _sources;
    private Dictionary<AgentItem, int>? _stacksPerSource;

    public BuffSimulationItemStack(IReadOnlyList<BuffStackItem> stacks) : base(stacks[0].Start, stacks[0].Start + stacks[0].Duration)
    {
        int count = stacks.Count;
        if (count > 0)
        {
            Stacks = new BuffSimulationItemBase[count];
            for (int i = 0; i < count; i++)
            {
                var stack = stacks[i];
                var hasSeed = !stack.SeedSrc.Is(stack.Src);
                var isExtension = stack.IsExtension;
                Stacks[i] = hasSeed ?
                        (
                        isExtension ?
                            stack.Src.IsEnglobingAgent ?
                                stack.SeedSrc.IsEnglobingAgent ?
                                    new BuffSimulationItemBaseEnglobingWithExtensionWithSeedEnglobing(stack)
                                    :
                                    new BuffSimulationItemBaseEnglobingWithExtensionWithSeed(stack)
                                :
                                stack.SeedSrc.IsEnglobingAgent ?
                                    new BuffSimulationItemBaseWithExtensionWithSeedEnglobing(stack)
                                    :
                                    new BuffSimulationItemBaseWithExtensionWithSeed(stack)
                            :
                            stack.Src.IsEnglobingAgent ?
                                stack.SeedSrc.IsEnglobingAgent ? 
                                    new BuffSimulationItemBaseEnglobingWithSeedEnglobing(stack) 
                                    : 
                                    new BuffSimulationItemBaseEnglobingWithSeed(stack)
                                :
                                stack.SeedSrc.IsEnglobingAgent ?
                                    new BuffSimulationItemBaseWithSeedEnglobing(stack)
                                    :
                                    new BuffSimulationItemBaseWithSeed(stack)
                    )
                    :
                    (
                        isExtension ?
                            stack.Src.IsEnglobingAgent ? new BuffSimulationItemBaseEnglobingWithExtension(stack) : new BuffSimulationItemBaseWithExtension(stack) 
                            :
                            stack.Src.IsEnglobingAgent ? new BuffSimulationItemBaseEnglobing(stack) : new BuffSimulationItemBase(stack)
                    )
                    ;
            }
        }
        else
        {
            Stacks = [ ]; // this is array.empty, reused object
        }
    }
    public override int GetStacks()
    {
        return Stacks.Length;
    }

    public override int GetStacks(SingleActor actor)
    {
        //NOTE(Rennorb): This method only gets called for ~5% of the instances created, so we don't create the buffer in the constructor.
        if(_stacksPerSource == null)
        {
            if(Stacks.Length > 0)
            {
                _stacksPerSource = new(10);
                foreach (var stack in Stacks)
                {
                    _stacksPerSource.IncrementValue(stack.Src);
                }
            }
            else
            {
                _stacksPerSource = [ ];
            }
        }

        return _stacksPerSource.GetValueOrDefault(actor.AgentItem);
    }

    /*public override IEnumerable<long> GetActualDurationPerStack()
    {
        return Stacks.Select(x => x.GetActualDuration());
    }*/

    public override IEnumerable<AgentItem> GetSources()
    {
        //NOTE(Rennorb): This method only gets called for ~5% of the instances created, so we don't create the buffer in the constructor.
        if(_sources == null)
        {
            var count = Stacks.Length;
            if(count > 0)
            {
                _sources = new AgentItem[count];
                for (int i = 0; i < count; i++)
                {
                    _sources[i] = Stacks[i].Src;
                }
            }
            else
            {
                _sources =  [ ]; // this is array.empty, reused object
            }
        }

        return _sources;
    }
}
