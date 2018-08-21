using LuckParser.Controllers;
using System;

namespace LuckParser.Models.ParseModels
{
    public class SkillItem
    {
        // Fields
        private int ID;
        private String name;
        private GW2APISkill apiSkill = null;
        private int cc = 0;

        // Constructor
        public SkillItem(int ID, String name)
        {
            name = name.Replace("\0", "");
            this.ID = ID;
            this.name = name;
        }

        // Public Methods
        public String[] ToStringArray()
        {
            String[] array = new String[2];
            array[0] = ID.ToString();
            array[1] = name.ToString();
            return array;
        }
        //setter
        public void SetGW2APISkill(GW2APIController apiController)
        {
            if (apiSkill == null)
            {
                GW2APISkill skillAPI = apiController.GetSkill(ID);

                if (skillAPI != null) {
                    this.apiSkill = skillAPI;
                    this.name = skillAPI.Name;
                }
                
            }
            
        }
        // Getters
        public int GetID()
        {
            return ID;
        }

        public String GetName()
        {
            if (ID == 1066) {
                return "Resurrect";
            }
            return name;
        }

        public GW2APISkill GetGW2APISkill() {
            return apiSkill;
        }
        public int GetCC() {
            return cc;
        }
        public void SetCCAPI(int cc)//this is 100% off the GW2 API is not a reliable source of finding skill CC
        {
            cc = 0;
            if (apiSkill != null)
            {
                GW2APISkillDetailed apiskilldet = (GW2APISkillDetailed)this.apiSkill;
                GW2APISkillCheck apiskillchec = (GW2APISkillCheck)this.apiSkill;
                GW2APIfacts[] factsList;
                if (apiskilldet != null)
                {
                    factsList = apiskilldet.Facts;
                }
                else
                {
                    factsList = apiskillchec.Facts;
                }
                bool daze = false;
                bool stun = false;
                bool knockdown = false;
                bool flaot = false;
                bool knockback = false;
                bool launch = false;
                bool pull = false;
               
                foreach (GW2APIfacts fact in factsList)
                {
                    if (daze == false)
                    {
                        if (fact.Text == "Daze" || fact.Status == "Daze")
                        {
                            if (fact.Duration < 1)
                            {
                                cc += 100;
                            }
                            else
                            {
                                cc += fact.Duration * 100;
                            }
                            daze = true;
                        }

                    }
                    if (stun == false)
                    {
                        if (fact.Text == "Stun" || fact.Status == "Stun")
                        {
                            if (fact.Duration < 1)
                            {
                                cc += 100;
                            }
                            else
                            {
                                cc += fact.Duration * 100;
                            }
                            stun = true;
                        }
                    }
                    if (knockdown == false)
                    {
                        if (fact.Text == "Knockdown" || fact.Status == "Knockdown")
                        {
                            if (fact.Duration < 1)
                            {
                                cc += 100;
                            }
                            else
                            {
                                cc += fact.Duration * 100;
                            }
                            knockdown = true;
                        }
                    }
                    if (launch == false)
                    {
                        if (fact.Text == "Launch" || fact.Status == "Launch")
                        {
                            
                                cc += 232;//Wiki says either 232 or 332 based on duration? launch doesnt provide duration in api however
                           
                            launch = true;
                        }
                    }
                    if (knockback == false)
                    {
                        if (fact.Text == "Knockback" || fact.Status == "Knockback")
                        {

                            cc += 150;//always 150 unless special case of 232 for ranger pet?
                            knockback = true;
                        }
                    }
                    if (pull == false)
                    {
                        if (fact.Text == "Pull" || fact.Status == "Pull")
                        {

                            cc += 150;

                            pull = true;
                        }
                    }
                    if (flaot == false)
                    {
                        if (fact.Text == "Float" || fact.Status == "Float")
                        {
                            if (fact.Duration < 1)
                            {
                                cc += 100;
                            }
                            else
                            {
                                cc += fact.Duration * 100;
                            }
                            flaot = true;
                        }
                    }
                    if (fact.Text == "Stone Duration" || fact.Status == "Stone Duration")
                    {
                        if (fact.Duration < 1)
                        {
                            cc += 100;
                        }
                        else
                        {
                            cc += fact.Duration * 100;
                        }
                        
                    }

                
                }
                if (ID == 30725)//toss elixer x
                {
                    cc = 300;
                }
                if (ID == 29519)//MOA signet
                {
                    cc = 1000;
                }
               
            }
        }
    }
}