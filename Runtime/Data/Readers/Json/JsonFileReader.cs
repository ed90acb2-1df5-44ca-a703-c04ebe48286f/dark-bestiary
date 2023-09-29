using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DarkBestiary.Data.Readers.Json
{
    public class JsonFileReader : IFileReader
    {
        private static readonly Dictionary<string, object> s_Cache = new();

        private readonly JsonSerializerSettings m_Settings = new()
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

        public void Write<T>(T model, string path)
        {
            var json = JsonConvert.SerializeObject(model, m_Settings);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, json);

            s_Cache[path] = model;
        }

        public T Read<T>(string path)
        {
            if (s_Cache.TryGetValue(path, out var value))
            {
                return (T) value;
            }

            var data = ReadFromFile<T>(path);

            s_Cache[path] = data;

            return data;
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
            return JsonConvert.DeserializeObject<T>(json, m_Settings);
        }
    }
}