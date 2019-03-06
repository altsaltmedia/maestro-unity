using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Simple Event")]
    public class SimpleEvent : ScriptableObject
    {

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
        readonly string xSimpleEvent;

        [Multiline]
        public string DeveloperDescription = "";
#endif
        public bool LogListenersOnRegister;
        public bool LogListenersOnRaise;

        readonly List<ISimpleEventListener> listeners = new List<ISimpleEventListener>();

        public void Raise()
        {
            if (LogListenersOnRaise == true) {
            }
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (LogListenersOnRaise == true) {
                    listeners[i].LogName(this.name + " event raised on ");
                }
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(ISimpleEventListener listener)
        {
            if (LogListenersOnRegister == true) {
                listener.LogName(this.name + " event - registered ");
            }
            listeners.Add(listener);
        }

        public void UnregisterListener(ISimpleEventListener listener)
        {
            listeners.Remove(listener);
        }

    }
}