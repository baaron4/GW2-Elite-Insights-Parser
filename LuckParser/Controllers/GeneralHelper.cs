using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LuckParser.Controllers
{
    public static class GeneralHelper
    {
        public static T MaxBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
        {
            return en.Select(t => new Tuple<T, R>(t, evaluate(t)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) > 0 ? next : max).Item1;
        }

        public static T MinBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
        {
            return en.Select(t => new Tuple<T, R>(t, evaluate(t)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) < 0 ? next : max).Item1;
        }


        public static string FindPattern(string source, string regex)
        {
            if (String.IsNullOrEmpty(source)) return null;
            Match match = Regex.Match(source, regex);
            if (match.Success) return match.Groups[1].Value;
            return null;
        }
        
        public static string GetProfIcon(string prof)
        {
            switch (prof)
            {
                case "Warrior":
                    return "https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png";
                case "Berserker":
                    return "https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png";
                case "Spellbreaker":
                    return "https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png";
                case "Guardian":
                    return "https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png";
                case "Dragonhunter":
                    return "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png";
                case "DragonHunter":
                    return "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png";
                case "Firebrand":
                    return "https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png";
                case "Revenant":
                    return "https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png";
                case "Herald":
                    return "https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png";
                case "Renegade":
                    return "https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png";
                case "Engineer":
                    return "https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png";
                case "Scrapper":
                    return "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png";
                case "Holosmith":
                    return "https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png";
                case "Ranger":
                    return "https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png";
                case "Druid":
                    return "https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png";
                case "Soulbeast":
                    return "https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png";
                case "Thief":
                    return "https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png";
                case "Daredevil":
                    return "https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png";
                case "Deadeye":
                    return "https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png";
                case "Elementalist":
                    return "https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png";
                case "Tempest":
                    return "https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png";
                case "Weaver":
                    return "https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png";
                case "Mesmer":
                    return "https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png";
                case "Chronomancer":
                    return "https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png";
                case "Mirage":
                    return "https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png";
                case "Necromancer":
                    return "https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png";
                case "Reaper":
                    return "https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png";
                case "Scourge":
                    return "https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png";
            }
            return "";
        }

        public static string GetNPCIcon(ushort id)
        {
            switch(ParseEnum.GetBossIDS(id))
            {
                case ParseEnum.BossIDS.ValeGuardian:
                    break;
                case ParseEnum.BossIDS.Gorseval:
                    break;
                case ParseEnum.BossIDS.Sabetha:
                    break;
                case ParseEnum.BossIDS.Slothasor:
                    break;
                case ParseEnum.BossIDS.Matthias:
                    break;
                case ParseEnum.BossIDS.KeepConstruct:
                    break;
                case ParseEnum.BossIDS.Xera:
                    break;
                case ParseEnum.BossIDS.Cairn:
                    break;
                case ParseEnum.BossIDS.MursaatOverseer:
                    break;
                case ParseEnum.BossIDS.Samarog:
                    break;
                case ParseEnum.BossIDS.Deimos:
                    break;
                case ParseEnum.BossIDS.SoullessHorror:
                    break;
                case ParseEnum.BossIDS.Dhuum:
                    break;
                case ParseEnum.BossIDS.ConjuredAmalgamate:
                    break;
                case ParseEnum.BossIDS.CALeftArm:
                    break;
                case ParseEnum.BossIDS.CARightArm:
                    break;
                case ParseEnum.BossIDS.Nikare:
                    break;
                case ParseEnum.BossIDS.Kenut:
                    break;
                case ParseEnum.BossIDS.Qadim:
                    break;
                case ParseEnum.BossIDS.MAMA:
                    break;
                case ParseEnum.BossIDS.Siax:
                    break;
                case ParseEnum.BossIDS.Ensolyss:
                    break;
                case ParseEnum.BossIDS.Skorvald:
                    break;
                case ParseEnum.BossIDS.Artsariiv:
                    break;
                case ParseEnum.BossIDS.Arkk:
                    break;
                case ParseEnum.BossIDS.LGolem:
                    break;
                case ParseEnum.BossIDS.AvgGolem:
                    break;
                case ParseEnum.BossIDS.StdGolem:
                    break;
                case ParseEnum.BossIDS.MassiveGolem:
                    break;
                case ParseEnum.BossIDS.MedGolem:
                    break;
            }
            switch(ParseEnum.GetTrashIDS(id))
            {

            }
            return "https://i.imgur.com/xCoypjS.png";
        }
    }
}
