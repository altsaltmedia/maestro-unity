/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Long Variable")]
    public class LongVariable : VariableBase
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Long Variable")]
        string DeveloperDescription = "";
#endif
        public long Value;
        public bool hasDefault;

        [ShowIf("hasDefault")]
        public long DefaultValue;

        public void SetValue(long value)
        {
            Value = value;
        }

        public void SetValue(LongVariable value)
        {
            Value = value.Value;
        }

        public void ApplyChange(long amount)
        {
            Value += amount;
        }

        public void ApplyChange(LongVariable amount)
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