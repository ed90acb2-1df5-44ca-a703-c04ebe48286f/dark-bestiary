using UnityEngine;

namespace DarkBestiary
{
    public static class Environment
    {
        #if UNITY_ANDROID
        public static readonly string PersistentDataPath = Application.persistentDataPath;
        public static readonly string StreamingAssetsPath = Application.streamingAssetsPath;
        #elif UNITY_IOS
        public static readonly string PersistentDataPath = Application.persistentDataPath;
        public static readonly string StreamingAssetsPath = Application.dataPath + "/Raw";
        #else
        public static readonly string s_PersistentDataPath = Application.persistentDataPath;
        public static readonly string s_StreamingAssetsPath = Application.streamingAssetsPath;
        #endif
    }
}