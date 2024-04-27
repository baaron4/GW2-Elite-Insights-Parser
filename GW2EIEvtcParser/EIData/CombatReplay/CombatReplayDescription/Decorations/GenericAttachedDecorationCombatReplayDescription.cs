
using GW2EIEvtcParser.ParsedData;
using System.Collections.Generic;

namespace GW2EIEvtcParser.EIData
{
    public abstract class GenericAttachedDecorationCombatReplayDescription : GenericDecorationCombatReplayDescription
    {
        public object ConnectedTo { get; }
        public object RotationConnectedTo { get; }

        private class SkillModeDescription
        {
            public object Owner { get; internal set; }

            public uint Category { get; internal set; }

            public long SkillID { get; internal set; }
            public bool IsBuff { get; internal set; }
        }
        SkillModeDescription SkillMode { get; } = null;

        internal GenericAttachedDecorationCombatReplayDescription(ParsedEvtcLog log, GenericAttachedDecoration decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs) : base(decoration)
        {
            ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
            RotationConnectedTo = decoration.RotationConnectedTo?.GetConnectedTo(map, log);
            IsMechanicOrSkill = true;
            if (decoration.SkillMode != null)
            {
                SkillMode = new SkillModeDescription
                {
                    Category = (uint)decoration.SkillMode.Category
                };
                if (log.Buffs.BuffsByIds.TryGetValue(decoration.SkillMode.SkillID, out Buff buff))
                {
                    if (!usedBuffs.ContainsKey(buff.ID)) {
                        usedBuffs.Add(buff.ID, buff);
                    }
                    SkillMode.IsBuff = true;
                } 
                else
                {
                    SkillItem skill = log.SkillData.Get(decoration.SkillMode.SkillID);
                    if (!usedSkills.ContainsKey(skill.ID))
                    {
                        usedSkills.Add(skill.ID, skill);
                    }
                    SkillMode.IsBuff = false;
                }
                if (decoration.SkillMode.Owner != null)
                {
                    SkillMode.Owner = decoration.SkillMode.Owner.GetConnectedTo(map, log);
                }
            }
        }
    }
}
