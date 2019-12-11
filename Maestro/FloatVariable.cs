/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Float Variable")]
    public class FloatVariable : VariableBase
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Float Variable")]
        string DeveloperDescription = "";
#endif
        [FormerlySerializedAs("Value")]
        [SerializeField]
        private float _value;

        public float value
        {
            get => _value;
            set => _value = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasDefault))]
        private float _defaultValue;

        [ShowIf("hasDefault")]
        public float defaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public void SetValue(float value)
        {
            _value = value;
        }

        public void SetValue(FloatVariable value)
        {
            _value = value.value;
        }

        public void ApplyChange(float amount)
        {
            _value += amount;
        }

        public void ApplyChange(FloatVariable amount)
        {
            _value += amount.value;
        }

        public void Multiply(float multiplier)
        {
            _value *= multiplier;
        }
        
        public void Multiply(FloatVariable multiplier)
        {
            _value *= multiplier.value;
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