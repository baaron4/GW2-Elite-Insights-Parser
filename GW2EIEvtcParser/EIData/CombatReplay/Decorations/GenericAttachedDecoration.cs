using System;

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

        public LineDecoration LineTo(GenericAttachedDecoration other, int growing, string color)
        {
            int start = Math.Max(this.Lifespan.start, other.Lifespan.start);
            int end = Math.Min(this.Lifespan.end, other.Lifespan.end);
            return new LineDecoration(growing, (start, end), color, this.ConnectedTo, other.ConnectedTo);
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
