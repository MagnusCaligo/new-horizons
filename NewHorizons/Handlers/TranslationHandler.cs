﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using NewHorizons.External.Configs;

#endregion

namespace NewHorizons.Handlers
{
    public static class TranslationHandler
    {
        private static readonly Dictionary<TextTranslation.Language, Dictionary<string, string>>
            _shipLogTranslationDictionary = new Dictionary<TextTranslation.Language, Dictionary<string, string>>();

        private static readonly Dictionary<TextTranslation.Language, Dictionary<string, string>>
            _dialogueTranslationDictionary = new Dictionary<TextTranslation.Language, Dictionary<string, string>>();

        private static readonly Dictionary<TextTranslation.Language, Dictionary<string, string>>
            _uiTranslationDictionary = new Dictionary<TextTranslation.Language, Dictionary<string, string>>();

        public enum TextType
        {
            SHIPLOG,
            DIALOGUE,
            UI
        }

        public static string GetTranslation(string text, TextType type)
        {
            Dictionary<TextTranslation.Language, Dictionary<string, string>> dictionary;
            var language = TextTranslation.Get().m_language;

            switch (type)
            {
                case TextType.SHIPLOG:
                    dictionary = _shipLogTranslationDictionary;
                    break;
                case TextType.DIALOGUE:
                    dictionary = _dialogueTranslationDictionary;
                    break;
                case TextType.UI:
                    dictionary = _uiTranslationDictionary;
                    break;
                default:
                    return text;
            }

            if (dictionary.TryGetValue(language, out var table))
                if (table.TryGetValue(text, out var translatedText))
                    return translatedText;

            // Try to default to English
            if (dictionary.TryGetValue(TextTranslation.Language.ENGLISH, out var englishTable))
                if (englishTable.TryGetValue(text, out var englishText))
                    return englishText;

            // Default to the key
            return text;
        }

        public static void RegisterTranslation(TextTranslation.Language language, TranslationConfig config)
        {
            if (config.shipLogDictionary != null && config.shipLogDictionary.Count() > 0)
            {
                if (!_shipLogTranslationDictionary.ContainsKey(language))
                    _shipLogTranslationDictionary.Add(language, new Dictionary<string, string>());
                foreach (var originalKey in config.shipLogDictionary.Keys)
                {
                    var key = originalKey.Replace("&lt;", "<").Replace("&gt;", ">").Replace("<![CDATA[", "")
                        .Replace("]]>", "");

                    if (!_shipLogTranslationDictionary[language].ContainsKey(key))
                        _shipLogTranslationDictionary[language].Add(key, config.shipLogDictionary[originalKey]);
                    else _shipLogTranslationDictionary[language][key] = config.shipLogDictionary[originalKey];
                }
            }

            if (config.dialogueDictionary != null && config.dialogueDictionary.Count() > 0)
            {
                if (!_dialogueTranslationDictionary.ContainsKey(language))
                    _dialogueTranslationDictionary.Add(language, new Dictionary<string, string>());
                foreach (var originalKey in config.dialogueDictionary.Keys)
                {
                    var key = originalKey.Replace("&lt;", "<").Replace("&gt;", ">").Replace("<![CDATA[", "")
                        .Replace("]]>", "");

                    if (!_dialogueTranslationDictionary[language].ContainsKey(key))
                        _dialogueTranslationDictionary[language].Add(key, config.dialogueDictionary[originalKey]);
                    else _dialogueTranslationDictionary[language][key] = config.dialogueDictionary[originalKey];
                }
            }

            if (config.uiDictionary != null && config.uiDictionary.Count() > 0)
            {
                if (!_uiTranslationDictionary.ContainsKey(language))
                    _uiTranslationDictionary.Add(language, new Dictionary<string, string>());
                foreach (var originalKey in config.uiDictionary.Keys)
                {
                    var key = originalKey.Replace("&lt;", "<").Replace("&gt;", ">").Replace("<![CDATA[", "")
                        .Replace("]]>", "");

                    if (!_uiTranslationDictionary[language].ContainsKey(key))
                        _uiTranslationDictionary[language].Add(key, config.uiDictionary[originalKey]);
                    else _uiTranslationDictionary[language][key] = config.uiDictionary[originalKey];
                }
            }
        }

        public static void AddDialogue(string rawText, params string[] rawPreText)
        {
            var key = string.Join(string.Empty, rawPreText) + rawText;

            var text = GetTranslation(rawText, TextType.DIALOGUE);

            TextTranslation.Get().m_table.Insert(key, text);
        }

        public static void AddShipLog(string rawText, params string[] rawPreText)
        {
            var key = string.Join(string.Empty, rawPreText) + rawText;

            var text = GetTranslation(rawText, TextType.SHIPLOG);

            TextTranslation.Get().m_table.InsertShipLog(key, text);
        }

        public static int AddUI(string rawText)
        {
            var uiTable = TextTranslation.Get().m_table.theUITable;

            var text = GetTranslation(rawText, TextType.UI).ToUpper();

            var key = uiTable.Keys.Max() + 1;
            try
            {
                // Ensure it doesn't already contain our UI entry
                var pair = uiTable.First(x => x.Value.Equals(text));
                if (pair.Equals(default(KeyValuePair<int, string>))) key = pair.Key;
            }
            catch (Exception)
            {
            }

            TextTranslation.Get().m_table.Insert_UI(key, text);

            return key;
        }

        public static void ClearTables()
        {
            _shipLogTranslationDictionary.Clear();
            _dialogueTranslationDictionary.Clear();
            _uiTranslationDictionary.Clear();
        }
    }
}