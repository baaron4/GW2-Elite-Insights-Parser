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
        private readonly static HashSet<string> _compressedFiles = new HashSet<string>()
        {
            ".zevtc",
            ".evtc.zip",
        };

        private readonly static HashSet<string> _tmpFiles = new HashSet<string>()
        {
            ".tmp.zip"
        };

        private readonly static HashSet<string> _supportedFiles = new HashSet<string>(_compressedFiles)
        {
            ".evtc"
        };

        public static bool IsCompressedFormat(string fileName)
        {
            foreach (string format in _compressedFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsTemporaryFormat(string fileName)
        {
            foreach (string format in _tmpFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsSupportedFormat(string fileName)
        {
            foreach (string format in _supportedFiles)
            {
                if (fileName.EndsWith(format, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

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
            if (string.IsNullOrEmpty(source)) return null;
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
            switch(ParseEnum.GetTargetIDS(id))
            {
                case ParseEnum.TargetIDS.ValeGuardian:
                    return "https://i.imgur.com/MIpP5pK.png";
                case ParseEnum.TargetIDS.Gorseval:
                    return "https://i.imgur.com/5hmMq12.png";
                case ParseEnum.TargetIDS.Sabetha:
                    return "https://i.imgur.com/UqbFp9S.png";
                case ParseEnum.TargetIDS.Slothasor:
                    return "https://i.imgur.com/h1xH3ER.png";
                case ParseEnum.TargetIDS.Matthias:
                    return "https://i.imgur.com/3uMMmTS.png";
                case ParseEnum.TargetIDS.KeepConstruct:
                    return "https://i.imgur.com/Kq0kL07.png";
                case ParseEnum.TargetIDS.Xera:
                    return "https://i.imgur.com/lYwJEyV.png";
                case ParseEnum.TargetIDS.Cairn:
                    return "https://i.imgur.com/gQY37Tf.png";
                case ParseEnum.TargetIDS.MursaatOverseer:
                    return "https://i.imgur.com/5LNiw4Y.png";
                case ParseEnum.TargetIDS.Samarog:
                    return "https://i.imgur.com/MPQhKfM.png";
                case ParseEnum.TargetIDS.Deimos:
                    return "https://i.imgur.com/mWfxBaO.png";
                case ParseEnum.TargetIDS.SoullessHorror:
                    return "https://i.imgur.com/jAiRplg.png";
                case ParseEnum.TargetIDS.Dhuum:
                    return "https://i.imgur.com/RKaDon5.png";
                case ParseEnum.TargetIDS.ConjuredAmalgamate:
                    return "https://i.imgur.com/C23rYTl.png";
                case ParseEnum.TargetIDS.CALeftArm:
                    return "https://i.imgur.com/qrkQvEY.png";
                case ParseEnum.TargetIDS.CARightArm:
                    return "https://i.imgur.com/MVwjtH7.png";
                case ParseEnum.TargetIDS.Nikare:
                    return "https://i.imgur.com/6yq45Cc.png";
                case ParseEnum.TargetIDS.Kenut:
                    return "https://i.imgur.com/TLykcrJ.png";
                case ParseEnum.TargetIDS.Qadim:
                    return "https://i.imgur.com/IfoHTHT.png";
                case ParseEnum.TargetIDS.MAMA:
                    return "https://i.imgur.com/1h7HOII.png";
                case ParseEnum.TargetIDS.Siax:
                    return "https://i.imgur.com/5C60cQb.png";
                case ParseEnum.TargetIDS.Ensolyss:
                    return "https://i.imgur.com/GUTNuyP.png";
                case ParseEnum.TargetIDS.Skorvald:
                    return "https://i.imgur.com/IOPAHRE.png";
                case ParseEnum.TargetIDS.Artsariiv:
                    return "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
                case ParseEnum.TargetIDS.Arkk:
                    return "https://i.imgur.com/u6vv8cW.png";
                case ParseEnum.TargetIDS.LGolem:
                    return "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                case ParseEnum.TargetIDS.AvgGolem:
                    return "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                case ParseEnum.TargetIDS.StdGolem:
                    return "https://wiki.guildwars2.com/images/8/8f/Mini_Professor_Mew.png";
                case ParseEnum.TargetIDS.MassiveGolem:
                    return "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                case ParseEnum.TargetIDS.MedGolem:
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
                case ExquisiteConjunction:
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
                case IcebornHydra:
                    return "https://i.imgur.com/LoYMBRU.png";
                case IceElemental:
                    return "https://i.imgur.com/pEkBeNp.png";
                case WyvernMatriarch:
                    return "https://i.imgur.com/kLKLSfv.png";
                case WyvernPatriarch:
                    return "https://i.imgur.com/vjjNSpI.png"; 
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
                case TemporalAnomaly:
                case TemporalAnomaly2:
                    return "https://i.imgur.com/MIpP5pK.png";
                case DOC:
                case BLIGHT:
                case PLINK:
                case CHOP:
                    return "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
            }
            return "https://i.imgur.com/HuJHqRZ.png";
        }


        public static string GetLink(string name)
        {
            switch (name)
            {
                case "Question":
                    return "https://wiki.guildwars2.com/images/d/de/Sword_slot.png";
                case "Sword":
                    return "https://wiki.guildwars2.com/images/0/07/Crimson_Antique_Blade.png";
                case "Axe":
                    return "https://wiki.guildwars2.com/images/d/d4/Crimson_Antique_Reaver.png";
                case "Dagger":
                    return "https://wiki.guildwars2.com/images/6/65/Crimson_Antique_Razor.png";
                case "Mace":
                    return "https://wiki.guildwars2.com/images/6/6d/Crimson_Antique_Flanged_Mace.png";
                case "Pistol":
                    return "https://wiki.guildwars2.com/images/4/46/Crimson_Antique_Revolver.png";
                case "Scepter":
                    return "https://wiki.guildwars2.com/images/e/e2/Crimson_Antique_Wand.png";
                case "Focus":
                    return "https://wiki.guildwars2.com/images/8/87/Crimson_Antique_Artifact.png";
                case "Shield":
                    return "https://wiki.guildwars2.com/images/b/b0/Crimson_Antique_Bastion.png";
                case "Torch":
                    return "https://wiki.guildwars2.com/images/7/76/Crimson_Antique_Brazier.png";
                case "Warhorn":
                    return "https://wiki.guildwars2.com/images/1/1c/Crimson_Antique_Herald.png";
                case "Greatsword":
                    return "https://wiki.guildwars2.com/images/5/50/Crimson_Antique_Claymore.png";
                case "Hammer":
                    return "https://wiki.guildwars2.com/images/3/38/Crimson_Antique_Warhammer.png";
                case "Longbow":
                    return "https://wiki.guildwars2.com/images/f/f0/Crimson_Antique_Greatbow.png";
                case "Shortbow":
                    return "https://wiki.guildwars2.com/images/1/17/Crimson_Antique_Short_Bow.png";
                case "Rifle":
                    return "https://wiki.guildwars2.com/images/1/19/Crimson_Antique_Musket.png";
                case "Staff":
                    return "https://wiki.guildwars2.com/images/5/5f/Crimson_Antique_Spire.png";

                case "Color-Warrior": return "rgb(255,209,102)";
                case "Color-Berserker": return "rgb(255,209,102)";
                case "Color-Spellbreaker": return "rgb(255,209,102)";
                case "Color-Guardian": return "rgb(114,193,217)";
                case "Color-Dragonhunter": return "rgb(114,193,217)";
                case "Color-Firebrand": return "rgb(114,193,217)";
                case "Color-Revenant": return "rgb(209,110,90)";
                case "Color-Herald": return "rgb(209,110,90)";
                case "Color-Renegade": return "rgb(209,110,90)";
                case "Color-Engineer": return "rgb(208,156,89)";
                case "Color-Scrapper": return "rgb(208,156,89)";
                case "Color-Holosmith": return "rgb(208,156,89)";
                case "Color-Ranger": return "rgb(140,220,130)";
                case "Color-Druid": return "rgb(140,220,130)";
                case "Color-Soulbeast": return "rgb(140,220,130)";
                case "Color-Thief": return "rgb(192,143,149)";
                case "Color-Daredevil": return "rgb(192,143,149)";
                case "Color-Deadeye": return "rgb(192,143,149)";
                case "Color-Elementalist": return "rgb(246,138,135)";
                case "Color-Tempest": return "rgb(246,138,135)";
                case "Color-Weaver": return "rgb(246,138,135)";
                case "Color-Mesmer": return "rgb(182,121,213)";
                case "Color-Chronomancer": return "rgb(182,121,213)";
                case "Color-Mirage": return "rgb(182,121,213)";
                case "Color-Necromancer": return "rgb(82,167,111)";
                case "Color-Reaper": return "rgb(82,167,111)";
                case "Color-Scourge": return "rgb(82,167,111)";
                case "Color-Boss": return "rgb(82,167,250)";

                case "Color-Warrior-NonBoss": return "rgb(125,109,66)";
                case "Color-Berserker-NonBoss": return "rgb(125,109,66)";
                case "Color-Spellbreaker-NonBoss": return "rgb(125,109,66)";
                case "Color-Guardian-NonBoss": return "rgb(62,101,113)";
                case "Color-Dragonhunter-NonBoss": return "rgb(62,101,113)";
                case "Color-Firebrand-NonBoss": return "rgb(62,101,113)";
                case "Color-Revenant-NonBoss": return "rgb(110,60,50)";
                case "Color-Herald-NonBoss": return "rgb(110,60,50)";
                case "Color-Renegade-NonBoss": return "rgb(110,60,50)";
                case "Color-Engineer-NonBoss": return "rgb(109,83,48)";
                case "Color-Scrapper-NonBoss": return "rgb(109,83,48)";
                case "Color-Holosmith-NonBoss": return "rgb(109,83,48)";
                case "Color-Ranger-NonBoss": return "rgb(75,115,70)";
                case "Color-Druid-NonBoss": return "rgb(75,115,70)";
                case "Color-Soulbeast-NonBoss": return "rgb(75,115,70)";
                case "Color-Thief-NonBoss": return "rgb(101,76,79)";
                case "Color-Daredevil-NonBoss": return "rgb(101,76,79)";
                case "Color-Deadeye-NonBoss": return "rgb(101,76,79)";
                case "Color-Elementalist-NonBoss": return "rgb(127,74,72)";
                case "Color-Tempest-NonBoss": return "rgb(127,74,72)";
                case "Color-Weaver-NonBoss": return "rgb(127,74,72)";
                case "Color-Mesmer-NonBoss": return "rgb(96,60,111)";
                case "Color-Chronomancer-NonBoss": return "rgb(96,60,111)";
                case "Color-Mirage-NonBoss": return "rgb(96,60,111)";
                case "Color-Necromancer-NonBoss": return "rgb(46,88,60)";
                case "Color-Reaper-NonBoss": return "rgb(46,88,60)";
                case "Color-Scourge-NonBoss": return "rgb(46,88,60)";
                case "Color-Boss-NonBoss": return "rgb(92,177,250)";

                case "Color-Warrior-Total": return "rgb(125,109,66)";
                case "Color-Berserker-Total": return "rgb(125,109,66)";
                case "Color-Spellbreaker-Total": return "rgb(125,109,66)";
                case "Color-Guardian-Total": return "rgb(62,101,113)";
                case "Color-Dragonhunter-Total": return "rgb(62,101,113)";
                case "Color-Firebrand-Total": return "rgb(62,101,113)";
                case "Color-Revenant-Total": return "rgb(110,60,50)";
                case "Color-Herald-Total": return "rgb(110,60,50)";
                case "Color-Renegade-Total": return "rgb(110,60,50)";
                case "Color-Engineer-Total": return "rgb(109,83,48)";
                case "Color-Scrapper-Total": return "rgb(109,83,48)";
                case "Color-Holosmith-Total": return "rgb(109,83,48)";
                case "Color-Ranger-Total": return "rgb(75,115,70)";
                case "Color-Druid-Total": return "rgb(75,115,70)";
                case "Color-Soulbeast-Total": return "rgb(75,115,70)";
                case "Color-Thief-Total": return "rgb(101,76,79)";
                case "Color-Daredevil-Total": return "rgb(101,76,79)";
                case "Color-Deadeye-Total": return "rgb(101,76,79)";
                case "Color-Elementalist-Total": return "rgb(127,74,72)";
                case "Color-Tempest-Total": return "rgb(127,74,72)";
                case "Color-Weaver-Total": return "rgb(127,74,72)";
                case "Color-Mesmer-Total": return "rgb(96,60,111)";
                case "Color-Chronomancer-Total": return "rgb(96,60,111)";
                case "Color-Mirage-Total": return "rgb(96,60,111)";
                case "Color-Necromancer-Total": return "rgb(46,88,60)";
                case "Color-Reaper-Total": return "rgb(46,88,60)";
                case "Color-Scourge-Total": return "rgb(46,88,60)";
                case "Color-Boss-Total": return "rgb(92,177,250)";

                case "Crit":
                    return "https://wiki.guildwars2.com/images/9/95/Critical_Chance.png";
                case "Scholar":
                    return "https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_the_Scholar.png";
                case "SwS":
                    return "https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png";
                case "Downs":
                    return "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png";
                case "Resurrect":
                    return "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png";
                case "Dead":
                    return "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png";
                case "Flank":
                    return "https://wiki.guildwars2.com/images/b/bb/Hunter%27s_Tactics.png";
                case "Glance":
                    return "https://wiki.guildwars2.com/images/f/f9/Weakness.png";
                case "Miss":
                    return "https://wiki.guildwars2.com/images/3/33/Blinded.png";
                case "Interupts":
                    return "https://wiki.guildwars2.com/images/7/79/Daze.png";
                case "Invuln":
                    return "https://wiki.guildwars2.com/images/e/eb/Determined.png";
                case "Blinded":
                    return "https://wiki.guildwars2.com/images/3/33/Blinded.png";
                case "Wasted":
                    return "https://wiki.guildwars2.com/images/b/b3/Out_Of_Health_Potions.png";
                case "Saved":
                    return "https://wiki.guildwars2.com/images/e/eb/Ready.png";
                case "Swap":
                    return "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png";
                case "Blank":
                    return "https://wiki.guildwars2.com/images/d/de/Sword_slot.png";
                case "Dodge":
                    return "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png";
                case "Bandage":
                    return "https://wiki.guildwars2.com/images/0/0c/Bandage.png";
                case "Stack":
                    return "https://wiki.guildwars2.com/images/e/ef/Commander_arrow_marker.png";

                case "Color-Aegis": return "rgb(102,255,255)";
                case "Color-Fury": return "rgb(255,153,0)";
                case "Color-Might": return "rgb(153,0,0)";
                case "Color-Protection": return "rgb(102,255,255)";
                case "Color-Quickness": return "rgb(255,0,255)";
                case "Color-Regeneration": return "rgb(0,204,0)";
                case "Color-Resistance": return "rgb(255, 153, 102)";
                case "Color-Retaliation": return "rgb(255, 51, 0)";
                case "Color-Stability": return "rgb(153, 102, 0)";
                case "Color-Swiftness": return "rgb(255,255,0)";
                case "Color-Vigor": return "rgb(102, 153, 0)";

                case "Color-Alacrity": return "rgb(0,102,255)";
                case "Color-Glyph of Empowerment": return "rgb(204, 153, 0)";
                case "Color-Grace of the Land": return "rgb(,,)";
                case "Color-Sun Spirit": return "rgb(255, 102, 0)";
                case "Color-Banner of Strength": return "rgb(153, 0, 0)";
                case "Color-Banner of Discipline": return "rgb(0, 51, 0)";
                case "Color-Spotter": return "rgb(0,255,0)";
                case "Color-Stone Spirit": return "rgb(204, 102, 0)";
                case "Color-Storm Spirit": return "rgb(102, 0, 102)";
                case "Color-Empower Allies": return "rgb(255, 153, 0)";

                case "Condi": return "https://wiki.guildwars2.com/images/5/54/Condition_Damage.png";
                case "Healing": return "https://wiki.guildwars2.com/images/8/81/Healing_Power.png";
                case "Tough": return "https://wiki.guildwars2.com/images/1/12/Toughness.png";
                default:
                    return "";
            }

        }
    }
}
