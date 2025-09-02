using GW2EIEvtcParser.ParsedData;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal abstract class BuffSimulationItemStack : BuffSimulationItem
{
    protected AgentItem[]? Sources;
    protected Dictionary<AgentItem, int>? StacksPerSource;

    protected static BuffSimulationItemBase[] GetStacks(IReadOnlyList<BuffStackItem> iStacks)
    {
        int count = iStacks.Count;
        BuffSimulationItemBase[] stacks;
        if (count > 0)
        {
            stacks = new BuffSimulationItemBase[count];
            for (int i = 0; i < count; i++)
            {
                var stack = iStacks[i];
                var hasSeed = !stack.SeedSrc.Is(stack.Src);
                var isExtension = stack.IsExtension;
                stacks[i] = hasSeed ?
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
            stacks = []; // this is array.empty, reused object
        }
        return stacks;
    }

    public BuffSimulationItemStack(IReadOnlyList<BuffStackItem> stacks) : base(stacks[0].Start, stacks[0].Start + stacks[0].Duration)
    {
    }
}
