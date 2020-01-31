/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using Sirenix.OdinInspector;
using UnityEditor;
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
        [ShowIf(nameof(useConstant))]
        [HideIf(nameof(hideConstantOptions))]
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
        [HideIf(nameof(useConstant))]
        private ColorVariable _variable;
        
        public override ScriptableObject GetVariable()
        {
            base.GetVariable();
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

        public Color GetValue()
        {
            return useConstant ? constantValue : (GetVariable() as ColorVariable).value;
        }

        public ColorVariable SetValue(GameObject callingObject, Color targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            ColorVariable colorVariable = GetVariable() as ColorVariable;
            colorVariable.StoreCaller(callingObject);
            colorVariable.SetValue(targetValue);
            return colorVariable;
        }
        
        public ColorVariable SetValue(GameObject callingObject, ColorVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            ColorVariable colorVariable = GetVariable() as ColorVariable;
            colorVariable.StoreCaller(callingObject);
            colorVariable.SetValue(targetValue.value);
            return colorVariable;
        }
        
#if UNITY_EDITOR        
        protected override bool ShouldPopulateReference()
        {
            if (useConstant == false && _variable == null) {
                return true;
            }

            return false;
        }

        protected override ScriptableObject ReadVariable()
        {
            return _variable;
        }
#endif
        
    }
}