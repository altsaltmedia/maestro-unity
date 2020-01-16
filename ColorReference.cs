/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ColorReference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        private Color _constantValue;

        public Color constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }

        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private ColorVariable _variable;

        public ColorVariable GetVariable(Object callingObject)
        {
#if UNITY_EDITOR
            this.parentObject = callingObject;
            if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                searchAttempted = true;
                LogMissingReferenceMessage(GetType().Name);
                _variable = Utils.GetScriptableObject(referenceName) as ColorVariable;
                if (_variable != null) {
                    LogFoundReferenceMessage(GetType().Name, _variable);
                }
            }
#endif
            return _variable;
        }
        
        public void SetVariable(ColorVariable value) => _variable = value;

        public ColorReference()
        { }

        public ColorReference(Color value)
        {
            useConstant = true;
            constantValue = value;
        }

        public Color GetValue(Object callingObject)
        {
            this.parentObject = callingObject;
            return useConstant ? constantValue : GetVariable(callingObject).value;
        }
        
        public ColorVariable SetValue(GameObject callingObject, ColorVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            ColorVariable colorVariable = GetVariable(callingObject);
            colorVariable.StoreCaller(callingObject);
            colorVariable.SetValue(targetValue.value);
            return colorVariable;
        }

        protected override void UpdateReferenceName()
        {
            if (_variable != null) {
                searchAttempted = false;
                referenceName = _variable.name;
            }
//            else {
//                referenceName = "";
//            }
        }
    }
}