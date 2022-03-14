using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace DarkBestiary.Data.Readers.Json
{
    public class JsonFileReader : IFileReader
    {
        private static readonly Dictionary<string, object> Cache = new Dictionary<string, object>();

        private readonly JsonSerializerSettings settings;

        public JsonFileReader()
        {
            this.settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = new List<JsonConverter>
                {
                    new EffectJsonConverter(),
                    new BehaviourJsonConverter(),
                    new ValidatorJsonConverter(),
                    new RewardJsonConverter(),
                }
            };
        }

        public void Write<T>(T model, string path)
        {
            var json = JsonConvert.SerializeObject(model, this.settings);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);

            Cache[path] = model;
        }

        public T Read<T>(string path)
        {
            if (Cache.ContainsKey(path))
            {
                return (T) Cache[path];
            }

            var data = Application.platform == RuntimePlatform.Android
                ? ReadFromApk<T>(path)
                : ReadFromFile<T>(path);

            Cache[path] = data;

            return data;
        }

        private T ReadFromApk<T>(string path)
        {
            using (var request = UnityWebRequest.Get(new Uri(path, UriKind.Absolute)))
            {
                request.SendWebRequest();

                while (!request.isDone)
                {
                }

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError(request.url + ": " + request.error);
                    return default;
                }

                return Deserialize<T>(request.downloadHandler.text);
            }
        }

        private T ReadFromFile<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default;
            }

            return Deserialize<T>(File.ReadAllText(path));
        }

        private T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, this.settings);
        }
    }
}