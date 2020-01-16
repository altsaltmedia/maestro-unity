using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro
{
    [Serializable]
    public class SimpleEventReference : ReferenceBase
    {
        [Required]
        [SerializeField]
        [FormerlySerializedAs("_simpleEvent")]
        [FormerlySerializedAs("simpleEvent")]
        [OnValueChanged(nameof(UpdateReferenceName))]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        protected SimpleEvent _variable;
        
        public SimpleEvent GetVariable(Object callingObject)
        {
#if UNITY_EDITOR
            this.parentObject = callingObject;
            if (searchAttempted == false && _variable == null && string.IsNullOrEmpty(referenceName) == false) {
                searchAttempted = true;
                LogMissingReferenceMessage(GetType().Name);
                _variable = Utils.GetScriptableObject(referenceName) as SimpleEvent;
                if (_variable != null) {
                    LogFoundReferenceMessage(GetType().Name, _variable);
                }
            }
#endif
            return _variable;
        }
        
        public void SetVariable(SimpleEvent value) => _variable = value;

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