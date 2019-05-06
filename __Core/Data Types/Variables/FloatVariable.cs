/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Float Variable")]
    public class FloatVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Float Variable")]
        string DeveloperDescription = "";
#endif
        public float Value;
        public bool hasDefault;

        [ShowIf("hasDefault")]
        public float DefaultValue;

        public void SetValue(float value)
        {
            Value = value;
        }

        public void SetValue(FloatVariable value)
        {
            Value = value.Value;
        }

        public void ApplyChange(float amount)
        {
            Value += amount;
        }

        public void ApplyChange(FloatVariable amount)
        {
            Value += amount.Value;
        }

        public void SetDefaultValue()
        {
            if (hasDefault) {
                Value = DefaultValue;
            } else {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }
        }
    }
}