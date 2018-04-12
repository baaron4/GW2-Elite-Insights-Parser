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
        private int damage;
        private Player player;

        public MechanicLog(int time, int skill_id, int damage, Player p) {
            this.time = time;
            this.skill_id = skill_id;
            this.damage = damage;
            this.player = p;
        }
        //getters
        public int GetTime() {
            return time;
        }
        public int GetSkill()
        {
            return skill_id;
        }
        public int GetDamage()
        {
            return damage;
        }
        public Player GetPlayer()
        {
            return player;
        }
    }
}
