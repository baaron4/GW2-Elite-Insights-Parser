using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LuckParser.EIData;
using LuckParser.Parser;
using LuckParser.Parser.ParsedData;
using static LuckParser.Parser.ParseEnum.TrashIDS;

namespace LuckParser
{
    public static class GeneralHelper
    {
        public static int PollingRate = 150;

        public static int BoonDigit = 2;
        public static int TimeDigit = 3;

        public static AgentItem UnknownAgent = new AgentItem();
        // use this for "null" in AbstractActor dictionaries
        public static Mob NullActor = new Mob(UnknownAgent);

        public static UTF8Encoding NoBOMEncodingUTF8 = new UTF8Encoding(false);

        public static void Add<K, T>(Dictionary<K, List<T>> dict, K key, T evt)
        {
            if (dict.TryGetValue(key, out List<T> list))
            {
                list.Add(evt);
            }
            else
            {
                dict[key] = new List<T>()
                {
                    evt
                };
            }
        }

        public static string UppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static T MaxBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
        {
            return en.Select(t => (value: t, eval: evaluate(t)))
                .Aggregate((max, next) => next.eval.CompareTo(max.eval) > 0 ? next : max).value;
        }

        public static T MinBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
        {
            return en.Select(t => (value: t, eval: evaluate(t)))
                .Aggregate((max, next) => next.eval.CompareTo(max.eval) < 0 ? next : max).value;
        }


