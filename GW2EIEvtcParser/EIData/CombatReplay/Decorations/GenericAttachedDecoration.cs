namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericAttachedDecoration : GenericDecoration
    {
        public Connector ConnectedTo { get; }

        public AgentConnector Owner { get; private set; }
        public bool DrawOnSelect { get; private set; }

        protected GenericAttachedDecoration((int start, int end) lifespan, Connector connector) : base(lifespan)
        {
            ConnectedTo = connector;
        }

        public virtual GenericAttachedDecoration UsingSkillMode(AbstractSingleActor owner, bool drawOnSelect = true)
        {
            if (owner == null)
            {
                Owner = null;
            } 
            else
            {
                Owner = new AgentConnector(owner);
            }
            DrawOnSelect = drawOnSelect;
            return this;
        }

    }
}
