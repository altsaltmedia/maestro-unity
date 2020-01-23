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
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private IntVariable _variable;

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
        
        public void SetVariable(IntVariable value)
        {
            _variable = value;
        }

        public IntReference()
        { }

        public IntReference(int value)
        {
            useConstant = true;
            constantValue = value;
        }

        public int GetValue()
        {
            return useConstant ? constantValue : (GetVariable() as IntVariable).value;
        }
        
        public IntVariable SetValue(GameObject callingObject, int targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.SetValue(targetValue);
            return intVariable;
        }
		
        public IntVariable SetValue(GameObject callingObject, IntVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.SetValue(targetValue);
            return intVariable;
        }
        
        public IntVariable ApplyChange(GameObject callingObject, int targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.ApplyChange(targetValue);
            return intVariable;
        }
		
        public IntVariable ApplyChange(GameObject callingObject, IntVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.ApplyChange(targetValue);
            return intVariable;
        }
        
        public IntVariable Multiply(GameObject callingObject, int targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.Multiply(targetValue);
            return intVariable;
        }
		
        public IntVariable Multiply(GameObject callingObject, IntVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.Multiply(targetValue);
            return intVariable;
        }
        
        public IntVariable ClampMax(GameObject callingObject, int targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.ClampMax(targetValue);
            return intVariable;
        }
		
        public IntVariable ClampMax(GameObject callingObject, IntVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.ClampMax(targetValue);
            return intVariable;
        }
        
        public IntVariable ClampMin(GameObject callingObject, int targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.ClampMin(targetValue);
            return intVariable;
        }
		
        public IntVariable ClampMin(GameObject callingObject, IntVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.ClampMin(targetValue);
            return intVariable;
        }
        
        public IntVariable SetToDistance(GameObject callingObject, int targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.SetToDistance(targetValue);
            return intVariable;
        }
		
        public IntVariable SetToDistance(GameObject callingObject, IntVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.SetToDistance(targetValue);
            return intVariable;
        }
        
        public IntVariable SetToRandom(GameObject callingObject)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.SetToRandom();
            return intVariable;
        }

        public IntVariable SetToDefaultValue(GameObject callingObject)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable() as IntVariable;
            intVariable.StoreCaller(callingObject);
            intVariable.SetToDefaultValue();
            return intVariable;
        }
    }
}