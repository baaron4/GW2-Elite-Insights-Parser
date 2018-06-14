namespace LuckParser.Models.ParseModels
{
    public class Mechanic
    {
        // Fields
       
        private int skill_id;
        private string name;
        private string altname;
        private int mechType;
        //X indicates not gathering mechanic in mech logs
        //0 boon on player
        //1 boon on boss X
        //2 skill by player X
        //3 skill on player
        //4 enemy boon stripped X
        //5 spawn check X
        //6 boss cast (check finished) X
        //7 player on player 
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
