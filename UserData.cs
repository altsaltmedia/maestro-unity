using System;
using RotaryHeart.Lib.SerializableDictionary;

namespace AltSalt.Maestro
{
    public class UserData
    {
        private UserPreferencesCollection _userPreferencesCollection = new UserPreferencesCollection();

        private UserPreferencesCollection userPreferencesCollection => _userPreferencesCollection;
        
        public UserPreferences GetUserPreferences(CustomKey userKey)
        {
            if (userPreferencesCollection.ContainsKey(userKey)) {
                return userPreferencesCollection[userKey];
            }
            else {
                userPreferencesCollection.Add(userKey, new UserPreferences(userKey));
                return userPreferencesCollection[userKey];
            }
        }

        [Serializable]
        public class UserPreferencesCollection : SerializableDictionaryBase<CustomKey, UserPreferences> { }
    }
}