        public static string FindPattern(string source, string regex)
        {
            if (string.IsNullOrEmpty(source))
            {
                return null;
            }

            Match match = Regex.Match(source, regex);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        public static string GetProfIcon(string prof)
        {
            return prof switch
            {
                "Warrior" => "https://wiki.guildwars2.com/images/4/43/Warrior_tango_icon_20px.png",
                "Berserker" => "https://wiki.guildwars2.com/images/d/da/Berserker_tango_icon_20px.png",
                "Spellbreaker" => "https://wiki.guildwars2.com/images/e/ed/Spellbreaker_tango_icon_20px.png",
                "Guardian" => "https://wiki.guildwars2.com/images/8/8c/Guardian_tango_icon_20px.png",
                "Dragonhunter" => "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png",
                "DragonHunter" => "https://wiki.guildwars2.com/images/c/c9/Dragonhunter_tango_icon_20px.png",
                "Firebrand" => "https://wiki.guildwars2.com/images/0/02/Firebrand_tango_icon_20px.png",
                "Revenant" => "https://wiki.guildwars2.com/images/b/b5/Revenant_tango_icon_20px.png",
                "Herald" => "https://wiki.guildwars2.com/images/6/67/Herald_tango_icon_20px.png",
                "Renegade" => "https://wiki.guildwars2.com/images/7/7c/Renegade_tango_icon_20px.png",
                "Engineer" => "https://wiki.guildwars2.com/images/2/27/Engineer_tango_icon_20px.png",
                "Scrapper" => "https://wiki.guildwars2.com/images/3/3a/Scrapper_tango_icon_200px.png",
                "Holosmith" => "https://wiki.guildwars2.com/images/2/28/Holosmith_tango_icon_20px.png",
                "Ranger" => "https://wiki.guildwars2.com/images/4/43/Ranger_tango_icon_20px.png",
                "Druid" => "https://wiki.guildwars2.com/images/d/d2/Druid_tango_icon_20px.png",
                "Soulbeast" => "https://wiki.guildwars2.com/images/7/7c/Soulbeast_tango_icon_20px.png",
                "Thief" => "https://wiki.guildwars2.com/images/7/7a/Thief_tango_icon_20px.png",
                "Daredevil" => "https://wiki.guildwars2.com/images/e/e1/Daredevil_tango_icon_20px.png",
                "Deadeye" => "https://wiki.guildwars2.com/images/c/c9/Deadeye_tango_icon_20px.png",
                "Elementalist" => "https://wiki.guildwars2.com/images/a/aa/Elementalist_tango_icon_20px.png",
                "Tempest" => "https://wiki.guildwars2.com/images/4/4a/Tempest_tango_icon_20px.png",
                "Weaver" => "https://wiki.guildwars2.com/images/f/fc/Weaver_tango_icon_20px.png",
                "Mesmer" => "https://wiki.guildwars2.com/images/6/60/Mesmer_tango_icon_20px.png",
                "Chronomancer" => "https://wiki.guildwars2.com/images/f/f4/Chronomancer_tango_icon_20px.png",
                "Mirage" => "https://wiki.guildwars2.com/images/d/df/Mirage_tango_icon_20px.png",
                "Necromancer" => "https://wiki.guildwars2.com/images/4/43/Necromancer_tango_icon_20px.png",
                "Reaper" => "https://wiki.guildwars2.com/images/1/11/Reaper_tango_icon_20px.png",
                "Scourge" => "https://wiki.guildwars2.com/images/0/06/Scourge_tango_icon_20px.png",
                "Sword" => "https://wiki.guildwars2.com/images/0/07/Crimson_Antique_Blade.png",
                _ => "",
            };
        }

        public static string GetNPCIcon(ushort id)
        {
            switch (ParseEnum.GetTargetIDS(id))
            {
                case ParseEnum.TargetIDS.WorldVersusWorld:
                    return "https://wiki.guildwars2.com/images/d/db/PvP_Server_Browser_%28map_icon%29.png";
                case ParseEnum.TargetIDS.ValeGuardian:
                    return "https://i.imgur.com/MIpP5pK.png";
                case ParseEnum.TargetIDS.Gorseval:
                    return "https://i.imgur.com/5hmMq12.png";
                case ParseEnum.TargetIDS.Sabetha:
                    return "https://i.imgur.com/UqbFp9S.png";
                case ParseEnum.TargetIDS.Slothasor:
                    return "https://i.imgur.com/h1xH3ER.png";
                case ParseEnum.TargetIDS.Berg:
                    return "https://i.imgur.com/tLMXqL7.png";
                case ParseEnum.TargetIDS.Narella:
                    return "https://i.imgur.com/FwMCoR0.png";
                case ParseEnum.TargetIDS.Zane:
                    return "https://i.imgur.com/tkPWMST.png";
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
                case ParseEnum.TargetIDS.Desmina:
                    return "https://i.imgur.com/jAiRplg.png";
                case ParseEnum.TargetIDS.BrokenKing:
                    return "https://i.imgur.com/FNgUmvL.png";
                case ParseEnum.TargetIDS.SoulEater:
                    return "https://i.imgur.com/Sd6Az8M.png";
                case ParseEnum.TargetIDS.EyeOfFate:
                case ParseEnum.TargetIDS.EyeOfJudgement:
                    return "https://i.imgur.com/kAgdoa5.png";
                case ParseEnum.TargetIDS.Dhuum:
                    return "https://i.imgur.com/RKaDon5.png";
                case ParseEnum.TargetIDS.ConjuredAmalgamate:
                    return "https://i.imgur.com/C23rYTl.png";
                case ParseEnum.TargetIDS.CALeftArm:
                    return "https://i.imgur.com/qrkQvEY.png";
                case ParseEnum.TargetIDS.CARightArm:
                    return "https://i.imgur.com/MVwjtH7.png";
                case ParseEnum.TargetIDS.Kenut:
                    return "https://i.imgur.com/6yq45Cc.png";
                case ParseEnum.TargetIDS.Nikare:
                    return "https://i.imgur.com/TLykcrJ.png";
                case ParseEnum.TargetIDS.Qadim:
                    return "https://i.imgur.com/IfoHTHT.png";
                case ParseEnum.TargetIDS.Freezie:
                    return "https://wiki.guildwars2.com/images/d/d9/Mini_Freezie.png";
                case ParseEnum.TargetIDS.Adina:
                    return "https://i.imgur.com/or3m1yb.png";
                case ParseEnum.TargetIDS.Sabir:
                    return "https://i.imgur.com/Q4WUXqw.png";
                case ParseEnum.TargetIDS.PeerlessQadim:
                    return "https://i.imgur.com/47uePpb.png";
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
            switch (ParseEnum.GetTrashIDS(id))
            {
                case Spirit:
                case Spirit2:
                case ChargedSoul:
                case HollowedBomber:
                    return "https://i.imgur.com/sHmksvO.png";
                case Saul:
                    return "https://i.imgur.com/ck2IsoS.png";
                case GamblerClones:
                    return "https://i.imgur.com/zMsBWEx.png";
                case GamblerReal:
                    return "https://i.imgur.com/J6oMITN.png";
                case Pride:
                    return "https://i.imgur.com/ePTXx23.png";
                case OilSlick:
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
                case EnergyOrb:
                    return "https://i.postimg.cc/NMNvyts0/Power-Ball.png";
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
                case BanditSaboteur:
                    return "https://i.imgur.com/jUKMEbD.png";
                case NarellaTornado:
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
                case SmallKillerTornado:
                case BigKillerTornado:
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
                case BanditBombardier:
                case SurgingSoul:
                case MazeMinotaur:
                case Enervator:
                    return "https://i.imgur.com/k79t7ZA.png";
                case HandOfErosion:
                case HandOfEruption:
                    return "https://i.imgur.com/reGQHhr.png";
                case VoltaicWisp:
                    return "https://i.imgur.com/C1mvNGZ.png";
                case ParalyzingWisp:
                    return "https://i.imgur.com/YBl8Pqo.png";
                case Pylon2:
                    return "https://i.imgur.com/b33vAEQ.png";
                case EntropicDistortion:
                    return "https://i.imgur.com/MIpP5pK.png";
                case SmallJumpyTornado:
                    return "https://i.imgur.com/WBJNgp7.png";
                case OrbSpider:
                    return "https://i.imgur.com/FB5VM9X.png";
                case Seekers:
                    return "https://i.imgur.com/FrPoluz.png";
                case BlueGuardian:
                    return "https://i.imgur.com/6CefnkP.png";
                case GreenGuardian:
                    return "https://i.imgur.com/nauDVYP.png";
                case RedGuardian:
                    return "https://i.imgur.com/73Uj4lG.png";
                case UnderworldReaper:
                    return "https://i.imgur.com/Tq6SYVe.png";
                case CagedWarg:
                case GreenSpirit1:
                case GreenSpirit2:
                case BanditSapper:
                case ProjectionArkk:
                case Prisoner1:
                case Prisoner2:
                case Pylon1:
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
                case FreeziesFrozenHeart:
                    return "https://wiki.guildwars2.com/images/9/9e/Mini_Freezie%27s_Heart.png";
                case RiverOfSouls:
                    return "https://i.imgur.com/4pXEnaX.png";
                case DhuumDesmina:
                    return "https://i.imgur.com/jAiRplg.png";
            }
            return "https://i.imgur.com/HuJHqRZ.png";
        }


        public static string GetLink(string name)
        {
            return name switch
            {
                "Question" => "https://wiki.guildwars2.com/images/d/de/Sword_slot.png",
                "Sword" => "https://wiki.guildwars2.com/images/0/07/Crimson_Antique_Blade.png",
                "Axe" => "https://wiki.guildwars2.com/images/d/d4/Crimson_Antique_Reaver.png",
                "Dagger" => "https://wiki.guildwars2.com/images/6/65/Crimson_Antique_Razor.png",
                "Mace" => "https://wiki.guildwars2.com/images/6/6d/Crimson_Antique_Flanged_Mace.png",
                "Pistol" => "https://wiki.guildwars2.com/images/4/46/Crimson_Antique_Revolver.png",
                "Scepter" => "https://wiki.guildwars2.com/images/e/e2/Crimson_Antique_Wand.png",
                "Focus" => "https://wiki.guildwars2.com/images/8/87/Crimson_Antique_Artifact.png",
                "Shield" => "https://wiki.guildwars2.com/images/b/b0/Crimson_Antique_Bastion.png",
                "Torch" => "https://wiki.guildwars2.com/images/7/76/Crimson_Antique_Brazier.png",
                "Warhorn" => "https://wiki.guildwars2.com/images/1/1c/Crimson_Antique_Herald.png",
                "Greatsword" => "https://wiki.guildwars2.com/images/5/50/Crimson_Antique_Claymore.png",
                "Hammer" => "https://wiki.guildwars2.com/images/3/38/Crimson_Antique_Warhammer.png",
                "Longbow" => "https://wiki.guildwars2.com/images/f/f0/Crimson_Antique_Greatbow.png",
                "Shortbow" => "https://wiki.guildwars2.com/images/1/17/Crimson_Antique_Short_Bow.png",
                "Rifle" => "https://wiki.guildwars2.com/images/1/19/Crimson_Antique_Musket.png",
                "Staff" => "https://wiki.guildwars2.com/images/5/5f/Crimson_Antique_Spire.png",

                "Color-Warrior" => "rgb(255,209,102)",
                "Color-Berserker" => "rgb(255,209,102)",
                "Color-Spellbreaker" => "rgb(255,209,102)",
                "Color-Guardian" => "rgb(114,193,217)",
                "Color-Dragonhunter" => "rgb(114,193,217)",
                "Color-Firebrand" => "rgb(114,193,217)",
                "Color-Revenant" => "rgb(209,110,90)",
                "Color-Herald" => "rgb(209,110,90)",
                "Color-Renegade" => "rgb(209,110,90)",
                "Color-Engineer" => "rgb(208,156,89)",
                "Color-Scrapper" => "rgb(208,156,89)",
                "Color-Holosmith" => "rgb(208,156,89)",
                "Color-Ranger" => "rgb(140,220,130)",
                "Color-Druid" => "rgb(140,220,130)",
                "Color-Soulbeast" => "rgb(140,220,130)",
                "Color-Thief" => "rgb(192,143,149)",
                "Color-Daredevil" => "rgb(192,143,149)",
                "Color-Deadeye" => "rgb(192,143,149)",
                "Color-Elementalist" => "rgb(246,138,135)",
                "Color-Tempest" => "rgb(246,138,135)",
                "Color-Weaver" => "rgb(246,138,135)",
                "Color-Mesmer" => "rgb(182,121,213)",
                "Color-Chronomancer" => "rgb(182,121,213)",
                "Color-Mirage" => "rgb(182,121,213)",
                "Color-Necromancer" => "rgb(82,167,111)",
                "Color-Reaper" => "rgb(82,167,111)",
                "Color-Scourge" => "rgb(82,167,111)",
                "Color-Boss" => "rgb(82,167,250)",

                "Color-Warrior-NonBoss" => "rgb(125,109,66)",
                "Color-Berserker-NonBoss" => "rgb(125,109,66)",
                "Color-Spellbreaker-NonBoss" => "rgb(125,109,66)",
                "Color-Guardian-NonBoss" => "rgb(62,101,113)",
                "Color-Dragonhunter-NonBoss" => "rgb(62,101,113)",
                "Color-Firebrand-NonBoss" => "rgb(62,101,113)",
                "Color-Revenant-NonBoss" => "rgb(110,60,50)",
                "Color-Herald-NonBoss" => "rgb(110,60,50)",
                "Color-Renegade-NonBoss" => "rgb(110,60,50)",
                "Color-Engineer-NonBoss" => "rgb(109,83,48)",
                "Color-Scrapper-NonBoss" => "rgb(109,83,48)",
                "Color-Holosmith-NonBoss" => "rgb(109,83,48)",
                "Color-Ranger-NonBoss" => "rgb(75,115,70)",
                "Color-Druid-NonBoss" => "rgb(75,115,70)",
                "Color-Soulbeast-NonBoss" => "rgb(75,115,70)",
                "Color-Thief-NonBoss" => "rgb(101,76,79)",
                "Color-Daredevil-NonBoss" => "rgb(101,76,79)",
                "Color-Deadeye-NonBoss" => "rgb(101,76,79)",
                "Color-Elementalist-NonBoss" => "rgb(127,74,72)",
                "Color-Tempest-NonBoss" => "rgb(127,74,72)",
                "Color-Weaver-NonBoss" => "rgb(127,74,72)",
                "Color-Mesmer-NonBoss" => "rgb(96,60,111)",
                "Color-Chronomancer-NonBoss" => "rgb(96,60,111)",
                "Color-Mirage-NonBoss" => "rgb(96,60,111)",
                "Color-Necromancer-NonBoss" => "rgb(46,88,60)",
                "Color-Reaper-NonBoss" => "rgb(46,88,60)",
                "Color-Scourge-NonBoss" => "rgb(46,88,60)",
                "Color-Boss-NonBoss" => "rgb(92,177,250)",

                "Color-Warrior-Total" => "rgb(125,109,66)",
                "Color-Berserker-Total" => "rgb(125,109,66)",
                "Color-Spellbreaker-Total" => "rgb(125,109,66)",
                "Color-Guardian-Total" => "rgb(62,101,113)",
                "Color-Dragonhunter-Total" => "rgb(62,101,113)",
                "Color-Firebrand-Total" => "rgb(62,101,113)",
                "Color-Revenant-Total" => "rgb(110,60,50)",
                "Color-Herald-Total" => "rgb(110,60,50)",
                "Color-Renegade-Total" => "rgb(110,60,50)",
                "Color-Engineer-Total" => "rgb(109,83,48)",
                "Color-Scrapper-Total" => "rgb(109,83,48)",
                "Color-Holosmith-Total" => "rgb(109,83,48)",
                "Color-Ranger-Total" => "rgb(75,115,70)",
                "Color-Druid-Total" => "rgb(75,115,70)",
                "Color-Soulbeast-Total" => "rgb(75,115,70)",
                "Color-Thief-Total" => "rgb(101,76,79)",
                "Color-Daredevil-Total" => "rgb(101,76,79)",
                "Color-Deadeye-Total" => "rgb(101,76,79)",
                "Color-Elementalist-Total" => "rgb(127,74,72)",
                "Color-Tempest-Total" => "rgb(127,74,72)",
                "Color-Weaver-Total" => "rgb(127,74,72)",
                "Color-Mesmer-Total" => "rgb(96,60,111)",
                "Color-Chronomancer-Total" => "rgb(96,60,111)",
                "Color-Mirage-Total" => "rgb(96,60,111)",
                "Color-Necromancer-Total" => "rgb(46,88,60)",
                "Color-Reaper-Total" => "rgb(46,88,60)",
                "Color-Scourge-Total" => "rgb(46,88,60)",
                "Color-Boss-Total" => "rgb(92,177,250)",

                "Crit" => "https://wiki.guildwars2.com/images/9/95/Critical_Chance.png",
                "Scholar" => "https://wiki.guildwars2.com/images/2/2b/Superior_Rune_of_the_Scholar.png",
                "SwS" => "https://wiki.guildwars2.com/images/1/1c/Bowl_of_Seaweed_Salad.png",
                "Downs" => "https://wiki.guildwars2.com/images/c/c6/Downed_enemy.png",
                "Resurrect" => "https://wiki.guildwars2.com/images/3/3d/Downed_ally.png",
                "Dead" => "https://wiki.guildwars2.com/images/4/4a/Ally_death_%28interface%29.png",
                "Flank" => "https://wiki.guildwars2.com/images/b/bb/Hunter%27s_Tactics.png",
                "Glance" => "https://wiki.guildwars2.com/images/f/f9/Weakness.png",
                "Miss" => "https://wiki.guildwars2.com/images/3/33/Blinded.png",
                "Interupts" => "https://wiki.guildwars2.com/images/7/79/Daze.png",
                "Invuln" => "https://wiki.guildwars2.com/images/e/eb/Determined.png",
                "Blinded" => "https://wiki.guildwars2.com/images/3/33/Blinded.png",
                "Wasted" => "https://wiki.guildwars2.com/images/b/b3/Out_Of_Health_Potions.png",
                "Saved" => "https://wiki.guildwars2.com/images/e/eb/Ready.png",
                "Swap" => "https://wiki.guildwars2.com/images/c/ce/Weapon_Swap_Button.png",
                "Blank" => "https://wiki.guildwars2.com/images/d/de/Sword_slot.png",
                "Dodge" => "https://wiki.guildwars2.com/images/archive/b/b2/20150601155307%21Dodge.png",
                "Bandage" => "https://wiki.guildwars2.com/images/0/0c/Bandage.png",
                "Stack" => "https://wiki.guildwars2.com/images/e/ef/Commander_arrow_marker.png",

                "Color-Aegis" => "rgb(102,255,255)",
                "Color-Fury" => "rgb(255,153,0)",
                "Color-Might" => "rgb(153,0,0)",
                "Color-Protection" => "rgb(102,255,255)",
                "Color-Quickness" => "rgb(255,0,255)",
                "Color-Regeneration" => "rgb(0,204,0)",
                "Color-Resistance" => "rgb(255, 153, 102)",
                "Color-Retaliation" => "rgb(255, 51, 0)",
                "Color-Stability" => "rgb(153, 102, 0)",
                "Color-Swiftness" => "rgb(255,255,0)",
                "Color-Vigor" => "rgb(102, 153, 0)",

                "Color-Alacrity" => "rgb(0,102,255)",
                "Color-Glyph of Empowerment" => "rgb(204, 153, 0)",
                "Color-Grace of the Land" => "rgb(,,)",
                "Color-Sun Spirit" => "rgb(255, 102, 0)",
                "Color-Banner of Strength" => "rgb(153, 0, 0)",
                "Color-Banner of Discipline" => "rgb(0, 51, 0)",
                "Color-Spotter" => "rgb(0,255,0)",
                "Color-Stone Spirit" => "rgb(204, 102, 0)",
                "Color-Storm Spirit" => "rgb(102, 0, 102)",
                "Color-Empower Allies" => "rgb(255, 153, 0)",

                "Condi" => "https://wiki.guildwars2.com/images/5/54/Condition_Damage.png",
                "Healing" => "https://wiki.guildwars2.com/images/8/81/Healing_Power.png",
                "Tough" => "https://wiki.guildwars2.com/images/1/12/Toughness.png",
                _ => "",
            };
        }
    }
}
