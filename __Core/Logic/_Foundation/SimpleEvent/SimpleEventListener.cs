﻿using UnityEngine;
using UnityEngine.Events;

namespace AltSalt
{
    [ExecuteInEditMode]
    public class SimpleEventListener : ISimpleEventListener
    {
        public delegate void OnTargetEventDelegate();
        public event OnTargetEventDelegate OnTargetEventExecuted;

        readonly SimpleEvent targetEvent;
        readonly GameObject parent;

        public SimpleEventListener(SimpleEvent eventToRegister, GameObject parentObject)
        {
            targetEvent = eventToRegister;
            parent = parentObject;
            targetEvent.RegisterListener(this);
        }

        public void OnEventRaised()
        {
            OnTargetEventExecuted();
        }

        public void DestroyListener()
        {
            targetEvent.UnregisterListener(this);
        }

        public GameObject GetGameObject()
        {
            return parent;
        }

        public void LogName(string callingInfo)
        {
            Debug.Log(callingInfo + parent, parent);
        }
    }
}