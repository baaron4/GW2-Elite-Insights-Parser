using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LuckParser.Models.ParseEnums;

namespace LuckParser.Models.ParseModels
{
    public class CombatItem
    {

        // Fields
        private int time;
        private long src_agent;
        private long dst_agent;
        private int value;
        private int buff_dmg;
        private int overstack_value;
        private int skill_id;
        private int src_instid;
        private int dst_instid;
        private int src_master_instid;
        private IFF iff;
        private int is_buff;
        private Result result;
        private Activation is_activation;
        private BuffRemove is_buffremove;
        private int is_ninety;
        private int is_fifty;
        private int is_moving;
        private StateChange is_statechange;
        private int is_flanking;

        // Constructor
        public CombatItem(int time, long src_agent, long dst_agent, int value, int buff_dmg, int overstack_value,
                int skill_id, int src_instid, int dst_instid, int src_master_instid, IFF iff, int buff, Result result,
                Activation is_activation,BuffRemove is_buffremove, int is_ninety, int is_fifty, int is_moving,
                StateChange is_statechange, int is_flanking)
        {
            this.time = time;
            this.src_agent = src_agent;
            this.dst_agent = dst_agent;
            this.value = value;
            this.buff_dmg = buff_dmg;
            this.overstack_value = overstack_value;
            this.skill_id = skill_id;
            this.src_instid = src_instid;
            this.dst_instid = dst_instid;
            this.src_master_instid = src_master_instid;
            this.iff = iff;
            this.is_buff = buff;
            this.result = result;
           this.is_activation = is_activation;
           this.is_buffremove = is_buffremove;
            this.is_ninety = is_ninety;
            this.is_fifty = is_fifty;
            this.is_moving = is_moving;
           this.is_statechange = is_statechange;
            this.is_flanking = is_flanking;
        }

        // Public Methods
        public String[] toStringArray()
        {
            String[] array = new String[20];
            array[0] = time.ToString();
            array[1] = string.Format("{0:X}", src_agent);//Long.toHexString(src_agent);
            array[2] = string.Format("{0:X}", dst_agent);//Long.toHexString(dst_agent);
            array[3] = value.ToString();
            array[4] = buff_dmg.ToString();
            array[5] = overstack_value.ToString();
            array[6] = skill_id.ToString();
            array[7] = src_instid.ToString();
            array[8] = dst_instid.ToString();
            array[9] = src_master_instid.ToString();
            array[10] = iff.getEnum();
            array[11] = is_buff.ToString();
            array[12] = result.getEnum(); ;
           array[13] = is_activation.getEnum();
           array[14] = is_buffremove.getEnum();
            array[15] = is_ninety.ToString();
            array[16] = is_fifty.ToString();
            array[17] = is_moving.ToString();
           array[18] = is_statechange.getEnum();
            array[19] = is_flanking.ToString();
            return array;
        }

        // Getters
        public int getTime()
        {
            return time;
        }

        public long getSrcAgent()
        {
            return src_agent;
        }

        public long getDstAgent()
        {
            return dst_agent;
        }

        public int getValue()
        {
            return value;
        }

        public int getBuffDmg()
        {
            return buff_dmg;
        }

        public int getOverstackValue()
        {
            return overstack_value;
        }

        public int getSkillID()
        {
            return skill_id;
        }

        public int getSrcInstid()
        {
            return src_instid;
        }

        public int getDstInstid()
        {
            return dst_instid;
        }

        public int getSrcMasterInstid()
        {
            return src_master_instid;
        }

        public IFF getIFF()
        {
            return iff;
        }

        public int isBuff()
        {
            return is_buff;
        }

        public Result getResult()
        {
            return result;
        }

        public Activation isActivation()
        {
            return is_activation;
        }

        public BuffRemove isBuffremove()
        {
            return is_buffremove;
        }

        public int isNinety()
        {
            return is_ninety;
        }

        public int isFifty()
        {
            return is_fifty;
        }

        public int isMoving()
        {
            return is_moving;
        }

        public int isFlanking()
        {
            return is_flanking;
        }

        public StateChange isStateChange()
        {
            return is_statechange;
        }

        // Setters
        public void setSrcAgent(long src_agent)
        {
            this.src_agent = src_agent;
        }

        public void setDstAgent(long dst_agent)
        {
            this.dst_agent = dst_agent;
        }

        public void setSrcInstid(int src_instid)
        {
            this.src_instid = src_instid;
        }

        public void setDstInstid(int dst_instid)
        {
            this.dst_instid = dst_instid;
        }
    }
}