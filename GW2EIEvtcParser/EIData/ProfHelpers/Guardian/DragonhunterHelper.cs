using System.Collections.Generic;
using static GW2EIEvtcParser.ArcDPSEnums;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;

namespace GW2EIEvtcParser.EIData
{
    internal static class DragonhunterHelper
    {

        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifierTarget(721, "Zealot's Aggression", "10% on crippled target", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParserHelper.Source.Dragonhunter, ByPresence, "https://wiki.guildwars2.com/images/7/7e/Zealot%27s_Aggression.png", DamageModifierMode.All),
        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {  
                new Buff("Justice",30232, ParserHelper.Source.Dragonhunter, BuffStackType.Stacking, 25, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/b/b0/Spear_of_Light.png"),
                new Buff("Shield of Courage (Active)", 29906, ParserHelper.Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Shield_of_Courage.png"),
                new Buff("Spear of Justice", 29632, ParserHelper.Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/f/f1/Spear_of_Justice.png"),
                new Buff("Shield of Courage", 29523, ParserHelper.Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Shield_of_Courage.png"),
                new Buff("Wings of Resolve", 30308, ParserHelper.Source.Dragonhunter, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/cb/Wings_of_Resolve.png"),
        };

    }
}
