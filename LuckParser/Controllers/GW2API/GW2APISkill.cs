namespace LuckParser.Controllers
{
    public class GW2APISkill
    {
        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string chat_link { get; set; }
        public string type { get; set; }
        public string weapon_type { get; set; }
        public string[] professions { get; set; }
        public string slot { get; set; }
        public string[] categories { get; set; }
        public GW2APIfacts[] facts { get; set; }

        // public string attunement { get; set; }
        //public string cost { get; set; }
        public string dual_wield { get; set; }
        // public int flip_skill { get; set; }
        // public int initiative { get; set; }
        // public int next_chain { get; set; }
        // public int prev_chain { get; set; }


        //public int toolbelt_skill { get; set; }


        public GW2APISkill() { }
    }

}