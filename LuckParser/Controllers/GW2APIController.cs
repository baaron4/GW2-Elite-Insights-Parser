using LuckParser.Controllers.GW2API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace LuckParser.Controllers
{
    public class GW2APIController
    {
        static HttpClient APIClient { get; set; }

        private void GetAPIClient()
        {
            if (APIClient == null)
            {
                APIClient = new HttpClient
                {
                    BaseAddress = new Uri("https://api.guildwars2.com")
                };
                APIClient.DefaultRequestHeaders.Accept.Clear();
                APIClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
            return;
        }

        /*private GW2APISkill GetGW2APISKillAsync(string path)
        {
            if (APIClient == null) { GetAPIClient(); }
            GW2APISkill skill = null;
            HttpResponseMessage response = APIClient.GetAsync(path).Result;
            if (response.IsSuccessStatusCode)
            {

                skill = JsonConvert.DeserializeObject<GW2APISkill>(response.Content.ReadAsStringAsync().Result);
            }
            else
            {//try again after a wait
                System.Threading.Thread.Sleep(1000);
                response = APIClient.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                {
                    skill = JsonConvert.DeserializeObject<GW2APISkill>(response.Content.ReadAsStringAsync().Result);

                }
            }
            return skill;
        }*/
        private List<GW2APISkill> GetListGW2APISkills()
        {
            if (APIClient == null) { GetAPIClient(); }
            List<GW2APISkill> skill_L = new List<GW2APISkill>();
            bool maxPageSizeReached = false;
            int page = 0;
            int pagesize = 200;
            while (!maxPageSizeReached)
            {
                string path = "/v2/skills?page=" + page + "&page_size=" + pagesize;
                HttpResponseMessage response = APIClient.GetAsync(path).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    GW2APISkill[] responseArray = JsonConvert.DeserializeObject<GW2APISkill[]>(data, new JsonSerializerSettings
                    {
                        ContractResolver = new DefaultContractResolver()
                        {
                            NamingStrategy = new CamelCaseNamingStrategy()
                        }
                    });
                    skill_L.AddRange(responseArray);
                }
                else
                {
                    maxPageSizeReached = true;
                }
                page++;
            }
           
            return skill_L;
        }
        private SkillList GetSkillList()
        {
            if (_listOfSkills.Items.Count == 0)
            {
                SetSkillList();
            }
            return _listOfSkills;
        }
        public List<int> WriteSkillListToFile()
        {
            FileStream fcreate = File.Open(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SkillList.json", FileMode.Create);

            fcreate.Close();


            Console.WriteLine("Getting API");
            //Get list from API
            GetAPIClient();

            _listOfSkills = new SkillList();
            HttpResponseMessage response = APIClient.GetAsync("/v2/skills").Result;
            List<int> failedList = new List<int>();
            if (response.IsSuccessStatusCode)
            {
                // Get Skill ID list           
                _listOfSkills.Items.AddRange(GetListGW2APISkills());
                StreamWriter writer = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SkillList.json");
                var serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                };
                serializer.Serialize(writer, _listOfSkills.Items);
                writer.Close();

            }
            return failedList;
        }
        private void SetSkillList()
        {

            if (_listOfSkills.Items.Count == 0)
            {
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                + "/Content/SkillList.json";
                if (File.Exists(path))
                {
                    if (new FileInfo(path).Length != 0)
                    {
                        Console.WriteLine("Reading Skilllist");
                        using (StreamReader reader = new StreamReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                        + "/Content/SkillList.json"))
                        {
                            JsonSerializer serializer = new JsonSerializer()
                            {
                                ContractResolver = new DefaultContractResolver()
                                {
                                    NamingStrategy = new CamelCaseNamingStrategy()
                                }
                            };
                            _listOfSkills.Items = (List<GW2APISkill>)serializer.Deserialize(reader, typeof(List<GW2APISkill>));
                            reader.Close();
                        }
                    }
                }
                if (_listOfSkills.Items.Count == 0)
                {
                    WriteSkillListToFile();
                }
            }
            return;
        }

        public class SkillList
        {
            public SkillList() { Items = new List<GW2APISkill>(); }
            public List<GW2APISkill> Items { get; set; }
        }

        static SkillList _listOfSkills = new SkillList();

        public GW2APISkill GetSkill(long id)
        {
            GW2APISkill skill = GetSkillList().Items.FirstOrDefault(x => x.Id == id);
            //if (skill == null) {
            //    string path = "/v2/skills/" + id;
            //    skill = GetGW2APISKill(path);
            //}
            return skill;
        }
        //-----------------------------------------------------------------------------
        private GW2APISpec GetGW2APISpec(string path)
        {
            if (APIClient == null) { GetAPIClient(); }
            System.Threading.Thread.Sleep(100);
            GW2APISpec spec = null;
            //path = "/v2/specializations/" + isElite
            HttpResponseMessage response = APIClient.GetAsync(path).Result;
            if (response.IsSuccessStatusCode)
            {
                spec = JsonConvert.DeserializeObject<GW2APISpec>(response.Content.ReadAsStringAsync().Result);

            }
            return spec;
        }

        private SpecList GetSpecList()
        {
            if (_listofSpecs.Items.Count == 0)
            {
                SetSpecList(); 
            }
            return _listofSpecs;
        }
        public List<int> WriteSpecListToFile()
        {
            FileStream fcreate = File.Open(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SpecList.json", FileMode.Create);

            fcreate.Close();


            Console.WriteLine("Getting API");
            //Get list from API
            GetAPIClient();

            _listofSpecs = new SpecList();
            HttpResponseMessage response = APIClient.GetAsync("/v2/specializations").Result;
            int[] idArray;
            List<int> failedList = new List<int>();
            if (response.IsSuccessStatusCode)
            {
                // Get Skill ID list
                idArray = JsonConvert.DeserializeObject<int[]>(response.Content.ReadAsStringAsync().Result);

                foreach (int id in idArray)
                {
                    GW2APISpec curSpec = new GW2APISpec();
                    curSpec = GetGW2APISpec("/v2/specializations/" + id);
                    if (curSpec != null)
                    {

                        _listofSpecs.Items.Add(curSpec);

                    }
                    else
                    {
                        Console.WriteLine("Failed to get response");//fail to retrieve
                        failedList.Add(id);
                    }

                }
                StreamWriter writer = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SpecList.json");

                var serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    ContractResolver = new DefaultContractResolver()
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                };
                serializer.Serialize(writer, _listofSpecs.Items);
                writer.Close();

            }
            return failedList;
        }
        
        private void SetSpecList()
        {

            if (_listofSpecs.Items.Count == 0)
            {
                string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                + "/Content/SpecList.json";
                if (File.Exists(path))
                {
                    if (new FileInfo(path).Length != 0)
                    {
                        Console.WriteLine("Reading SpecList");
                        using (StreamReader reader = new StreamReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                        + "/Content/SpecList.json"))
                        {
                            JsonSerializer serializer = new JsonSerializer()
                            {
                                ContractResolver = new DefaultContractResolver()
                                {
                                    NamingStrategy = new CamelCaseNamingStrategy()
                                }
                            };
                            _listofSpecs.Items = (List<GW2APISpec>)serializer.Deserialize(reader, typeof(List<GW2APISpec>));
                            reader.Close();
                        }
                    }
                }
                if (_listofSpecs.Items.Count == 0)//if nothing in file or fail write new file
                {
                    WriteSpecListToFile();
                }

            }
            return;
        }

        public string GetAgentProfString(uint prof, uint elite)
        {
            // non player
            if (elite == 0xFFFFFFFF)
            {
                if ((prof & 0xffff0000) == 0xffff0000)
                {
                    return "GDG";
                }
                else
                {
                    return "NPC";
                }
            }
            // base profession
            else if (elite == 0)
            {
                switch (prof)
                {
                    case 1:
                        return "Guardian";
                    case 2:
                        return "Warrior";
                    case 3:
                        return "Engineer";
                    case 4:
                        return "Ranger";
                    case 5:
                        return "Thief";
                    case 6:
                        return "Elementalist";
                    case 7:
                        return "Mesmer";
                    case 8:
                        return "Necromancer";
                    case 9:
                        return "Revenant";
                }
            }
            // old elite
            else if (elite == 1)
            {
                switch (prof)
                {
                    case 1:
                        return "Dragonhunter";
                    case 2:
                        return "Berserker";
                    case 3:
                        return "Scrapper";
                    case 4:
                        return "Druid";
                    case 5:
                        return "Daredevil";
                    case 6:
                        return "Tempest";
                    case 7:
                        return "Chronomancer";
                    case 8:
                        return "Reaper";
                    case 9:
                        return "Herald";
                }

            }
            // new way
            else
            {
                GW2APISpec spec = GetSpec((int)elite);
                if (spec.Elite)
                {
                    return spec.Name;
                }
                else
                {
                    return spec.Profession;
                }
            }
            throw new InvalidDataException("Unknown profession");
        }

        public class SpecList
        {
            public SpecList() { Items = new List<GW2APISpec>(); }

            public List<GW2APISpec> Items { get; set; }
        }
     
        static SpecList _listofSpecs = new SpecList();

        public GW2APISpec GetSpec(int id)
        {
            GW2APISpec spec = GetSpecList().Items.FirstOrDefault(x => x.Id == id);
            
            return spec;
        }
    }
}
