using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GW2EIEvtcParser
{
    public static class ParserHelper
    {

        internal static AgentItem _unknownAgent = new AgentItem();
        // use this for "null" in AbstractActor dictionaries
        internal static NPC _nullActor = new NPC(_unknownAgent);

        internal static void SafeSkip(Stream stream, long bytesToSkip)
        {
            if (stream.CanSeek)
            {
                stream.Seek(bytesToSkip, SeekOrigin.Current);
            }
            else
            {
                while (bytesToSkip > 0)
                {
                    stream.ReadByte();
                    --bytesToSkip;
                }
            }
        }


        internal const int PollingRate = 150;

        internal const int BuffDigit = 3;
        internal const int TimeDigit = 3;

        internal const long ServerDelayConstant = 10;
        internal const long BuffSimulatorDelayConstant = 50;

        internal const int PhaseTimeLimit = 1000;


        public enum Source
        {
            Common,
            Item,
            Necromancer, Reaper, Scourge,
            Elementalist, Tempest, Weaver,
            Mesmer, Chronomancer, Mirage,
            Warrior, Berserker, Spellbreaker,
            Revenant, Herald, Renegade,
            Guardian, Dragonhunter, Firebrand,
            Thief, Daredevil, Deadeye,
            Ranger, Druid, Soulbeast,
            Engineer, Scrapper, Holosmith,
            FightSpecific,
            FractalInstability,
            Unknown
        };

        internal static T MaxBy<T, TComparable>(this IEnumerable<T> en, Func<T, TComparable> evaluate) where TComparable : IComparable<TComparable>
        {
            return en.Select(t => (value: t, eval: evaluate(t)))
                .Aggregate((max, next) => next.eval.CompareTo(max.eval) > 0 ? next : max).value;
        }

        internal static T MinBy<T, TComparable>(this IEnumerable<T> en, Func<T, TComparable> evaluate) where TComparable : IComparable<TComparable>
        {
            return en.Select(t => (value: t, eval: evaluate(t)))
                .Aggregate((max, next) => next.eval.CompareTo(max.eval) < 0 ? next : max).value;
        }

        /*
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
        */
        public static List<Source> ProfToEnum(string prof)
        {
            switch (prof)
            {
                case "Druid":
                    return new List<Source> { Source.Ranger, Source.Druid };
                case "Soulbeast":
                    return new List<Source> { Source.Ranger, Source.Soulbeast };
                case "Ranger":
                    return new List<Source> { Source.Ranger };
                case "Scrapper":
                    return new List<Source> { Source.Engineer, Source.Scrapper };
                case "Holosmith":
                    return new List<Source> { Source.Engineer, Source.Holosmith };
                case "Engineer":
                    return new List<Source> { Source.Engineer };
                case "Daredevil":
                    return new List<Source> { Source.Thief, Source.Daredevil };
                case "Deadeye":
                    return new List<Source> { Source.Thief, Source.Deadeye };
                case "Thief":
                    return new List<Source> { Source.Thief };
                case "Weaver":
                    return new List<Source> { Source.Elementalist, Source.Weaver };
                case "Tempest":
                    return new List<Source> { Source.Elementalist, Source.Tempest };
                case "Elementalist":
                    return new List<Source> { Source.Elementalist };
                case "Mirage":
                    return new List<Source> { Source.Mesmer, Source.Mirage };
                case "Chronomancer":
                    return new List<Source> { Source.Mesmer, Source.Chronomancer };
                case "Mesmer":
                    return new List<Source> { Source.Mesmer };
                case "Scourge":
                    return new List<Source> { Source.Necromancer, Source.Scourge };
                case "Reaper":
                    return new List<Source> { Source.Necromancer, Source.Reaper };
                case "Necromancer":
                    return new List<Source> { Source.Necromancer };
                case "Spellbreaker":
                    return new List<Source> { Source.Warrior, Source.Spellbreaker };
                case "Berserker":
                    return new List<Source> { Source.Warrior, Source.Berserker };
                case "Warrior":
                    return new List<Source> { Source.Warrior };
                case "Firebrand":
                    return new List<Source> { Source.Guardian, Source.Firebrand };
                case "Dragonhunter":
                    return new List<Source> { Source.Guardian, Source.Dragonhunter };
                case "Guardian":
                    return new List<Source> { Source.Guardian };
                case "Renegade":
                    return new List<Source> { Source.Revenant, Source.Renegade };
                case "Herald":
                    return new List<Source> { Source.Revenant, Source.Herald };
                case "Revenant":
                    return new List<Source> { Source.Revenant };
            }
            return new List<Source> { Source.Unknown };
        }

        internal static string GetProfIcon(string prof)
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
        internal static string GetNPCIcon(int id)
        {
            switch (ArcDPSEnums.GetTargetID(id))
            {
                case ArcDPSEnums.TargetID.WorldVersusWorld:
                    return "https://wiki.guildwars2.com/images/d/db/PvP_Server_Browser_%28map_icon%29.png";
                case ArcDPSEnums.TargetID.ValeGuardian:
                    return "https://i.imgur.com/MIpP5pK.png";
                case ArcDPSEnums.TargetID.Gorseval:
                    return "https://i.imgur.com/5hmMq12.png";
                case ArcDPSEnums.TargetID.Sabetha:
                    return "https://i.imgur.com/UqbFp9S.png";
                case ArcDPSEnums.TargetID.Slothasor:
                    return "https://i.imgur.com/h1xH3ER.png";
                case ArcDPSEnums.TargetID.Berg:
                    return "https://i.imgur.com/tLMXqL7.png";
                case ArcDPSEnums.TargetID.Narella:
                    return "https://i.imgur.com/FwMCoR0.png";
                case ArcDPSEnums.TargetID.Zane:
                    return "https://i.imgur.com/tkPWMST.png";
                case ArcDPSEnums.TargetID.Matthias:
                    return "https://i.imgur.com/3uMMmTS.png";
                case ArcDPSEnums.TargetID.KeepConstruct:
                    return "https://i.imgur.com/Kq0kL07.png";
                case ArcDPSEnums.TargetID.Xera:
                    return "https://i.imgur.com/lYwJEyV.png";
                case ArcDPSEnums.TargetID.Cairn:
                    return "https://i.imgur.com/gQY37Tf.png";
                case ArcDPSEnums.TargetID.MursaatOverseer:
                    return "https://i.imgur.com/5LNiw4Y.png";
                case ArcDPSEnums.TargetID.Samarog:
                    return "https://i.imgur.com/MPQhKfM.png";
                case ArcDPSEnums.TargetID.Deimos:
                    return "https://i.imgur.com/mWfxBaO.png";
                case ArcDPSEnums.TargetID.SoullessHorror:
                case ArcDPSEnums.TargetID.Desmina:
                    return "https://i.imgur.com/jAiRplg.png";
                case ArcDPSEnums.TargetID.BrokenKing:
                    return "https://i.imgur.com/FNgUmvL.png";
                case ArcDPSEnums.TargetID.SoulEater:
                    return "https://i.imgur.com/Sd6Az8M.png";
                case ArcDPSEnums.TargetID.EyeOfFate:
                case ArcDPSEnums.TargetID.EyeOfJudgement:
                    return "https://i.imgur.com/kAgdoa5.png";
                case ArcDPSEnums.TargetID.Dhuum:
                    return "https://i.imgur.com/RKaDon5.png";
                case ArcDPSEnums.TargetID.ConjuredAmalgamate:
                    return "https://i.imgur.com/C23rYTl.png";
                case ArcDPSEnums.TargetID.CALeftArm:
                    return "https://i.imgur.com/qrkQvEY.png";
                case ArcDPSEnums.TargetID.CARightArm:
                    return "https://i.imgur.com/MVwjtH7.png";
                case ArcDPSEnums.TargetID.Kenut:
                    return "https://i.imgur.com/6yq45Cc.png";
                case ArcDPSEnums.TargetID.Nikare:
                    return "https://i.imgur.com/TLykcrJ.png";
                case ArcDPSEnums.TargetID.Qadim:
                    return "https://i.imgur.com/IfoHTHT.png";
                case ArcDPSEnums.TargetID.Freezie:
                    return "https://wiki.guildwars2.com/images/d/d9/Mini_Freezie.png";
                case ArcDPSEnums.TargetID.Adina:
                    return "https://i.imgur.com/or3m1yb.png";
                case ArcDPSEnums.TargetID.Sabir:
                    return "https://i.imgur.com/Q4WUXqw.png";
                case ArcDPSEnums.TargetID.PeerlessQadim:
                    return "https://i.imgur.com/47uePpb.png";
                case ArcDPSEnums.TargetID.IcebroodConstruct:
                case ArcDPSEnums.TargetID.IcebroodConstructFraenir:
                    return "https://i.imgur.com/dpaZFa5.png";
                case ArcDPSEnums.TargetID.ClawOfTheFallen:
                    return "https://i.imgur.com/HF85QpV.png";
                case ArcDPSEnums.TargetID.VoiceOfTheFallen:
                    return "https://i.imgur.com/BdTGXMU.png";
                case ArcDPSEnums.TargetID.VoiceAndClaw:
                    return "https://i.imgur.com/V1rJBnq.png";
                case ArcDPSEnums.TargetID.FraenirOfJormag:
                    return "https://i.imgur.com/MxudnKp.png";
                case ArcDPSEnums.TargetID.Boneskinner:
                    return "https://i.imgur.com/7HPdKDQ.png";
                case ArcDPSEnums.TargetID.WhisperOfJormag:
                    return "https://i.imgur.com/lu9ZLVq.png";
                case ArcDPSEnums.TargetID.VariniaStormsounder:
                    return "https://i.imgur.com/2o8TtiM.png";
                case ArcDPSEnums.TargetID.MAMA:
                    return "https://i.imgur.com/1h7HOII.png";
                case ArcDPSEnums.TargetID.Siax:
                    return "https://i.imgur.com/5C60cQb.png";
                case ArcDPSEnums.TargetID.Ensolyss:
                    return "https://i.imgur.com/GUTNuyP.png";
                case ArcDPSEnums.TargetID.Skorvald:
                    return "https://i.imgur.com/IOPAHRE.png";
                case ArcDPSEnums.TargetID.Artsariiv:
                    return "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
                case ArcDPSEnums.TargetID.Arkk:
                    return "https://i.imgur.com/u6vv8cW.png";
                case ArcDPSEnums.TargetID.AiKeeperOfThePeak:
                    return "https://i.imgur.com/eCXjoAS.png";
                case ArcDPSEnums.TargetID.AiKeeperOfThePeak2:
                    return "https://i.imgur.com/I8nwhAw.png";
                case ArcDPSEnums.TargetID.LGolem:
                    return "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                case ArcDPSEnums.TargetID.AvgGolem:
                    return "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                case ArcDPSEnums.TargetID.StdGolem:
                    return "https://wiki.guildwars2.com/images/8/8f/Mini_Professor_Mew.png";
                case ArcDPSEnums.TargetID.MassiveGolem:
                    return "https://wiki.guildwars2.com/images/3/33/Mini_Snuggles.png";
                case ArcDPSEnums.TargetID.MedGolem:
                    return "https://wiki.guildwars2.com/images/c/cb/Mini_Mister_Mittens.png";
                case ArcDPSEnums.TargetID.TwistedCastle:
                    return "https://i.imgur.com/ZBm5Uga.png";
            }
            switch (ArcDPSEnums.GetTrashID(id))
            {
                case ArcDPSEnums.TrashID.Spirit:
                case ArcDPSEnums.TrashID.Spirit2:
                case ArcDPSEnums.TrashID.ChargedSoul:
                case ArcDPSEnums.TrashID.HollowedBomber:
                case ArcDPSEnums.TrashID.GuiltDemon:
                case ArcDPSEnums.TrashID.DoubtDemon:
                    return "https://i.imgur.com/sHmksvO.png";
                case ArcDPSEnums.TrashID.Saul:
                    return "https://i.imgur.com/ck2IsoS.png";
                case ArcDPSEnums.TrashID.GamblerClones:
                    return "https://i.imgur.com/zMsBWEx.png";
                case ArcDPSEnums.TrashID.GamblerReal:
                    return "https://i.imgur.com/J6oMITN.png";
                case ArcDPSEnums.TrashID.Pride:
                    return "https://i.imgur.com/ePTXx23.png";
                case ArcDPSEnums.TrashID.OilSlick:
                case ArcDPSEnums.TrashID.Oil:
                    return "https://i.imgur.com/R26VgEr.png";
                case ArcDPSEnums.TrashID.Tear:
                    return "https://i.imgur.com/N9seps0.png";
                case ArcDPSEnums.TrashID.Gambler:
                case ArcDPSEnums.TrashID.Drunkard:
                case ArcDPSEnums.TrashID.Thief:
                    return "https://i.imgur.com/vINeVU6.png";
                case ArcDPSEnums.TrashID.TormentedDead:
                case ArcDPSEnums.TrashID.Messenger:
                    return "https://i.imgur.com/1J2BTFg.png";
                case ArcDPSEnums.TrashID.Enforcer:
                    return "https://i.imgur.com/elHjamF.png";
                case ArcDPSEnums.TrashID.Echo:
                    return "https://i.imgur.com/kcN9ECn.png";
                case ArcDPSEnums.TrashID.Core:
                case ArcDPSEnums.TrashID.ExquisiteConjunction:
                    return "https://i.imgur.com/yI34iqw.png";
                case ArcDPSEnums.TrashID.Jessica:
                case ArcDPSEnums.TrashID.Olson:
                case ArcDPSEnums.TrashID.Engul:
                case ArcDPSEnums.TrashID.Faerla:
                case ArcDPSEnums.TrashID.Caulle:
                case ArcDPSEnums.TrashID.Henley:
                case ArcDPSEnums.TrashID.Galletta:
                case ArcDPSEnums.TrashID.Ianim:
                    return "https://i.imgur.com/qeYT1Bf.png";
                case ArcDPSEnums.TrashID.InsidiousProjection:
                    return "https://i.imgur.com/9EdItBS.png";
                case ArcDPSEnums.TrashID.EnergyOrb:
                    return "https://i.postimg.cc/NMNvyts0/Power-Ball.png";
                case ArcDPSEnums.TrashID.UnstableLeyRift:
                    return "https://i.imgur.com/YXM3igs.png";
                case ArcDPSEnums.TrashID.RadiantPhantasm:
                    return "https://i.imgur.com/O5VWLyY.png";
                case ArcDPSEnums.TrashID.CrimsonPhantasm:
                    return "https://i.imgur.com/zP7Bvb4.png";
                case ArcDPSEnums.TrashID.Storm:
                    return "https://i.imgur.com/9XtNPdw.png";
                case ArcDPSEnums.TrashID.IcePatch:
                    return "https://i.imgur.com/yxKJ5Yc.png";
                case ArcDPSEnums.TrashID.BanditSaboteur:
                    return "https://i.imgur.com/jUKMEbD.png";
                case ArcDPSEnums.TrashID.NarellaTornado:
                case ArcDPSEnums.TrashID.Tornado:
                    return "https://i.imgur.com/e10lZMa.png";
                case ArcDPSEnums.TrashID.Jade:
                    return "https://i.imgur.com/ivtzbSP.png";
                case ArcDPSEnums.TrashID.Zommoros:
                    return "https://i.imgur.com/BxbsRCI.png";
                case ArcDPSEnums.TrashID.AncientInvokedHydra:
                    return "https://i.imgur.com/YABLiBz.png";
                case ArcDPSEnums.TrashID.IcebornHydra:
                    return "https://i.imgur.com/LoYMBRU.png";
                case ArcDPSEnums.TrashID.IceElemental:
                    return "https://i.imgur.com/pEkBeNp.png";
                case ArcDPSEnums.TrashID.WyvernMatriarch:
                    return "https://i.imgur.com/kLKLSfv.png";
                case ArcDPSEnums.TrashID.WyvernPatriarch:
                    return "https://i.imgur.com/vjjNSpI.png";
                case ArcDPSEnums.TrashID.ApocalypseBringer:
                    return "https://i.imgur.com/0LGKCn2.png";
                case ArcDPSEnums.TrashID.ConjuredGreatsword:
                    return "https://i.imgur.com/vHka0QN.png";
                case ArcDPSEnums.TrashID.ConjuredShield:
                    return "https://i.imgur.com/wUiI19S.png";
                case ArcDPSEnums.TrashID.GreaterMagmaElemental1:
                case ArcDPSEnums.TrashID.GreaterMagmaElemental2:
                    return "https://i.imgur.com/sr146T6.png";
                case ArcDPSEnums.TrashID.LavaElemental1:
                case ArcDPSEnums.TrashID.LavaElemental2:
                    return "https://i.imgur.com/mydwiYy.png";
                case ArcDPSEnums.TrashID.PyreGuardian:
                case ArcDPSEnums.TrashID.SmallKillerTornado:
                case ArcDPSEnums.TrashID.BigKillerTornado:
                    return "https://i.imgur.com/6zNPTUw.png";
                case ArcDPSEnums.TrashID.QadimLamp:
                    return "https://i.imgur.com/89Kjv0N.png";
                case ArcDPSEnums.TrashID.PyreGuardianRetal:
                    return "https://i.imgur.com/WC6LRkO.png";
                case ArcDPSEnums.TrashID.PyreGuardianStab:
                    return "https://i.imgur.com/ISa0urR.png";
                case ArcDPSEnums.TrashID.PyreGuardianProtect:
                    return "https://i.imgur.com/jLW7rpV.png";
                case ArcDPSEnums.TrashID.ReaperofFlesh:
                    return "https://i.imgur.com/Notctbt.png";
                case ArcDPSEnums.TrashID.Kernan:
                    return "https://i.imgur.com/WABRQya.png";
                case ArcDPSEnums.TrashID.Knuckles:
                    return "https://i.imgur.com/m1y8nJE.png";
                case ArcDPSEnums.TrashID.Karde:
                    return "https://i.imgur.com/3UGyosm.png";
                case ArcDPSEnums.TrashID.Rigom:
                    return "https://i.imgur.com/REcGMBe.png";
                case ArcDPSEnums.TrashID.Guldhem:
                    return "https://i.imgur.com/xa7Fefn.png";
                case ArcDPSEnums.TrashID.Scythe:
                    return "https://i.imgur.com/INCGLIK.png";
                case ArcDPSEnums.TrashID.BanditBombardier:
                case ArcDPSEnums.TrashID.SurgingSoul:
                case ArcDPSEnums.TrashID.MazeMinotaur:
                case ArcDPSEnums.TrashID.Enervator:
                case ArcDPSEnums.TrashID.WhisperEcho:
                case ArcDPSEnums.TrashID.CharrTank:
                case ArcDPSEnums.TrashID.PropagandaBallon:
                case ArcDPSEnums.TrashID.FearDemon:
                case ArcDPSEnums.TrashID.SorrowDemon1:
                case ArcDPSEnums.TrashID.SorrowDemon2:
                case ArcDPSEnums.TrashID.SorrowDemon3:
                case ArcDPSEnums.TrashID.SorrowDemon4:
                case ArcDPSEnums.TrashID.SorrowDemon5:
                    return "https://i.imgur.com/k79t7ZA.png";
                case ArcDPSEnums.TrashID.HandOfErosion:
                case ArcDPSEnums.TrashID.HandOfEruption:
                    return "https://i.imgur.com/reGQHhr.png";
                case ArcDPSEnums.TrashID.VoltaicWisp:
                    return "https://i.imgur.com/C1mvNGZ.png";
                case ArcDPSEnums.TrashID.ParalyzingWisp:
                    return "https://i.imgur.com/YBl8Pqo.png";
                case ArcDPSEnums.TrashID.Pylon2:
                    return "https://i.imgur.com/b33vAEQ.png";
                case ArcDPSEnums.TrashID.EntropicDistortion:
                    return "https://i.imgur.com/MIpP5pK.png";
                case ArcDPSEnums.TrashID.SmallJumpyTornado:
                    return "https://i.imgur.com/WBJNgp7.png";
                case ArcDPSEnums.TrashID.OrbSpider:
                    return "https://i.imgur.com/FB5VM9X.png";
                case ArcDPSEnums.TrashID.Seekers:
                    return "https://i.imgur.com/FrPoluz.png";
                case ArcDPSEnums.TrashID.BlueGuardian:
                    return "https://i.imgur.com/6CefnkP.png";
                case ArcDPSEnums.TrashID.GreenGuardian:
                    return "https://i.imgur.com/nauDVYP.png";
                case ArcDPSEnums.TrashID.RedGuardian:
                    return "https://i.imgur.com/73Uj4lG.png";
                case ArcDPSEnums.TrashID.UnderworldReaper:
                    return "https://i.imgur.com/Tq6SYVe.png";
                case ArcDPSEnums.TrashID.CagedWarg:
                case ArcDPSEnums.TrashID.GreenSpirit1:
                case ArcDPSEnums.TrashID.GreenSpirit2:
                case ArcDPSEnums.TrashID.BanditSapper:
                case ArcDPSEnums.TrashID.ProjectionArkk:
                case ArcDPSEnums.TrashID.PrioryExplorer:
                case ArcDPSEnums.TrashID.PrioryScholar:
                case ArcDPSEnums.TrashID.VigilRecruit:
                case ArcDPSEnums.TrashID.VigilTactician:
                case ArcDPSEnums.TrashID.Prisoner1:
                case ArcDPSEnums.TrashID.Prisoner2:
                case ArcDPSEnums.TrashID.Pylon1:
                    return "https://i.imgur.com/0koP4xB.png";
                case ArcDPSEnums.TrashID.FleshWurm:
                    return "https://i.imgur.com/o3vX9Zc.png";
                case ArcDPSEnums.TrashID.Hands:
                    return "https://i.imgur.com/8JRPEoo.png";
                case ArcDPSEnums.TrashID.TemporalAnomaly:
                case ArcDPSEnums.TrashID.TemporalAnomaly2:
                    return "https://i.imgur.com/MIpP5pK.png";
                case ArcDPSEnums.TrashID.DOC:
                case ArcDPSEnums.TrashID.BLIGHT:
                case ArcDPSEnums.TrashID.PLINK:
                case ArcDPSEnums.TrashID.CHOP:
                    return "https://wiki.guildwars2.com/images/4/47/Mini_Baron_von_Scrufflebutt.png";
                case ArcDPSEnums.TrashID.FreeziesFrozenHeart:
                    return "https://wiki.guildwars2.com/images/9/9e/Mini_Freezie%27s_Heart.png";
                case ArcDPSEnums.TrashID.RiverOfSouls:
                    return "https://i.imgur.com/4pXEnaX.png";
                case ArcDPSEnums.TrashID.DhuumDesmina:
                    return "https://i.imgur.com/jAiRplg.png";
                //case CastleFountain:
                //    return "https://i.imgur.com/xV0OPWL.png";
                case ArcDPSEnums.TrashID.HauntingStatue:
                    return "https://i.imgur.com/7IQDyuK.png";
                case ArcDPSEnums.TrashID.GreenKnight:
                case ArcDPSEnums.TrashID.RedKnight:
                case ArcDPSEnums.TrashID.BlueKnight:
                    return "https://i.imgur.com/lpBm4d6.png";
            }
            return "https://i.imgur.com/HuJHqRZ.png";
        }


        private static readonly HashSet<string> _compressedFiles = new HashSet<string>()
        {
            ".zevtc",
            ".evtc.zip",
        };

        private static readonly HashSet<string> _tmpFiles = new HashSet<string>()
        {
            ".tmp.zip"
        };

        private static readonly HashSet<string> _supportedFiles = new HashSet<string>(_compressedFiles)
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

        public static List<string> GetSupportedFormats()
        {
            return new List<string>(_supportedFiles);
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


        internal static string GetString(Stream stream, int length, bool nullTerminated = true)
        {
            byte[] bytes = new byte[length];
            stream.Read(bytes, 0, length);
            if (nullTerminated)
            {
                for (int i = 0; i < length; ++i)
                {
                    if (bytes[i] == 0)
                    {
                        length = i;
                        break;
                    }
                }
            }
            return System.Text.Encoding.UTF8.GetString(bytes, 0, length);
        }
    }
}
