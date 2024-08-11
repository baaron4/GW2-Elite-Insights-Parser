using System.Collections.Generic;
using System.Linq;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;
using GW2EIEvtcParser.ParsedData;

namespace GW2EIBuilders.HtmlModels.HTMLActors
{

    internal class PlayerDto : ActorDto
    {
        public int Group { get; set; }
        public string Acc { get; set; }
        public string Profession { get; set; }

        public bool IsPoV { get; set; }

        public bool IsCommander { get; set; }
        public List<string[]> CommanderStates { get; set; }
        public List<string> L1Set { get; } = new List<string>();
        public List<string> L2Set { get; } = new List<string>();
        public List<string> A1Set { get; } = new List<string>();
        public List<string> A2Set { get; } = new List<string>();
        public string ColTarget { get; set; }
        public string ColCleave { get; set; }
        public string ColTotal { get; set; }
        public bool IsFake { get; set; }
        public bool NotInSquad { get; set; }

        private static (string, string, string) GetSpecGraphColor(ParserHelper.Spec baseSpec)
        {
            switch (baseSpec)
            {

                case ParserHelper.Spec.Warrior:
                    return ("rgb(255,209,102)", "rgb(190,159,84)", "rgb(125,109,66)");
                case ParserHelper.Spec.Guardian:
                    return ("rgb(114,193,217)", "rgb(88,147,165)", "rgb(62,101,113)");
                case ParserHelper.Spec.Revenant:
                    return ("rgb(209,110,90)", "rgb(159,85,70)", "rgb(110,60,50)");
                case ParserHelper.Spec.Engineer:
                    return ("rgb(208,156,89)", "rgb(158,119,68)", "rgb(109,83,48)");
                case ParserHelper.Spec.Ranger:
                    return ("rgb(140,220,130)", "rgb(107,167,100)", "rgb(75,115,70)");
                case ParserHelper.Spec.Thief:
                    return ("rgb(192,143,149)", "rgb(146,109,114)", "rgb(101,76,79)");
                case ParserHelper.Spec.Elementalist:
                    return ("rgb(246,138,135)", "rgb(186,106,103)", "rgb(127,74,72)");
                case ParserHelper.Spec.Mesmer:
                    return ("rgb(182,121,213)", "rgb(139,90,162)", "rgb(96,60,111)");
                case ParserHelper.Spec.Necromancer:
                    return ("rgb(82,167,111)", "rgb(64,127,85)", "rgb(46,88,60)");
                default:
                    return ("", "", "");
            }

        }

        public PlayerDto(AbstractSingleActor actor, ParsedEvtcLog log, ActorDetailsDto details) : base(actor, log, details)
        {
            Group = actor.Group;
            Acc = actor.Account;
            Profession = actor.Spec.ToString();
            IsPoV = log.LogData.PoV == actor.AgentItem;
            if (actor is Player p)
            {
                IsCommander = p.IsCommander(log);
                CommanderStates = IsCommander ? p.GetCommanderStatesNoTagValues(log).Select(x => new string[2] { ParserHelper.ToDurationString(x.Start), ParserHelper.ToDurationString(x.End) }).ToList() : null;
            }
            (ColTarget, ColCleave, ColTotal) = GetSpecGraphColor(actor.BaseSpec);
            IsFake = actor.IsFakeActor;
            NotInSquad = !(actor is Player);
            BuildWeaponSets(actor, log);
        }

        private static void BuildWeaponSets((string mh, string oh) set, List<string> listToSet)
        {
            if (set.mh == WeaponSets.Unknown && set.oh == WeaponSets.Unknown)
            {
                return;
            }
            listToSet.Add(set.mh);
            if (set.oh != WeaponSets.TwoHand)
            {
                listToSet.Add(set.oh);
            }
        }

        private void BuildWeaponSets(AbstractSingleActor actor, ParsedEvtcLog log)
        {
            WeaponSets weps = actor.GetWeaponSets(log);
            BuildWeaponSets(weps.LandSet1, L1Set);
            BuildWeaponSets(weps.LandSet2, L2Set);
            BuildWeaponSets(weps.WaterSet1, A1Set);
            BuildWeaponSets(weps.WaterSet2, A2Set);
        }
    }
}
