using System;
using Newtonsoft.Json;

namespace DarkBestiary.Behaviours
{
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum StatusFlags
    {
        None = 0,
        Slow = 1 << 1,
        Stun = 1 << 2,
        Swiftness = 1 << 3,
        Disarm = 1 << 4,
        Silence = 1 << 5,
        Weakness = 1 << 6,
        Invisibility = 1 << 7,
        Immobilization = 1 << 8,
        Invulnerability = 1 << 9,
        Adrenaline = 1 << 10,
        Blind = 1 << 11,
        Sleep = 1 << 12,
        Bleeding = 1 << 13,
        Burning = 1 << 14,
        Poison = 1 << 15,
        Confusion = 1 << 16,
        Taunt = 1 << 17,
        Polymorph = 1 << 18,
        Immortal = 1 << 19,
        Freecasting = 1 << 20,
        Undead = 1 << 21,
        MindControl = 1 << 22,
    }
}