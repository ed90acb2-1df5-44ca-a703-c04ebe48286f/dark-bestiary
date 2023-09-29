using System;
using Newtonsoft.Json;

namespace DarkBestiary.Skills
{
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum SkillTargetFlags
    {
        None,
        Self,
        Flying,
        Enemy,
        Ally,
        Corpse,
        Unoccupied
    }
}