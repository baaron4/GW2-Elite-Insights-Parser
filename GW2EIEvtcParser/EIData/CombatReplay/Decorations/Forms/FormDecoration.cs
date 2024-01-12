using System.IO;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class FormDecoration : GenericAttachedDecoration
    {

        public bool Filled { get; private set; } = true;
        public string Color { get; protected set; }
        public int GrowingEnd { get; private set; }
        public bool GrowingReverse { get; private set; }

        protected FormDecoration((long , long) lifespan, string color, GeographicalConnector connector) : base(lifespan, connector)
        {
            Color = color;
            GrowingEnd = (int)lifespan.Item1;
        }

        protected FormDecoration((long, long) lifespan, Color color, double opacity, GeographicalConnector connector) : this(lifespan, color.WithAlpha(opacity).ToString(true), connector)
        {
        }

        public virtual FormDecoration UsingFilled(bool filled)
        {
            Filled = filled;
            return this;
        }

        public virtual FormDecoration UsingGrowingEnd(long growingEnd, bool reverse = false)
        {
            GrowingEnd = growingEnd <= Lifespan.start ? Lifespan.start : (int)growingEnd;
            GrowingReverse = reverse;
            return this;
        }

        public abstract FormDecoration Copy();

        public abstract FormDecoration GetBorderDecoration(string borderColor = null);
        public FormDecoration GetBorderDecoration(Color borderColor, double opacity)
        {
            return GetBorderDecoration(borderColor.WithAlpha(opacity).ToString(true));
        }

    }
}
