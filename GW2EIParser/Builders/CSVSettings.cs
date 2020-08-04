using System;
using System.Configuration;
using System.IO;

namespace GW2EIParser.Builders
{
    public class CSVSettings
    {
        public string Delimiter { get; }
        public CSVSettings(string delimiter)
        {
            Delimiter = delimiter;
        }
    }
}
