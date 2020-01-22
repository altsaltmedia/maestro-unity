/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Variables/Long Variable")]
    public class LongVariable : ModifiableEditorVariable
    {
        protected override string title => nameof(LongVariable);

        [SerializeField]
        private long _value;

        public long value
        {
            get => _value;
            private set => _value = value;
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
            if (CallerRegistered() == false) return;

            this.value = value;

            SignalChange();
        }

        public void SetValue(LongVariable value)
        {
            if (CallerRegistered() == false) return;

            this.value = value.value;

            SignalChange();
        }

        public void ApplyChange(long amount)
        {
            if (CallerRegistered() == false) return;

            value += amount;

            SignalChange();
        }

        public void ApplyChange(LongVariable amount)
        {
            if (CallerRegistered() == false) return;

            value += amount.value;

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