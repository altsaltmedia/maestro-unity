using UnityEngine;
using UnityEngine.Events;

namespace AltSalt
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
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

        public void LogName(string callingInfo)
        {
            Debug.Log(callingInfo + parent, parent);
        }
    }
}