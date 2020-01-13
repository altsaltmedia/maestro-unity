/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Variables/Bool Variable")]
    public class BoolVariable : ModifiableEditorVariable
    {
        protected override string title => nameof(BoolVariable);

        [SerializeField]
        private bool _value;

        public bool value
        {
            get => _value;
            private set => _value = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasDefault))]
        private bool _defaultValue;

        public bool defaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public void SetValue(bool value)
        {
            if (CallerRegistered() == false) return;
            
            this.value = value;
            
            SignalChange();
        }

        public void SetValue(BoolVariable value)
        {
            if (CallerRegistered() == false) return;
            
            this.value = value.value;
            
            SignalChange();
        }

        public void Toggle()
        {
            if (CallerRegistered() == false) return;
            
            value = !value;
            
            SignalChange();
        }

        public override void SetToDefaultValue()
        {
            if (CallerRegistered() == false) return;
            
            if (hasDefault == true)  {
                value = defaultValue;
            } else {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }
            
            SignalChange();
        }
    }
}