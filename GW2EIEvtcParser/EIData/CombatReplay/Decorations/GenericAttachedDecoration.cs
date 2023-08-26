using System;
using System.Security.Cryptography.X509Certificates;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericAttachedDecoration : GenericDecoration
    {
        [Flags]
        public enum SkillModeCategory : uint
        {
            NotApplicable = 0,
            ShowOnSelect = 1 << 0,
            ImportantBuffs = 1 << 1,
            ProjectileManagement = 1 << 2,
            Heal = 1 << 3,
            Cleanse = 1 << 4,
            Strip = 1 << 5,
            Portal = 1 << 6,
        }


        public Connector ConnectedTo { get; }
        public RotationConnector RotationConnectedTo { get; protected set; }

        public SkillConnector Owner { get; private set; }
        public SkillModeCategory SkillCategory { get; private set; }

        public ParserHelper.Spec Spec { get; private set; }

        public long SkillID { get; private set; }

        protected GenericAttachedDecoration((int start, int end) lifespan, Connector connector) : base(lifespan)
        {
            ConnectedTo = connector;
        }

        /// <summary>Creates a new line towards the other decoration</summary>
        public LineDecoration LineTo(GenericAttachedDecoration other, int growing, string color)
        {
            int start = Math.Max(Lifespan.start, other.Lifespan.start);
            int end = Math.Min(Lifespan.end, other.Lifespan.end);
            return new LineDecoration(growing, (start, end), color, ConnectedTo, other.ConnectedTo);
        }

        public virtual GenericAttachedDecoration UsingRotationConnector(RotationConnector rotationConnectedTo)
        {
            RotationConnectedTo = rotationConnectedTo;
            return this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">Owner of the skill, will use master if current is a minion</param>
        /// <param name="spec">Spec of the skill, put Unknown for skills not specific to a certain spec</param>
        /// <param name="skillID">ID of the skill</param>
        /// <param name="category">Category of the skill</param>
        /// <returns></returns>
        public virtual GenericAttachedDecoration UsingSkillMode(AbstractSingleActor owner, ParserHelper.Spec spec, long skillID = 0, SkillModeCategory category = SkillModeCategory.NotApplicable)
        {
            if (owner == null)
            {
                Owner = null;
                SkillCategory = SkillModeCategory.NotApplicable;
                Spec = ParserHelper.Spec.Unknown;
                SkillID = 0;
            } 
            else
            {
                Owner = new SkillConnector(owner.AgentItem.GetFinalMaster());
                SkillCategory = category;
                SkillCategory |= SkillModeCategory.ShowOnSelect;
                Spec = spec;
                SkillID = skillID;
            }
            return this;
        }

        /// <summary>
        /// No Spec version of UsingSkillMode
        /// </summary>
        /// <param name="owner">Owner of the skill, will use master if current is a minion</param>
        /// <param name="skillID">ID of the skill</param>
        /// <param name="category">Category of the skill</param>
        /// <returns></returns>
        public GenericAttachedDecoration UsingSkillMode(AbstractSingleActor owner, long skillID = 0, SkillModeCategory category = SkillModeCategory.NotApplicable)
        {
            return UsingSkillMode(owner, ParserHelper.Spec.Unknown, skillID, category);
        }

    }
}
