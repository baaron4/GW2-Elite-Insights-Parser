using System;
using System.Security.Cryptography.X509Certificates;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericAttachedDecoration : GenericDecoration
    {

        public Connector ConnectedTo { get; }
        public RotationConnector RotationConnectedTo { get; protected set; }
        public SkillModeDescriptor SkillMode;

        protected GenericAttachedDecoration((long , long ) lifespan, Connector connector) : base(lifespan)
        {
            ConnectedTo = connector;
        }

        /// <summary>Creates a new line towards the other decoration</summary>
        public LineDecoration LineTo(GenericAttachedDecoration other, string color)
        {
            int start = Math.Max(Lifespan.start, other.Lifespan.start);
            int end = Math.Min(Lifespan.end, other.Lifespan.end);
            return new LineDecoration((start, end), color, ConnectedTo, other.ConnectedTo);
        }

        public virtual GenericAttachedDecoration UsingRotationConnector(RotationConnector rotationConnectedTo)
        {
            RotationConnectedTo = rotationConnectedTo;
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="skill">Skill information</param>
        /// <returns></returns>
        public virtual GenericAttachedDecoration UsingSkillMode(SkillModeDescriptor skill)
        {
            SkillMode = skill;
            return this;
        }
    }
}
