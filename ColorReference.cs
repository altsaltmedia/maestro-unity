/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

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
        private ColorVariable _variable;

        public ColorVariable variable
        {
            get
            {
#if UNITY_EDITOR
                if (hasSearchedForAsset == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                    hasSearchedForAsset = true;
                    LogMissingReferenceMessage(GetType().Name);
                    _variable = Utils.GetScriptableObject(referenceName) as ColorVariable;
                }
#endif
                return _variable;
            }
            set => _variable = value;
        }

        public ColorReference()
        { }

        public ColorReference(Color value)
        {
            useConstant = true;
            constantValue = value;
        }

        public Color value => useConstant ? constantValue : variable.value;

        protected override void UpdateReferenceName()
        {
            if (_variable != null) {
                hasSearchedForAsset = false;
                referenceName = _variable.name;
            }
//            else {
//                referenceName = "";
//            }
        }

        public static implicit operator Color(ColorReference reference)
        {
            return reference.value;
        }
    }
}