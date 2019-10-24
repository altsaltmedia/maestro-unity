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
        [SerializeField]
        private long _value;

        public long value
        {
            get => _value;
            set => _value = value;
        }

        [SerializeField]
        private long _defaultValue;

        [ShowIf("hasDefault")]
        public long defaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public void SetValue(long value)
        {
            this.value = value;
        }

        public void SetValue(LongVariable value)
        {
            this.value = value.value;
        }

        public void ApplyChange(long amount)
        {
            value += amount;
        }

        public void ApplyChange(LongVariable amount)
        {
            value += amount.value;
        }

        public override void SetDefaultValue()
        {
            if (hasDefault) {
                value = defaultValue;
            } else {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }
        }
    }
}