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
        private int time;
        private int damage;
        private int skill_id;
        private int buff;
        private Result result;
        private int is_ninety;
        private int is_moving;
        private int is_flanking;
        private Activation is_activation;
        

        // Constructor
        public DamageLog(int time, int damage, int skill_id, int buff, Result result, int is_ninety, int is_moving,
                int is_flanking,Activation is_activation)
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

        // Getters
        public int getTime()
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

        public int isNinety()
        {
            return is_ninety;
        }

        public int isMoving()
        {
            return is_moving;
        }

        public int isFlanking()
        {
            return is_flanking;
        }
        public Activation isActivation()
        {
            return is_activation;
        }
    }
}