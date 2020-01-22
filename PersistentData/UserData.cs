using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [Serializable]
    public class UserData : ScriptableObject
    {
        [SerializeField]
        private UserPreferencesCollection _userPreferencesCollection = new UserPreferencesCollection();

        private UserPreferencesCollection userPreferencesCollection => _userPreferencesCollection;
        
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