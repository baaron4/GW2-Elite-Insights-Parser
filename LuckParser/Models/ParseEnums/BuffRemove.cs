using System;

namespace LuckParser.Models.ParseEnums
{
    public class BuffRemove
    {
    //    // Constants
    //NONE(0),

    //ALL(1),

    //SINGLE(2),

    //MANUAL(3);

        // Fields
        private int ID;

        // Constructors
        public BuffRemove(int ID)
        {
            this.ID = ID;
        }

        // Public Methods
        public String getEnum()
        {
            switch (ID)
            {
                case 0:
                    return "NONE";
                case 1:
                    return "ALL";
                case 2:
                    return "SINGLE";
                case 3:
                    return "MANUAL";

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