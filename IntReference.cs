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
    public class IntReference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        private int _constantValue;

        public int constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }

        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        private IntVariable _variable;

        public IntVariable variable
        {
            get
            {
#if UNITY_EDITOR
                if (hasSearchedForAsset == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                    hasSearchedForAsset = true;
                    LogMissingReferenceMessage(GetType().Name);
                    _variable = Utils.GetScriptableObject(referenceName) as IntVariable;
                }
#endif
                return _variable;
            }
            set => _variable = value;
        }

        public IntReference()
        { }

        public IntReference(int value)
        {
            useConstant = true;
            constantValue = value;
        }

        public int value => useConstant ? constantValue : variable.value;

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

        public static implicit operator int(IntReference reference)
        {
            return reference.value;
        }
    }
}