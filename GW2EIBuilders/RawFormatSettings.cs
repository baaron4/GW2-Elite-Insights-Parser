using System;

namespace GW2EIBuilders
{
    public class RawFormatSettings: BuilderSettings
    {
        public bool RawFormatTimelineArrays { get; }

        public RawFormatSettings(string parserName, Version version, bool rawFormatTimelineArrays) : base(parserName, version)
        {
            RawFormatTimelineArrays = rawFormatTimelineArrays;
        }
    }
}
