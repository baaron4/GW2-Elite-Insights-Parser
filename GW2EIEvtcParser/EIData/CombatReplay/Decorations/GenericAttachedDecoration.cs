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

        public AgentConnector Owner { get; private set; }
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
            int start = Math.Max(this.Lifespan.start, other.Lifespan.start);
            int end = Math.Min(this.Lifespan.end, other.Lifespan.end);
            return new LineDecoration(growing, (start, end), color, this.ConnectedTo, other.ConnectedTo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">Owner of the skill, will use master if current is a minion</param>
        /// <param name="category"></param>
        /// <returns></returns>
        public virtual GenericAttachedDecoration UsingSkillMode(AbstractSingleActor owner, SkillModeCategory category = SkillModeCategory.NotApplicable, ParserHelper.Spec spec = ParserHelper.Spec.Unknown, long skillID = 0)
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
                Owner = new AgentConnector(owner.AgentItem.GetFinalMaster());
                SkillCategory = category;
                SkillCategory |= SkillModeCategory.ShowOnSelect;
                Spec = spec;
                SkillID = skillID;
            }
            return this;
        }

    }
}
