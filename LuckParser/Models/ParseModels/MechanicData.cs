using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckParser.Models.ParseModels
{
    public class MechanicData
    {
        private List<MechanicLog> m_Data;
        private List<Mechanic> globalMechList = new List<Mechanic>();

        public MechanicData() {
            this.m_Data = new List<MechanicLog>();
        }

        public void AddItem(MechanicLog mLog) {
            this.m_Data.Add(mLog);
        }

        public List<MechanicLog> GetMDataLogs() {
            return this.m_Data;
        }
        public List<Mechanic> GetMechList(int bossid) {
            if (globalMechList.Count == 0) {
                //set list
               // globalMechList.Add(new Mechanic(0, "DOWN", 0, 0, "symbol:'circle',color:'rgb(0,0,255)',", "Down"));
               // globalMechList.Add(new Mechanic(0, "DEAD", 0, 0, "symbol:'circle',color:'rgb(0,0,255)',", "Dead"));
                //VG
                globalMechList.Add(new Mechanic(31860,"Unstable Magic Spike",3, 15438, "symbol:'circle',color:'rgb(0,0,255)',", "Green Teleport blue"));
                globalMechList.Add(new Mechanic(31392, "Unstable Magic Spike", 3, 15438, "symbol:'circle',color:'rgb(0,0,255)',", "Boss Teleport blue"));

                globalMechList.Add(new Mechanic(31340, "Distributed Magic", 3, 15438, "symbol:'circle',color:'rgb(0,128,0)',", "Green Team"));
                globalMechList.Add(new Mechanic(31391, "Distributed Magic", 3, 15438, "symbol:'circle',color:'rgb(0,128,0)',", "Green Team"));
                globalMechList.Add(new Mechanic(31529, "Distributed Magic", 3, 15438, "symbol:'circle',color:'rgb(0,128,0)',", "Green Team"));
                globalMechList.Add(new Mechanic(31750, "Distributed Magic", 3, 15438, "symbol:'circle',color:'rgb(0,128,0)',", "Green Team"));
                //green fail?

                globalMechList.Add(new Mechanic(31886, "Magic Pulse", 3, 15438, "symbol:'circle',color:'rgb(255,0,0)',", "Hit by Red"));

                globalMechList.Add(new Mechanic(0, "Pylon Attunement: Red", 0, 15438, "symbol:'square',color:'rgb(255,0,0)',", "Attune:Red"));
                globalMechList.Add(new Mechanic(31317, "Pylon Attunement: Blue", 0, 15438, "symbol:'square',color:'rgb(0,0,255)',", "Attune:Blue"));
                globalMechList.Add(new Mechanic(0, "Pylon Attunement: Green", 0, 15438, "symbol:'square',color:'rgb(0,128,0)',", "Attune:Green"));

                globalMechList.Add(new Mechanic(31413, "Blue Pylon Power", 4, 15438, "symbol:'square',color:'rgb(0,0,255)',", "Striped Blue Invuln"));

                globalMechList.Add(new Mechanic(31539, "Unstable Pylon", 3, 15438, "symbol:'hexagram',color:'rgb(255,0,0)',", "Floor Damage"));
                globalMechList.Add(new Mechanic(31828, "Unstable Pylon", 3, 15438, "symbol:'hexagram',color:'rgb(0,0,255)',", "Floor Damage"));
                globalMechList.Add(new Mechanic(31884, "Unstable Pylon", 3, 15438, "symbol:'hexagram',color:'rgb(0,128,0)',", "Floor Damage"));


                //gors
                globalMechList.Add(new Mechanic(31875, "Spectral Impact", 3, 15429, "symbol:'hexagram',color:'rgb(255,0,0)',", "Slam"));

                globalMechList.Add(new Mechanic(31623, "Ghastly Prison", 3, 15429, "symbol:'pentagon',color:'rgb(0,0,255)',", "Egg"));

                globalMechList.Add(new Mechanic(31498, "Spectral Darkness", 0, 15429, "symbol:'circle',color:'rgb(0,0,255)',", "Orb Debuff"));

                globalMechList.Add(new Mechanic(31722, "Spirited Fusion", 1, 15429, "symbol:'square',color:'rgb(0,0,255)',", "Ate Spirit"));
                //black dmg

                //sab
                globalMechList.Add(new Mechanic(34108, "Shell-Shocked", 0, 15375, "symbol:'circle',color:'rgb(255,0,0)',", "Canons"));
                globalMechList.Add(new Mechanic(31473, "Sapper Bomb", 0, 15375, "symbol:'circle',color:'rgb(0,128,0)',", "Green Bomb"));
                globalMechList.Add(new Mechanic(31485, "Time Bomb", 0, 15375, "symbol:'circle',color:'rgb(0,0,255)',", "Time Bomb"));//or 3? buff or hits
                globalMechList.Add(new Mechanic(31332, "Firestorm", 3, 15375, "symbol:'square',color:'rgb(255,0,0)',", "Flamewall"));
                globalMechList.Add(new Mechanic(31544, "Flak Shot", 3, 15375, "symbol:'hexagram',color:'rgb(255,0,0)',", "Flak"));

                //sloth
                globalMechList.Add(new Mechanic(34479, "Tantrum", 3, 16123, "symbol:'circle',color:'rgb(255,0,0)',", "Tantrum"));
                globalMechList.Add(new Mechanic(34387, "Volatile Poison", 0, 16123, "symbol:'circle',color:'rgb(0,128,0)',", "Poisin Buff"));
                globalMechList.Add(new Mechanic(34481, "Volatile Poison", 3, 16123, "symbol:'hexagram',color:'rgb(0,128,0)',", "Poisin AOE"));
                globalMechList.Add(new Mechanic(34516, "Halitosis", 3, 16123, "symbol:'hexagram',color:'rgb(255,0,0)',", "Flame Breathe"));
                globalMechList.Add(new Mechanic(34482, "Spore Release", 3, 16123, "symbol:'pentagon',color:'rgb(0,128,0)',", "Shake"));
                globalMechList.Add(new Mechanic(34362, "Magic Transformation", 0, 16123, "symbol:'square',color:'rgb(0,128,0)',", "Slub Transform"));
                globalMechList.Add(new Mechanic(34496, "Nauseated", 0, 16123, "symbol:'square',color:'rgb(0,0,255)',", "Slub CD"));
                globalMechList.Add(new Mechanic(34508, "Fixated", 0, 16123, "symbol:'star',color:'rgb(0,0,255)',", "Fixated"));

                //matt
                globalMechList.Add(new Mechanic(34380, "Oppressive Gaze", 3, 16115, "symbol:'hexagram',color:'rgb(0,0,255)',", "Hadouken"));//human
                globalMechList.Add(new Mechanic(34371, "Oppressive Gaze", 3, 16115, "symbol:'hexagram',color:'rgb(0,0,255)',", "Hadouken"));//abom
                globalMechList.Add(new Mechanic(34404, "Shards of Rage", 3, 16115, "symbol:'hexagram',color:'rgb(255,0,0)',", "Backflip Shards"));//human
                globalMechList.Add(new Mechanic(34411, "Shards of Rage", 3, 16115, "symbol:'hexagram',color:'rgb(255,0,0)',", "Backflip Shards"));//abom
                globalMechList.Add(new Mechanic(34450, "Unstable Blood Magic", 0, 16115, "symbol:'diamond',color:'rgb(255,0,0)',", "Bomb"));
                globalMechList.Add(new Mechanic(34416, "Corruption", 0, 16115, "symbol:'circle',color:'rgb(255,0,0)',", "Corruption"));
                globalMechList.Add(new Mechanic(34442, "Sacrifice", 0, 16115, "symbol:'circle',color:'rgb(75,0,130)',", "Sacrifice"));
                globalMechList.Add(new Mechanic(34367, "Unbalanced", 0, 16115, "symbol:'square',color:'rgb(75,30,150)',", "Rain Knockdown"));
                globalMechList.Add(new Mechanic(34422, "Blood Fueled", 1, 16115, "symbol:'square',color:'rgb(255,0,0)',", "Ate Reflects(Bad)"));
                globalMechList.Add(new Mechanic(34428, "Blood Fueled", 1, 16115, "symbol:'square',color:'rgb(255,0,0)',", "Ate Reflects(Bad)"));
                globalMechList.Add(new Mechanic(34376, "Blood Shield", 1, 16115, "symbol:'octagon',color:'rgb(255,0,0)',", "Bubble"));
                globalMechList.Add(new Mechanic(34518, "Blood Shield", 1, 16115, "symbol:'octagon',color:'rgb(255,0,0)',", "Bubble"));

                //KC
                globalMechList.Add(new Mechanic(34912, "Fixate", 0, 16235, "symbol:'star',color:'rgb(0,0,250)',", "Fixate"));
                globalMechList.Add(new Mechanic(34925, "Fixate", 0, 16235, "symbol:'star',color:'rgb(0,0,250)',", "Fixate"));
                globalMechList.Add(new Mechanic(35077, "Hail of Fury", 3, 16235, "symbol:'hexagram',color:'rgb(0,0,250)',", "Debris"));
                globalMechList.Add(new Mechanic(0, "Insidious Projection", 5, 16235, "symbol:'octagram',color:'rgb(0,0,250)',", "Merge"));//Spawn check
                globalMechList.Add(new Mechanic(35137, "Phantasmal Blades", 3, 16235, "symbol:'star',color:'rgb(250,0,0)',", "Bunker and rotate"));
                globalMechList.Add(new Mechanic(35064, "Phantasmal Blades", 3, 16235, "symbol:'star',color:'rgb(250,0,0)',", "Bunker and rotate"));
                globalMechList.Add(new Mechanic(35086, "Tower Drop", 3, 16235, "symbol:'circle',color:'rgb(250,0,0)',", "Tower Drop"));
                //hit orb

                //Xera
                globalMechList.Add(new Mechanic(35128, "Temporal Shred", 3, 16246, "symbol:'circle',color:'rgb(250,0,0)',", "Orb"));
                globalMechList.Add(new Mechanic(34913, "Temporal Shred", 3, 16246, "symbol:'square',color:'rgb(250,0,0)',", "Orb Aoe"));
                globalMechList.Add(new Mechanic(35000, "Intervention", 0, 16246, "symbol:'circle',color:'rgb(75,30,150)',", "Bubble"));
                globalMechList.Add(new Mechanic(35168, "Bloodstone Protection", 0, 16246, "symbol:'hourglass',color:'rgb(75,30,150)',", "In Bubble"));
                globalMechList.Add(new Mechanic(34887, "Summon Fragment", 6, 16246, "symbol:'diamond',color:'rgb(75,30,150)',", "CC Too long"));
                globalMechList.Add(new Mechanic(34965, "Derangement", 0, 16246, "symbol:'square',color:'rgb(75,30,150)',", "Stacking buff"));
                globalMechList.Add(new Mechanic(35084, "Bending Chaos", 0, 16246, "symbol:'pentagon',color:'rgb(75,30,150)',", "Button 1"));
                globalMechList.Add(new Mechanic(35162, "Shifting Chaos", 0, 16246, "symbol:'hexagon',color:'rgb(75,30,150)',", "Button 2"));
                globalMechList.Add(new Mechanic(35032, "Twisting Chaos", 0, 16246, "symbol:'octagon',color:'rgb(75,30,150)',", "Button 3"));
                //teleport
                //hit move orb

                //Cairn
                globalMechList.Add(new Mechanic(38113, "Displacement", 3, 17194, "symbol:'circle',color:'rgb(0,0,250)',", "Blue TP"));
                globalMechList.Add(new Mechanic(37611, "Spatial Manipulation", 3, 17194, "symbol:'circle',color:'rgb(0,150,0)',", "Green"));
                globalMechList.Add(new Mechanic(37629, "Spatial Manipulation", 3, 17194, "symbol:'circle',color:'rgb(0,150,0)',", "Green"));
                globalMechList.Add(new Mechanic(37642, "Spatial Manipulation", 3, 17194, "symbol:'circle',color:'rgb(0,150,0)',", "Green"));
                globalMechList.Add(new Mechanic(37673, "Spatial Manipulation", 3, 17194, "symbol:'circle',color:'rgb(0,150,0)',", "Green"));
                globalMechList.Add(new Mechanic(38074, "Spatial Manipulation", 3, 17194, "symbol:'circle',color:'rgb(0,150,0)',", "Green"));
                globalMechList.Add(new Mechanic(38302, "Spatial Manipulation", 3, 17194, "symbol:'circle',color:'rgb(0,150,0)',", "Green"));
                globalMechList.Add(new Mechanic(38313, "Meteor Swarm", 3, 17194, "symbol:'diamond',color:'rgb(250,0,0)',", "Knockback crystal"));
                globalMechList.Add(new Mechanic(38049, "Shared Agony", 0, 17194, "symbol:'circle',color:'rgb(250,0,0)',", "Agony Buff"));//could flip
                globalMechList.Add(new Mechanic(38210, "Shared Agony", 3, 17194, "symbol:'hexagon',color:'rgb(250,0,0)',", "Agony Damage"));//could flip
                globalMechList.Add(new Mechanic(38060, "Energy Surge", 3, 17194, "symbol:'triangle-left',color:'rgb(250,0,0)',", "Leap"));
                globalMechList.Add(new Mechanic(37631, "Orbital Sweep", 3, 17194, "symbol:'circle',color:'rgb(0,0,250)',", "Sweep"));

                //mo
                globalMechList.Add(new Mechanic(37788, "Jade Explosion", 3, 17172, "symbol:'circle',color:'rgb(250,0,0)',", "Jade Dieing"));
                globalMechList.Add(new Mechanic(37779, "Claim", 0, 17172, "symbol:'circle',color:'rgb(0,0,250)',", "Claim"));
                globalMechList.Add(new Mechanic(37697, "Dispel", 0, 17172, "symbol:'square',color:'rgb(0,0,250)',", "Dispel"));
                globalMechList.Add(new Mechanic(37813, "Protect", 0, 17172, "symbol:'square',color:'rgb(0,158,0)',", "Protect"));
                globalMechList.Add(new Mechanic(38155, "Mursaat Overseer's Shield", 0, 17172, "symbol:'circle',color:'rgb(0,158,0)',", "Bubble Invuln"));
                globalMechList.Add(new Mechanic(38184, "Enemy Tile", 3, 17172, "symbol:'square',color:'rgb(255,0,0)',", "Tile Dmg"));

                //sam
                globalMechList.Add(new Mechanic(37996, "Shockwave", 3, 17188, "symbol:'circle',color:'rgb(0,0,255)',", "Knockback"));
                globalMechList.Add(new Mechanic(38168, "Prisoner Sweep", 3, 17188, "symbol:'hexagon',color:'rgb(255,0,0)',", "Sweep"));
                globalMechList.Add(new Mechanic(38305, "Bludgeon", 3, 17188, "symbol:'square',color:'rgb(255,0,0)',", "Slam"));
                globalMechList.Add(new Mechanic(37868, "Fixate: Samarog", 0, 17188, "symbol:'star',color:'rgb(0,0,255)',", "Fixate: Samarog"));
                globalMechList.Add(new Mechanic(38223, "Fixate: Guldhem", 0, 17188, "symbol:'star',color:'rgb(0,150,0)',", "Fixate: Guldhem"));
                globalMechList.Add(new Mechanic(37693, "Fixate: Rigom", 0, 17188, "symbol:'star',color:'rgb(255,0,0)',", "Fixate: Rigom"));
                globalMechList.Add(new Mechanic(37693, "Fixate: Rigom", 0, 17188, "symbol:'star',color:'rgb(255,0,0)',", "Fixate: Rigom"));
                globalMechList.Add(new Mechanic(37966, "Big Hug", 0, 17188, "symbol:'circle',color:'rgb(0,150,0)',", "Big Green"));
                globalMechList.Add(new Mechanic(38247, "Small Hug", 0, 17188, "symbol:'octagon',color:'rgb(0,150,0)',", "Small Green"));
                globalMechList.Add(new Mechanic(38260, "Inevitable Betrayal", 3, 17188, "symbol:'triangle-down',color:'rgb(255,0,0)',", "Failed Kissing"));
                globalMechList.Add(new Mechanic(37851, "Inevitable Betrayal", 3, 17188, "symbol:'triangle-down',color:'rgb(255,0,0)',", "Failed Kissing"));
                globalMechList.Add(new Mechanic(37901, "Effigy Pulse", 3, 17188, "symbol:'triangle-nw',color:'rgb(255,0,0)',", "Stood in Spear"));
                globalMechList.Add(new Mechanic(37816, "Spear Impact", 3, 17188, "symbol:'triangle-sw',color:'rgb(255,0,0)',", "Spear Spawned"));
                //  globalMechList.Add(new Mechanic(37816, "Brutalize", 3, 17188, "symbol:'star-square',color:'rgb(255,0,0)',", "CC Target")); casted without dmg odd

                //deiimos
                globalMechList.Add(new Mechanic(37716, "Rapid Decay", 3, 17154, "symbol:'circle',color:'rgb(0,0,0)',", "Black Oil"));
                globalMechList.Add(new Mechanic(38208, "Annihilate", 3, 17154, "symbol:'circle',color:'rgb(255,0,0)',", "Boss Smash"));
                globalMechList.Add(new Mechanic(37929, "Annihilate", 3, 17154, "symbol:'circle',color:'rgb(255,0,0)',", "Chain Smash"));
                globalMechList.Add(new Mechanic(37980, "Demonic Shock Wave", 3, 17154, "symbol:'circle',color:'rgb(255,0,0)',", "10% Smash"));
                globalMechList.Add(new Mechanic(37982, "Demonic Shock Wave", 3, 17154, "symbol:'circle',color:'rgb(255,0,0)',", "10% Smash"));
                globalMechList.Add(new Mechanic(37733, "Tear Instability", 0, 17154, "symbol:'diamond',color:'rgb(0,150,0)',", "Tear"));
                globalMechList.Add(new Mechanic(37613, "Mind Crush", 3, 17154, "symbol:'square',color:'rgb(250,0,0)',", "Mind Crush"));
                globalMechList.Add(new Mechanic(38187, "Weak Minded", 0, 17154, "symbol:'square',color:'rgb(0,0,250)',", "Weak Minded"));
                //mlist.Add("Chosen by Eye of Janthir");
                //mlist.Add("");//tp from drunkard
                //mlist.Add("");//bon currupt from thief
                //mlist.Add("Teleport");//to demonic realm

                //horror
                globalMechList.Add(new Mechanic(47327, "Vortex Slash", 3, 19767, "symbol:'circle',color:'rgb(250,0,0)',", "Donut Inner"));
                globalMechList.Add(new Mechanic(48432, "Vortex Slash", 3, 19767, "symbol:'circle-open',color:'rgb(250,0,0)',", "Donut Outer"));
                globalMechList.Add(new Mechanic(47430, "Soul Rift", 3, 19767, "symbol:'circle',color:'rgb(250,140,0)',", "Golem AOE"));
                globalMechList.Add(new Mechanic(48363, "Quad Slash", 3, 19767, "symbol:'trianlge-up',color:'rgb(250,140,0)',", "Slices"));
                globalMechList.Add(new Mechanic(47915, "Quad Slash", 3, 19767, "symbol:'trianlge-up',color:'rgb(250,140,0)',", "Slices"));
                globalMechList.Add(new Mechanic(47363, "Spinning Slash", 3, 19767, "symbol:'trianlge-up',color:'rgb(250,140,0)',", "Sythe"));
                globalMechList.Add(new Mechanic(47434, "Fixated", 0, 19767, "symbol:'circle',color:'rgb(0,0,250)',", "Fixate"));
                globalMechList.Add(new Mechanic(47414, "Necrosis", 0, 19767, "symbol:'square',color:'rgb(0,150,0)',", "Necrosis Debuff"));

                //dhuum
                globalMechList.Add(new Mechanic(48172, "Hateful Ephemera", 3, 19450, "symbol:'square',color:'rgb(0,150,0)',", "Golem"));//Buff or dmg?
                globalMechList.Add(new Mechanic(47646, "Arcing Affliction", 0, 19450, "symbol:'hexagon',color:'rgb(250,0,0)',", "Arcing Affliction DMG"));//Buff or dmg?
                globalMechList.Add(new Mechanic(48121, "Arcing Affliction", 3, 19450, "symbol:'circle',color:'rgb(250,0,0)',", "Arcing Affliction"));//Buff or dmg?
                globalMechList.Add(new Mechanic(47476, "Residual Affliction", 0, 19450, "symbol:'square',color:'rgb(0,0,250)',", "Bomb CD"));
                globalMechList.Add(new Mechanic(47335, "Soul Shackle", 7, 19450, "symbol:'diamond',color:'rgb(0,0,250)',", "Shackle"));//4 calls probably this one
                globalMechList.Add(new Mechanic(47561, "Slash", 3, 19450, "symbol:'triangle',color:'rgb(0,150,0)',", "Cone boon rip"));
                globalMechList.Add(new Mechanic(48752, "Cull", 3, 19450, "symbol:'diamond',color:'rgb(0,150,0)',", "Crack"));
                globalMechList.Add(new Mechanic(48760, "Putrid Bomb", 3, 19450, "symbol:'circle',color:'rgb(0,75,0)',", "Mark"));
                globalMechList.Add(new Mechanic(48398, "Cataclysmic Cycle", 3, 19450, "symbol:'triangle',color:'rgb(250,0,0)',", "Suck Center"));
                globalMechList.Add(new Mechanic(48176, "Death Mark", 3, 19450, "symbol:'circle',color:'rgb(250,140,0)',", "Dip Aoe"));
                globalMechList.Add(new Mechanic(48210, "Greater Death Mark", 3, 19450, "symbol:'circle-open',color:'rgb(250,140,0)',", "Suck Dmg"));
              //  globalMechList.Add(new Mechanic(48281, "Mortal Coil", 0, 19450, "symbol:'circle',color:'rgb(0,150,0)',", "Green Orbs"));
                globalMechList.Add(new Mechanic(46950, "Fractured Spirit", 0, 19450, "symbol:'square',color:'rgb(0,150,0)',", "Green Orbs CD"));

                
            }
            if (bossid > 0)
            {
                return globalMechList.Where(x => x.GetBossID() == bossid || x.GetBossID() == 0).ToList();
            }
            else {
                return globalMechList;
            }
           
        }
        public Mechanic GetMech(MechanicLog m_log) {
            return GetMechList(0).FirstOrDefault(x => x.GetSkill() == m_log.GetSkill());
        }
        public string GetPLoltyShape(string mechName)
        {
            switch (mechName)
            {
                case "DOWN":
                    return "symbol:'cross',color:'rgb(255,0,0)',";
                case "DEAD":
                    return "symbol:'x',color:'rgb(190,190,190)',";
                default:
                    return "";
            }
        }

        
    }
}
