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
    public class LongReference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        private long _constantValue;

        public long constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }

        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        private LongVariable _variable;

        public LongVariable variable
        {
            get
            {
#if UNITY_EDITOR
                if (hasSearchedForAsset == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                    hasSearchedForAsset = true;
                    LogMissingReferenceMessage(GetType().Name);
                    _variable = Utils.GetScriptableObject(referenceName) as LongVariable;
                }
#endif
                return _variable;
            }
            set => _variable = value;
        }

        public LongReference()
        { }

        public LongReference(long value)
        {
            useConstant = true;
            constantValue = value;
        }

        public long value => useConstant ? constantValue : variable.value;

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

        public static implicit operator long(LongReference reference)
        {
            return reference.value;
        }
    }
}