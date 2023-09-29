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
        public static I18N Instance => s_Instance ?? (s_Instance = Container.Instance.Instantiate<I18N>());
        private static I18N s_Instance;

        public Dictionary<string, I18NDictionaryInfo> Dictionaries { get; } = new();

        private readonly II18NStringRepository m_I18NStringRepository;
        private readonly I18NDictionary m_Fallback;

        private I18NDictionary m_Active;

        private static I18NDictionary LoadDictionary(string locale)
        {
            var json = File.ReadAllText(Environment.s_StreamingAssetsPath + "/compiled/i18n/" + locale + ".json");

            return JsonConvert.DeserializeObject<I18NDictionary>(json);
        }

        public static string GetDefaultLocale()
        {
            return CultureInfo.CurrentCulture.Name;
        }

        public I18N(II18NStringRepository i18NStringRepository)
        {
            m_I18NStringRepository = i18NStringRepository;

            var files = Directory.GetFiles(Environment.s_StreamingAssetsPath + "/compiled/i18n", "*.json");

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

            m_Fallback = LoadDictionary("en-US");

            ChangeLocale(GetDefaultLocale());
        }

        public void ChangeLocale(string locale)
        {
            m_Active = Dictionaries.ContainsKey(locale) ? LoadDictionary(locale) : m_Fallback;
        }

        public string Translate(string key)
        {
            var dictionary = m_Active.Data.ContainsKey(key) ? m_Active : m_Fallback;

            return dictionary.Data.ContainsKey(key) ? dictionary.Data[key] : key;
        }

        public I18NString Get(string? key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return new I18NString(new I18NStringData(""));
            }

            var i18NString = m_I18NStringRepository.FindByKey(key);

            return i18NString.IsNullOrEmpty() ? new I18NString(new I18NStringData(key)) : i18NString;
        }
    }
}