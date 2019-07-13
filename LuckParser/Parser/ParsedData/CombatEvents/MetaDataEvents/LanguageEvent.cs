namespace LuckParser.Parser.ParsedData.CombatEvents
{
    public class LanguageEvent : AbstractMetaDataEvent
    {
        public enum LanguageEnum : ulong { English = 0, Missing = 1, French = 2, Germah = 3, Spanish = 4, Unknown = 5}
        
        public LanguageEnum Language { get; }

        public LanguageEvent(CombatItem evtcItem, long offset) : base(evtcItem, offset)
        {
            Language = evtcItem.SrcAgent < (ulong)LanguageEnum.Unknown ? (LanguageEnum)evtcItem.SrcAgent
                : LanguageEnum.Unknown;
        }

    }
}
