using System;
using System.Collections.Generic;

namespace LuckParser.Models.ParseModels
{
    public class SkillData
    {
        public ICollection<SkillItem> Values
        {
            get
            {
                return _skills.Values;
            }
        }
        // Fields
        private readonly Dictionary<long, SkillItem> _skills = new Dictionary<long, SkillItem>();
        
        
        // Public Methods

        public SkillItem Get(long ID)
        {
            if (_skills.TryGetValue(ID, out SkillItem value))
            {
                return value;
            }
            return null;
        }

        public SkillItem GetOrDummy(long ID)
        {
            if (_skills.TryGetValue(ID, out SkillItem value))
            {
                return value;
            }
            return new SkillItem(ID, "UNKNOWN");
        }

        public void Add(SkillItem skillItem)
        {
            _skills.Add(skillItem.ID, skillItem);
        }
    }
}