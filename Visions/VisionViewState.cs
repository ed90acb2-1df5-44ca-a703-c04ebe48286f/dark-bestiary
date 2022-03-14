using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DarkBestiary.Visions
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum VisionViewState
    {
        Locked,
        Unlocked,
        Completed,
        Skipped,
        Revealed,
    }
}