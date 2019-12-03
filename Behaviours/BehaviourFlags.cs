using System;
using Newtonsoft.Json;

namespace DarkBestiary.Behaviours
{
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum BehaviourFlags
    {
        None = 0,
        Hidden = 1 << 1,
        Negative = 1 << 2,
        Positive = 1 << 3,
        Offensive = 1 << 4,
        Defensive = 1 << 5,
        BreaksOnDealDamage = 1 << 6,
        BreaksOnDealDirectDamage = 1 << 7,
        BreaksOnTakeDamage = 1 << 8,
        BreaksOnCasterDeath = 1 << 9,
        BreaksOnCrowdControl = 1 << 10,
        BreaksOnCast = 1 << 11,
        Physical = 1 << 12,
        Magical = 1 << 13,
        Dispellable = 1 << 14,
        Temporary = 1 << 15,
        MonsterModifier = 1 << 16,
        MonsterAffix = 1 << 17,
        EpisodeAffix = 1 << 18,
        DoNotRemoveOnDeath = 1 << 19,
        Food = 1 << 20,
        IgnoreImmunity = 1 << 21,
    }
}