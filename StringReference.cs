using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class StringReference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        private string _constantValue;

        private string constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }

        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private StringVariable _variable;

        public StringVariable GetVariable(Object callingObject)
        {
#if UNITY_EDITOR
            this.parentObject = callingObject;
            if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                searchAttempted = true;
                LogMissingReferenceMessage(GetType().Name);
                var variableSearch = Utils.GetScriptableObject(referenceName) as StringVariable;
                if (variableSearch != null) {
                    Undo.RecordObject(callingObject, "save variable reference");
                    _variable = variableSearch;
                    LogFoundReferenceMessage(GetType().Name, _variable);
                }
            }
#endif
            return _variable;
        }

        public void SetVariable(StringVariable value) => _variable = value;
        
        public StringReference()
        { }

        public StringReference(string value)
        {
            useConstant = true;
            constantValue = value;
        }

        public string GetValue(Object callingObject)
        {
            this.parentObject = callingObject;
            return useConstant ? constantValue : GetVariable(callingObject).value;
        }
        
        public StringVariable SetValue(GameObject callingObject, string targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            StringVariable stringVariable = GetVariable(callingObject);
            stringVariable.StoreCaller(callingObject);
            stringVariable.SetValue(targetValue);
            return stringVariable;
        }
        
        public StringVariable SetValue(GameObject callingObject, StringVariable targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            StringVariable stringVariable = GetVariable(callingObject);
            stringVariable.StoreCaller(callingObject);
            stringVariable.SetValue(targetValue.value);
            return stringVariable;
        }

        protected override void UpdateReferenceName()
        {
            if (_variable != null) {
                referenceName = _variable.name;
            }
//            else {
//                referenceName = "";
//            }
        }
        
    }
}