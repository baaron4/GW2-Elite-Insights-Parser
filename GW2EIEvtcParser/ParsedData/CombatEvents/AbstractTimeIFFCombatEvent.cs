namespace GW2EIEvtcParser.ParsedData
{
    public abstract class AbstractTimeIFFCombatEvent : AbstractTimeCombatEvent
    {
        private readonly ArcDPSEnums.IFF _iff;

        public bool ToFriendly => _iff == ArcDPSEnums.IFF.Friend;
        public bool ToFoe => _iff == ArcDPSEnums.IFF.Foe;
        public bool ToUnknown => _iff == ArcDPSEnums.IFF.Unknown;

        internal AbstractTimeIFFCombatEvent(long time, ArcDPSEnums.IFF iff) : base(time)
        {
            _iff = iff;
        }
    }
}
