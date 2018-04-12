using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class MechanicData
    {
        private List<MechanicLog> m_Data;

        public MechanicData() {
            this.m_Data = new List<MechanicLog>();
        }

        public void AddItem(MechanicLog mLog) {
            this.m_Data.Add(mLog);
        }

        public List<MechanicLog> GetMDataLogs() {
            return this.m_Data;
        }
        public List<string> GetMechanicNameList() {
             List<string> mlist = new List<string>();
            //VG
            mlist.Add("name");//damage from red orb
            mlist.Add("name");//hit by blue tp
            mlist.Add("name");//hit by green aoe
            mlist.Add("name");//failed green aoe
            mlist.Add("name");//3 pylon atunments
            //gors
            mlist.Add("name");//slam
            mlist.Add("name");//black
            mlist.Add("name");//orb buff
            mlist.Add("name");//egg
            //sabetha
            mlist.Add("name");//canon launcher
            mlist.Add("name");//green bomb
            mlist.Add("name");//timedbomb
            mlist.Add("name");//killed turret
            //sloth
            //matt
            //kc
            //xera
            //cairn
            //mo
            //sam
            //deiimos
            //horror
            //dhuum






            return mlist;
        }
    }
}
