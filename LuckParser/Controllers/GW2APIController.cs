using LuckParser.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LuckParser.Controllers
{
   public class GW2APIController
    {
        static HttpClient APIClient { get; set; }
        private void GetAPIClient()
        {
            if (APIClient == null)
            {
                APIClient = new HttpClient();
                APIClient.BaseAddress = new Uri("https://api.guildwars2.com");
                APIClient.DefaultRequestHeaders.Accept.Clear();
                APIClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
            return;
        }
        private GW2APISkill GetGW2APISKill(string path)
        {
            //System.Threading.Thread.Sleep(100);
            if (APIClient == null) { GetAPIClient(); }
            GW2APISkill skill = null;
            HttpResponseMessage response = APIClient.GetAsync(path).Result;
            if (response.IsSuccessStatusCode)
            {
                skill = response.Content.ReadAsAsync<GW2APISkill>().Result;
            }
            else {//try again after a wait
                System.Threading.Thread.Sleep(1000);
                response = APIClient.GetAsync(path).Result;
                if (response.IsSuccessStatusCode) {
                    skill = response.Content.ReadAsAsync<GW2APISkill>().Result;
                }
            }
            return skill;
        }
        private SkillList GetSkillList()
        {
            if (ListOfSkills.Items.Count == 0)
            {
                SetSkillList();
            }
            return ListOfSkills;
        }

        private void SetSkillList()
        {

            if (ListOfSkills.Items.Count == 0)
            {

                if (new FileInfo(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
                + "/Content/SkillList.txt").Length != 0)
                {
                    Console.WriteLine("Reading Skilllist");

                    //Get list from local XML
                    using (var reader = new StreamReader(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
                    + "/Content/SkillList.txt"))
                    {
                        XmlSerializer deserializer = new XmlSerializer(typeof(SkillList));
                        ListOfSkills = (SkillList)deserializer.Deserialize(reader);

                        reader.Close();
                    }
                }
                //used for wiritng new XMLs
                //FileStream fcreate = File.Open(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
                //+ "/SkillList.txt", FileMode.Create);

                //fcreate.Close();

                //if (ListOfSkills.Items.Count == 0)
                //{
                //    Console.WriteLine("Getting APi");
                //    //Get list from API
                //    GetAPIClient();
                //    ListOfSkills = new SkillList();
                //    HttpResponseMessage response = APIClient.GetAsync("/v2/skills").Result;
                //    int[] idArray;
                //    if (response.IsSuccessStatusCode)
                //    {
                //        // Get Skill ID list
                //        idArray = response.Content.ReadAsAsync<int[]>().Result;

                //        foreach (int id in idArray)
                //        {
                //            GW2APISkill curSkill = new GW2APISkill();
                //            curSkill = GetGW2APISKill("/v2/skills/" + id);
                //            if (curSkill != null)
                //            {

                //                    ListOfSkills.Items.Add(curSkill);

                //            }
                //            else
                //            {
                //                Console.WriteLine("Fail to get response");//fail to retrieve
                //            }

                //        }
                //        Stream stream = System.IO.File.OpenWrite(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location)
                // + "/Content/SkillList.txt");
                //        XmlSerializer xmlSer = new XmlSerializer(typeof(List<GW2APISkill>));
                //        xmlSer.Serialize(stream, ListOfSkills.Items);
                //        stream.Close();
                //    }
                //}
            }



            return;
        }
        [XmlRoot("ArrayOfGW2APISkill")]
        public class SkillList
        {
            public SkillList() { Items = new List<GW2APISkill>(); }
            [XmlElement("GW2APISkill")]
            public List<GW2APISkill> Items { get; set; }
        }
        static SkillList ListOfSkills = new SkillList();

        public  GW2APISkill GetSkill(int id) {
            GW2APISkill skill = GetSkillList().Items.FirstOrDefault(x => x.id == id);
            //if (skill == null) {
            //    string path = "/v2/skills/" + id;
            //    skill = GetGW2APISKill(path);
            //}
            return skill;
        }
        
    }
}
