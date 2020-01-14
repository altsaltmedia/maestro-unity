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

        public IntVariable GetVariable(Object callingObject)
        {
#if UNITY_EDITOR
            this.parentObject = callingObject;
            if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                searchAttempted = true;
                LogMissingReferenceMessage(GetType().Name);
                _variable = Utils.GetScriptableObject(referenceName) as IntVariable;
                if (_variable != null) {
                    LogFoundReferenceMessage(GetType().Name, _variable);
                }
            }
#endif
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

        public int GetValue(Object callingObject)
        {
            this.parentObject = callingObject;
            return useConstant ? constantValue : GetVariable(callingObject).value;
        }
        
        public IntVariable SetValue(GameObject callingObject, int targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
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

            IntVariable intVariable = GetVariable(callingObject);
            intVariable.StoreCaller(callingObject);
            intVariable.SetToDefaultValue();
            return intVariable;
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