using System;

namespace GW2EIParser.Exceptions
{
    public class SkipException : Exception
    {
        public SkipException() : base("Error Encountered: Option enabled - Failed logs are skipped")
        {
        }

    }
}
