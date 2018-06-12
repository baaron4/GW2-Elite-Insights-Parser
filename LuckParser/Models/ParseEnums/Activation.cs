namespace LuckParser.Models.ParseEnums
{
    public class Activation
    {
        // Constants
    //NONE(0),

    //NORMAL(1),

    //QUICKNESS(2),

    //CANCEL_FIRE(3),

    //CANCEL_CANCEL(4),

    //RESET(5);

        // Fields
        private int ID;

        // Constructors
        public Activation(int ID)
        {
            this.ID = ID;
        }

        // Public Methods
        public string getEnum()
        {
            switch (ID)
            {
                case 0:
                    return "NONE";
                case 1:
                    return "NORMAL";
                case 2:
                    return "QUICKNESS";
                case 3:
                    return "CANCEL_FIRE";
                case 4:
                    return "CANCEL_CANCEL";
                case 5:
                    return "RESET";
                
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