using LuckParser.Controllers;
using System;

namespace LuckParser.Models.ParseModels
{
    public class SkillItem
    {
        // Fields
        private int _id;
        private String _name;
        private GW2APISkill _apiSkill = null;
        private int _cc = 0;

        // Constructor
        public SkillItem(int ID, String name)
        {
            name = name.Replace("\0", "");
            _id = ID;
            _name = name;
        }

        // Public Methods
        public String[] ToStringArray()
        {
            String[] array = new String[2];
            array[0] = _id.ToString();
            array[1] = _name.ToString();
            return array;
        }
        //setter
        public void SetGW2APISkill(GW2APIController apiController)
        {
            if (_apiSkill == null)
            {
                GW2APISkill skillAPI = apiController.GetSkill(_id);

                if (skillAPI != null) {
                    _apiSkill = skillAPI;
                    _name = skillAPI.Name;
                }
                
            }
            
        }
        // Getters
        public int GetID()
        {
            return _id;
        }

        public String GetName()
        {
            if (_id == 1066) {
                return "Resurrect";
            }
            return _name;
        }

        public GW2APISkill GetGW2APISkill() {
            return _apiSkill;
        }
        public int GetCC() {
            return _cc;
        }
        public void SetCCAPI(int cc)//this is 100% off the GW2 API is not a reliable source of finding skill CC
        {
            cc = 0;
            if (_apiSkill != null)
            {
                GW2APISkillDetailed apiskilldet = (GW2APISkillDetailed)_apiSkill;
                GW2APISkillCheck apiskillchec = (GW2APISkillCheck)_apiSkill;
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
                if (_id == 30725)//toss elixer x
                {
                    cc = 300;
                }
                if (_id == 29519)//MOA signet
                {
                    cc = 1000;
                }
               
            }
        }
    }
}