using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class MechanicLog
    {
        // Fields
        private int time;
        private int skill_id;
        private string name;
        private int damage;
        private Player player;
        private string plotlyShape;

        public MechanicLog(int time, int skill_id,string name, int damage, Player p,string plotlyShape) {
            this.time = time;
            this.skill_id = skill_id;
            this.name = name;
            this.damage = damage;
            this.player = p;
            this.plotlyShape = plotlyShape;
        }
        //getters
        public int GetTime() {
            return time;
        }
        public int GetSkill()
        {
            return skill_id;
        }
        public string GetName() {
            return name;
        }
        public int GetDamage()
        {
            return damage;
        }
        public Player GetPlayer()
        {
            return player;
        }
        public string GetPlotly() {
            return plotlyShape;
        }
    }
}
