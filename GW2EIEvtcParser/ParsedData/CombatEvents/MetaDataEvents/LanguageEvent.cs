namespace GW2EIEvtcParser.ParsedData
{
    public class LanguageEvent : AbstractMetaDataEvent
    {
        public enum LanguageEnum : byte { English = 0, Missing = 1, French = 2, German = 3, Spanish = 4, Unknown = 5 }

        public LanguageEnum Language { get; }

        internal LanguageEvent(CombatItem evtcItem) : base(evtcItem)
        {
            Language = evtcItem.SrcAgent < (byte)LanguageEnum.Unknown ? (LanguageEnum)evtcItem.SrcAgent
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
