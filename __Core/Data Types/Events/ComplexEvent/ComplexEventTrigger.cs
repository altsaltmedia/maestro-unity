using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    public class ComplexEventTrigger : TriggerBase
    {
        [Required]
        [SerializeField]
        ComplexEvent complexEvent;

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

        public void RaiseEvent(GameObject caller, EventPayload eventPayload)
        {
            complexEvent.StoreCaller(caller);
            complexEvent.Raise(eventPayload);
        }
    }
}