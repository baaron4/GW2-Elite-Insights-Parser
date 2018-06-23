namespace LuckParser.Models.ParseModels
{
    public class GW2APISkill
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string chat_link { get; set; }
        public string type { get; set; }
        public string weapon_type { get; set; }
        public string[] professions { get; set; }
        public string slot { get; set; }
        public string[] categories { get; set; }
        //public string[]  facts { get; set; }

        // public string attunment { get; set; }
        //public string cost { get; set; }
        // public string duel_wield { get; set; }
        // public int flip_skill { get; set; }
        // public int inititiative { get; set; }
        // public int next_chain { get; set; }
        // public int prev_chain { get; set; }


        //public int toolbelt_skill { get; set; }
       

        public GW2APISkill() { }
    }
   
}
