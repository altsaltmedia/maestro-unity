using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class UserDataKeyReference : ReferenceBase
    {
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private UserDataKey _variable;

        public UserDataKey GetVariable(Object callingObject)
        {
#if UNITY_EDITOR
            this.parentObject = callingObject;
            if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                searchAttempted = true;
                LogMissingReferenceMessage(GetType().Name);
                var variableSearch = Utils.GetScriptableObject(referenceName) as UserDataKey;
                if (variableSearch != null) {
                    Undo.RecordObject(callingObject, "save variable reference");
                    _variable = variableSearch;
                    LogFoundReferenceMessage(GetType().Name, _variable);
                }
            }
#endif
            return _variable;
        }
        
        public void SetVariable(UserDataKey value)
        {
            _variable = value;
        }
        
        
        public UserDataKeyReference()
        { }

        public UserDataKeyReference(UserDataKey value)
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