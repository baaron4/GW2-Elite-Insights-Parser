using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class Mechanic
    {
        // Fields
       
        private int skill_id;
        private string name;
        private string altname;
        private int mechType;
        //0 boon on player
        //1 boon on boss
        //2 skill by player
        //3 skill on player
        //4 enemy boon stripped
        //5 spawn check
        //6 boss cast (check finished)
        private ushort bossid;
        private string plotlyShape;

        public Mechanic( int skill_id, string name, int mechtype, ushort bossid, string plotlyShape,string friendlyName)
        {
            this.skill_id = skill_id;
            this.name = name;
            this.mechType = mechtype;
            this.bossid = bossid;
            this.plotlyShape = plotlyShape;
            this.altname = friendlyName;
        }
        //getters
       
        public int GetSkill()
        {
            return skill_id;
        }
        public string GetName()
        {
            return name;
        }
        public int GetMechType()
        {
            return mechType;
        }
        public ushort GetBossID() {
            return bossid;
        }

        public string GetPlotly()
        {
            return plotlyShape;
        }
        public string GetAltName()
        {
            return altname;
        }
    }
}
