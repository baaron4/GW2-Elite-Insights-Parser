using System.IO;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class FormDecoration : GenericAttachedDecoration
    {

        public bool Filled { get; private set; }
        public string Color { get; }
        public int GrowingEnd { get; private set; }

        protected FormDecoration((long , long) lifespan, string color, Connector connector) : base(lifespan, connector)
        {
            Color = color;
        }

        public virtual FormDecoration UsingFilled(bool filled)
        {
            Filled = filled;
            return this;
        }

        public virtual FormDecoration UsingGrowing(long growingEnd, bool reverse)
        {
            if (growingEnd < 0)
            {
                throw new InvalidDataException("GrowingEnd must be positive");
            }
            GrowingEnd = (int)growingEnd;
            if (reverse)
            {
                GrowingEnd *= -1;
            }
            return this;
        }

        public abstract FormDecoration Copy();

    }
}
