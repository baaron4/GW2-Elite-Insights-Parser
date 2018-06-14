using System;

namespace LuckParser.Models.ParseEnums
{
    public class Result
    {

    // Constants
    //NORMAL(0),
    //CRIT(1),
    //GLANCE(2),
    //BLOCK(3),
    //EVADE(4),
    //INTERRUPT(5),
    //ABSORB(6),
    //BLIND(7),
    //KILLING_BLOW(8);

        // Fields
        private int ID;

        // Constructors
        public Result(int ID)
        {
            this.ID = ID;
        }

        // Public Methods
        public String getEnum()
        {
            switch (ID) {
                case 0:
                    return "NORMAL";
                case 1:
                    return "CRIT";
                case 2:
                    return "GLANCE";
                case 3:
                    return "BLOCK";
                case 4:
                    return "EVADE";
                case 5:
                    return "INTERRUPT";
                case 6:
                    return "ABSORB";
                case 7:
                    return "BLIND";
                case 8:
                    return "KILLING_BLOW";
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