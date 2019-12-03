using System;
using System.Collections.Generic;
using DarkBestiary.Scenarios.Scenes;

namespace DarkBestiary.Data
{
    [Serializable]
    public class SceneData : Identity<int>
    {
        public string Prefab;
        public EnvironmentData Environment = new EnvironmentData();
        public bool HasNoExit;
        public bool IsScripted;
    }

    [Serializable]
    public class EnvironmentData : Identity<int>
    {
        public int Index;
        public string NameKey;
        public string DescriptionKey;
        public string Ambience;
        public List<WeatherData> Weather = new List<WeatherData>();
    }

    [Serializable]
    public class WeatherData
    {
        public WeatherType Type;
        public string Prefab;
    }
}