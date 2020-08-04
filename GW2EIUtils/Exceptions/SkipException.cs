using System;

namespace GW2EIUtils.Exceptions
{
    public class SkipException : Exception
    {
        public SkipException() : base("Option enabled - Failed logs are skipped")
        {
        }

    }
}
