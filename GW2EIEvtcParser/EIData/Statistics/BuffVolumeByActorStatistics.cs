using System;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData;

public class BuffVolumeByActorStatistics
{
    private Dictionary<SingleActor, double> _incomingBy = [];
    public IReadOnlyDictionary<SingleActor, double> IncomingBy => _incomingBy;
    private Dictionary<SingleActor, double> _incomingByExtensionBy = [];
    public IReadOnlyDictionary<SingleActor, double> IncomingByExtensionBy => _incomingByExtensionBy;

    private static void FillDictionnaries(ParsedEvtcLog log, Buff buff, AgentItem src, IEnumerable<AbstractBuffApplyEvent> group, BuffVolumeByActorStatistics buffs, BuffVolumeByActorStatistics buffsActive, long phaseDuration, long activePhaseDuration)
    {
        SingleActor actor = log.FindActor(src);
        double incoming = 0;
        double incomingByExtension = 0;
        foreach (AbstractBuffApplyEvent abae in group)
        {
            if (abae is BuffApplyEvent bae)
            {
                // We ignore infinite duration buffs
                if (bae.AppliedDuration >= int.MaxValue)
                {
                    continue;
                }
                incoming += bae.AppliedDuration;
            }
            if (abae is BuffExtensionEvent bee)
            {
                incomingByExtension += bee.ExtendedDuration;
                if (activePhaseDuration > 0)
                {
                    incomingByExtension += bee.ExtendedDuration;
                }
            }

        }
        incoming += incomingByExtension;

        if (buff.Type == BuffType.Duration)
        {
            incoming *= 100.0;
            incomingByExtension *= 100.0;
        }
        buffs._incomingBy[actor] = Math.Round(incoming / phaseDuration, ParserHelper.BuffDigit);
        buffs._incomingByExtensionBy[actor] = Math.Round(incomingByExtension / phaseDuration, ParserHelper.BuffDigit);
        if (activePhaseDuration > 0)
        {
            buffsActive._incomingBy[actor] = Math.Round(incoming / activePhaseDuration, ParserHelper.BuffDigit);
            buffsActive._incomingByExtensionBy[actor] = Math.Round(incomingByExtension / activePhaseDuration, ParserHelper.BuffDigit);
        }
        else
        {
            buffsActive._incomingBy[actor] = 0.0;
            buffsActive._incomingByExtensionBy[actor] = 0.0;
        }
    }


    internal static (BuffVolumeByActorStatistics, BuffVolumeByActorStatistics) GetBuffVolumeByActor(ParsedEvtcLog log, Buff buff, SingleActor dstActor, long start, long end)
    {
        long phaseDuration = end - start;
        long activePhaseDuration = dstActor.GetActiveDuration(log, start, end);

        var buffs = new BuffVolumeByActorStatistics();
        var buffsActive = new BuffVolumeByActorStatistics();

        var applies = dstActor.GetBuffApplyEventsOnByID(log, start, end, buff.ID, null);
        var appliesBySrc = applies.GroupBy(x => x.CreditedBy);
        foreach (var group in appliesBySrc)
        {
            if (group.Key.IsEnglobingAgent)
            {
                foreach (var src in group.Key.EnglobedAgentItems)
                {
                    FillDictionnaries(log, buff, group.Key, group.Where(x => x.Time >= src.FirstAware && x.Time <= src.LastAware), buffs, buffsActive, phaseDuration, activePhaseDuration);
                }
            } 
            else
            {
                FillDictionnaries(log, buff, group.Key, group, buffs, buffsActive, phaseDuration, activePhaseDuration);
            }
        }
        return (buffs, buffsActive);
    }

}
