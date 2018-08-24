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
        readonly int _id;
        private String _name;
        private GW2APISkill _apiSkill;
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
            array[1] = _name;
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
                    _name = skillAPI.name;
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
            if (_id == ResurrectId) {
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
        public void SetCCAPI()//this is 100% off the GW2 API is not a reliable source of finding skill CC
        {
            _cc = 0;
            if (_apiSkill != null)
            {
                GW2APISkillDetailed apiskilldet = (GW2APISkillDetailed)_apiSkill;
                GW2APISkillCheck apiskillchec = (GW2APISkillCheck)_apiSkill;
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
                                _cc += 100;
                            }
                            else
                            {
                                _cc += fact.duration * 100;
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
                                _cc += 100;
                            }
                            else
                            {
                                _cc += fact.duration * 100;
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
                                _cc += 100;
                            }
                            else
                            {
                                _cc += fact.duration * 100;
                            }
                            knockdown = true;
                        }
                    }
                    if (launch == false)
                    {
                        if (fact.text == "Launch" || fact.status == "Launch")
                        {

                            _cc += 232;//Wiki says either 232 or 332 based on duration? launch doesnt provide duration in api however
                           
                            launch = true;
                        }
                    }
                    if (knockback == false)
                    {
                        if (fact.text == "Knockback" || fact.status == "Knockback")
                        {

                            _cc += 150;//always 150 unless special case of 232 for ranger pet?
                            knockback = true;
                        }
                    }
                    if (pull == false)
                    {
                        if (fact.text == "Pull" || fact.status == "Pull")
                        {

                            _cc += 150;

                            pull = true;
                        }
                    }
                    if (flaot == false)
                    {
                        if (fact.text == "Float" || fact.status == "Float")
                        {
                            if (fact.duration < 1)
                            {
                                _cc += 100;
                            }
                            else
                            {
                                _cc += fact.duration * 100;
                            }
                            flaot = true;
                        }
                    }
                    if (fact.text == "Stone Duration" || fact.status == "Stone Duration")
                    {
                        if (fact.duration < 1)
                        {
                            _cc += 100;
                        }
                        else
                        {
                            _cc += fact.duration * 100;
                        }
                        
                    }

                
                }
                if (_id == 30725)//toss elixer x
                {
                    _cc = 300;
                }
                if (_id == 29519)//MOA signet
                {
                    _cc = 1000;
                }
               
            }
        }
    }
}