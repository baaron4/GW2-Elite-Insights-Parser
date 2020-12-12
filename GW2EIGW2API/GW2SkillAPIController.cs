using System;
using System.Collections.Generic;
using System.IO;
using GW2EIGW2API.GW2API;

namespace GW2EIGW2API
{
    internal class GW2SkillAPIController
    {
        private const string APIPath = "/v2/skills";

        private GW2APIUtilities.APIItems<GW2APISkill> _apiSkills = new GW2APIUtilities.APIItems<GW2APISkill>();
        private static List<GW2APISkill> GetGW2APISkills()
        {
            Console.WriteLine("Getting skills from API");

            return GW2APIUtilities.GetGW2APIItems<GW2APISkill>(APIPath);
        }
        internal GW2APIUtilities.APIItems<GW2APISkill> GetAPISkills(string cachePath)
        {
            if (_apiSkills.Items.Count == 0)
            {
                SetAPISkills(cachePath);
            }
            return _apiSkills;
        }

        internal void WriteAPISkillsToFile(string filePath)
        {
            FileStream fcreate = File.Open(filePath, FileMode.Create);
            fcreate.Close();

            List<GW2APISkill> skills = GetGW2APISkills();
            var writer = new StreamWriter(filePath);
            GW2APIUtilities.Serializer.Serialize(writer, skills);
            writer.Close();

            // refresh API cache
            _apiSkills = new GW2APIUtilities.APIItems<GW2APISkill>(skills);
        }
        private void SetAPISkills(string filePath)
        {
            if (File.Exists(filePath) && new FileInfo(filePath).Length != 0)
            {
                Console.WriteLine("Reading Skilllist");
                using (var reader = new StreamReader(filePath))
                {
                    var skillList = (List<GW2APISkill>)GW2APIUtilities.Deserializer.Deserialize(reader, typeof(List<GW2APISkill>));
                    _apiSkills = new GW2APIUtilities.APIItems<GW2APISkill>(skillList);
                    reader.Close();
                }
            }
            else
            {
                _apiSkills = new GW2APIUtilities.APIItems<GW2APISkill>(GetGW2APISkills());
            }
        }
    }
}

