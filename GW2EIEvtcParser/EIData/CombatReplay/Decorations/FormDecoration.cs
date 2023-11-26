using System.IO;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class FormDecoration : GenericAttachedDecoration
    {

        public bool Filled { get; private set; } = true;
        public string Color { get; protected set; }
        public int GrowingEnd { get; private set; }
        public bool GrowingReverse { get; private set; }

        protected FormDecoration((long , long) lifespan, string color, Connector connector) : base(lifespan, connector)
        {
            Color = color;
        }

        public virtual FormDecoration UsingFilled(bool filled)
        {
            Filled = filled;
            return this;
        }

        public virtual FormDecoration UsingGrowingEnd(long growingEnd, bool reverse = false)
        {
            if (GrowingReverse)
            {
                throw new InvalidDataException("GrowingEnd must be positive");
            }
            GrowingEnd = growingEnd < Lifespan.start ? 0 : (int)growingEnd;
            GrowingReverse = reverse;
            return this;
        }

        public abstract FormDecoration Copy();

        public abstract FormDecoration GetBorderDecoration(string borderColor = null);

    }
}
