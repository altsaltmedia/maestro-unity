using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using RotaryHeart.Lib.SerializableDictionary;


namespace AltSalt
{

    [CreateAssetMenu(menuName = "AltSalt/Modify/Text Collection Bank")]
    [Serializable]
    public class TextCollectionBank : ScriptableObject
    {
        [SerializeField]
        [Header("Text Collection Bank")]
        public TextCollection textCollection;

        public string GetText(TextFamily textFamily, string key)
        {
            return textCollection[textFamily][key];
        }

    }

    [Serializable]
    public class TextCollection : SerializableDictionaryBase<TextFamily, TextNodes> { }

    [Serializable]
    public class TextNodes : SerializableDictionaryBase<string, string> { }

}