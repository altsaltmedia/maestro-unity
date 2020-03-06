using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using RotaryHeart.Lib.SerializableDictionary;


namespace AltSalt.Maestro
{

    [CreateAssetMenu(menuName = "Maestro/Content Extension/Text Collection Bank")]
    [Serializable]
    public class TextCollectionBank : ScriptableObject
    {
        [SerializeField]
        [Header("Text Collection Bank")]
        public TextCollection textCollection;

        [ShowInInspector]
        [ReadOnly]
        [InfoBox("Active text family should be set by TextTools in editor or a ModifyHandler at runtime")]
        private TextFamily _activeTextFamily;

        private TextFamily activeTextFamily
        {
            get => _activeTextFamily;
            set => _activeTextFamily = value;
        }

        public bool GetText(string key, out string text)
        {
            activeTextFamily = TextFamily.GetActiveTextFamily(textCollection.Keys.ToList());

            if (activeTextFamily != null) {

                if (textCollection[activeTextFamily].ContainsKey(key)) {
                    text = textCollection[activeTextFamily][key];
                    return true;
                }

                text =
                    $"Key {key} not found under priority text family {activeTextFamily.name} in text collection {this.name}";
                return false;
            }

            text = $"No active text families found in {this.name}";
            return false;
        }
        
#if UNITY_EDITOR
        public string GetActiveTextFamilyName()
        {
            return activeTextFamily != null ? activeTextFamily.name : "No active text family.";
        }
#endif
    }

    [Serializable]
    public class TextCollection : SerializableDictionaryBase<TextFamily, TextNodes> { }

    [Serializable]
    public class TextNodes : SerializableDictionaryBase<string, string> { }

}