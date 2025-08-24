using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData;

public class BuffVolumeByActorStatistics
{
    private Dictionary<SingleActor, double> _incomingBy = [];
    public IReadOnlyDictionary<SingleActor, double> IncomingBy => _incomingBy;
    private Dictionary<SingleActor, double> _incomingByExtensionBy = [];
    public IReadOnlyDictionary<SingleActor, double> IncomingByExtensionBy => _incomingByExtensionBy;


    internal static (BuffVolumeByActorStatistics, BuffVolumeByActorStatistics) GetBuffVolumeByActor(ParsedEvtcLog log, Buff buff, SingleActor dstActor, long start, long end)
    {
        long phaseDuration = end - start;
        long activePhaseDuration = dstActor.GetActiveDuration(log, start, end);

        var buffs = new BuffVolumeByActorStatistics();
        var buffsActive = new BuffVolumeByActorStatistics();

        var applies = log.CombatData.GetBuffApplyDataByIDByDst(buff.ID, dstActor.AgentItem);
        var appliesBySrc = applies.GroupBy(x => x.CreditedBy);
        foreach (var group in appliesBySrc)
        {
            SingleActor? actor = log.FindActor(group.Key);
            double incoming = 0;
            double incomingByExtension = 0;
            foreach (AbstractBuffApplyEvent abae in group)
            {
                if (abae.Time >= start && abae.Time <= end)
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
        return (buffs, buffsActive);
    }

}
