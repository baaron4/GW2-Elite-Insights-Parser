using System.Diagnostics;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.EIData.BuffSimulators;

internal abstract class AbstractBuffSimulator(ParsedEvtcLog log, Buff buff, BuffStackItemPool pool)
{
    // Fields
    public readonly List<BuffSimulationItem>          GenerationSimulation      = [];
    public readonly List<BuffSimulationItemOverstack> OverstackSimulationResult = [];
    public readonly List<BuffSimulationItemWasted>    WasteSimulationResult     = [];

    public readonly Buff Buff = buff;

    protected readonly ParsedEvtcLog Log = log;

    protected readonly BuffStackItemPool Pool = pool;


    // Abstract Methods
    /// <summary>
    /// Make sure the last element does not overflow the log
    /// </summary>
    /// <param name="logDuration">Duration of the log</param>
    private void Trim(long logDuration)
    {
        for (int i = GenerationSimulation.Count - 1; i >= 0; i--)
        {
            BuffSimulationItem data = GenerationSimulation[i];
            if (data.End > logDuration)
            {
                data.OverrideEnd(logDuration);
            }
            else
            {
                break;
            }
        }
    }

    protected abstract void UpdateSimulator(BuffEvent buffEvent);

    public void Simulate(List<BuffEvent> buffEvents, long logStart, long logEnd)
    {
        if (GenerationSimulation.Count != 0)
        {
            return;
        }
        GenerationSimulation.Capacity = (int)(buffEvents.Count * 1.2);
        WasteSimulationResult.Capacity = (int)(GenerationSimulation.Capacity / 2);
        long timePrev = buffEvents.Count > 0 ? Math.Min(buffEvents[0].Time, logStart) : logStart;
        foreach (BuffEvent buffEvent in buffEvents)
        {
            long timeCur = buffEvent.Time;
            Debug.Assert(timeCur >= timePrev, "Negative passed time in boon simulation");

            Update(timeCur - timePrev);
            UpdateSimulator(buffEvent);
            timePrev = timeCur;
        }
        Update(logEnd - timePrev);

        Trim(logEnd);
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

