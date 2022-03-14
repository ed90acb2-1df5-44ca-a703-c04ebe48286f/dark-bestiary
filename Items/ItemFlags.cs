using System;
using Newtonsoft.Json;

namespace DarkBestiary.Items
{
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum ItemFlags
    {
        None = 0,
        Stackable = 1 << 1,
        Craftable = 1 << 2,
        Gambleable = 1 << 3,
        Dismantable = 1 << 4,
        HasRandomSuffix = 1 << 5,
        HasRandomSocketCount = 1 << 6,
        HasRandomAffixes = 1 << 7,
        HasBlueprint = 1 << 8,
        Droppable = 1 << 9,
        UniqueEquipped = 1 << 10,
        QuestReward = 1 << 11,
        CampaignOnly = 1 << 12,
        VisionsOnly = 1 << 13,
        Illusory = 1 << 14,
        Sharpening = 1 << 15,
    }
}