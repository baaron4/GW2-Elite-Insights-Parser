using System;

namespace LuckParser
{
    public class SkipException : Exception
    {
        public SkipException() : base("Option enabled: Failed logs are skipped")
        {
        }

    }
}
