/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Bool Variable")]
    public class BoolVariable : VariableBase
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Bool Variable")]
        string DeveloperDescription = "";
#endif
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
            this.value = value;
        }

        public void SetValue(BoolVariable value)
        {
            this.value = value.value;
        }

        public void Toggle()
        {
            value = !value;
        }

        public override void SetDefaultValue()
        {
            if (hasDefault == true)  {
                value = defaultValue;
            } else {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }
        }
    }
}