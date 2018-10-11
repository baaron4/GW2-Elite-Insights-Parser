using LuckParser.Controllers;
using System;

namespace LuckParser.Models.ParseModels
{
    public class SkillItem
    {
        public const int ResurrectId = 1066;
        public const int BandageId = 1175;
        public const int DodgeId = 65001;
        public const int WeaponSwapId = -2;

        // Fields
        public readonly int ID;
        private string _name;
        public string Name
        {
            get
            {
                return ID == ResurrectId ? "Resurrect" : _name;
            }
        }
        public GW2APISkill ApiSkill { get; private set; }
        public int CC { get; private set; }

        // Constructor
        public SkillItem(int ID, string name)
        {
            name = name.Replace("\0", "");
            this.ID = ID;
            _name = name;
        }

        // Public Methods
        public string[] ToStringArray()
        {
            string[] array = new string[2];
            array[0] = ID.ToString();
            array[1] = _name;
            return array;
        }
        //setter
        public void SetGW2APISkill(GW2APIController apiController)
        {
            if (ApiSkill == null)
            {
                GW2APISkill skillAPI = apiController.GetSkill(ID);

                if (skillAPI != null) {
                    ApiSkill = skillAPI;
                    _name = skillAPI.name;
                }
                
            }
            
        }

        public void SetCCAPI()//this is 100% off the GW2 API is not a reliable source of finding skill CC
        {
            CC = 0;
            if (ApiSkill != null)
            {
                GW2APISkillDetailed apiskilldet = (GW2APISkillDetailed)ApiSkill;
                GW2APISkillCheck apiskillchec = (GW2APISkillCheck)ApiSkill;
                GW2APIfacts[] factsList = apiskilldet != null ? apiskilldet.facts : apiskillchec.facts;
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
                        if (fact.text == "Daze" || fact.status == "Daze")
                        {
                            if (fact.duration < 1)
                            {
                                CC += 100;
                            }
                            else
                            {
                                CC += fact.duration * 100;
                            }
                            daze = true;
                        }

                    }
                    if (stun == false)
                    {
                        if (fact.text == "Stun" || fact.status == "Stun")
                        {
                            if (fact.duration < 1)
                            {
                                CC += 100;
                            }
                            else
                            {
                                CC += fact.duration * 100;
                            }
                            stun = true;
                        }
                    }
                    if (knockdown == false)
                    {
                        if (fact.text == "Knockdown" || fact.status == "Knockdown")
                        {
                            if (fact.duration < 1)
                            {
                                CC += 100;
                            }
                            else
                            {
                                CC += fact.duration * 100;
                            }
                            knockdown = true;
                        }
                    }
                    if (launch == false)
                    {
                        if (fact.text == "Launch" || fact.status == "Launch")
                        {

                            CC += 232;//Wiki says either 232 or 332 based on duration? launch doesn't provide duration in api however
                           
                            launch = true;
                        }
                    }
                    if (knockback == false)
                    {
                        if (fact.text == "Knockback" || fact.status == "Knockback")
                        {

                            CC += 150;//always 150 unless special case of 232 for ranger pet?
                            knockback = true;
                        }
                    }
                    if (pull == false)
                    {
                        if (fact.text == "Pull" || fact.status == "Pull")
                        {

                            CC += 150;

                            pull = true;
                        }
                    }
                    if (flaot == false)
                    {
                        if (fact.text == "Float" || fact.status == "Float")
                        {
                            if (fact.duration < 1)
                            {
                                CC += 100;
                            }
                            else
                            {
                                CC += fact.duration * 100;
                            }
                            flaot = true;
                        }
                    }
                    if (fact.text == "Stone Duration" || fact.status == "Stone Duration")
                    {
                        if (fact.duration < 1)
                        {
                            CC += 100;
                        }
                        else
                        {
                            CC += fact.duration * 100;
                        }
                        
                    }

                
                }
                if (ID == 30725)//toss elixir x
                {
                    CC = 300;
                }
                if (ID == 29519)//MOA signet
                {
                    CC = 1000;
                }
               
            }
        }
    }
}