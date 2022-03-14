using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DarkBestiary.Data;
using DarkBestiary.Data.Repositories;
using Newtonsoft.Json;
using UnityEngine;

namespace DarkBestiary
{
    public class I18N
    {
        public static I18N Instance => instance ?? (instance = Container.Instance.Instantiate<I18N>());
        private static I18N instance;

        public Dictionary<string, I18NDictionaryInfo> Dictionaries { get; }
            = new Dictionary<string, I18NDictionaryInfo>();

        private readonly II18NStringRepository i18NStringRepository;
        private readonly I18NDictionary fallback;

        private I18NDictionary active;

        private static I18NDictionary LoadDictionary(string locale)
        {
            var json = File.ReadAllText(Environment.StreamingAssetsPath + "/compiled/i18n/" + locale + ".json");

            return JsonConvert.DeserializeObject<I18NDictionary>(json);
        }

        public static string GetDefaultLocale()
        {
            return CultureInfo.CurrentCulture.Name;
        }

        public I18N(II18NStringRepository i18NStringRepository)
        {
            this.i18NStringRepository = i18NStringRepository;

            var files = Directory.GetFiles(Environment.StreamingAssetsPath + "/compiled/i18n", "*.json");

            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var data = JsonConvert.DeserializeObject<I18NDictionary>(json);

                    Dictionaries.Add(data.Name, new I18NDictionaryInfo(data.Name, data.DisplayName));
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception.Message);
                }
            }

            this.fallback = LoadDictionary("en-US");

            ChangeLocale(GetDefaultLocale());
        }

        public void ChangeLocale(string locale)
        {
            this.active = Dictionaries.ContainsKey(locale) ? LoadDictionary(locale) : this.fallback;
        }

        public string Translate(string key)
        {
            var dictionary = this.active.Data.ContainsKey(key) ? this.active : this.fallback;

            return dictionary.Data.ContainsKey(key) ? dictionary.Data[key] : key;
        }

        public I18NString Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return new I18NString(new I18NStringData(""));
            }

            var i18NString = this.i18NStringRepository.FindByKey(key);

            return i18NString.IsNullOrEmpty() ? new I18NString(new I18NStringData(key)) : i18NString;
        }
    }
}