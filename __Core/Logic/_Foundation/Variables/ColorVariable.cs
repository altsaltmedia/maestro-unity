/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Color Variable")]
    [ExecuteInEditMode]
    public class ColorVariable : VariableBase
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Color Variable")]
        string DeveloperDescription = "";
#endif
        [FormerlySerializedAs("Value")]
        [SerializeField]
        private Color _value = new Color(1,1,1,1);

        public Color value
        {
            get => _value;
            set => _value = value;
        }

        [SerializeField]
        private Color _defaultValue;

        [ShowIf(nameof(hasDefault))]
        public Color defaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }

        public void SetValue(Color value)
        {
            this.value = value;
        }

        public void SetValue(ColorVariable value)
        {
            this.value = value.value;
        }

        public void SetTransparent()
        {
            value = new Color(0, 0, 0, 0);
        }

        public void SetOpaque()
        {
            value = new Color(1, 1, 1, 1);
        }

        public override void SetDefaultValue()
        {
            if(hasDefault) {
                value = defaultValue;   
            } else {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }
        }
    }
}