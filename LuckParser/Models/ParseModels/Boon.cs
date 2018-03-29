using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LuckParser.Models.ParseModels
{
    public class Boon
    {
        // Boon
        //MIGHT("Might", "MGHT", "intensity", 25),

        //QUICKNESS("Quickness", "QCKN", "duration", 5),

        //FURY("Fury", "FURY", "duration", 9),

        //PROTECTION("Protection", "PROT", "duration", 5),

        //// Mesmer (also Ventari Revenant o.o)
        //ALACRITY("Alacrity", "ALAC", "duration", 9),

        //// Ranger
        //SPOTTER("Spotter", "SPOT", "duration", 1),

        //SPIRIT_OF_FROST("Spirit of Frost", "FRST", "duration", 1),

        //SUN_SPIRIT("Sun Spirit", "SUNS", "duration", 1),

        //STONE_SPIRIT("Stone Spirit", "STNE", "duration", 1),

        //STORM_SPIRIT("Storm Spirit", "STRM", "duration", 1),

        //GLYPH_OF_EMPOWERMENT("Glyph of Empowerment", "GOFE", "duration", 1),

        //GRACE_OF_THE_LAND("Grace of the Land", "GOTL", "intensity", 5),

        //// Warrior
        //EMPOWER_ALLIES("Empower Allies", "EALL", "duration", 1),

        //BANNER_OF_STRENGTH("Banner of Strength", "STRB", "duration", 1),

        //BANNER_OF_DISCIPLINE("Banner of Discipline", "DISC", "duration", 1),

        //BANNER_OF_TACTICS("Banner of Tactics", "TACT", "duration", 1),

        //BANNER_OF_DEFENCE("Banner of Defence", "DEFN", "duration", 1),

        //// Revenant
        //ASSASSINS_PRESENCE("Assassin's Presence", "ASNP", "duration", 1),

        //NATURALISTIC_RESONANCE("Naturalistic Resonance", "NATR", "duration", 1),

        //// Engineer
        //PINPOINT_PRECISION("Pinpoint Distribution", "PIND", "duration", 1),

        //// Elementalist
        //SOOTHING_MIST("Soothing Mist", "MIST", "duration", 1),

        //// Necro
        //VAMPIRIC_PRESENCE("Vampiric Presence", "VAMP", "duration", 1),

        //// Thief
        //LEAD_ATTACKS("Lead Attacks", "LEAD", "intensity", 15),

        //LOTUS_TRAINING("Lotus Training", "LOTS", "duration", 1),

        //BOUNDING_DODGER("Bounding Dodger", "BDOG", "duration", 1),

        //// Equipment
        //MASTERFUL_CONCENTRATION("Masterful Concentration", "CONC", "duration", 1);
        // THORNS_RUNE("Thorns", "THRN", "intensity", 5);

        // Fields
        private String name;
        private String abrv;
        private String type;
        private int capacity;

        // Constructor
        private Boon(String name, String abrv, String type, int capacity)
        {
            this.name = name;
            this.abrv = abrv;
            this.type = type;
            this.capacity = capacity;
        }

        // Public Methods
        public static Boon getEnum(String name)
        {
            foreach (Boon b in getList())
            {
                if (b.getName() == name)
                {
                    return b;
                }
            }
            return null;
        }
        public static List<Boon> getList(){
            List<Boon> boonList = new List<Boon>();
            boonList.Add(new Boon("Might","MGHT","intensity",25));
            boonList.Add(new Boon("Fury", "FURY", "duration", 9));//or 3m and 30s
            boonList.Add(new Boon("Quickness", "QCKN", "duration", 5));
            boonList.Add(new Boon("Alacrity", "ALAC", "duration", 9));

            boonList.Add(new Boon("Protection", "PROT", "duration", 5));
            boonList.Add(new Boon("Regeneration", "REGEN", "duration", 5));
            boonList.Add(new Boon("Vigor", "VIGR", "duration", 5));
            boonList.Add(new Boon("743", "AEG", "duration", 9));
            boonList.Add(new Boon("Stability", "STAB", "duration", 9));
            boonList.Add(new Boon("Swiftness", "SWFT", "duration", 9));
            boonList.Add(new Boon("Retaliation", "RETAL", "duration", 9));
            boonList.Add(new Boon("Resistance", "RES", "duration", 5));
           
            boonList.Add(new Boon("Spotter", "SPOT", "duration", 1));
            boonList.Add(new Boon("Spirit of Frost", "FRST", "duration", 1));
            boonList.Add(new Boon("Sun Spirit", "SUNS", "duration", 1));
            boonList.Add(new Boon("Stone Spirit", "STNE", "duration", 1));
            boonList.Add(new Boon("Storm Spirit", "STRM", "duration", 1));
            boonList.Add(new Boon("Glyph of Empowerment", "GOFE", "duration", 1));
            boonList.Add(new Boon("Grace of the Land", "GOTL", "intensity", 5));
            boonList.Add(new Boon("Empower Allies", "EALL", "duration", 1));
            boonList.Add(new Boon("Banner of Strength", "STRB", "duration", 1));
            boonList.Add(new Boon("Banner of Discipline", "DISC", "duration", 1));
            //boonList.Add(new Boon("Banner of Tactics", "TACT", "duration", 1));
            //boonList.Add(new Boon("Banner of Defence", "DEFN", "duration", 1));
            //boonList.Add(new Boon("Assassin's Presence", "ASNP", "duration", 1));
            //boonList.Add(new Boon("Naturalistic Resonance", "NATR", "duration", 1));
            //boonList.Add(new Boon("Pinpoint Distribution", "PIND", "duration", 1));
            //boonList.Add(new Boon("Soothing Mist", "MIST", "duration", 1));
            //boonList.Add(new Boon("Vampiric Presence", "VAMP", "duration", 1));
            //boonList.Add(new Boon("Lead Attacks", "LEAD", "intensity", 15));
            //boonList.Add(new Boon("Lotus Training", "LOTS", "duration", 1));
            //boonList.Add(new Boon("Bounding Dodger", "BDOG", "duration", 1));
            //boonList.Add(new Boon("Masterful Concentration", "CONC", "duration", 1));


            return boonList;
        }

        //public static String[] getArray()
        //{
        //    List<String> boonList = new List<String>();
        //    foreach (Boon b in values())
        //    {
        //        boonList.add(b.getAbrv());
        //    }
        //    return boonList.toArray(new String[boonList.size()]);
        //}

        //public static List<String> getList()
        //{
        //    List<String> boonList = new ArrayList<String>();
        //    for (Boon b : values())
        //    {
        //        boonList.add(b.getName());
        //    }
        //    return boonList;
        //}

        // Getters
        public String getName()
        {
            return this.name;
        }

        public String getAbrv()
        {
            return this.abrv;
        }

        public String getType()
        {
            return this.type;
        }

        public int getCapacity()
        {
            return this.capacity;
        }
    }
}