using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class SkillItem
    {
        // Fields
        private int ID;
        private String name;

        // Constructor
        public SkillItem(int ID, String name)
        {
            this.ID = ID;
            this.name = name;
        }

        // Public Methods
        public String[] toStringArray()
        {
            String[] array = new String[2];
            array[0] = ID.ToString();
            array[1] = name.ToString();
            return array;
        }

        // Getters
        public int getID()
        {
            return ID;
        }

        public String getName()
        {
            return name;
        }
    }
}