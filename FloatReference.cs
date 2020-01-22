/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class FloatReference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        private float _constantValue;

        public float constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }

        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private FloatVariable _variable;
        
        
        public FloatVariable GetVariable(Object callingObject)
        {
#if UNITY_EDITOR
            this.parentObject = callingObject;
            if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                searchAttempted = true;
                LogMissingReferenceMessage(GetType().Name);
                var variableSearch = Utils.GetScriptableObject(referenceName) as FloatVariable;
                if (variableSearch != null) {
                    
                    SerializedObject serializedObject = new SerializedObject(callingObject);
                    string fieldName = "";
                    FieldInfo[] fields = callingObject.GetType().GetFields();
                    for (int i = 0; i < fields.Length; i++) {
                        var fieldValue = fields[i].GetValue(callingObject);
                        if (fieldValue == this) {
                            fieldName = fields[i].Name;
                        }
                    }

                    SerializedProperty serializedProperty = serializedObject.FindProperty(fieldName);
                    serializedProperty.FindPropertyRelative(nameof(_variable)).objectReferenceValue = variableSearch;
                    serializedObject.ApplyModifiedProperties();
                    
                    LogFoundReferenceMessage(GetType().Name, _variable);
                }
            }
#endif
            return _variable;
        }
        
        public void SetVariable(FloatVariable value)
        {
            _variable = value;
        }

        public FloatReference()
        { }

        public FloatReference(float value)
        {
            useConstant = true;
            constantValue = value;
        }

        public float GetValue(Object callingObject)
        {
            this.parentObject = callingObject;
            return useConstant ? constantValue : GetVariable(callingObject).value;
        }
        
        public FloatVariable SetValue(GameObject callingObject, float targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.SetValue(targetValue);
            return floatVariable;
        }
		
        public FloatVariable SetValue(GameObject callingObject, FloatVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.SetValue(targetValue);
            return floatVariable;
        }
        
        public FloatVariable ApplyChange(GameObject callingObject, float targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.ApplyChange(targetValue);
            return floatVariable;
        }
		
        public FloatVariable ApplyChange(GameObject callingObject, FloatVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.ApplyChange(targetValue);
            return floatVariable;
        }
        
        public FloatVariable Multiply(GameObject callingObject, float targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.Multiply(targetValue);
            return floatVariable;
        }
		
        public FloatVariable Multiply(GameObject callingObject, FloatVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.Multiply(targetValue);
            return floatVariable;
        }
        
        public FloatVariable ClampMax(GameObject callingObject, float targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.ClampMax(targetValue);
            return floatVariable;
        }
		
        public FloatVariable ClampMax(GameObject callingObject, FloatVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.ClampMax(targetValue);
            return floatVariable;
        }
        
        public FloatVariable ClampMin(GameObject callingObject, float targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.ClampMin(targetValue);
            return floatVariable;
        }
		
        public FloatVariable ClampMin(GameObject callingObject, FloatVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.ClampMin(targetValue);
            return floatVariable;
        }
        
        public FloatVariable SetToDistance(GameObject callingObject, float targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.SetToDistance(targetValue);
            return floatVariable;
        }
		
        public FloatVariable SetToDistance(GameObject callingObject, FloatVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.SetToDistance(targetValue);
            return floatVariable;
        }
        
        public FloatVariable SetToRandom(GameObject callingObject)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.SetToRandom();
            return floatVariable;
        }

        public FloatVariable SetToSquareMagnitude(GameObject callingObject, V2Variable sourceValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.SetToSquareMagnitude(sourceValue);
            return floatVariable;
        }
        
        public FloatVariable SetToDefaultValue(Object callingObject, string sourceScene, string sourceName)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject, sourceScene, sourceName);
            floatVariable.SetToDefaultValue();
            return floatVariable;
        }
        
        public FloatVariable SetToDefaultValue(GameObject callingObject)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            FloatVariable floatVariable = GetVariable(callingObject);
            floatVariable.StoreCaller(callingObject);
            floatVariable.SetToDefaultValue();
            return floatVariable;
        }

        protected override void UpdateReferenceName()
        {
            if (GetVariable(parentObject) != null) {
                searchAttempted = false;
                referenceName = _variable.name;
            }
//            else {
//                referenceName = "";
//            }
        }
        
    }
}