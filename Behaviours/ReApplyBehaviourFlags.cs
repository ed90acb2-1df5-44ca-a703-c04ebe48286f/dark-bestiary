using System;
using Newtonsoft.Json;

namespace DarkBestiary.Behaviours
{
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum ReApplyBehaviourFlags
    {
        None = 0,
        RefreshDuration = 1 << 1,
        RefreshEffect = 1 << 2,
        StackDuration = 1 << 3,
        StackEffect = 1 << 4,
    }
}