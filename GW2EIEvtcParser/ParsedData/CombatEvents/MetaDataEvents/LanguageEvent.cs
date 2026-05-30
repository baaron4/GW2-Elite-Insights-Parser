using static GW2EIEvtcParser.ArcDPSEnums;

namespace GW2EIEvtcParser.ParsedData;

public class LanguageEvent : MetaDataEvent
{

    public readonly LanguageEnum Language;

    internal LanguageEvent(CombatItem evtcItem) : base(evtcItem)
    {
        Language = GetLanguage(evtcItem);
    }

    internal static LanguageEnum GetLanguage(CombatItem evtcItem)
    {
        return ArcDPSEnums.GetLanguage((byte)evtcItem.SrcAgent);
    }

}
