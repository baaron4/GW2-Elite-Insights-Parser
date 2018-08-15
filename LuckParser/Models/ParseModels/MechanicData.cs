using LuckParser.Models.DataModels;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.ParseModels
{
    public class MechanicData
    {
        private List<MechanicLog> m_Data;

        public MechanicData()
        {
            m_Data = new List<MechanicLog>();
        }

        public void AddItem(MechanicLog mLog)
        {
            m_Data.Add(mLog);
        }

        public List<MechanicLog> GetMDataLogs()
        {
            return m_Data;
        }

    }
}