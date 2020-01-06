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
    [CreateAssetMenu(menuName = "AltSalt/Variables/Float Variable")]
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
            this.value = value;
        }

        public void SetValue(FloatVariable value)
        {
            this.value = value.value;
        }

        public void ApplyChange(float amount)
        {
            value += amount;
        }

        public void ApplyChange(FloatVariable amount)
        {
            value += amount.value;
        }

        public void Multiply(float multiplier)
        {
            value *= multiplier;
        }
        
        public void Multiply(FloatVariable multiplier)
        {
            value *= multiplier.value;
        }

        public void ClampMax(float max)
        {
            if (value > max) {
                value = max;
            }
        }
        
        public void ClampMax(FloatVariable max)
        {
            if (value > max.value) {
                value = max.value;
            }
        }
        
        public void ClampMin(float max)
        {
            if (value < max) {
                value = max;
            }
        }
        
        public void ClampMin(FloatVariable max)
        {
            if (value < max.value) {
                value = max.value;
            }
        }

        public void SetToSquareMagnitude(V2Variable v2Variable)
        {
            value = v2Variable.value.sqrMagnitude;
        }

        public void SetToDistance(float value)
        {
            this.value = Mathf.Abs(this.value - value);
        }
        
        public void SetToDistance(FloatVariable value)
        {
            this.value = Mathf.Abs(this.value - value.value);
        }

        public void SetToRandom()
        {
            this.value = Random.value;
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