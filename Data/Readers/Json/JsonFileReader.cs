using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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

            if (!File.Exists(path))
            {
                return default;
            }

            var json = File.ReadAllText(path);
            var data = JsonConvert.DeserializeObject<T>(json, this.settings);

            Cache[path] = data;

            return data;
        }
    }
}