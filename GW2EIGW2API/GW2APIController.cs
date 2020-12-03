using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using GW2EIGW2API.GW2API;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GW2EIGW2API
{
    public class GW2APIController
    {
        private readonly GW2SkillAPIController skillAPIController = new GW2SkillAPIController();
        private readonly GW2SpecAPIController specAPIController = new GW2SpecAPIController();
        private readonly GW2TraitAPIController traitAPIController = new GW2TraitAPIController();
        /// <summary>
        /// API Cache init with a cache file locations, 
        /// If the files are present, the content will be used to initialize the API caches
        /// Otherwise the caches will be built from GW2 API calls
        /// </summary>
        /// <param name="skillLocation"></param>
        /// <param name="specLocation"></param>
        /// <param name="traitLocation"></param>
        public GW2APIController(string skillLocation, string specLocation, string traitLocation)
        {
            skillAPIController.GetAPISkills(skillLocation);
            specAPIController.GetAPISpecs(specLocation);
            //traitAPIController.GetAPITraits(traitLocation);
        }

        /// <summary>
        /// Cacheless API initialization
        /// </summary>
        public GW2APIController()
        {
            skillAPIController.GetAPISkills(null);
            specAPIController.GetAPISpecs(null);
            //traitAPIController.GetAPITraits(null);
        }

        //----------------------------------------------------------------------------- SKILLS

        /// <summary>
        /// Returns GW2APISkill item
        /// Warning: this method is not thread safe, 
        /// Make sure to initialize the cache before hand if you intend to call this method from different threads
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GW2APISkill GetAPISkill(long id)
        {
            if (skillAPIController.GetAPISkills(null).Items.TryGetValue(id, out GW2APISkill skill))
            {
                return skill;
            }
            return null;
        }

        public void WriteAPISkillsToFile(string filePath)
        {
            skillAPIController.WriteAPISkillsToFile(filePath);
        }

        //----------------------------------------------------------------------------- SPECS
        /// <summary>
        /// Returns GW2APISpec item
        /// Warning: this method is not thread safe, 
        /// Make sure to initialize the cache before hand if you intend to call this method from different threads
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GW2APISpec GetAPISpec(int id)
        {
            if (specAPIController.GetAPISpecs(null).Items.TryGetValue(id, out GW2APISpec spec))
            {
                return spec;
            }
            return null;
        }

        public void WriteAPISpecsToFile(string filePath)
        {
            specAPIController.WriteAPISpecsToFile(filePath);
        }

        //----------------------------------------------------------------------------- TRAITS


        /// <summary>
        /// Returns GW2APITrait item
        /// Warning: this method is not thread safe, 
        /// Make sure to initialize the cache before hand if you intend to call this method from different threads
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GW2APITrait GetAPITrait(long id)
        {
            if (traitAPIController.GetAPITraits(null).Items.TryGetValue(id, out GW2APITrait trait))
            {
                return trait;
            }
            return null;
        }
        public void WriteAPITraitsToFile(string filePath)
        {
            traitAPIController.WriteAPITraitsToFile(filePath);
        }

    }
}

