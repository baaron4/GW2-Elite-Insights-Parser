namespace GW2EIParser.EIData
{
    public abstract class GenericAttachedDecoration : GenericDecoration
    {
        public Connector ConnectedTo { get; }

        protected GenericAttachedDecoration((int start, int end) lifespan, Connector connector) : base(lifespan)
        {
            ConnectedTo = connector;
        }

    }
}
