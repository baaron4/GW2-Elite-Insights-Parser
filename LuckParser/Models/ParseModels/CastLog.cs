using LuckParser.Models.ParseEnums;

namespace LuckParser.Models.ParseModels
{
    public  class CastLog
    {
        // Fields
        private long time;
        private int skill_id;
        private int exp_dur;
        private int act_dur;
        private Activation start_activation;
        private Activation end_activation;


        // Constructor
        public CastLog(long time, int skill_id, int exp_dur, Activation start_activation,int act_dur,Activation end_activation)
        {
            this.time = time;
            this.skill_id = skill_id;
            this.exp_dur = exp_dur;
            this.start_activation = start_activation;
            this.act_dur = act_dur;
            this.end_activation = end_activation;
        }
        //start cast log
        public CastLog(long time, int skill_id, int exp_dur, Activation start_activation)
        {
            this.time = time;
            this.skill_id = skill_id;
            this.exp_dur = exp_dur;
            this.start_activation = start_activation;
            
        }

        // Getters
        public long getTime()
        {
            return time;
        }
        public int getID()
        {
            return skill_id;
        }
        public int getExpDur()
        {
            return exp_dur;
        }
        public Activation startActivation()
        {
            return start_activation;
        }
        public int getActDur()
        {
            return act_dur;
        }
        public Activation endActivation()
        {
            return end_activation;
        }
    }
}

