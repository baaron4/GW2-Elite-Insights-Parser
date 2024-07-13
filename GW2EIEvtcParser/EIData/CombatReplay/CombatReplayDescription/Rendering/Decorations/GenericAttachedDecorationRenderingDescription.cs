
using System.Collections.Generic;
using GW2EIEvtcParser.ParsedData;
using static GW2EIEvtcParser.EIData.GenericAttachedDecoration;

namespace GW2EIEvtcParser.EIData
{
    internal abstract class GenericAttachedDecorationRenderingDescription : GenericDecorationRenderingDescription
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
        public object SkillMode { get; } = null;

        internal GenericAttachedDecorationRenderingDescription(ParsedEvtcLog log, GenericAttachedDecorationRenderingData decoration, CombatReplayMap map, Dictionary<long, SkillItem> usedSkills, Dictionary<long, Buff> usedBuffs, string metadataSignature) : base(decoration, metadataSignature)
        {
            ConnectedTo = decoration.ConnectedTo.GetConnectedTo(map, log);
            RotationConnectedTo = decoration.RotationConnectedTo?.GetConnectedTo(map, log);
            IsMechanicOrSkill = true;
            if (decoration.SkillMode != null)
            {
                var skillModeDescription = new SkillModeDescription
                {
                    Category = (uint)decoration.SkillMode.Category
                };
                if (log.Buffs.BuffsByIds.TryGetValue(decoration.SkillMode.SkillID, out Buff buff))
                {
                    if (!usedBuffs.ContainsKey(buff.ID))
                    {
                        usedBuffs.Add(buff.ID, buff);
                    }
                    skillModeDescription.IsBuff = true;
                }
                else
                {
                    SkillItem skill = log.SkillData.Get(decoration.SkillMode.SkillID);
                    if (!usedSkills.ContainsKey(skill.ID))
                    {
                        usedSkills.Add(skill.ID, skill);
                    }
                    skillModeDescription.IsBuff = false;
                }
                if (decoration.SkillMode.Owner != null)
                {
                    skillModeDescription.Owner = decoration.SkillMode.Owner.GetConnectedTo(map, log);
                }
                SkillMode = skillModeDescription;
            }
        }
    }
}
