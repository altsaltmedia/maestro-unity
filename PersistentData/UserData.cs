using System;
using System.Collections.Generic;
using System.Reflection;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class UserData : ScriptableObject
    {
        [SerializeField]
        private UserPreferencesCollection _userPreferencesCollection = new UserPreferencesCollection();

        private UserPreferencesCollection userPreferencesCollection => _userPreferencesCollection;
        
        private void OnEnable()
        {
            foreach (KeyValuePair<UserDataKey,UserPreferences> userPreferences in userPreferencesCollection) {
                FieldInfo[] fields = typeof(UserPreferences).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                for (int i = 0; i < fields.Length; i++) {
                    var fieldValue = fields[i].GetValue(userPreferences.Value);
                    if (fieldValue is ReferenceBase variableReference) {
                        variableReference.isSystemReference = true;
                    }
                }
            }
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void RefreshDependencies()
        {
            foreach (KeyValuePair<UserDataKey, UserPreferences> userPreferencesItem in userPreferencesCollection) {
                userPreferencesItem.Value.RefreshDependencies(userPreferencesItem.Key);
            }            
        }

        public UserPreferences GetUserPreferences(UserDataKey userKey)
        {
            if (userPreferencesCollection.ContainsKey(userKey)) {
                return userPreferencesCollection[userKey];
            }

            userPreferencesCollection.Add(userKey, new UserPreferences(this, userKey));
            EditorUtility.SetDirty(this);
            return userPreferencesCollection[userKey];
        }

        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public UserPreferencesCollection SetDefaults()
        {
            foreach (KeyValuePair<UserDataKey, UserPreferences> userPreferencesItem in userPreferencesCollection) {
                userPreferencesItem.Value.SetDefaults(this, userPreferencesItem.Key);
            }

            return userPreferencesCollection;
        }

        [Serializable]
        public class UserPreferencesCollection : SerializableDictionaryBase<UserDataKey, UserPreferences> { }
    }
}