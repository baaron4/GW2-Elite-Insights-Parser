using System;

namespace GW2EIBuilders
{
    public abstract class BuilderSettings
    {
        public Version Version { get; }
        public string ParserName { get; }
        protected BuilderSettings(string parserName, Version version)
        {
            ParserName = parserName;
            Version = version;
        }
    }
}
