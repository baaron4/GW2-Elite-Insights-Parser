using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LuckParser.Models.ParseEnums;

namespace LuckParser.Models.ParseModels
{
    public class DamageLog
    {
        // Fields
        private long time;
        private int damage;
        private int skill_id;
        private int buff;
        private Result result;
        private ushort is_ninety;
        private ushort is_moving;
        private ushort is_flanking;
        private Activation is_activation;
        private ushort is_shields;
        private long src_agent;
        private ushort src_instid;

        // Constructor
        public DamageLog(long time, int damage, int skill_id, int buff, Result result, ushort is_ninety, ushort is_moving,
                ushort is_flanking, Activation is_activation)
        {
            this.time = time;
            this.damage = damage;
            this.skill_id = skill_id;
            this.buff = buff;
            this.result = result;
            this.is_ninety = is_ninety;
            this.is_moving = is_moving;
            this.is_flanking = is_flanking;
            this.is_activation = is_activation;
        }
        public DamageLog(long time, long srcagent, ushort instid, int damage, int skill_id, int buff, Result result, ushort is_ninety, ushort is_moving,
               ushort is_flanking, Activation is_activation)
        {
            this.time = time;
            this.damage = damage;
            this.skill_id = skill_id;
            this.buff = buff;
            this.result = result;
            this.is_ninety = is_ninety;
            this.is_moving = is_moving;
            this.is_flanking = is_flanking;
            this.is_activation = is_activation;
            this.src_agent = srcagent;
            this.src_instid = instid;

        }
        public DamageLog(long time, int damage, int skill_id, int buff, Result result, ushort is_ninety, ushort is_moving,
               ushort is_flanking, Activation is_activation, ushort is_shields)
        {
            this.time = time;
            this.damage = damage;
            this.skill_id = skill_id;
            this.buff = buff;
            this.result = result;
            this.is_ninety = is_ninety;
            this.is_moving = is_moving;
            this.is_flanking = is_flanking;
            this.is_activation = is_activation;
            this.is_shields = is_shields;
        }
        public DamageLog(long time, long srcagent, ushort instid, int damage, int skill_id, int buff, Result result, ushort is_ninety, ushort is_moving,
              ushort is_flanking, Activation is_activation, ushort is_shields)
        {
            this.time = time;
            this.damage = damage;
            this.skill_id = skill_id;
            this.buff = buff;
            this.result = result;
            this.is_ninety = is_ninety;
            this.is_moving = is_moving;
            this.is_flanking = is_flanking;
            this.is_activation = is_activation;
            this.src_agent = srcagent;
            this.src_instid = instid;
            this.is_shields = is_shields;

        }

        public DamageLog(long time, CombatItem c)
        {
            this.time = time;
            this.damage = c.getValue();
            this.skill_id = c.getSkillID();
            this.buff = c.isBuff();
            this.result = c.getResult();
            this.is_ninety = c.isNinety();
            this.is_moving = c.isMoving();
            this.is_flanking = c.isMoving();
            this.is_activation = c.isActivation();
            this.src_agent = c.getSrcAgent();
            this.src_instid = c.getSrcInstid();
            this.is_shields = c.isShields();

        }
        // Getters
        public long getTime()
        {
            return time;
        }

        public int getDamage()
        {
            return damage;
        }

        public int getID()
        {
            return skill_id;
        }

        public int isCondi()
        {
            return buff;
        }

        public Result getResult()
        {
            return result;
        }

        public ushort isNinety()
        {
            return is_ninety;
        }

        public ushort isMoving()
        {
            return is_moving;
        }

        public ushort isFlanking()
        {
            return is_flanking;
        }
        public Activation isActivation()
        {
            return is_activation;
        }
        public ushort isShields() {
            return is_shields;
        }
        public long getSrcAgent()
        {
            return src_agent;
        }
        public ushort getInstidt()
        {
            return src_instid;
        }
    }
}