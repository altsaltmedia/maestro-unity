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
    public class FloatVariable : ModifiableEditorVariable
    {
        protected override string title => nameof(FloatVariable);
        
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
            if (CallerRegistered() == false) return;

            this.value = value;

            SignalChange();
        }

        public void SetValue(FloatVariable value)
        {
            if (CallerRegistered() == false) return;

            this.value = value.value;

            SignalChange();
        }

        public void ApplyChange(float amount)
        {
            if (CallerRegistered() == false) return;

            value += amount;

            SignalChange();
        }

        public void ApplyChange(FloatVariable amount)
        {
            if (CallerRegistered() == false) return;

            value += amount.value;

            SignalChange();
        }

        public void Multiply(float multiplier)
        {
            if (CallerRegistered() == false) return;

            value *= multiplier;

            SignalChange();
        }
        
        public void Multiply(FloatVariable multiplier)
        {
            if (CallerRegistered() == false) return;

            value *= multiplier.value;

            SignalChange();
        }

        public void ClampMax(float max)
        {
            if (CallerRegistered() == false) return;

            if (value > max) {
                value = max;
            }

            SignalChange();
        }
        
        public void ClampMax(FloatVariable max)
        {
            if (CallerRegistered() == false) return;

            if (value > max.value) {
                value = max.value;
            }

            SignalChange();
        }
        
        public void ClampMin(float max)
        {
            if (CallerRegistered() == false) return;

            if (value < max) {
                value = max;
            }

            SignalChange();
        }
        
        public void ClampMin(FloatVariable max)
        {
            if (CallerRegistered() == false) return;

            if (value < max.value) {
                value = max.value;
            }

            SignalChange();
        }
        
        public void SetToDistance(float value)
        {
            if (CallerRegistered() == false) return;

            this.value = Mathf.Abs(this.value - value);

            SignalChange();
        }
        
        public void SetToDistance(FloatVariable value)
        {
            if (CallerRegistered() == false) return;

            this.value = Mathf.Abs(this.value - value.value);

            SignalChange();
        }

        public void SetToRandom()
        {
            if (CallerRegistered() == false) return;

            this.value = Random.value;

            SignalChange();
        }
        
        public void SetToSquareMagnitude(V2Variable v2Variable)
        {
            if (CallerRegistered() == false) return;

            value = v2Variable.value.sqrMagnitude;

            SignalChange();
        }


        public override void SetToDefaultValue()
        {
            if (CallerRegistered() == false) return;

            if (hasDefault) {
                value = defaultValue;
            } else {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }

            SignalChange();
        }
    }
}