/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Variables/Int Variable")]
    public class IntVariable : ModifiableEditorVariable
    {
        protected override string title => nameof(IntVariable);
        
        [SerializeField]
        public int _value;

        public int value
        {
            get => _value;
            set => _value = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasDefault))]
        private int _defaultValue;

        public int defaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public void SetValue(int value)
        {
            this.value = value;
        }

        public void SetValue(IntVariable value)
        {
            this.value = value.value;
        }

        public void ApplyChange(int amount)
        {
            value += amount;
        }

        public void ApplyChange(IntVariable amount)
        {
            value += amount.value;
        }

        public override void SetToDefaultValue()
        {
            if (hasDefault) {
                value = defaultValue;
            } else {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }
        }
    }
}