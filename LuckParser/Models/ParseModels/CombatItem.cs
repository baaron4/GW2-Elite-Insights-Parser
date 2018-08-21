using System;
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
        private uint overstack_value;
        private long skill_id;
        private ushort src_instid;
        private ushort dst_instid;
        private ushort src_master_instid;
        private ushort dst_master_instid;
        private ParseEnum.IFF iff;
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
        public CombatItem(long time, ulong src_agent, ulong dst_agent, int value, int buff_dmg, uint overstack_value,
               long skill_id, ushort src_instid, ushort dst_instid, ushort src_master_instid, ushort dst_master_instid, ParseEnum.IFF iff, ushort is_buff, ParseEnum.Result result,
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
            this.dst_master_instid = dst_master_instid;
            this.iff = iff;
            this.is_buff = is_buff;
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
        public long GetTime()
        {
            return time;
        }

        public ulong GetSrcAgent()
        {
            return src_agent;
        }

        public ulong GetDstAgent()
        {
            return dst_agent;
        }

        public int GetValue()
        {
            return value;
        }

        public int GetBuffDmg()
        {
            return buff_dmg;
        }

        public uint GetOverstackValue()
        {
            return overstack_value;
        }

        public long GetSkillID()
        {
            return skill_id;
        }

        public ushort GetSrcInstid()
        {
            return src_instid;
        }

        public ushort GetDstInstid()
        {
            return dst_instid;
        }

        public ushort GetSrcMasterInstid()
        {
            return src_master_instid;
        }

        public ushort GetDstMasterInstid()
        {
            return dst_master_instid;
        }

        public ParseEnum.IFF GetIFF()
        {
            return iff;
        }

        public ushort IsBuff()
        {
            return is_buff;
        }

        public ParseEnum.Result GetResult()
        {
            return result;
        }

        public ParseEnum.Activation IsActivation()
        {
            return is_activation;
        }

        public ParseEnum.BuffRemove IsBuffremove()
        {
            return is_buffremove;
        }

        public ushort IsNinety()
        {
            return is_ninety;
        }

        public ushort IsFifty()
        {
            return is_fifty;
        }

        public ushort IsMoving()
        {
            return is_moving;
        }

        public ushort IsFlanking()
        {
            return is_flanking;
        }

        public ParseEnum.StateChange IsStateChange()
        {
            return is_statechange;
        }
        public ushort IsShields()
        {
            return is_shields;
        }
        // Setters
        public void SetSrcAgent(ulong src_agent)
        {
            this.src_agent = src_agent;
        }

        public void SetDstAgent(ulong dst_agent)
        {
            this.dst_agent = dst_agent;
        }

        public void SetSrcInstid(ushort src_instid)
        {
            this.src_instid = src_instid;
        }

        public void SetDstInstid(ushort dst_instid)
        {
            this.dst_instid = dst_instid;
        }
    }
}