namespace LuckParser.Models.ParseEnums
{


    public class IFF
    {
        // Constants
    //FRIEND(0),

    //FOE(1),

    //UNKNOWN(2);

        // Fields
        private int ID;

        // Constructors
        public IFF(int ID)
        {
            this.ID = ID;
        }

        // Public Methods
        public string getEnum()
        {
            switch (ID) {
                case 0:
                    return "FRIEND";
                case 1:
                    return "FOE";
                case 2:
                    return "UNKNOWN";
                case 3:
                    return "BROKE";
            }
            return "NOID";

        }

        // Getters
        public int getID()
        {
            return ID;
        }
    }
}