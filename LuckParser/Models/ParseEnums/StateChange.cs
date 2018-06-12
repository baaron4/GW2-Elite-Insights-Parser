using System;

namespace LuckParser.Models.ParseEnums
{
    public class StateChange
    {
        // Constants
    //NORMAL(0),

    //ENTER_COMBAT(1),

    //EXIT_COMBAT(2),

    //CHANGE_UP(3),

    //CHANGE_DEAD(4),

    //CHANGE_DOWN(5),

    //SPAWN(6),

    //DESPAWN(7),

    //HEALTH_UPDATE(8),

    //LOG_START(9),

    //LOG_END(10),

    //WEAPON_SWAP(11),

    //MAX_HEALTH_UPDATE(12),

    //POINT_OF_VIEW(13),

    //CBTS_LANGUAGE(14);

        // Fields
        private int ID;

        // Constructors
        public StateChange(int ID)
        {
            this.ID = ID;
        }

        // Public Methods
        public String getEnum()
        {
            switch (ID)
            {
                case 0:
                    return "NORMAL";
                case 1:
                    return "ENTER_COMBAT";
                case 2:
                    return "EXIT_COMBAT";
                case 3:
                    return "CHANGE_UP";
                case 4:
                    return "CHANGE_DEAD";
                case 5:
                    return "CHANGE_DOWN";
                case 6:
                    return "SPAWN";
                case 7:
                    return "DESPAWN";
                case 8:
                    return "HEALTH_UPDATE";
                case 9:
                    return "LOG_START";
                case 10:
                    return "LOG_END";
                case 11:
                    return "WEAPON_SWAP";
                case 12:
                    return "MAX_HEALTH_UPDATE";
                case 13:
                    return "POINT_OF_VIEW";
                case 14:
                    return "CBTS_LANGUAGE";
                case 15:
                    return "GWBUILD";
                case 16:
                    return "SHARDID";
                case 17:
                    return "REWARD";
                case 18:
                    return "BUFFINITIAL";
                default:
                    return "UNKNOWN";
            }
        }
        // Getters
        public int getID()
        {
            return ID;
        }
    }
}