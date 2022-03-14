using System;
using Newtonsoft.Json;

namespace DarkBestiary
{
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum DamageInfoFlags
    {
        None = 0,
        Critical = 1 << 1,
        Backstab = 1 << 2,
        Dodged = 1 << 3,
        Blocked = 1 << 4,
        Reflected = 1 << 5,
        SpiritLink = 1 << 6,
        Invulnerable = 1 << 7,
        Cleave = 1 << 8,
        Thorns = 1 << 9,
    }
}