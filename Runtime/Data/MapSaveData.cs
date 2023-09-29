using System;
using System.Collections.Generic;
using DarkBestiary.Map;

namespace DarkBestiary.Data
{
    [Serializable]
    public class MapSaveData
    {
        public int LastCompletedEncounterIndex;
        public List<MapEncounterSaveData> Encounters = new();
    }

    [Serializable]
    public class MapEncounterSaveData
    {
        public MapEncounterData Data;
        public MapEncounterState State;
        public bool IsHidden;
    }
}