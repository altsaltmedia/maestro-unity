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
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private LongVariable _variable;
        
        public override ScriptableObject GetVariable()
        {
            base.GetVariable();
            return _variable;
        }

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
        
        public void SetVariable(LongVariable value)
        {
            _variable = value;
        }

        public LongReference()
        { }

        public LongReference(long value)
        {
            useConstant = true;
            constantValue = value;
        }

        public long GetValue()
        {
            return useConstant ? constantValue : (GetVariable() as LongVariable).value;
        }
    }
}