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
            private set => _value = value;
        }

        [SerializeField]
        [ShowIf(nameof(hasDefault))]
        private Color _defaultValue;

        public Color defaultValue
        {
            get => _defaultValue;
            set => _defaultValue = value;
        }
        
        public void SetValue(GameObject callingObject, Color value)
        {
            StoreCaller(callingObject);
            
            this.value = value;
            
            SignalChange();
        }

        public void SetValue(Color value)
        {
            if (CallerRegistered() == false) return;
            
            this.value = value;
            
            SignalChange();
        }
        
        public void SetTransparent()
        {
            if (CallerRegistered() == false) return;
            
            value = new Color(0, 0, 0, 0);
            
            SignalChange();
        }

        public void SetOpaque()
        {
            if (CallerRegistered() == false) return;
            
            value = new Color(1, 1, 1, 1);
            
            SignalChange();
        }

        public override void SetToDefaultValue()
        {
            if (CallerRegistered() == false) return;
            
            if(hasDefault) {
                value = defaultValue;   
            } else {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }
            
            SignalChange();
        }
    }
}