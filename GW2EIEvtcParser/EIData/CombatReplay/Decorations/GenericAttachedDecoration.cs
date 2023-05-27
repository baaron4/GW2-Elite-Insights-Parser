using System;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericAttachedDecoration : GenericDecoration
    {
        public Connector ConnectedTo { get; }

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
    }
}
