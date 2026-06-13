namespace GW2EIJSON;

/// <summary>
/// WvW map data. \n
/// Shard IDs should be in :\n
/// 11001: Moogooloo \n
/// 11002: Rall's Rest \n
/// 11003: Domain of Torment \n
/// 11004: Yohlon Haven \n
/// 11005: Tombs of Drascir \n
/// 11006: Hall of Judgment \n
/// 11007: Throne of Balthazar \n
/// 11008: Dwayna's Temple \n
/// 11009: Abaddon's Prison \n
/// 11010: Cathedral of Blood \n
/// 11011: Lutgardis Conservatory \n
/// 11012: Mosswood \n
/// 11013: Mithric Cliffs \n
/// 11014: Lagula's Kraal \n
/// 11015: De Molish Post \n
/// 11016: Olafstead \n
/// 11017: Beacon's Perch \n
/// 11018: Granite Citadel \n
/// 12001: Skrittsburgh \n
/// 12002: Fortune's Vale \n
/// 12003: Silent Woods \n
/// 12004: Ettin's Back \n
/// 12005: Domain of Anguish \n
/// 12006: Palawadan \n
/// 12007: Bloodstone Gulch \n
/// 12008: Frost Citadel \n
/// 12009: Dragrimmar \n
/// 12010: Grenth's Door \n
/// 12011: Mirror of Lyssa \n
/// 12012: Melandru's Dome \n
/// 12013: Kormir's Library \n
/// 12014: Great House Aviary \n
/// 12015: Bava Nisos \n
/// 12016: Temple of Febe \n
/// 12017: Gyala Hatchery \n
/// 12018: Grekvelnn Burrows \n
/// 12019: Stormguard \n
/// 12020: Sifhalla \n
/// 12021: Hrothgar's Pass \n
/// 18001: Moogooloo \n
/// 18002: Titan's Staircase \n
/// 18003: Skrittsburgh \n
/// 18004: Seven Pines \n
/// 18005: Phoenix Dawn \n
/// 18006: Thornwatch \n
/// 18007: Griffonfall \n
/// 18008: Stonefall \n
/// 18009: First Haven \n
/// 18010: Dragon's Claw \n
/// 18011: Giant's Rise \n
/// 18012: Reaper's Corridor \n
/// 18013: Fortune's Vale \n
/// 18014: Silent Woods \n
/// 18015: Grenth's Door \n
/// Ignore if outside of those values.
/// </summary>
public class JsonWvWMapData
{
    public class JsonWvWObjectiveData
    {
        /// <summary>
        /// ID of the map the objective is attached to
        /// </summary>
        public int MapID;
        /// <summary>
        /// ID of the objective
        /// </summary>
        public int ObjectiveID;
        /// <summary>
        /// Type of the objective in: \n
        /// "Camp" \n
        /// "Tower" \n
        /// "Keep" \n
        /// "Ruins" \n
        /// "Castle"
        /// </summary>
        public string ObjectiveType = "";
        /// <summary>
        /// Array of [TeamID, Time] representing the current ownership of the objective between current and next. \n
        /// If last element, ownership did not change until the end of the log.
        /// </summary>
        public List<long[]> Owners = [];
    }

    /// <summary>
    /// Red Team's shard ID
    /// </summary>
    public uint RedShardID;
    /// <summary>
    /// Blue Team's shard ID
    /// </summary>
    public uint BlueShardID;
    /// <summary>
    /// Green Team's shard ID
    /// </summary>
    public uint GreenShardID;

    /// <summary>
    /// Red Team's team ID, can be 0 if red team is not present in the log
    /// </summary>
    public uint RedTeamID;
    /// <summary>
    /// Blue Team's team ID, can be 0 if blue team is not present in the log
    /// </summary>
    public uint BlueTeamID;
    /// <summary>
    /// Green Team's team ID, can be 0 if green team is not present in the log
    /// </summary>
    public uint GreenTeamID;

    public List<JsonWvWObjectiveData> ObjectiveData = [];
}
