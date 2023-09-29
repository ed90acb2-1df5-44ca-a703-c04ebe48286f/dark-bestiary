using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DarkBestiary.Map
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MapEncounterState
    {
        Locked,
        Unlocked,
        Completed,
        Revealed,
    }
}