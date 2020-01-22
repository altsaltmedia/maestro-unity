using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class SimpleSignalReference : ReferenceBase
    {
        [SerializeField]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        private SimpleSignal _variable;

        public SimpleSignal GetVariable(Object callingObject)
        {
#if UNITY_EDITOR
            this.parentObject = callingObject;
            if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                searchAttempted = true;
                LogMissingReferenceMessage(GetType().Name);
                var variableSearch = Utils.GetScriptableObject(referenceName) as SimpleSignal;
                if (variableSearch != null) {
                    _variable = variableSearch;
                    EditorUtility.SetDirty(callingObject);
                    LogFoundReferenceMessage(GetType().Name, _variable);
                }
            }
#endif
            return _variable;
        }
        
        public void SetVariable(SimpleSignal value)
        {
            _variable = value;
        }
        
        public SimpleSignalReference()
        { }

        public SimpleSignalReference(SimpleSignal value)
        {
            _variable = value;
        }

        protected override void UpdateReferenceName()
        {
			if (GetVariable(parentObject) != null) {
				searchAttempted = false;
				referenceName = GetVariable(parentObject).name;
			}
//			else {
//				referenceName = "";
//			}
        }
    }
}