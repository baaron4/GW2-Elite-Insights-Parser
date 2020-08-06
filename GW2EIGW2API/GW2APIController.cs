using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using GW2EIGW2API.GW2API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GW2EIGW2API
{
    public static class GW2APIController
    {
        private static readonly DefaultContractResolver DefaultJsonContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };
        private static HttpClient APIClient { get; set; }

        private static void GetAPIClient()
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
        // INIT

        public static void InitAPICache()
        {
            SetAPISkills();
            //SetAPITraits();
            SetAPISpecs();
        }

        //----------------------------------------------------------------------------- SKILLS

        private class APISkills
        {
            public APISkills() { 
                Items = new Dictionary<long, GW2APISkill>(); 
            }

            public APISkills(List<GW2APISkill> skills)
            {
                Items = skills.ToDictionary(x => x.Id);
            }

            public Dictionary<long, GW2APISkill> Items { get; }
        }

        private static APISkills _apiSkills = new APISkills();

        public static GW2APISkill GetAPISkill(long id)
        {
            if (GetAPISkills().Items.TryGetValue(id, out GW2APISkill skill))
            {
                return skill;
            }
            return null;
        }
        private static List<GW2APISkill> GetGW2APISkills()
        {
            var skill_L = new List<GW2APISkill>();
            bool maxPageSizeReached = false;
            int page = 0;
            int pagesize = 200;
            while (!maxPageSizeReached)
            {
                string path = "/v2/skills?page=" + page + "&page_size=" + pagesize;
                HttpResponseMessage response = APIClient.GetAsync(new Uri(path, UriKind.Relative)).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    GW2APISkill[] responseArray = JsonConvert.DeserializeObject<GW2APISkill[]>(data, new JsonSerializerSettings
                    {
                        ContractResolver = DefaultJsonContractResolver
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
        private static APISkills GetAPISkills()
        {
            if (_apiSkills.Items.Count == 0)
            {
                throw new InvalidDataException("API Cache not initialized");
            }
            return _apiSkills;
        }
        public static List<int> WriteAPISkillsToFile()
        {
            FileStream fcreate = File.Open(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SkillList.json", FileMode.Create);

            fcreate.Close();


            Console.WriteLine("Getting API");
            //Get list from API
            GetAPIClient();

            var skillList = new List<GW2APISkill>();
            HttpResponseMessage response = APIClient.GetAsync(new Uri("/v2/skills", UriKind.Relative)).Result;
            var failedList = new List<int>();
            if (response.IsSuccessStatusCode)
            {
                // Get Skill ID list           
                skillList.AddRange(GetGW2APISkills());
                var writer = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SkillList.json");
                var serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.None,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    ContractResolver = DefaultJsonContractResolver
                };
                serializer.Serialize(writer, skillList);
                writer.Close();

            }
            _apiSkills = new APISkills(skillList);
            return failedList;
        }
        private static void SetAPISkills()
        {

            if (_apiSkills.Items.Count == 0)
            {
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                + "/Content/SkillList.json";
                if (File.Exists(path))
                {
                    if (new FileInfo(path).Length != 0)
                    {
                        Console.WriteLine("Reading Skilllist");
                        using (var reader = new StreamReader(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                        + "/Content/SkillList.json"))
                        {
                            var serializer = new JsonSerializer()
                            {
                                ContractResolver = DefaultJsonContractResolver
                            };
                            var skillList = (List<GW2APISkill>)serializer.Deserialize(reader, typeof(List<GW2APISkill>));
                            _apiSkills = new APISkills(skillList);
                            reader.Close();
                        }
                    }
                }
            }
            return;
        }
        //----------------------------------------------------------------------------- SPECS
        private class APISpecs
        {
            public APISpecs() { 
                Items = new Dictionary<int, GW2APISpec>(); 
            }

            public APISpecs(List<GW2APISpec> specs)
            {
                Items = specs.ToDictionary(x => x.Id);
            }

            public Dictionary<int, GW2APISpec> Items { get; }
        }

        private static APISpecs _apiSpecs = new APISpecs();

        public static GW2APISpec GetAPISpec(int id)
        {
            if (GetAPISpecs().Items.TryGetValue(id, out GW2APISpec spec))
            {
                return spec;
            }
            return null;
        }
        private static GW2APISpec GetGW2APISpec(string path)
        {
            GW2APISpec spec = null;
            //path = "/v2/specializations/" + isElite
            HttpResponseMessage response = APIClient.GetAsync(new Uri(path, UriKind.Relative)).Result;
            if (response.IsSuccessStatusCode)
            {
                spec = JsonConvert.DeserializeObject<GW2APISpec>(response.Content.ReadAsStringAsync().Result);

            }
            return spec;
        }

        private static APISpecs GetAPISpecs()
        {
            if (_apiSpecs.Items.Count == 0)
            {
                throw new InvalidDataException("API Cache not initialized");
            }
            return _apiSpecs;
        }
        public static List<int> WriteAPISpecsToFile()
        {
            FileStream fcreate = File.Open(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SpecList.json", FileMode.Create);

            fcreate.Close();


            Console.WriteLine("Getting API");
            //Get list from API
            GetAPIClient();

            var specList = new List<GW2APISpec>();
            HttpResponseMessage response = APIClient.GetAsync(new Uri("/v2/specializations", UriKind.Relative)).Result;
            int[] idArray;
            var failedList = new List<int>();
            if (response.IsSuccessStatusCode)
            {
                // Get Skill ID list
                idArray = JsonConvert.DeserializeObject<int[]>(response.Content.ReadAsStringAsync().Result);

                foreach (int id in idArray)
                {
                    GW2APISpec curSpec = GetGW2APISpec("/v2/specializations/" + id);
                    if (curSpec != null)
                    {

                        specList.Add(curSpec);

                    }
                    else
                    {
                        Console.WriteLine("Failed to get response");//fail to retrieve
                        failedList.Add(id);
                    }

                }
                var writer = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/SpecList.json");

                var serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.None,
                    ContractResolver = DefaultJsonContractResolver
                };
                serializer.Serialize(writer, specList);
                writer.Close();

            }
            _apiSpecs = new APISpecs(specList);
            return failedList;
        }

        private static void SetAPISpecs()
        {

            if (_apiSpecs.Items.Count == 0)
            {
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                + "/Content/SpecList.json";
                if (File.Exists(path))
                {
                    if (new FileInfo(path).Length != 0)
                    {
                        Console.WriteLine("Reading SpecList");
                        using (var reader = new StreamReader(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                        + "/Content/SpecList.json"))
                        {
                            var serializer = new JsonSerializer()
                            {
                                ContractResolver = DefaultJsonContractResolver
                            };
                            var specList = (List<GW2APISpec>)serializer.Deserialize(reader, typeof(List<GW2APISpec>));
                            _apiSpecs = new APISpecs(specList);
                            reader.Close();
                        }
                    }
                }
            }
            return;
        }

        //----------------------------------------------------------------------------- TRAITS

        private class APITraits
        {
            public APITraits() { 
                Items = new Dictionary<long, GW2APITrait>(); 
            }

            public APITraits(List<GW2APITrait> traits)
            {
                Items = traits.ToDictionary(x => x.Id);
            }

            public Dictionary<long, GW2APITrait> Items { get;}
        }

        private static APITraits _apiTraits = new APITraits();

        public static GW2APITrait GetAPITrait(long id)
        {
            if (GetAPITraits().Items.TryGetValue(id, out GW2APITrait trait))
            {
                return trait;
            }
            return null;
        }
        private static List<GW2APITrait> GetListGW2APITraits()
        {
            var trait_L = new List<GW2APITrait>();
            bool maxPageSizeReached = false;
            int page = 0;
            int pagesize = 200;
            while (!maxPageSizeReached)
            {
                string path = "/v2/traits?page=" + page + "&page_size=" + pagesize;
                HttpResponseMessage response = APIClient.GetAsync(new Uri(path, UriKind.Relative)).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    GW2APITrait[] responseArray = JsonConvert.DeserializeObject<GW2APITrait[]>(data, new JsonSerializerSettings
                    {
                        ContractResolver = DefaultJsonContractResolver
                    });
                    trait_L.AddRange(responseArray);
                }
                else
                {
                    maxPageSizeReached = true;
                }
                page++;
            }

            return trait_L;
        }
        private static APITraits GetAPITraits()
        {
            if (_apiTraits.Items.Count == 0)
            {
                throw new InvalidDataException("API Cache not initialized");
            }
            return _apiTraits;
        }
        public static List<int> WriteAPITraitsToFile()
        {
            FileStream fcreate = File.Open(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/TraitList.json", FileMode.Create);

            fcreate.Close();


            Console.WriteLine("Getting API");
            //Get list from API
            GetAPIClient();

            var traitList = new List<GW2APITrait>();
            HttpResponseMessage response = APIClient.GetAsync(new Uri("/v2/traits", UriKind.Relative)).Result;
            var failedList = new List<int>();
            if (response.IsSuccessStatusCode)
            {
                // Get Skill ID list           
                traitList.AddRange(GetListGW2APITraits());
                var writer = new StreamWriter(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
            + "/Content/TraitList.json");
                var serializer = new JsonSerializer
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.None,
                    DefaultValueHandling = DefaultValueHandling.Ignore,
                    ContractResolver = DefaultJsonContractResolver
                };
                serializer.Serialize(writer, traitList);
                writer.Close();

            }
            _apiTraits = new APITraits(traitList);
            return failedList;
        }
        private static void SetAPITraits()
        {

            if (_apiTraits.Items.Count == 0)
            {
                string path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                + "/Content/TraitList.json";
                if (File.Exists(path))
                {
                    if (new FileInfo(path).Length != 0)
                    {
                        Console.WriteLine("Reading Traitlist");
                        using (var reader = new StreamReader(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)
                        + "/Content/TraitList.json"))
                        {
                            var serializer = new JsonSerializer()
                            {
                                ContractResolver = DefaultJsonContractResolver
                            };
                            var traitList = (List<GW2APITrait>)serializer.Deserialize(reader, typeof(List<GW2APITrait>));
                            _apiTraits = new APITraits(traitList);
                            reader.Close();
                        }
                    }
                }
            }
            return;
        }
    }
}

