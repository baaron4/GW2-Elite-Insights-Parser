using static GW2EIEvtcParser.EIData.Buff;

namespace GW2EIEvtcParser.EIData;

public class BuffByActorStatistics
{
    private readonly Dictionary<SingleActor, double> _generatedBy = [];
    public IReadOnlyDictionary<SingleActor, double> GeneratedBy => _generatedBy;
    private readonly Dictionary<SingleActor, double> _generatedPresenceBy = [];
    public IReadOnlyDictionary<SingleActor, double> GeneratedPresenceBy => _generatedPresenceBy;
    private readonly Dictionary<SingleActor, double> _overstackedBy = [];
    public IReadOnlyDictionary<SingleActor, double> OverstackedBy => _overstackedBy;
    private readonly Dictionary<SingleActor, double> _wastedFrom = [];
    public IReadOnlyDictionary<SingleActor, double> WastedFrom => _wastedFrom;
    private readonly Dictionary<SingleActor, double> _unknownExtensionFrom = [];
    public IReadOnlyDictionary<SingleActor, double> UnknownExtensionFrom => _unknownExtensionFrom;
    private readonly Dictionary<SingleActor, double> _extensionBy = [];
    public IReadOnlyDictionary<SingleActor, double> ExtensionBy => _extensionBy;
    private readonly Dictionary<SingleActor, double> _extendedFrom = [];
    public IReadOnlyDictionary<SingleActor, double> ExtendedFrom => _extendedFrom;


    internal static (BuffByActorStatistics, BuffByActorStatistics) GetBuffByActor(ParsedEvtcLog log, Buff buff, SingleActor dst, long start, long end, BuffDistribution buffDistribution)
    {
        long phaseDuration = end - start;
        long activePhaseDuration = dst.GetActiveDuration(log, start, end);
        var buffs = new BuffByActorStatistics();
        var buffsActive = new BuffByActorStatistics();
        foreach (SingleActor actor in buffDistribution.GetSrcs(buff.ID, log))
        {
            long gen = buffDistribution.GetGeneration(buff.ID, actor.AgentItem);
            double generated = gen;
            double generatedPresence = 0;
            double overstacked = (buffDistribution.GetOverstack(buff.ID, actor.AgentItem) + gen);
            double wasted = buffDistribution.GetWaste(buff.ID, actor.AgentItem);
            double unknownExtension = buffDistribution.GetUnknownExtension(buff.ID, actor.AgentItem);
            double extension = buffDistribution.GetExtension(buff.ID, actor.AgentItem);
            double extended = buffDistribution.GetExtended(buff.ID, actor.AgentItem);


            if (buff.Type == BuffType.Duration)
            {
                generated *= 100.0;
                overstacked *= 100.0;
                wasted *= 100.0;
                unknownExtension *= 100.0;
                extension *= 100.0;
                extended *= 100.0;
            } 
            else if (dst.GetBuffPresence(log, start, end, actor).TryGetValue(buff.ID, out var presenceValue)) 
            {
                generatedPresence = 100.0 * presenceValue;
            }
            buffs._generatedBy[actor] = Math.Round(generated / phaseDuration, ParserHelper.BuffDigit);
            buffs._generatedPresenceBy[actor] = Math.Round(generatedPresence / phaseDuration, ParserHelper.BuffDigit);
            buffs._overstackedBy[actor] = Math.Round(overstacked / phaseDuration, ParserHelper.BuffDigit);
            buffs._wastedFrom[actor] = Math.Round(wasted / phaseDuration, ParserHelper.BuffDigit);
            buffs._unknownExtensionFrom[actor] = Math.Round(unknownExtension / phaseDuration, ParserHelper.BuffDigit);
            buffs._extensionBy[actor] = Math.Round(extension / phaseDuration, ParserHelper.BuffDigit);
            buffs._extendedFrom[actor] = Math.Round(extended / phaseDuration, ParserHelper.BuffDigit);
            if (activePhaseDuration > 0)
            {
                buffsActive._generatedBy[actor] = Math.Round(generated / activePhaseDuration, ParserHelper.BuffDigit);
                buffsActive._generatedPresenceBy[actor] = Math.Round(generated / activePhaseDuration, ParserHelper.BuffDigit);
                buffsActive._overstackedBy[actor] = Math.Round(overstacked / activePhaseDuration, ParserHelper.BuffDigit);
                buffsActive._wastedFrom[actor] = Math.Round(wasted / activePhaseDuration, ParserHelper.BuffDigit);
                buffsActive._unknownExtensionFrom[actor] = Math.Round(unknownExtension / activePhaseDuration, ParserHelper.BuffDigit);
                buffsActive._extensionBy[actor] = Math.Round(extension / activePhaseDuration, ParserHelper.BuffDigit);
                buffsActive._extendedFrom[actor] = Math.Round(extended / activePhaseDuration, ParserHelper.BuffDigit);
            }
            else
            {
                buffsActive._generatedBy[actor] = 0.0;
                buffsActive._generatedPresenceBy[actor] = 0.0;
                buffsActive._overstackedBy[actor] = 0.0;
                buffsActive._wastedFrom[actor] = 0.0;
                buffsActive._unknownExtensionFrom[actor] = 0.0;
                buffsActive._extensionBy[actor] = 0.0;
                buffsActive._extendedFrom[actor] = 0.0;
            }
        }
        return (buffs, buffsActive);
    }

}
