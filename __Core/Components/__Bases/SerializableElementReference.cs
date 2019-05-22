using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public abstract class SerializableElementReference
    {
        [ReadOnly]
        [SerializeField]
        [ValidateInput("IsPopulated", "Please set a game object reference to retrieve lookup ID.")]
        int lookupID;

        [NonSerialized]
        [ShowInInspector]
        [OnValueChanged("PopulateID")]
        [InfoBox("This field will not be saved — instead, it will automatically fill in the lookup ID, which will be used to repopulate this reference between sessions.")]
        public ReferableElement gameObjectReference;

        public void PopulateID()
        {
            lookupID = gameObjectReference.GetID();
        }

        public int GetLookupID()
        {
            return lookupID;
        }

        protected static bool IsPopulated(int attribute)
        {
            if(attribute != 0) {
                return true;
            }
            return false;
        }
    }
}