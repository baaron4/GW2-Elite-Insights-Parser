using System;

namespace GW2EIBuilders
{
    public class CSVSettings : BuilderSettings
    {
        public string Delimiter { get; }
        public CSVSettings(string parserName, Version version, string delimiter) : base(parserName, version)
        {
            Delimiter = delimiter;
        }
    }
}
