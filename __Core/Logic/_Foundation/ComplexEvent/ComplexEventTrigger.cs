using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class ComplexEventTrigger : EventTriggerBase
    {
        [Required]
        [SerializeField]
        ComplexEvent complexEvent;

        public ComplexEvent ComplexEventTarget {
            get {
                return complexEvent;
            }
            set {
                complexEvent = value;
            }
        }

        public void RaiseEvent(GameObject caller)
        {
            complexEvent.StoreCaller(caller);
            complexEvent.Raise();
        }

        public void RaiseEvent(GameObject caller, string value)
        {
            complexEvent.StoreCaller(caller);
            complexEvent.Raise(value);
        }

        public void RaiseEvent(GameObject caller, int value)
        {
            complexEvent.StoreCaller(caller);
            complexEvent.Raise(value);
        }

        public void RaiseEvent(GameObject caller, bool value)
        {
            complexEvent.StoreCaller(caller);
            complexEvent.Raise(value);
        }

        public void RaiseEvent(GameObject caller, ScriptableObject value)
        {
            complexEvent.StoreCaller(caller);
            complexEvent.Raise(value);
        }

        public void RaiseEvent(GameObject caller, UnityEngine.Object value)
        {
            complexEvent.StoreCaller(caller);
            complexEvent.Raise(value);
        }

        public void RaiseEvent(GameObject caller, EventPayload eventPayload)
        {
            complexEvent.StoreCaller(caller);
            complexEvent.Raise(eventPayload);
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName)
        {
            complexEvent.StoreCaller(caller, sourceScene, sourceName);
            complexEvent.Raise();
        }

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName, ScriptableObject value)
        {
            complexEvent.StoreCaller(caller, sourceScene, sourceName);
            complexEvent.Raise(value);
        }
    }
}