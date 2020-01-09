using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [Serializable]
    public class SimpleEventTrigger : EventTriggerBase
    {
        [Required]
        [SerializeField]
        private SimpleEvent simpleEvent;
        
        public SimpleEvent SimpleEventTarget {
            get {
                return simpleEvent;
            }
            set {
                simpleEvent = value;
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
