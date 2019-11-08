﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
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

        public void RaiseEvent(GameObject caller, object value)
        {
            complexEvent.StoreCaller(caller);

            if (value is string) {

                complexEvent.Raise((string)value);

            } else if (value is float) {

                complexEvent.Raise((float)value);

            } else if (value is bool) {

                complexEvent.Raise((bool)value);

            } else if (value is ScriptableObject) {

                complexEvent.Raise((ScriptableObject)value);

            } else {

                complexEvent.Raise(value);
            }
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

        public void RaiseEvent(UnityEngine.Object caller, string sourceScene, string sourceName, object value)
        {
            complexEvent.StoreCaller(caller, sourceScene, sourceName);
            complexEvent.Raise(value);
        }
    }
}