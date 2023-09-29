using Newtonsoft.Json;

namespace DarkBestiary.Map
{
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum MapEncounterType
    {
        Undefined,
        Random,
        Vendor,
        Eatery,
        Gamble,
        Scenario,
        Buff,
        Loot,
        Skill,
        Watchtower,
    }
}