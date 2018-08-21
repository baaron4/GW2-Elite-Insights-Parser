namespace LuckParser.Models.ParseModels
{
    public class GW2APISkillDetailed : GW2APISkill
    {
        public GW2APIfactsDetail[] facts { get; set; }

        public GW2APISkillDetailed() { }
    }
}