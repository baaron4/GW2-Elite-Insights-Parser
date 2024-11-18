namespace GW2EIEvtcParser.ParsedData;

public class LanguageEvent : MetaDataEvent
{
    public enum LanguageEnum : byte
    {
        English = 0,
        Missing = 1,
        French = 2,
        German = 3,
        Spanish = 4,
        Chinese = 5,
        Unknown = 6
    }

    public readonly LanguageEnum Language;

    internal LanguageEvent(CombatItem evtcItem) : base(evtcItem)
    {
        Language = GetLanguage(evtcItem);
    }

    internal static LanguageEnum GetLanguage(CombatItem evtcItem)
    {
        return evtcItem.SrcAgent < (byte)LanguageEnum.Unknown ? (LanguageEnum)evtcItem.SrcAgent
            : LanguageEnum.Unknown;
    }

    public override string ToString()
    {
        switch (Language)
        {
            case LanguageEnum.English:
                return "English";
            case LanguageEnum.Missing:
                return "Missing";
            case LanguageEnum.French:
                return "French";
            case LanguageEnum.German:
                return "German";
            case LanguageEnum.Spanish:
                return "Spanish";
            case LanguageEnum.Chinese:
                return "Chinese";
        }
        return "Unknown";
    }

}
