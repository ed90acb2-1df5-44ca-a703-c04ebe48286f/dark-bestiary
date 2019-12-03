using System;
using Newtonsoft.Json;

namespace DarkBestiary.Skills
{
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum SkillFlags
    {
        None = 0,
        Passive = 1 << 1,
        Consumable = 1 << 2,
        MeleeWeapon = 1 << 3,
        RangedWeapon = 1 << 4,
        MagicWeapon = 1 << 5,
        Magic = 1 << 6,
        Movement = 1 << 7,
        CheckLineOfSight = 1 << 8,
        Delayed = 1 << 9,
        EndTurn = 1 << 10,
        Monster = 1 << 11,
        Talent = 1 << 12,
        Item = 1 << 13,
        DualWield = 1 << 14,
        FixedRange = 1 << 15,
    }
}