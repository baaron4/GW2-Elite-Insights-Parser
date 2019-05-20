using LuckParser.Models.ParseModels;
using LuckParser.Parser;
using System.Collections.Generic;
using System.Linq;

namespace LuckParser.Models.HtmlModels
{

    public class PlayerDto
    {
        public int Group;
        public int CombatReplayID;
        public string Name;
        public string Acc;
        public string Profession;
        public uint Condi;
        public uint Conc;
        public uint Heal;
        public uint Tough;
        public readonly List<MinionDto> Minions = new List<MinionDto>();
        public readonly List<string> L1Set = new List<string>();
        public readonly List<string> L2Set = new List<string>();
        public readonly List<string> A1Set = new List<string>();
        public readonly List<string> A2Set = new List<string>();
        public string ColTarget;
        public string ColCleave;
        public string ColTotal;
        public bool IsConjure;
        public ActorDetailsDto Details;

        public PlayerDto(Player player, ParsedLog log, bool cr, ActorDetailsDto details)
        {
            Group = player.Group;
            Name = player.Character;
            Acc = player.Account;
            Profession = player.Prof;
            Condi = player.Condition;
            Conc = player.Concentration;
            Heal = player.Healing;
            Tough = player.Toughness;
            ColTarget = GeneralHelper.GetLink("Color-" + player.Prof);
            ColCleave = GeneralHelper.GetLink("Color-" + player.Prof + "-NonBoss");
            ColTotal = GeneralHelper.GetLink("Color-" + player.Prof + "-Total");
            IsConjure = (player.IsFakeActor);
            BuildWeaponSets(player, log);
            Details = details;
            if (cr && !IsConjure)
            {
                CombatReplayID = player.GetCombatReplayID(log);
            }
            foreach (KeyValuePair<string, Minions> pair in player.GetMinions(log))
            {
                Minions.Add(new MinionDto()
                {
                    Id = pair.Value.MinionID,
                    Name = pair.Key.TrimEnd(" \0".ToArray())
                });
            }
        }

        private void BuildWeaponSets(string[] weps, int offset, List<string> set1, List<string> set2)
        {

            for (int j = 0; j < 4; j++)
            {
                var wep = weps[j + offset];
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

        private void BuildWeaponSets(Player player, ParsedLog log)
        {
            string[] weps = player.GetWeaponsArray(log);
            BuildWeaponSets(weps, 0, L1Set, L2Set);
            BuildWeaponSets(weps, 4, A1Set, A2Set);
        }
    }
}
