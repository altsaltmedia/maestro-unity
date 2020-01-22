using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class V2Reference : VariableReference
    {
        [FormerlySerializedAs("ConstantValue")]
        [SerializeField]
        private Vector2 _constantValue;

        public Vector2 constantValue
        {
            get => _constantValue;
            set => _constantValue = value;
        }

        [FormerlySerializedAs("Variable")]
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private V2Variable _variable;

        public V2Variable GetVariable(Object callingObject)
        {
#if UNITY_EDITOR
            this.parentObject = callingObject;
            if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                searchAttempted = true;
                LogMissingReferenceMessage(GetType().Name);
                var variableSearch = Utils.GetScriptableObject(referenceName) as V2Variable;
                if (variableSearch != null) {
                    Undo.RecordObject(callingObject, "save variable reference");
                    _variable = variableSearch;
                    LogFoundReferenceMessage(GetType().Name, _variable);
                }
            }
#endif
            return _variable;
        }
        public void SetVariable(V2Variable value)
        {
            _variable = value;
        }

        public V2Reference()
        { }

        public V2Reference(Vector2 value)
        {
            useConstant = true;
            constantValue = value;
        }

        public Vector2 GetValue(Object callingObject)
        {
            this.parentObject = callingObject;
            return useConstant ? constantValue : GetVariable(callingObject).value;
        }
        
        public V2Variable SetValue(GameObject callingObject, Vector2 targetValue)
        {
            if (useConstant == true) {
                LogDefaultChangeError(callingObject);
                return null;
            }

            V2Variable v2Variable = GetVariable(callingObject);
            v2Variable.StoreCaller(callingObject);
            v2Variable.SetValue(targetValue);
            return v2Variable;
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