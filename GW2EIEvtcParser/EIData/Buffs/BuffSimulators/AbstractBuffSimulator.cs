using System.Diagnostics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal abstract class AbstractBuffSimulator(ParsedEvtcLog log, Buff buff, BuffStackItemPool pool)
{
    // Fields
    public readonly List<BuffSimulationItem>          GenerationSimulation      = []; //TODO(Rennorb) @perf
    public readonly List<BuffSimulationItemOverstack> OverstackSimulationResult = []; //TODO(Rennorb) @perf
    public readonly List<BuffSimulationItemWasted>    WasteSimulationResult     = []; //TODO(Rennorb) @perf

    public readonly Buff Buff = buff;

    protected readonly ParsedEvtcLog Log = log;

    protected readonly BuffStackItemPool Pool = pool;


    // Abstract Methods
    /// <summary>
    /// Make sure the last element does not overflow the fight
    /// </summary>
    /// <param name="fightDuration">Duration of the fight</param>
    private void Trim(long fightDuration)
    {
        for (int i = GenerationSimulation.Count - 1; i >= 0; i--)
        {
            BuffSimulationItem data = GenerationSimulation[i];
            if (data.End > fightDuration)
            {
                data.OverrideEnd(fightDuration);
            }
            else
            {
                break;
            }
        }
    }

    protected abstract void UpdateSimulator(BuffEvent buffEvent);

    public void Simulate(List<BuffEvent> buffEvents, long fightStart, long fightEnd)
    {
        if (GenerationSimulation.Count != 0)
        {
            return;
        }
        GenerationSimulation.Capacity = (int)(buffEvents.Count * 1.2);
        WasteSimulationResult.Capacity = (int)(GenerationSimulation.Capacity / 2);
        long timePrev = buffEvents.Count > 0 ? Math.Min(buffEvents[0].Time, fightStart) : fightStart;
        foreach (BuffEvent buffEvent in buffEvents)
        {
            long timeCur = buffEvent.Time;
            Debug.Assert(timeCur >= timePrev, "Negative passed time in boon simulation");

            Update(timeCur - timePrev);
            UpdateSimulator(buffEvent);
            timePrev = timeCur;
        }
        Update(fightEnd - timePrev);

        Trim(fightEnd);
        GenerationSimulation.RemoveAll(x => x.Duration <= 0);
        AfterSimulate();
    }

    protected abstract void AfterSimulate();

    protected abstract void Update(long timePassed);

    public abstract void Add(long duration, AgentItem src, long time, uint stackID, bool addedActive, long overridenDuration, uint overridenStackID);

    public abstract void Remove(AgentItem by, long removedDuration, int removedStacks, long time, BuffRemove removeType, uint stackID);

    public abstract void Extend(long extension, long oldValue, AgentItem src, long time, uint stackID);

    public abstract void Activate(uint stackID);
    public abstract void Reset(uint stackID, long toDuration);
}

