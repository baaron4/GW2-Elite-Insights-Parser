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

        // Constructor abrv does not matter unused
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
           
           
        
            boonList.Add(new Boon("Empower Allies", "EALL", "duration", 1));
            boonList.Add(new Boon("Banner of Strength", "STRB", "duration", 1));
            boonList.Add(new Boon("Banner of Discipline", "DISC", "duration", 1));
            
            return boonList;
        }

        public static List<Boon> getMainList()
        {
            List<Boon> boonList = new List<Boon>();
            boonList.Add(new Boon("Might", "MGHT", "intensity", 25));
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

            return boonList;
        }
        public static List<Boon> getSharableProfList()
        {
            List<Boon> boonList = new List<Boon>();
            boonList.Add(new Boon("Spotter","SPTR","duration",1));
            boonList.Add(new Boon("Spirit of Frost", "SPTF", "duration", 1));
            boonList.Add(new Boon("Sun Spirit", "SPTS", "duration", 1));
            boonList.Add(new Boon("Stone Spirit", "SPTST", "duration", 1));
            boonList.Add(new Boon("Empower Allies", "EA", "duration", 1));
            boonList.Add(new Boon("Banner of Strength", "BNRS", "duration", 1));
            boonList.Add(new Boon("Banner of Discipline", "BNRD", "duration", 1));
            boonList.Add(new Boon("Assassin's Presence", "ASNP", "duration", 1));
            boonList.Add(new Boon("Naturalistic Resonance", "NATR", "duration", 1));
            boonList.Add(new Boon("Pinpoint Distribution", "PIND", "duration", 1));
            boonList.Add(new Boon("Increased Condition Damage", "PIND", "duration", 1));
            boonList.Add(new Boon("Soothing Mist", "MIST", "duration", 1));
            boonList.Add(new Boon("Vampiric Aura", "VAMP", "duration", 1));
            return boonList;
        }
        public static List<Boon> getAllProfList()
        {
            List<Boon> boonList = new List<Boon>();
            boonList.Add(new Boon("Stealth", "STLT", "duration", 1));//13017
            boonList.Add(new Boon("Revealed", "STLT", "duration", 1));//13017
            boonList.Add(new Boon("Superspeed", "SUSP", "duration", 1));//5974
            boonList.Add(new Boon("Invulnerability", "INVN", "duration", 1));
            boonList.Add(new Boon("Unblockable", "INVN", "duration", 1));
            //Auras
            boonList.Add(new Boon("Chaos Armor", "CHAM", "duration", 1));
            boonList.Add(new Boon("Fire Shield", "FRSHD", "duration", 1));//5677
            boonList.Add(new Boon("Frost Aura", "FRAU", "duration", 1));//5579
            boonList.Add(new Boon("Light Aura", "LTAU", "duration", 1));
            boonList.Add(new Boon("Magnetic Aura", "MGAU", "duration", 1));//5684
            boonList.Add(new Boon("Shocking Aura", "SKAU", "duration", 1));//5577

            //Race sepecfic
            boonList.Add(new Boon("Take Root", "SPTR", "duration", 1));
            boonList.Add(new Boon("Become the Bear", "SPTR", "duration", 1));
            boonList.Add(new Boon("Become the Raven", "SPTR", "duration", 1));
            boonList.Add(new Boon("Become the Snow Leopard", "SPTR", "duration", 1));
            boonList.Add(new Boon("Become the Wolf", "SPTR", "duration", 1));
            boonList.Add(new Boon("Avatar of Melandru", "SPTR", "duration", 1));//12368
            boonList.Add(new Boon("Power Suit", "SPTR", "duration", 1));
            boonList.Add(new Boon("Reaper of Grenth", "SPTR", "duration", 1));//12366
            boonList.Add(new Boon("Charrooka", "PIND", "duration", 1));

            //Profession specefic effects
            //revenant
            //skills
            boonList.Add(new Boon("Crystal Hibernation", "BNRD", "duration", 1));
            boonList.Add(new Boon("Vengeful Hammers", "BNRD", "duration", 1));
            boonList.Add(new Boon("Rite of the Great Dwarf", "BNRD", "duration", 1));
            boonList.Add(new Boon("Embrace the Darkness", "BNRD", "duration", 1));
            boonList.Add(new Boon("Enchanted Daggers", "BNRD", "intensity", 6));
            boonList.Add(new Boon("Impossible Odds", "BNRD", "duration", 1));
            //signets
            boonList.Add(new Boon("Facet of Light", "BNRD", "duration", 1));
            boonList.Add(new Boon("Infuse Light", "BNRD", "duration", 1));
            boonList.Add(new Boon("Facet of Darkness", "BNRD", "duration", 1));
            boonList.Add(new Boon("Facet of Elements", "BNRD", "duration", 1));
            boonList.Add(new Boon("Facet of Strength", "BNRD", "duration", 1));
            boonList.Add(new Boon("Facet of Chaos", "BNRD", "duration", 1));
            boonList.Add(new Boon("Facet of Nature", "BNRD", "duration", 1));
            boonList.Add(new Boon("Naturalistic Resonance", "BNRD", "duration", 1));
            //attunments
            boonList.Add(new Boon("Legendary Centaur Stance", "BNRD", "duration", 1));
            boonList.Add(new Boon("Legendary Dragon Stance", "BNRD", "duration", 1));
            boonList.Add(new Boon("Legendary Dwarf Stance", "BNRD", "duration", 1));
            boonList.Add(new Boon("Legendary Demon Stance", "BNRD", "duration", 1));
            boonList.Add(new Boon("Legendary Assassin Stance", "BNRD", "duration", 1));
            boonList.Add(new Boon("Legendary Renegade Stance", "BNRD", "duration", 1));
            //summons
            boonList.Add(new Boon("Breakrazor's Bastion", "BNRD", "duration", 1));
            boonList.Add(new Boon("Razorclaw's Rage", "BNRD", "duration", 1));
            boonList.Add(new Boon("Soulcleave's Summit", "BNRD", "duration", 1));
            //traits
            boonList.Add(new Boon("Vicious Lacerations", "BNRD", "intensity", 5));
            boonList.Add(new Boon("Assassin's Presence", "ASNP", "duration", 1));
            boonList.Add(new Boon("Expose Defenses", "ASNP", "duration", 1));
            boonList.Add(new Boon("Invoking Harmony", "ASNP", "duration", 1));
            boonList.Add(new Boon("Selfless Amplification", "ASNP", "duration", 1));
            boonList.Add(new Boon("Hardening Persistence", "ASNP", "intensity", 8));
            boonList.Add(new Boon("Soothing Bastion", "ASNP", "duration", 1));
            boonList.Add(new Boon("Kalla's Fervor", "ASNP", "duration", 5));

            //warrior
            //skills
            boonList.Add(new Boon("Riposte", "BNRD", "duration", 1));
            boonList.Add(new Boon("Counterattack", "BNRD", "duration", 1));
            boonList.Add(new Boon("Flames of War", "BNRD", "duration", 1));
            boonList.Add(new Boon("Blood Reckoning", "BNRD", "duration", 1));
            boonList.Add(new Boon("Rock Guard", "BNRD", "duration", 1));
            boonList.Add(new Boon("Sight beyond Sight", "BNRD", "duration", 1));

            //signets
            boonList.Add(new Boon("Healing Signet", "SPTR", "duration", 1));
            boonList.Add(new Boon("Dolyak Signet", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Fury", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Might", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Stamina", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Rage", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Ferocity", "SPTR", "duration", 1));
            //summons
            boonList.Add(new Boon("Banner of Strength", "BNRS", "duration", 1));
            boonList.Add(new Boon("Banner of Discipline", "BNRD", "duration", 1));
            boonList.Add(new Boon("Banner of Tactics", "BNRS", "duration", 1));
            boonList.Add(new Boon("Banner of Defense", "BNRD", "duration", 1));
            //stancces
            boonList.Add(new Boon("Shield Stance", "BNRD", "duration", 1));
            boonList.Add(new Boon("Berserker's Stance", "BNRD", "duration", 1));
            boonList.Add(new Boon("Enduring Pain", "BNRD", "duration", 1));
            boonList.Add(new Boon("Balanced Stance", "BNRD", "duration", 1));
            boonList.Add(new Boon("Defiant Stance", "BNRD", "duration", 1));
            //traits
            boonList.Add(new Boon("Empower Allies", "EA", "duration", 1));
            boonList.Add(new Boon("Peak Performance", "BNRD", "duration", 1));
            boonList.Add(new Boon("Furious Surge", "SPTR", "intensity", 25));
            boonList.Add(new Boon("Health Gain per Adrenaline bar Spent", "SPTR", "intensity", 3));
            boonList.Add(new Boon("Rousing Resilience", "BNRD", "duration", 1));
            boonList.Add(new Boon("Always Angry", "BNRD", "duration", 1));
            boonList.Add(new Boon("Full Counter", "BNRD", "duration", 1));
            boonList.Add(new Boon("Attacker's Insight", "BNRD", "intensity", 5));

            //guardian
            //skills
            boonList.Add(new Boon("Zealot's Flame", "PIND", "duration", 1));
            boonList.Add(new Boon("Purging Flames", "PIND", "duration", 1));
            boonList.Add(new Boon("Litany of Wrath", "PIND", "duration", 1));
            boonList.Add(new Boon("Renewed Focus", "PIND", "duration", 1));
            //signets
            boonList.Add(new Boon("Signet of Resolve", "SORV", "duration", 1));
            boonList.Add(new Boon("Bane Signet", "BNSG", "duration", 1));
            boonList.Add(new Boon("Signet of Judgment", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Mercy", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Wrath", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Courage", "SPTR", "duration", 1));
            //traits
            boonList.Add(new Boon("Virute of Justice", "PIND", "duration", 1));
            boonList.Add(new Boon("Justice", "PIND", "duration", 1));
            boonList.Add(new Boon("Virute of Courage", "PIND", "duration", 1));
            boonList.Add(new Boon("Shield of Courage", "PIND", "duration", 1));
            boonList.Add(new Boon("Virute of Resolve", "PIND", "duration", 1));
            boonList.Add(new Boon("Strength in Numbers", "PIND", "duration", 1));
            boonList.Add(new Boon("Invigorated Bulwark", "PIND", "intensity", 5));
            boonList.Add(new Boon("Force of Will", "PIND", "duration", 1));//not sure if intensity
            boonList.Add(new Boon("Tome of Justice", "PIND", "duration", 1));
            boonList.Add(new Boon("Tome of Courage", "PIND", "duration", 1));
            boonList.Add(new Boon("Tome of Resolve", "PIND", "duration", 1));
            boonList.Add(new Boon("Ashes of the Just", "PIND", "intensity", 25));
            boonList.Add(new Boon("Eternal Oasis", "PIND", "duration", 1));
            boonList.Add(new Boon("Unbroken Lines", "PIND", "duration", 1));
            boonList.Add(new Boon("Quickfire", "PIND", "duration", 1));

            //Engie
            //skills
            boonList.Add(new Boon("Static Shield", "SPTR", "duration", 1));
            boonList.Add(new Boon("Absorb", "SPTR", "duration", 1));
            boonList.Add(new Boon("A.E.D.", "SPTR", "duration", 1));
            boonList.Add(new Boon("Elixir S", "SPTR", "duration", 1));
            boonList.Add(new Boon("Elixir X", "SPTR", "duration", 1));
            boonList.Add(new Boon("Utility Goggles", "SPTR", "duration", 1));
            boonList.Add(new Boon("Slick Shoes", "SPTR", "duration", 1));
            boonList.Add(new Boon("Watchful Eye", "SPTR", "duration", 1));
            boonList.Add(new Boon("Cooling Vapor", "SPTR", "duration", 1));
            boonList.Add(new Boon("Photon Wall Deployed", "SPTR", "duration", 1));
            boonList.Add(new Boon("Spectrum Shield", "SPTR", "duration", 1));
            boonList.Add(new Boon("Gear Shield", "SPTR", "duration", 1));
            //Transforms
            boonList.Add(new Boon("Rampage", "SPTR", "duration", 1));
            boonList.Add(new Boon("Photon Forge", "SPTR", "duration", 1));
            //Traits
            boonList.Add(new Boon("Laser's Edge", "SPTR", "duration", 1));
            boonList.Add(new Boon("Afterburner", "SPTR", "intensity", 5));
            boonList.Add(new Boon("Iron Blooded", "SPTR", "intensity", 25));
            boonList.Add(new Boon("Streamlined Kits", "SPTR", "duration", 1));
            boonList.Add(new Boon("Kinetic Charge", "SPTR", "intensity", 5));
            boonList.Add(new Boon("Pinpoint Distribution", "PIND", "duration", 1));
            boonList.Add(new Boon("Increased Condition Damage", "PIND", "duration", 1));

            //Ranger
            boonList.Add(new Boon("Celestial Avatar", "SPTR", "duration", 1));
            //signets
            boonList.Add(new Boon("Signet of Renewal", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Stone", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of the Hunt", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of the Wild", "SPTR", "duration", 1));
            //Summons
            //Need reloook
            boonList.Add(new Boon("Spirit of Frost", "SPTF", "duration", 1));
            boonList.Add(new Boon("Sun Spirit", "SPTS", "duration", 1));
            boonList.Add(new Boon("Stone Spirit", "SPTST", "duration", 1));
            boonList.Add(new Boon("Storm Spirit", "SPTST", "duration", 1));
            //skills
            boonList.Add(new Boon("Attack of Opportunity", "SPTR", "duration", 1));
            boonList.Add(new Boon("Call of the Wild", "SPTR", "duration", 1));
            boonList.Add(new Boon("Strength of the pack!", "SPTR", "duration", 1));
            boonList.Add(new Boon("Sick 'Em!", "SPTR", "duration", 1));
            boonList.Add(new Boon("Sharpening Stones", "SPTR", "intenstiy", 10));
            boonList.Add(new Boon("Ancestral Grace", "SPTR", "duration", 1));
            boonList.Add(new Boon("Glyph of Empowerment", "SPTR", "duration", 1));
            boonList.Add(new Boon("Dolyak Stance", "SPTR", "duration", 1));
            boonList.Add(new Boon("Griffon Stance", "SPTR", "duration", 1));
            boonList.Add(new Boon("Moa Stance", "SPTR", "duration", 1));
            boonList.Add(new Boon("Vulture Stance", "SPTR", "duration", 1));
            boonList.Add(new Boon("Bear Stance", "SPTR", "duration", 1));
            boonList.Add(new Boon("One Wolf Pack", "SPTR", "duration", 1));
            boonList.Add(new Boon("Sharpen Spines", "SPTR", "intensity", 5));
            //traits
            boonList.Add(new Boon("Spotter", "SPTR", "duration", 1));
            boonList.Add(new Boon("Opening Strike", "SPTR", "duration", 1));
            boonList.Add(new Boon("Quick Draw", "SPTR", "duration", 1));
            boonList.Add(new Boon("On Dodge", "SPTR", "duration", 1));
            boonList.Add(new Boon("Natural Mender", "SPTR", "duration", 1));
            boonList.Add(new Boon("Lingering Light", "SPTR", "duration", 1));
            boonList.Add(new Boon("Deadly", "SPTR", "duration", 1));
            boonList.Add(new Boon("Ferocious", "SPTR", "duration", 1));
            boonList.Add(new Boon("Supportive", "SPTR", "duration", 1));
            boonList.Add(new Boon("Versatile", "SPTR", "duration", 1));
            boonList.Add(new Boon("Stout", "SPTR", "duration", 1));
            boonList.Add(new Boon("Unstoppable Union", "SPTR", "duration", 1));
            boonList.Add(new Boon("Twice as Vicious", "SPTR", "duration", 1));

            //thief
            boonList.Add(new Boon("Deadeyes's Gaze", "SPTR", "duration", 1));
            //signets
            boonList.Add(new Boon("Signet of Malice", "SPTR", "duration", 1));
            boonList.Add(new Boon("Assassin's Signet", "SPTR", "duration", 1));//13047 44597
            boonList.Add(new Boon("Infiltrator's Signet", "SPTR", "duration", 1));//13063
            boonList.Add(new Boon("Signet of Agility", "SPTR", "duration", 1));//13061
            boonList.Add(new Boon("Signet of Shadows", "SPTR", "duration", 1));//13059
            //poisins
            boonList.Add(new Boon("Ice Drake Venom", "SPTR", "intensity", 4));//13095
            boonList.Add(new Boon("Devourer Venom", "SPTR", "intensity", 2));//
            boonList.Add(new Boon("Skale Venom", "SPTR", "intensity", 4));//13054
            boonList.Add(new Boon("Spider Venom", "SPTR", "intensity", 6));//
            boonList.Add(new Boon("Basilisk Venom", "SPTR", "intensity", 6));//
                                                                             //Physical
            boonList.Add(new Boon("Palm Strike", "SPTR", "duration", 1));//
            boonList.Add(new Boon("Pulmonary Impact", "SPTR", "intensity", 2));//

            boonList.Add(new Boon("Infiltration", "SPTR", "duration", 1));//
            //Transforms
            boonList.Add(new Boon("Dagger Storm", "SPTR", "duration", 1));//
            boonList.Add(new Boon("Kneeling", "SPTR", "duration", 1));//
            //traits
            boonList.Add(new Boon("Maleficent Seven", "SPTR", "duration", 1));
            boonList.Add(new Boon("Hidden Killer", "SPTR", "duration", 1));//
            boonList.Add(new Boon("Lead Attacks", "SPTR", "intensity", 15));//
            boonList.Add(new Boon("Instant Reflexes", "SPTR", "duration", 1));//
            boonList.Add(new Boon("Lotus Training", "SPTR", "duration", 1));//
            boonList.Add(new Boon("Unhindered Combatant", "SPTR", "duration", 1));//
            boonList.Add(new Boon("Bounding Dodger", "SPTR", "duration", 1));//

            //mesmer 
            boonList.Add(new Boon("Distortion", "SPTR", "duration", 1));
            //signets
            boonList.Add(new Boon("Signet of the Ether", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Domination", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Illusions", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Inspiration", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Midnight", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Humility", "SPTR", "duration", 1));
            //skills
            boonList.Add(new Boon("Blur", "SPTR", "duration", 1));
            boonList.Add(new Boon("Mirror", "SPTR", "duration", 1));
            boonList.Add(new Boon("Echo", "SPTR", "duration", 1));
            boonList.Add(new Boon("Illusion of Life", "SPTR", "duration", 1));
            boonList.Add(new Boon("Time Echo", "SPTR", "duration", 1));
            //traits
            boonList.Add(new Boon("Fencer's Finesse", "SPTR", "intensity", 10));
            boonList.Add(new Boon("Illusionary Defense", "SPTR", "intensity", 5));
            boonList.Add(new Boon("Compunding Power", "SPTR", "intensity", 5));
            boonList.Add(new Boon("Phantasmal Force", "SPTR", "intensity", 25));
            boonList.Add(new Boon("Mirage Cloak", "SPTR", "duration", 1));

            //Necro
            //forms
            boonList.Add(new Boon("Lich Form", "SPTR", "duration", 1));
            boonList.Add(new Boon("Death Shroud", "SPTR", "duration", 1));
            boonList.Add(new Boon("Reaper's Shroud", "SPTR", "duration", 1));
            //Signets
            boonList.Add(new Boon("Signet of Vampirism", "SPTR", "duration", 1));
            boonList.Add(new Boon("Plague Signet", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Spite", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of the Locust", "SPTR", "duration", 1));
            boonList.Add(new Boon("Signet of Undeath", "SPTR", "duration", 1));
            //skills
            boonList.Add(new Boon("Spectral Walk", "SPTR", "duration", 1));
            //traits
            boonList.Add(new Boon("Corrupter's Defense", "SPTR", "intenstiy", 10));
            boonList.Add(new Boon("Vampiric Aura", "VAMP", "duration", 1));
            boonList.Add(new Boon("Last Rites", "VAMP", "duration", 1));
            boonList.Add(new Boon("Sadistic Searing", "VAMP", "duration", 1));

            
            //ele
            //signets
            boonList.Add(new Boon("Signet of Restoration", "SPTR", "duration", 1));//739
            boonList.Add(new Boon("Signet of Air", "SPTR", "duration", 1));//5590
            boonList.Add(new Boon("Signet of Earth", "SPTR", "duration", 1));//5592
            boonList.Add(new Boon("Signet of Fire", "SPTR", "duration", 1));//5544
            boonList.Add(new Boon("Signet of Water", "SPTR", "duration", 1));//5591
            //attunments
            boonList.Add(new Boon("Fire Attunement", "SPTR", "duration", 1));//5585
            boonList.Add(new Boon("Water Attunement", "SPTR", "duration", 1));
            boonList.Add(new Boon("Air Attunement", "SPTR", "duration", 1));//5575
            boonList.Add(new Boon("Earth Attunement", "SPTR", "duration", 1));//5580
            //forms
            boonList.Add(new Boon("Mist Form", "SPTR", "duration", 1));//5543
            boonList.Add(new Boon("Ride the Lightning", "SPTR", "duration", 1));//5588
            boonList.Add(new Boon("Vapor Form", "SPTR", "duration", 1));
            boonList.Add(new Boon("Tornado", "SPTR", "duration", 1));//5534
            boonList.Add(new Boon("Whirlpool", "SPTR", "duration", 1));
            //conjures
            boonList.Add(new Boon("Conjure Earth Attributes", "SPTR", "duration", 1));//15788
            boonList.Add(new Boon("Conjure Flame Attributes", "SPTR", "duration", 1));//15789
            boonList.Add(new Boon("Conjure Frost Attributes", "SPTR", "duration", 1));//15790
            boonList.Add(new Boon("Conjure Lightning Attributes", "SPTR", "duration", 1));//15791
            boonList.Add(new Boon("Conjure Fire Attributes", "SPTR", "duration", 1));//15792
                                                    //Extras
            boonList.Add(new Boon("Arcane Power", "SPTR", "duration", 1));//5582
            boonList.Add(new Boon("Arcane Shield", "SPTR", "duration", 1));//5640
            boonList.Add(new Boon("Renewal of Fire", "SPTR", "duration", 1));//5764
            boonList.Add(new Boon("Glyph of Elemental Power", "SPTR", "duration", 1));//5739 5741 5740 5742
            boonList.Add(new Boon("Rebound", "SPTR", "duration", 1));//31337
            boonList.Add(new Boon("Rock Barrier", "SPTR", "duration", 1));//34633 750
            boonList.Add(new Boon("Magnetic Wave", "SPTR", "duration", 1));//15794
            boonList.Add(new Boon("Obsidian Flesh", "SPTR", "duration", 1));//5667
            //Traits
            boonList.Add(new Boon("Harmonious Conduit", "SPTR", "duration", 1));//31353
            boonList.Add(new Boon("Fresh Air", "SPTR", "duration", 1));//31353
            boonList.Add(new Boon("Soothing Mist", "MIST", "duration", 1));
            boonList.Add(new Boon("Lesser Arcane Shield", "SPTR", "duration", 1));
            boonList.Add(new Boon("Weaver's Prowess", "SPTR", "duration", 1));
            boonList.Add(new Boon("Elements of Rage", "SPTR", "duration", 1));
            boonList.Add(new Boon("bleh", "SPTR", "duration", 1));
    
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