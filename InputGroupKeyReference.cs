using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class InputGroupKeyReference : ReferenceBase
    {
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private InputGroupKey _variable;

        public InputGroupKey GetVariable(Object callingObject)
        {
#if UNITY_EDITOR
            this.parentObject = callingObject;
            if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                searchAttempted = true;
                LogMissingReferenceMessage(GetType().Name);
                var variableSearch = Utils.GetScriptableObject(referenceName) as InputGroupKey;
                if (variableSearch != null) {
                    _variable = variableSearch;
                    EditorUtility.SetDirty(callingObject);
                    LogFoundReferenceMessage(GetType().Name, _variable);
                }
            }
#endif
            return _variable;
        }
        
        public void SetVariable(InputGroupKey value)
        {
            _variable = value;
        }
        
        
        public InputGroupKeyReference()
        { }

        public InputGroupKeyReference(InputGroupKey value)
        {
            _variable = value;
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