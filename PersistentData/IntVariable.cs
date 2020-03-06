/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "Maestro/Variables/Int Variable")]
    public class IntVariable : ModifiableEditorVariable
    {
        protected override string title => nameof(IntVariable);
        
        [SerializeField]
        public int _value;

        public int value
        {
            get => _value;
            private set => _value = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasDefault))]
        private int _defaultValue;

        public int defaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public void SetValue(GameObject callinObject, int value)
        {
            StoreCaller(callinObject);

            this.value = value;

            SignalChange();
        }
        
        public void SetValue(int value)
        {
            if (CallerRegistered() == false) return;

            this.value = value;

            SignalChange();
        }

        public void SetValue(IntVariable value)
        {
            if (CallerRegistered() == false) return;

            this.value = value.value;

            SignalChange();
        }

        public void ApplyChange(int amount)
        {
            if (CallerRegistered() == false) return;

            value += amount;

            SignalChange();
        }

        public void ApplyChange(IntVariable amount)
        {
            if (CallerRegistered() == false) return;

            value += amount.value;

            SignalChange();
        }

        public void Multiply(int multiplier)
        {
            if (CallerRegistered() == false) return;

            value *= multiplier;

            SignalChange();
        }
        
        public void Multiply(IntVariable multiplier)
        {
            if (CallerRegistered() == false) return;

            value *= multiplier.value;

            SignalChange();
        }

        public void ClampMax(int max)
        {
            if (CallerRegistered() == false) return;

            if (value > max) {
                value = max;
            }

            SignalChange();
        }
        
        public void ClampMax(IntVariable max)
        {
            if (CallerRegistered() == false) return;

            if (value > max.value) {
                value = max.value;
            }

            SignalChange();
        }
        
        public void ClampMin(int max)
        {
            if (CallerRegistered() == false) return;

            if (value < max) {
                value = max;
            }

            SignalChange();
        }
        
        public void ClampMin(IntVariable max)
        {
            if (CallerRegistered() == false) return;

            if (value < max.value) {
                value = max.value;
            }

            SignalChange();
        }
        
        public void SetToDistance(int value)
        {
            if (CallerRegistered() == false) return;

            this.value = Mathf.Abs(this.value - value);

            SignalChange();
        }
        
        public void SetToDistance(IntVariable value)
        {
            if (CallerRegistered() == false) return;

            this.value = Mathf.Abs(this.value - value.value);

            SignalChange();
        }

        public void SetToRandom()
        {
            if (CallerRegistered() == false) return;

            this.value = (int)Random.value;

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