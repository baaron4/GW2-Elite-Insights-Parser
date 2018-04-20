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

        public MechanicData() {
            this.m_Data = new List<MechanicLog>();
        }

        public void AddItem(MechanicLog mLog) {
            this.m_Data.Add(mLog);
        }

        public List<MechanicLog> GetMDataLogs() {
            return this.m_Data;
        }
        public string GetPLoltyShape(string mechName) {
            switch (mechName) {
                case "DOWN":
                    return "symbol:'cross',color:'rgb(255,0,0)',";
                case "DEAD":
                    return "symbol:'x',color:'rgb(190,190,190)',";
                case "Magic Pulse":
                    return "symbol:'circle',color:'rgb(255,0,0)',";
                case "Unstable Magic Spike":
                    return "symbol:'circle',color:'rgb(0,0,255)',";
                case "Distributed Magic":
                    return "symbol:'circle',color:'rgb(0,128,0)',";
                case "Pylon Attunement: Red":
                    return "symbol:'square',color:'rgb(255,0,0)',";
                case "Pylon Attunement: Blue":
                    return "symbol:'square',color:'rgb(0,0,255)',";
                case "Pylon Attunement: Green":
                    return "symbol:'square',color:'rgb(0,128,0)',";
                default:
                    return "";
            }
        }
        public List<string> GetMechanicNameList() {
             List<string> mlist = new List<string>();
            //VG
            mlist.Add("Magic Pulse");//damage from red orb
            mlist.Add("Unstable Magic Spike");//hit by blue tp
            mlist.Add("Distributed Magic");//hit by green aoe
           // mlist.Add("");//failed green aoe
            mlist.Add("Pylon Attunement: Red");//3 pylon atunments
            mlist.Add("Pylon Attunement: Blue");//3 pylon atunments
            mlist.Add("Pylon Attunement: Green");//3 pylon atunments
            //gors
            mlist.Add("Spectral Impact");//slam
          //  mlist.Add("");//black
            mlist.Add("Spectral Darkness");//orb buff
            mlist.Add("Ghastly Prison");//egg
            mlist.Add("Spirited Fusion");//ate spirits
            //sabetha
            mlist.Add("Shell-Shocked");//canon launcher
            mlist.Add("Sapper Bomb");//green bomb
            mlist.Add("Time Bomb");//timedbomb
           // mlist.Add("");//killed turret
            mlist.Add("Flak Shot");//flak
            //sloth
            mlist.Add("Halitosis");//flame breath
            mlist.Add("Spore Release");//shake
            mlist.Add("Tantrum");//tantrum
            mlist.Add("Volatile Poison");//Poisin activatable
            mlist.Add("Magic Transformation");//be slub
            mlist.Add("Nauseated");//cant slub
                                   //matt
            mlist.Add("Blood Fueled");//damage increase
            mlist.Add("Blood Shield");//bubble
            mlist.Add("Unbalanced");//too many stacks of rain
            mlist.Add("Oppressive Gaze");//hadokin
            mlist.Add("Unstable Blood Magic");//activatable drop
            mlist.Add("Corruption ");//sacrifice
            //kc
          //  mlist.Add(""); //hit construct core
            mlist.Add("Fixated");//fixated by dude
            mlist.Add("Hail of Fury");//debris
            mlist.Add("Insidious Projection");//merge
            mlist.Add("Phantasmal Blades");//
            mlist.Add("Tower Drop");
            //xera
            mlist.Add("Derangement");
           // mlist.Add("");//red field
           // mlist.Add("");//red orb
            mlist.Add("Intervention");//spec action key
         //   mlist.Add("");//teleport
             //cairn
            mlist.Add("Displacement");//blue tp
            mlist.Add("Energy Surge");//leap
            mlist.Add("Shared Agony");//red aoe buff
            mlist.Add("Spatial Manipulation");//failed green
            mlist.Add("Orbital Sweep");//sweep
            mlist.Add("Meteor Swarm");//hit by proj
            //mo
            mlist.Add("Corporal Punishment");//Jade Scout
            mlist.Add("Claim");//claim buff
            mlist.Add("Dispel");//claim buff
            //mlist.Add("Protect");//protect buff
            //sam
          //  mlist.Add("");//merge aoe
            mlist.Add("Effigy Pulse");//spear
            mlist.Add("Fixate: Guldhem");
            mlist.Add("Fixate: Rigom");
            mlist.Add("Fixate: Samarog");
            mlist.Add("Inevitab;e Betrayal");//merge dmg
            mlist.Add("Prisoner Sweep");//3 swipes
            mlist.Add("Shockwave");
            mlist.Add("Slam");
          //  mlist.Add("");//small merge aoe
            mlist.Add("Spear Impact");//spear spawning
            mlist.Add("Brutalize");//target of cc
            //deiimos
            mlist.Add("Annihilate");//knockback
            mlist.Add("Chosen by Eye of Janthir");
            mlist.Add("Rapid Decay");//black oil
           // mlist.Add("");//tp from drunkard
           // mlist.Add("");//bon currupt from thief
            mlist.Add("Mind Crush");//aoe wiper
            mlist.Add("Tear Instability");//got tear
            mlist.Add("Teleport");//to demonic realm
            mlist.Add("Weak Minded");//blocked mincrush
            //horror
            mlist.Add("Death Bloom");//cones
            mlist.Add("Vortex Slash");//dodge aoe
            mlist.Add("Spinning Slash");//syth
            //dhuum
            mlist.Add("Arcing Affliction");//getting bomb
            mlist.Add("Residual Affliction");//bomb cd
            mlist.Add("Cataclysmic Cycle");//pulled in by cone
            mlist.Add("Greater Death Mark");//dip
            mlist.Add("Putrid Bomb");//bomb
            mlist.Add("Soul Shackle");//shackle
            mlist.Add("Mortal Coil");//orbs
            mlist.Add("Fractured Spirit");//cant do orbs again
            return mlist;
        }
    }
}
