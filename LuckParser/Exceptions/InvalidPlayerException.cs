using System;

namespace LuckParser.Exceptions
{
    public class InvalidPlayerException : Exception
    {
        public bool Squadless { get; }

        public InvalidPlayerException(bool squadless) : base("Player agent is not correct")
        {
            Squadless = squadless;
        }

    }
}
