using System.Collections.Generic;

namespace GW2EIEvtcParser.ParsedData
{
    public class SkillData
    {
        // Fields
        private readonly Dictionary<long, SkillItem> _skills = new Dictionary<long, SkillItem>();

        // Public Methods

        internal SkillData()
        {

        }

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

        internal HashSet<long> IgnoreInequalities = new HashSet<long>();

        public bool IgnoreInequality(long ID)
        {
            return IgnoreInequalities.Contains(ID);
        }

        internal void Add(SkillItem skillItem)
        {
            if (!_skills.ContainsKey(skillItem.ID))
            {
                _skills.Add(skillItem.ID, skillItem);
            }
        }

        internal void CombineWithSkillInfo(Dictionary<long, SkillInfoEvent> skillInfoEvents)
        {
            foreach (KeyValuePair<long, SkillItem> pair in _skills)
            {
                if (skillInfoEvents.TryGetValue(pair.Key, out SkillInfoEvent skillInfoEvent))
                {
                    pair.Value.AttachSkillInfoEvent(skillInfoEvent);
                }
            }
        }

    }
}
