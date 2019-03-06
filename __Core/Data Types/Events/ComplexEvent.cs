using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Complex Event")]
    public class ComplexEvent : ScriptableObject
    {

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
        readonly string xComplexEvent;

        [Multiline]
        public string DeveloperDescription = "";
#endif
        public bool LogListenersOnRegister;
        public bool LogListenersOnRaise;

		private List<ComplexEventListenerBehaviour> listeners = new List<ComplexEventListenerBehaviour>();

        public void Raise(string value)
        {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (LogListenersOnRaise == true) {
                    Debug.Log(this.name + " raised! Following listeners activated:");
                    Debug.Log(listeners[i]);
                }
                listeners[i].OnEventRaised(new EventPayload(value));
            }
        }

        public void Raise(int value)
        {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (LogListenersOnRaise == true) {
                    Debug.Log(this.name + " raised! Following listeners activated:");
                    Debug.Log(listeners[i]);
                }
                listeners[i].OnEventRaised(new EventPayload(value));
            }
        }

        public void Raise(bool value)
        {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (LogListenersOnRaise == true) {
                    Debug.Log(this.name + " raised! Following listeners activated:");
                    Debug.Log(listeners[i]);
                }
                listeners[i].OnEventRaised(new EventPayload(value));
            }
        }

        public void Raise(EventPayload eventPayload)
		{
			for (int i = listeners.Count - 1; i >= 0; i--) {
                if (LogListenersOnRaise == true) {
                    Debug.Log(this.name + " raised! Following listeners activated:");
                    Debug.Log(listeners[i]);
                }
                listeners[i].OnEventRaised(eventPayload);
			}
		}
		
		public void RegisterListener(ComplexEventListenerBehaviour listener)
		{
            if (LogListenersOnRegister == true) {
                Debug.Log(this.name + " - the following listener was registered:");
                Debug.Log(listener);
            }
			listeners.Add(listener);
		}
		
		public void UnregisterListener(ComplexEventListenerBehaviour listener)
		{
			listeners.Remove(listener);
		}

	}
}