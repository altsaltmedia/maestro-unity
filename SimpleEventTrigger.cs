using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class SimpleEventTrigger : ReferenceBase
    {
        [Required]
        [SerializeField]
        [FormerlySerializedAs("simpleEvent")]
        [OnValueChanged(nameof(UpdateReferenceName))]
        private SimpleEvent _simpleEvent;
        
        public SimpleEvent simpleEvent {
            get
            {
                if (searchAttempted == false && _simpleEvent == null && string.IsNullOrEmpty(referenceName) == false) {
                    searchAttempted = true;
                    LogMissingReferenceMessage(GetType().Name);
                    _simpleEvent = Utils.GetScriptableObject(referenceName) as SimpleEvent;
                    if (_simpleEvent != null) {
                        LogFoundReferenceMessage(GetType().Name, _simpleEvent);
                    }
                }
                return _simpleEvent;
            }
            set => _simpleEvent = value;
        }
        
        protected override void UpdateReferenceName()
        {
            if (simpleEvent != null) {
                referenceName = simpleEvent.name;
            }
        }

        public void RaiseEvent(GameObject caller)
        {
            simpleEvent.StoreCaller(caller);
            simpleEvent.Raise();
        }

        public void RaiseEvent(GameObject caller, string sourceScene, string sourceName)
        {
            simpleEvent.StoreCaller(caller, sourceScene, sourceName);
            simpleEvent.Raise();
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName)
        {
            simpleEvent.StoreCaller(caller, sourceScene, sourceName);
            simpleEvent.Raise();
        }
    }

}
