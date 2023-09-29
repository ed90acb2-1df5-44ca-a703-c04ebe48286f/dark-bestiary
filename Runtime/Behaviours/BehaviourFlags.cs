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
        BreaksOnStartTurn = 1 << 11,
        BreaksOnEndTurn = 1 << 12,
        BreaksOnCombatEnd = 1 << 13,
        BreaksOnCast = 1 << 14,
        BreaksOnWeaponSwap = 1 << 15,
        Physical = 1 << 16,
        Magical = 1 << 17,
        Dispellable = 1 << 18,
        Temporary = 1 << 19,
        MonsterModifier = 1 << 20,
        MonsterAffix = 1 << 21,
        EpisodeAffix = 1 << 22,
        ItemAffix = 1 << 23,
        DoNotRemoveOnDeath = 1 << 24,
        DoNotRemoveOnScenarioExit = 1 << 25,
        DoNotRemoveOnVisionScenarioExit = 1 << 26,
        Food = 1 << 27,
        IgnoreImmunity = 1 << 28,
        Ascension = 1 << 29,
        Unique = 1 << 30,
        Oneshot = 1 << 31,
    }
}