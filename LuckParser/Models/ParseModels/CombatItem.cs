using System;
using LuckParser.Models.ParseEnums;
using LuckParser.Models.DataModels;

namespace LuckParser.Models.ParseModels
{
    public class CombatItem
    {

        // Fields
        private long time;
        private ulong src_agent;
        private ulong dst_agent;
        private int value;
        private int buff_dmg;
        private ushort overstack_value;
        private int skill_id;
        private ushort src_instid;
        private ushort dst_instid;
        private ushort src_master_instid;
        private IFF iff;
        private ushort is_buff;
        private ParseEnum.Result result;
        private ParseEnum.Activation is_activation;
        private ParseEnum.BuffRemove is_buffremove;
        private ushort is_ninety;
        private ushort is_fifty;
        private ushort is_moving;
        private ParseEnum.StateChange is_statechange;
        private ushort is_flanking;
        private ushort is_shields;
        // Constructor
        public CombatItem(long time, ulong src_agent, ulong dst_agent, int value, int buff_dmg, ushort overstack_value,
                int skill_id, ushort src_instid, ushort dst_instid, ushort src_master_instid, IFF iff, ushort buff, ParseEnum.Result result,
                ParseEnum.Activation is_activation, ParseEnum.BuffRemove is_buffremove, ushort is_ninety, ushort is_fifty, ushort is_moving,
                ParseEnum.StateChange is_statechange, ushort is_flanking)
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
        public CombatItem(long time, ulong src_agent, ulong dst_agent, int value, int buff_dmg, ushort overstack_value,
               int skill_id, ushort src_instid, ushort dst_instid, ushort src_master_instid, IFF iff, ushort buff, ParseEnum.Result result,
               ParseEnum.Activation is_activation, ParseEnum.BuffRemove is_buffremove, ushort is_ninety, ushort is_fifty, ushort is_moving,
               ParseEnum.StateChange is_statechange, ushort is_flanking, ushort is_shields)
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

        // Getters
        public long getTime()
        {
            return time;
        }

        public ulong getSrcAgent()
        {
            return src_agent;
        }

        public ulong getDstAgent()
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

        public ParseEnum.Result getResult()
        {
            return result;
        }

        public ParseEnum.Activation isActivation()
        {
            return is_activation;
        }

        public ParseEnum.BuffRemove isBuffremove()
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

        public ParseEnum.StateChange isStateChange()
        {
            return is_statechange;
        }
        public ushort isShields() {
            return is_shields;
        }
        // Setters
        public void setSrcAgent(ulong src_agent)
        {
            this.src_agent = src_agent;
        }

        public void setDstAgent(ulong dst_agent)
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