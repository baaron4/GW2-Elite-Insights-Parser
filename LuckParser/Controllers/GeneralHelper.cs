using LuckParser.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LuckParser.Models.DataModels.ParseEnum.TrashIDS;

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
                case "Sword":
                    return "https://wiki.guildwars2.com/images/0/07/Crimson_Antique_Blade.png";
            }
            return "";
        }

        public static string GetNPCIcon(ushort id)
        {
            switch(ParseEnum.GetBossIDS(id))
            {
                case ParseEnum.BossIDS.ValeGuardian:
                    return "https://i.imgur.com/MIpP5pK.png";
                case ParseEnum.BossIDS.Gorseval:
                    return "https://i.imgur.com/5hmMq12.png";
                case ParseEnum.BossIDS.Sabetha:
                    return "https://i.imgur.com/UqbFp9S.png";
                case ParseEnum.BossIDS.Slothasor:
                    return "https://i.imgur.com/h1xH3ER.png";
                case ParseEnum.BossIDS.Matthias:
                    return "https://i.imgur.com/3uMMmTS.png";
                case ParseEnum.BossIDS.KeepConstruct:
                    return "https://i.imgur.com/Kq0kL07.png";
                case ParseEnum.BossIDS.Xera:
                    return "https://i.imgur.com/lYwJEyV.png";
                case ParseEnum.BossIDS.Cairn:
                    return "https://i.imgur.com/gQY37Tf.png";
                case ParseEnum.BossIDS.MursaatOverseer:
                    return "https://i.imgur.com/5LNiw4Y.png";
                case ParseEnum.BossIDS.Samarog:
                    return "https://i.imgur.com/MPQhKfM.png";
                case ParseEnum.BossIDS.Deimos:
                    return "https://i.imgur.com/mWfxBaO.png";
                case ParseEnum.BossIDS.SoullessHorror:
                    return "https://i.imgur.com/jAiRplg.png";
                case ParseEnum.BossIDS.Dhuum:
                    return "https://i.imgur.com/RKaDon5.png";
                case ParseEnum.BossIDS.ConjuredAmalgamate:
                    return "https://i.imgur.com/C23rYTl.png";
                case ParseEnum.BossIDS.CALeftArm:
                    return "https://i.imgur.com/qrkQvEY.png";
                case ParseEnum.BossIDS.CARightArm:
                    return "https://i.imgur.com/MVwjtH7.png";
                case ParseEnum.BossIDS.Nikare:
                    return "https://i.imgur.com/6yq45Cc.png";
                case ParseEnum.BossIDS.Kenut:
                    return "https://i.imgur.com/TLykcrJ.png";
                case ParseEnum.BossIDS.Qadim:
                    return "https://i.imgur.com/IfoHTHT.png";
                case ParseEnum.BossIDS.MAMA:
                    return "https://i.imgur.com/1h7HOII.png";
                case ParseEnum.BossIDS.Siax:
                    return "https://i.imgur.com/5C60cQb.png";
                case ParseEnum.BossIDS.Ensolyss:
                    return "https://i.imgur.com/GUTNuyP.png";
                case ParseEnum.BossIDS.Skorvald:
                    return "https://i.imgur.com/IOPAHRE.png";
                case ParseEnum.BossIDS.Artsariiv:
                    return "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
                case ParseEnum.BossIDS.Arkk:
                    return "https://i.imgur.com/u6vv8cW.png";
                case ParseEnum.BossIDS.LGolem:
                    return "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                case ParseEnum.BossIDS.AvgGolem:
                    return "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                case ParseEnum.BossIDS.StdGolem:
                    return "https://wiki.guildwars2.com/images/8/8f/Mini_Professor_Mew.png";
                case ParseEnum.BossIDS.MassiveGolem:
                    return "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                case ParseEnum.BossIDS.MedGolem:
                    return "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
            }
            switch(ParseEnum.GetTrashIDS(id))
            {
                case Spirit:
                case Spirit2:
                case ChargedSoul:
                    return "https://i.imgur.com/sHmksvO.png";
                case Saul:
                    return "https://i.imgur.com/ck2IsoS.png";
                case GamblerClones:
                    return "https://i.imgur.com/zMsBWEx.png";
                case GamblerReal:
                    return "https://i.imgur.com/J6oMITN.png";
                case Pride:
                    return "https://i.imgur.com/ePTXx23.png";
                case Oil:
                    return "https://i.imgur.com/R26VgEr.png";
                case Tear:
                    return "https://i.imgur.com/N9seps0.png";
                case Gambler:
                case Drunkard:
                case Thief:
                    return "https://i.imgur.com/vINeVU6.png";
                case TormentedDead:
                case Messenger:
                    return "https://i.imgur.com/1J2BTFg.png";
                case Enforcer:
                    return "https://i.imgur.com/elHjamF.png";
                case Echo:
                    return "https://i.imgur.com/kcN9ECn.png";
                case Core:
                    return "https://i.imgur.com/yI34iqw.png";
                case Jessica:
                case Olson:
                case Engul:
                case Faerla:
                case Caulle:
                case Henley:
                case Galletta:
                case Ianim:
                    return "https://i.imgur.com/qeYT1Bf.png";
                case InsidiousProjection:
                    return "https://i.imgur.com/9EdItBS.png";
                case UnstableLeyRift:
                    return "https://i.imgur.com/YXM3igs.png";
                case RadiantPhantasm:
                    return "https://i.imgur.com/O5VWLyY.png";
                case CrimsonPhantasm:
                    return "https://i.imgur.com/zP7Bvb4.png";
                case Storm:
                    return "https://i.imgur.com/9XtNPdw.png";
                case IcePatch:
                    return "https://i.imgur.com/yxKJ5Yc.png";
                case Tornado:
                    return "https://i.imgur.com/e10lZMa.png";
                case Jade:
                    return "https://i.imgur.com/ivtzbSP.png";
                case Zommoros:
                    return "https://i.imgur.com/BxbsRCI.png";
                case AncientInvokedHydra:
                    return "https://i.imgur.com/YABLiBz.png";
                case WyvernMatriarch:
                    return "https://i.imgur.com/vjjNSpI.png";
                case WyvernPatriarch:
                    return "https://i.imgur.com/kLKLSfv.png";
                case ApocalypseBringer:
                    return "https://i.imgur.com/0LGKCn2.png";
                case ConjuredGreatsword:
                    return "https://i.imgur.com/vHka0QN.png";
                case ConjuredShield:
                    return "https://i.imgur.com/wUiI19S.png";
                case GreaterMagmaElemental1:
                case GreaterMagmaElemental2:
                    return "https://i.imgur.com/sr146T6.png";
                case LavaElemental1:
                case LavaElemental2:
                    return "https://i.imgur.com/mydwiYy.png"; 
                case PyreGuardian:
                    return "https://i.imgur.com/6zNPTUw.png";
                case ReaperofFlesh:
                    return "https://i.imgur.com/Notctbt.png";
                case Kernan:
                    return "https://i.imgur.com/WABRQya.png";
                case Knuckles:
                    return "https://i.imgur.com/m1y8nJE.png";
                case Karde:
                    return "https://i.imgur.com/3UGyosm.png";
                case Rigom:
                    return "https://i.imgur.com/REcGMBe.png";
                case Guldhem:
                    return "https://i.imgur.com/xa7Fefn.png";
                case Scythe:
                    return "https://i.imgur.com/INCGLIK.png";
                case SurgingSoul:
                    return "https://i.imgur.com/k79t7ZA.png";
                case Seekers:
                    return "https://i.imgur.com/FrPoluz.png";
                case BlueGuardian:
                    return "https://i.imgur.com/6CefnkP.png";
                case GreenGuardian:
                    return "https://i.imgur.com/nauDVYP.png";
                case RedGuardian:
                    return "https://i.imgur.com/73Uj4lG.png";
                case UnderworldReaper:
                case BanditSapper:
                    return "https://i.imgur.com/0koP4xB.png";
                case FleshWurm:
                    return "https://i.imgur.com/o3vX9Zc.png";
                case Hands:
                    return "https://i.imgur.com/8JRPEoo.png";
            }
            return "https://i.imgur.com/HuJHqRZ.png";
        }
    }
}
