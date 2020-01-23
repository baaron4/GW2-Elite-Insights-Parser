namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class LanguageEvent : AbstractMetaDataEvent
    {
        public enum LanguageEnum : ulong { English = 0, Missing = 1, French = 2, German = 3, Spanish = 4, Unknown = 5 }

        public LanguageEnum Language { get; }

        public LanguageEvent(CombatItem evtcItem) : base(evtcItem)
        {
            Language = evtcItem.SrcAgent < (ulong)LanguageEnum.Unknown ? (LanguageEnum)evtcItem.SrcAgent
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
            }
            return "Unknown";
        }

    }
}
