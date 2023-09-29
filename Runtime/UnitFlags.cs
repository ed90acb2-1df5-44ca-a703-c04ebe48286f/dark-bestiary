﻿using System;
using Newtonsoft.Json;

namespace DarkBestiary
{
    [Flags]
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum UnitFlags
    {
        None = 0,
        Playable = 1 << 1,
        BlocksMovement = 1 << 2,
        BlocksVision = 1 << 3,
        Dummy = 1 << 4,
        Beast = 1 << 5,
        Insect = 1 << 6,
        Corpseless = 1 << 7,
        Humanoid = 1 << 8,
        Magical = 1 << 9,
        Ethereal = 1 << 10,
        Ooze = 1 << 11,
        Plant = 1 << 12,
        Wooden = 1 << 13,
        Undead = 1 << 14,
        Demon = 1 << 15,
        Boss = 1 << 16,
        Summoned = 1 << 17,
        Immovable = 1 << 18,
        Structure = 1 << 19,
        Stone = 1 << 20,
        Mechanical = 1 << 21,
        Airborne = 1 << 22,
        CampaignOnly = 1 << 23,
        MovingViaScript = 1 << 24,
    }
}