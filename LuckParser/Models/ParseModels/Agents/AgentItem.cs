using System;

namespace LuckParser.Models.ParseModels
{
    public class AgentItem
    {
        // Fields
        private ulong _agent;
        private ushort _id = 0;
        private ulong _masterAgent = 0;
        private ushort _instid = 0;
        private long _firstAware = 0;
        private long _lastAware = long.MaxValue;
        private String _name;
        private String _prof;
        private int _toughness = 0;
        private int _healing = 0;
        private int _condition = 0;
        private int _concentration = 0;

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
            array[0] = string.Format("{0:X}", _agent);//Long.toHexString(agent); 
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