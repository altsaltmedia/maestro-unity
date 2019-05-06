using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using RotaryHeart.Lib.SerializableDictionary;

namespace AltSalt
{

    [CreateAssetMenu(menuName = "AltSalt/Modify/Localization Corpus")]
    public class LocalizationCorpus : ScriptableObject
    {
        [SerializeField]
        [Header("Localization Corpus")]
        public LanguageSet languageSet;

        public string GetText(Language language, string key)
        {
            return languageSet[language][key];
        }

    }

    [Serializable]
    public class LanguageSet : SerializableDictionaryBase<Language, TextNode> { }

    [Serializable]
    public class TextNode : SerializableDictionaryBase<string, string> { }

}