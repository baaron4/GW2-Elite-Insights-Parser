namespace LuckParser.Models.ParseModels
{
    public class GW2APISkill
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        public string Chat_link { get; set; }
        public string Type { get; set; }
        public string Weapon_type { get; set; }
        public string[] Professions { get; set; }
        public string Slot { get; set; }
        public string[] Categories { get; set; }
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
