namespace LuckParser.Models.ParseModels
{
    public class GW2APIfacts
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }

        public string Target { get; set; }
        //Buff
        public string Status { get; set; }
        public string Description { get; set; }
        public int ApplyCount { get; set; }
        public int Duration { get; set; }
        //ComboField
        public string FieldType { get; set; }
        //ComboFinisher
        public string FinisherType { get; set; }
        public float Percent { get; set; }
        //Damage
        public int HitCount { get; set; }
        public float DmgMultipler { get; set; }
        //Distance
        public int Distance { get; set; }
        //Duration
        //public int duration { get; set; }
        //Heal
        //public int hit_count { get; set; }
        //HealingAdjust
        //NoData
        //Number
        //value
        //Percent
        //percent
        //PrefixedBuff
        public GW2APIfacts Prefix { get; set; }

        public GW2APIfacts() { }
    }
}
