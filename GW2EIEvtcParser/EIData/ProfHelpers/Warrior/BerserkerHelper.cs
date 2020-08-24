using System.Collections.Generic;
using static GW2EIEvtcParser.EIData.Buff;
using static GW2EIEvtcParser.EIData.DamageModifier;

namespace GW2EIEvtcParser.EIData
{
    internal class BerserkerHelper
    {
        /////////////////////
        internal static readonly List<InstantCastFinder> InstantCastFinder = new List<InstantCastFinder>()
        {
            new DamageCastFinder(31289, 31289, 500, 97950, ulong.MaxValue), // King of Fires
        };

        internal static readonly List<DamageModifier> DamageMods = new List<DamageModifier>
        {
            new BuffDamageModifier(34099, "Always Angry", "7% per stack", DamageSource.NoPets, 7.0, DamageType.All, DamageType.All, ParserHelper.Source.Berserker, ByPresence, "https://wiki.guildwars2.com/images/6/63/Always_Angry.png", 0 , 96406, DamageModifierMode.PvE),
            new BuffDamageModifier(29502, "Bloody Roar", "10% while in berserk", DamageSource.NoPets, 10.0, DamageType.Power, DamageType.All, ParserHelper.Source.Berserker, ByPresenceNonMulti, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", 0 , 96406, DamageModifierMode.PvE),
            new BuffDamageModifier(29502, "Bloody Roar", "20% while in berserk", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParserHelper.Source.Berserker, ByPresenceNonMulti, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", 96406 , 97950, DamageModifierMode.PvE),
            new BuffDamageModifier(29502, "Bloody Roar", "20% while in berserk", DamageSource.NoPets, 20.0, DamageType.Power, DamageType.All, ParserHelper.Source.Berserker, ByPresence, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", 97950 , ulong.MaxValue, DamageModifierMode.PvE),
            new BuffDamageModifier(29502, "Bloody Roar", "15% while in berserk", DamageSource.NoPets, 15.0, DamageType.Power, DamageType.All, ParserHelper.Source.Berserker, ByPresence, "https://wiki.guildwars2.com/images/e/e1/Bloody_Roar.png", 97950 , ulong.MaxValue, DamageModifierMode.sPvPWvW),

        };

        internal static readonly List<Buff> Buffs = new List<Buff>
        {
                new Buff("Berserk",29502, ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff,"https://wiki.guildwars2.com/images/4/44/Berserk.png"),
                new Buff("Flames of War", 31708, ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/6f/Flames_of_War_%28warrior_skill%29.png"),
                new Buff("Blood Reckoning", 29466 , ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/d/d6/Blood_Reckoning.png"),
                new Buff("Rock Guard", 34256 , ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/c/c7/Shattering_Blow.png"),
                new Buff("Feel No Pain (Savage Instinct)",55030, ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/4/4d/Savage_Instinct.png", 96406, ulong.MaxValue),
                new Buff("Always Angry",34099, ParserHelper.Source.Berserker, BuffNature.GraphOnlyBuff, "https://wiki.guildwars2.com/images/6/63/Always_Angry.png", 0 , 96406),
        };


    }
}
