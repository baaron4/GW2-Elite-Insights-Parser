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
        public List<string> FirstSet;
        public List<string> SecondSet;
        public string ColTarget;
        public string ColCleave;
        public string ColTotal;
        public bool IsConjure;

        public PlayerDto(Player player, ParsedLog log, bool cr)
        {
            Group = player.Group;
            Name = player.Character;
            Acc = player.Account.TrimStart(':');
            Profession = player.Prof;
            Condi = player.Condition;
            Conc = player.Concentration;
            Heal = player.Healing;
            Tough = player.Toughness;
            ColTarget = GeneralHelper.GetLink("Color-" + player.Prof);
            ColCleave = GeneralHelper.GetLink("Color-" + player.Prof + "-NonBoss");
            ColTotal = GeneralHelper.GetLink("Color-" + player.Prof + "-Total");
            IsConjure = (player.Account == ":Conjured Sword");
            BuildWeaponSets(player, log);
            if (cr && !IsConjure)
            {
                CombatReplayID = player.GetCombatReplayID();
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

        private void BuildWeaponSets(Player player, ParsedLog log)
        {
            string[] weps = player.GetWeaponsArray(log);
            List<string> firstSet = new List<string>();
            List<string> secondSet = new List<string>();
            for (int j = 0; j < weps.Length; j++)
            {
                var wep = weps[j];
                if (wep != null)
                {
                    if (wep != "2Hand")
                    {
                        if (j > 1)
                        {
                            secondSet.Add(wep);
                        }
                        else
                        {
                            firstSet.Add(wep);
                        }
                    }
                }
                else
                {
                    if (j > 1)
                    {
                        secondSet.Add("Unknown");
                    }
                    else
                    {
                        firstSet.Add("Unknown");
                    }
                }
            }
            if (firstSet[0] == "Unknown" && firstSet[1] == "Unknown")
            {
                FirstSet = new List<string>();
            }
            else
            {
                FirstSet = firstSet;
            }
            if (secondSet[0] == "Unknown" && secondSet[1] == "Unknown")
            {
                SecondSet = new List<string>();
            }
            else
            {
                SecondSet = secondSet;
            }
        }
    }
}
