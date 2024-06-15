using System;
using static GW2EIEvtcParser.ParserHelper;

namespace GW2EIEvtcParser.EIData
{
    internal class SkillModeDescriptor
    {
        [Flags]
        public enum SkillModeCategory : uint
        {
            /// <summary>Unknown category</summary>
            NotApplicable = 0,

            /// <summary>Visible on player select</summary>
            ShowOnSelect = 1 << 0,

            /// <summary>Applies important buffs to allies</summary>
            ImportantBuffs = 1 << 1,

            /// <summary>Reflects or blocks projectiles</summary>
            ProjectileManagement = 1 << 2,

            /// <summary>Heals allies</summary>
            Heal = 1 << 3,

            /// <summary>Cleanses conditions from allies</summary>
            Cleanse = 1 << 4,

            /// <summary>Strips boons from enemies</summary>
            Strip = 1 << 5,

            /// <summary>Active portal</summary>
            Portal = 1 << 6,

            /// <summary>Crowd Control</summary>
            CC = 1 << 7,
        }

        public SkillConnector Owner;
        public Spec Spec;
        public long SkillID = 0;
        public SkillModeCategory Category = SkillModeCategory.NotApplicable;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner">Owner of the skill, will use master if current is a minion</param>
        /// <param name="spec">Spec of the skill, put Unknown for skills not specific to a certain spec</param>
        /// <param name="skillID">ID of the skill</param>
        /// <param name="category">Category of the skill</param>
        /// <returns></returns>
        public SkillModeDescriptor(AbstractSingleActor owner, Spec spec, long skillID = 0, SkillModeCategory category = SkillModeCategory.NotApplicable)
        {
            if (owner == null)
            {
                throw new InvalidOperationException("SkillModeDescriptor must have an owner");
            }
            Owner = new SkillConnector(owner.AgentItem.GetFinalMaster());
            Category = category;
            Category |= SkillModeCategory.ShowOnSelect;
            Spec = spec;
            SkillID = skillID;
        }


        /// <summary>
        /// No Spec version of SkillDescriptor
        /// </summary>
        /// <param name="owner">Owner of the skill, will use master if current is a minion</param>
        /// <param name="skillID">ID of the skill</param>
        /// <param name="category">Category of the skill</param>
        /// <returns></returns>
        public SkillModeDescriptor(AbstractSingleActor owner, long skillID = 0, SkillModeCategory category = SkillModeCategory.NotApplicable) : this(owner, Spec.Unknown, skillID, category)
        {
        }
    }
}
