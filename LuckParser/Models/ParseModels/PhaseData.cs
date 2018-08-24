using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class PhaseData
    {
        private long _start;
        private readonly long _end;
        private string _name;
        private readonly List<AgentItem> _redirection = new List<AgentItem>();

        public PhaseData(long start, long end)
        {
            _start = start;
            _end = end;
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public string GetName()
        {
            return _name;
        }

        public void AddRedirection(AgentItem agent)
        {
            _redirection.Add(agent);
        }

        public List<AgentItem> GetRedirection()
        {
            return _redirection;
        }

        public long GetDuration(string format = "ms")
        {
            switch (format)
            {
                case "m":
                    return (_end - _start) / 60000;
                case "s":
                    return (_end - _start) / 1000;
                default:
                    return (_end - _start);
            }

        }

        public bool InInterval(long time, long offset = 0)
        {
            return _start <= time - offset && time - offset <= _end;
        }

        public void OverrideStart(long offset)
        {
            if (_redirection.Count > 0)
            {
                _start = _redirection.Min(x => x.GetFirstAware())- offset;
            }
        }

        public long GetStart()
        {
            return _start;
        }

        public long GetEnd()
        {
            return _end;
        }
    }
}
