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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">Owner of the skill, will use master if current is a minion</param>
        /// <param name="drawOnSelect"></param>
        /// <returns></returns>
        public virtual GenericAttachedDecoration UsingSkillMode(AbstractSingleActor owner, bool drawOnSelect = true)
        {
            if (owner == null)
            {
                Owner = null;
            } 
            else
            {
                Owner = new AgentConnector(owner.AgentItem.GetFinalMaster());
            }
            DrawOnSelect = drawOnSelect;
            return this;
        }

    }
}
