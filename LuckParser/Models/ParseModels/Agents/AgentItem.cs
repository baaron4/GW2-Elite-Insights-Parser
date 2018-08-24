using System;

namespace LuckParser.Models.ParseModels
{
    public class AgentItem
    {
        // Fields
        private readonly ulong _agent;
        private readonly ushort _id;
        private ulong _masterAgent;
        private ushort _instid;
        private long _firstAware;
        private long _lastAware = long.MaxValue;
        private readonly String _name;
        private readonly String _prof;
        private readonly int _toughness;
        private readonly int _healing;
        private readonly int _condition;
        private readonly int _concentration;

        // Constructors
        public AgentItem(ulong agent, String name, String prof, int toughness, int healing, int condition, int concentration)
        {
            _agent = agent;
            _name = name;
            _prof = prof;
            if (prof.Contains(":"))
            {
                _id = UInt16.Parse(_prof.Split(':')[1]);
            }
            _toughness = toughness;
            _healing = healing;
            _condition = condition;
            _concentration = concentration;
        }

        // Public Methods
        public String[] ToStringArray()
        {
            String[] array = new String[9];
            array[0] = _agent.ToString(); 
            array[1] = _instid.ToString();
            array[2] = _firstAware.ToString();
            array[3] = _lastAware.ToString();
            array[4] = _name;
            array[5] = _prof;
            array[6] = _toughness.ToString();
            array[7] = _healing.ToString();
            array[8] = _condition.ToString();
            return array;
        }

        // Getters
        public ulong GetAgent()
        {
            return _agent;
        }

        public ushort GetInstid()
        {
            return _instid;
        }

        public long GetFirstAware()
        {
            return _firstAware;
        }

        public long GetLastAware()
        {
            return _lastAware;
        }

        public String GetName()
        {
           // name = name.Replace("\0", "");
            
            return _name;
        }

        public String GetProf()
        {
            return _prof;
        }

        public ulong GetMasterAgent()
        {
            return _masterAgent;
        }

        public int GetToughness()
        {
            return _toughness;
        }

        public int GetHealing()
        {
            return _healing;
        }

        public int GetCondition()
        {
            return _condition;
        }

        public int GetConcentration()
        {
            return _concentration;
        }

        public ushort GetID()
        {
            return _id;
        }

        // Setters
        public void SetInstid(ushort instid)
        {
            _instid = instid;
        }

        public void SetMasterAgent(ulong master)
        {
            _masterAgent = master;
        }

        public void SetFirstAware(long firstAware)
        {
            _firstAware = firstAware;
        }

        public void SetLastAware(long lastAware)
        {
            _lastAware = lastAware;
        }
    }
}