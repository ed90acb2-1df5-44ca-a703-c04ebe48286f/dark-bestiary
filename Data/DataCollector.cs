using System.Collections.Generic;
using System.IO;
using System.Linq;
using DarkBestiary.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DarkBestiary.Data
{
    public static class DataCollector
    {
        public static void Collect()
        {
            foreach (var file in ConcatData())
            {
                File.WriteAllText(Environment.StreamingAssetsPath + "/compiled/data/" + file.Key, file.Value);
            }

            foreach (var dictionary in ConcatI18N())
            {
                File.WriteAllText(
                    Environment.StreamingAssetsPath + "/compiled/i18n/" + dictionary.Value.Name + ".json",
                    JsonConvert.SerializeObject(dictionary.Value));
            }
        }

        private static Dictionary<string, I18NDictionary> ConcatI18N()
        {
            foreach (var file in Directory.GetFiles(Environment.StreamingAssetsPath + "/compiled/i18n", "*.json"))
            {
                File.Delete(file);
            }

            var core = Directory.GetFiles(Environment.StreamingAssetsPath + "/core/i18n", "*.json")
                .Select(file => JsonConvert.DeserializeObject<I18NDictionary>(File.ReadAllText(file)))
                .ToDictionary(dictionary => dictionary.Name);

            foreach (var folder in Directory.GetDirectories(Environment.StreamingAssetsPath + "/mods/"))
            {
                var i18npath = folder + "/i18n";

                if (!Directory.Exists(i18npath))
                {
                    continue;
                }

                foreach (var file in Directory.GetFiles(i18npath, "*.json"))
                {
                    var dictionary = JsonConvert.DeserializeObject<I18NDictionary>(File.ReadAllText(file));

                    if (!core.ContainsKey(dictionary.Name))
                    {
                        core.Add(dictionary.Name, dictionary);
                        continue;
                    }

                    foreach (var translation in dictionary.Data)
                    {
                        core[dictionary.Name].Data.AddOrSet(translation.Key, translation.Value);
                    }
                }
            }

            return core;
        }

        private static string[] GetModFolders()
        {
            var modpath = Environment.StreamingAssetsPath + "/mods/";

            if (!Directory.Exists(modpath))
            {
                Directory.CreateDirectory(modpath);
            }

            return Directory.GetDirectories(modpath);
        }

        private static Dictionary<string, string> ConcatData()
        {
            var core = Directory.GetFiles(Environment.StreamingAssetsPath + "/core/data", "*.json").ToDictionary(Path.GetFileName, ToDictionary);

            foreach (var folder in GetModFolders())
            {
                var datapath = folder + "/data";

                if (!Directory.Exists(datapath))
                {
                    continue;
                }

                foreach (var file in Directory.GetFiles(datapath, "*.json"))
                {
                    var filename = Path.GetFileName(file);

                    if (!core.ContainsKey(filename))
                    {
                        continue;
                    }

                    foreach (var element in ToDictionary(file))
                    {
                        core[filename].AddOrSet(element.Key, element.Value);
                    }
                }
            }

            return core.ToDictionary(file => file.Key, file => $"[\n{string.Join(",\n", file.Value.Select(element => element.Value))}\n]");
        }

        private static Dictionary<int, string> ToDictionary(string file)
        {
            return JArray.Parse(File.ReadAllText(file)).ToDictionary(entry => entry.Value<int>("Id"), entry => entry.ToString());
        }
    }
}