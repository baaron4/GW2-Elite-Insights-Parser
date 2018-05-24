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
        private long time;
        private long src_agent;
        private long dst_agent;
        private int value;
        private int buff_dmg;
        private ushort overstack_value;
        private int skill_id;
        private ushort src_instid;
        private ushort dst_instid;
        private ushort src_master_instid;
        private IFF iff;
        private ushort is_buff;
        private Result result;
        private Activation is_activation;
        private BuffRemove is_buffremove;
        private ushort is_ninety;
        private ushort is_fifty;
        private ushort is_moving;
        private StateChange is_statechange;
        private ushort is_flanking;
        private ushort is_shields;
        // Constructor
        public CombatItem(long time, long src_agent, long dst_agent, int value, int buff_dmg, ushort overstack_value,
                int skill_id, ushort src_instid, ushort dst_instid, ushort src_master_instid, IFF iff, ushort buff, Result result,
                Activation is_activation,BuffRemove is_buffremove, ushort is_ninety, ushort is_fifty, ushort is_moving,
                StateChange is_statechange, ushort is_flanking)
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
        public CombatItem(long time, long src_agent, long dst_agent, int value, int buff_dmg, ushort overstack_value,
               int skill_id, ushort src_instid, ushort dst_instid, ushort src_master_instid, IFF iff, ushort buff, Result result,
               Activation is_activation, BuffRemove is_buffremove, ushort is_ninety, ushort is_fifty, ushort is_moving,
               StateChange is_statechange, ushort is_flanking, ushort is_shields)
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
            this.is_shields = is_shields;
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
            array[20] = is_shields.ToString();
            return array;
        }

        // Getters
        public long getTime()
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

        public ushort getOverstackValue()
        {
            return overstack_value;
        }

        public int getSkillID()
        {
            return skill_id;
        }

        public ushort getSrcInstid()
        {
            return src_instid;
        }

        public ushort getDstInstid()
        {
            return dst_instid;
        }

        public ushort getSrcMasterInstid()
        {
            return src_master_instid;
        }

        public IFF getIFF()
        {
            return iff;
        }

        public ushort isBuff()
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

        public ushort isNinety()
        {
            return is_ninety;
        }

        public ushort isFifty()
        {
            return is_fifty;
        }

        public ushort isMoving()
        {
            return is_moving;
        }

        public ushort isFlanking()
        {
            return is_flanking;
        }

        public StateChange isStateChange()
        {
            return is_statechange;
        }
        public ushort isShields() {
            return is_shields;
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

        public void setSrcInstid(ushort src_instid)
        {
            this.src_instid = src_instid;
        }

        public void setDstInstid(ushort dst_instid)
        {
            this.dst_instid = dst_instid;
        }
    }
}