using System.Collections.Generic;

namespace GW2EIEvtcParser.ParsedData
{
    public class SkillData
    {
        // Fields
        private readonly Dictionary<long, SkillItem> _skills = new Dictionary<long, SkillItem>();
        private readonly GW2EIGW2API.GW2APIController _apiController;
        public long DodgeId { get; }
        public long GenericBreakbarId { get; }
        // Public Methods

        internal SkillData(GW2EIGW2API.GW2APIController apiController, int arcdpsVersion)
        {
            _apiController = apiController;
            if (arcdpsVersion >= 20220307)
            {
                DodgeId = SkillItem._DodgeId20220307;
                GenericBreakbarId = SkillItem._GenericBreakbarId20220307;
            } 
            else
            {
                DodgeId = SkillItem._DodgeId;
                GenericBreakbarId = SkillItem._GenericBreakbarId;
            }
        }

        public SkillItem Get(long ID)
        {
            if (_skills.TryGetValue(ID, out SkillItem value))
            {
                return value;
            }
            Add(ID, SkillItem.DefaultName);
            return _skills[ID];
        }

        internal HashSet<long> NotAccurate = new HashSet<long>();

        public bool IsNotAccurate(long ID)
        {
            return NotAccurate.Contains(ID);
        }

        internal void Add(long id, string name)
        {
            if (!_skills.ContainsKey(id))
            {
                _skills.Add(id, new SkillItem(id, name, _apiController));
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
