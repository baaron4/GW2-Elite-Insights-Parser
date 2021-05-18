using System.Collections.Generic;
using GW2EIEvtcParser;
using GW2EIEvtcParser.EIData;

namespace GW2EIBuilders.HtmlModels
{

    internal class PlayerDto : ActorDto
    {
        public int Group { get; set; }
        public string Acc { get; set; }
        public string Profession { get; set; }

        public bool IsPoV { get; set; }

        public bool IsCommander { get; set; }
        public List<string> L1Set { get; } = new List<string>();
        public List<string> L2Set { get; } = new List<string>();
        public List<string> A1Set { get; } = new List<string>();
        public List<string> A2Set { get; } = new List<string>();
        public string ColTarget { get; set; }
        public string ColCleave { get; set; }
        public string ColTotal { get; set; }
        public bool IsFake { get; set; }
        public bool NotInSquad { get; set; }

        public PlayerDto(AbstractSingleActor actor, ParsedEvtcLog log, ActorDetailsDto details) : base(actor, log, details)
        {
            Group = actor.Group;
            Acc = actor.Account;
            Profession = actor.Prof;
            IsPoV = log.LogData.PoV == actor.AgentItem;
            IsCommander = actor.HasCommanderTag;
            ColTarget = HTMLBuilder.GetLink("Color-" + actor.Prof);
            ColCleave = HTMLBuilder.GetLink("Color-" + actor.Prof + "-NonBoss");
            ColTotal = HTMLBuilder.GetLink("Color-" + actor.Prof + "-Total");
            IsFake = actor.IsFakeActor;
            NotInSquad = !(actor is Player);
            BuildWeaponSets(actor, log);
        }

        private static void BuildWeaponSets(IReadOnlyList<string> weps, int offset, List<string> set1, List<string> set2)
        {

            for (int j = 0; j < 4; j++)
            {
                string wep = weps[j + offset];
                if (wep != null)
                {
                    if (wep != "2Hand")
                    {
                        if (j > 1)
                        {
                            set2.Add(wep);
                        }
                        else
                        {
                            set1.Add(wep);
                        }
                    }
                }
                else
                {
                    if (j > 1)
                    {
                        set2.Add("Unknown");
                    }
                    else
                    {
                        set1.Add("Unknown");
                    }
                }
            }
            if (set1[0] == "Unknown" && set1[1] == "Unknown")
            {
                set1.Clear();
            }
            if (set2[0] == "Unknown" && set2[1] == "Unknown")
            {
                set2.Clear();
            }
        }

        private void BuildWeaponSets(AbstractSingleActor actor, ParsedEvtcLog log)
        {
            IReadOnlyList<string> weps = actor.GetWeaponsArray(log);
            BuildWeaponSets(weps, 0, L1Set, L2Set);
            BuildWeaponSets(weps, 4, A1Set, A2Set);
        }
    }
}
