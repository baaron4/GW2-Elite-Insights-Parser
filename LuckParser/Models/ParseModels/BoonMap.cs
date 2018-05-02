using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class BoonMap
    {
        // Fields
        private String name;
        private int boonID;
        private List<BoonLog> b_log;
        
        // Constructors
        public BoonMap(String name,int boonID, List<BoonLog> b_log)
        {
            this.name = name;
            this.boonID = boonID;
            this.b_log = b_log;
        }

        // Public Methods
       

        // Getters
        public String getName()
        {
            return name;
        }
        public int getID() {
            return boonID;
        }

        public List<BoonLog> getBoonLog()
        {
            return b_log;
        }



        // Setters

        public void setBoonLog(List<BoonLog> bloglist)
        {
            this.b_log = bloglist;
        }

    }

}