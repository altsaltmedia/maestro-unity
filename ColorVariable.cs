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
    [CreateAssetMenu(menuName = "AltSalt/Variables/Color Variable")]
    [ExecuteInEditMode]
    public class ColorVariable : ModifiableEditorVariable
    {
        protected override string title => nameof(ColorVariable);
        
        [FormerlySerializedAs("Value")]
        [SerializeField]
        private Color _value = new Color(1,1,1,1);

        public Color value
        {
            get => _value;
            set => _value = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasDefault))]
        private Color _defaultValue;

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

        public override void SetToDefaultValue()
        {
            if(hasDefault) {
                value = defaultValue;   
            } else {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }
        }
    }
}