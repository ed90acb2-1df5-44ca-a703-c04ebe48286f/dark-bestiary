using System;
using Newtonsoft.Json;

namespace DarkBestiary
{
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum DamageFlags
    {
        None = 0,
        CantBeDodged = 1 << 1,
        CantBeBlocked = 1 << 2,
        CantBeCritical = 1 << 3,
        Melee = 1 << 4,
        Magic = 1 << 5,
        Ranged = 1 << 6,
        Dot = 1 << 7,
        True = 1 << 8,
    }
}