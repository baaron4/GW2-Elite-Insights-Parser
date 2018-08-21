namespace LuckParser.Models.ParseModels
{
    public class GW2APISpec
    {
        // [Serializable]
       
            public int Id { get; set; }
            public string Name { get; set; }
            public string Profession { get; set; }
            public bool Elite { get; set; }
            //minor_traits
            //major_traits
            public string Icon { get; set; }
            public string Background { get; set; }

            public GW2APISpec() { }
            public bool GetElite() { return Elite; }
        


        
    }
}