using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Parser.ParsedData
{
    public class SkillData
    {
        public ICollection<SkillItem> Values => _skills.Values;
        // Fields
        private readonly Dictionary<long, SkillItem> _skills = new Dictionary<long, SkillItem>();

        // Public Methods

        public SkillItem Get(long ID)
        {
            if (_skills.TryGetValue(ID, out SkillItem value))
            {
                return value;
            }
            var item = new SkillItem(ID, "UNKNOWN");
            Add(item);
            return item;
        }

        public void Add(SkillItem skillItem)
        {
            if (!_skills.ContainsKey(skillItem.ID))
            {
                _skills.Add(skillItem.ID, skillItem);
            }
        }

        public void CombineWithSkillInfo(CombatData combatData)
        {
            foreach (KeyValuePair<long, SkillItem> pair in _skills)
            {
                SkillInfoEvent skillInfoEvent = combatData.GetSkillInfoEvent(pair.Key);
                if (skillInfoEvent != null)
                {
                    pair.Value.AttachSkillInfoEvent(skillInfoEvent);
                }
            }
        }

    }
}
