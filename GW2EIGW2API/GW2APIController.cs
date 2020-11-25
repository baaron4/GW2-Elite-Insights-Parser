using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using GW2EIGW2API.GW2API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GW2EIGW2API
{
    public static class GW2APIController
    {
       
        /// <summary>
        /// API Cache init with a cache file locations, 
        /// If the files are present, the content will be used to initialize the API caches
        /// Otherwise the caches will be built from GW2 API calls
        /// </summary>
        /// <param name="skillLocation"></param>
        /// <param name="specLocation"></param>
        /// <param name="traitLocation"></param>
        public static void InitAPICache(string skillLocation, string specLocation, string traitLocation)
        {
            GW2SkillAPIController.GetAPISkills(skillLocation);
            GW2SpecAPIController.GetAPISpecs(specLocation);
            //GW2TraitAPIController.GetAPITraits(traitLocation);
        }

        /// <summary>
        /// Cacheless API initialization
        /// </summary>
        public static void InitAPICache()
        {
            GW2SkillAPIController.GetAPISkills(null);
            GW2SpecAPIController.GetAPISpecs(null);
            //GW2TraitAPIController.GetAPITraits(null);
        }

        //----------------------------------------------------------------------------- SKILLS

        /// <summary>
        /// Returns GW2APISkill item
        /// Warning: this method is not thread safe, 
        /// Make sure to initialize the cache before hand if you intend to call this method from different threads
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static GW2APISkill GetAPISkill(long id)
        {
            if (GW2SkillAPIController.GetAPISkills(null).Items.TryGetValue(id, out GW2APISkill skill))
            {
                return skill;
            }
            return null;
        }

        public static void WriteAPISkillsToFile(string filePath)
        {
            GW2SkillAPIController.WriteAPISkillsToFile(filePath);
        }

        //----------------------------------------------------------------------------- SPECS
        /// <summary>
        /// Returns GW2APISpec item
        /// Warning: this method is not thread safe, 
        /// Make sure to initialize the cache before hand if you intend to call this method from different threads
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static GW2APISpec GetAPISpec(int id)
        {
            if (GW2SpecAPIController.GetAPISpecs(null).Items.TryGetValue(id, out GW2APISpec spec))
            {
                return spec;
            }
            return null;
        }

        public static void WriteAPISpecsToFile(string filePath)
        {
            GW2SpecAPIController.WriteAPISpecsToFile(filePath);
        }

        //----------------------------------------------------------------------------- TRAITS


        /// <summary>
        /// Returns GW2APITrait item
        /// Warning: this method is not thread safe, 
        /// Make sure to initialize the cache before hand if you intend to call this method from different threads
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static GW2APITrait GetAPITrait(long id)
        {
            if (GW2TraitAPIController.GetAPITraits(null).Items.TryGetValue(id, out GW2APITrait trait))
            {
                return trait;
            }
            return null;
        }
        public static void WriteAPITraitsToFile(string filePath)
        {
            GW2TraitAPIController.WriteAPITraitsToFile(filePath);
        }

    }
}

