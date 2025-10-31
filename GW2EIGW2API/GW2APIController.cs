using GW2EIGW2API.GW2API;

[assembly: CLSCompliant(false)]
namespace GW2EIGW2API;

public class GW2APIController
{
    private readonly GW2SkillAPIController skillAPIController = new();
    private readonly GW2SpecAPIController specAPIController = new();
    private readonly GW2TraitAPIController traitAPIController = new();
    private readonly GW2MapAPIController mapAPIController = new();
    /// <summary>
    /// API Cache init with a cache file locations, 
    /// If the files are present, the content will be used to initialize the API caches
    /// Otherwise the caches will be built from GW2 API calls
    /// </summary>
    /// <param name="skillLocation"></param>
    /// <param name="specLocation"></param>
    /// <param name="traitLocation"></param>
    /// <param name="mapLocation"></param>
    public GW2APIController(string skillLocation, string specLocation, string traitLocation, string mapLocation)
    {
        skillAPIController.GetAPISkills(skillLocation);
        specAPIController.GetAPISpecs(specLocation);
        mapAPIController.GetAPIMaps(mapLocation);
        //traitAPIController.GetAPITraits(traitLocation);
    }

    /// <summary>
    /// Cacheless API initialization
    /// </summary>
    public GW2APIController()
    {
        skillAPIController.GetAPISkills(null);
        specAPIController.GetAPISpecs(null);
        mapAPIController.GetAPIMaps(null);
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
        return skillAPIController.GetAPISkills(null).Items.TryGetValue(id, out GW2APISkill skill) ? skill : null;
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
        return specAPIController.GetAPISpecs(null).Items.TryGetValue(id, out GW2APISpec spec) ? spec : null;
    }

    public void WriteAPISpecsToFile(string filePath)
    {
        specAPIController.WriteAPISpecsToFile(filePath);
    }

    //----------------------------------------------------------------------------- MAPS
    /// <summary>
    /// Returns GW2APIMap item
    /// Warning: this method is not thread safe, 
    /// Make sure to initialize the cache before hand if you intend to call this method from different threads
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public GW2APIMap GetAPIMap(int id)
    {
        return mapAPIController.GetAPIMaps(null).Items.TryGetValue(id, out GW2APIMap map) ? map : null;
    }

    public void WriteAPIMapsToFile(string filePath)
    {
        mapAPIController.WriteAPIMapsToFile(filePath);
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
        return traitAPIController.GetAPITraits(null).Items.TryGetValue(id, out GW2APITrait trait) ? trait : null;
    }
    public void WriteAPITraitsToFile(string filePath)
    {
        traitAPIController.WriteAPITraitsToFile(filePath);
    }

    public string GetSpec(uint prof, uint elite)
    {
        // Non player agents - Gadgets = GDG
        if (elite == 0xFFFFFFFF)
        {
            return (prof & 0xffff0000) == 0xffff0000 ? "GDG" : "NPC";
        }
        // Old way - Base Profession
        else if (elite == 0)
        {
            switch (prof)
            {
                case 1: return "Guardian";
                case 2: return "Warrior";
                case 3: return "Engineer";
                case 4: return "Ranger";
                case 5: return "Thief";
                case 6: return "Elementalist";
                case 7: return "Mesmer";
                case 8: return "Necromancer";
                case 9: return "Revenant";
                default: return UNKNOWN_SPEC;
            }
        }
        // Old way - Elite Specialization (HoT)
        else if (elite == 1)
        {
            switch (prof)
            {
                case 1: return "Dragonhunter";
                case 2: return "Berserker";
                case 3: return "Scrapper";
                case 4: return "Druid";
                case 5: return "Daredevil";
                case 6: return "Tempest";
                case 7: return "Chronomancer";
                case 8: return "Reaper";
                case 9: return "Herald";
                default: return UNKNOWN_SPEC;
            }
        }
        // Current way
        else
        {
            GW2APISpec spec = GetAPISpec((int)elite);
            if (spec == null)
            {
                return UNKNOWN_SPEC;
            }
            return spec.Elite ? spec.Name : spec.Profession;
        }
        throw new InvalidOperationException("Unexpected profession pattern in GetSpec");
    }

    public static readonly string UNKNOWN_SPEC = "Unknown";

}

