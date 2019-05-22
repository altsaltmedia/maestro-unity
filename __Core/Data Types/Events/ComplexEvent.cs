using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Events/Complex Event")]
    public class ComplexEvent : ScriptableObject
    {

#if UNITY_EDITOR
        [Multiline]
        [Header("Complex Event")]
        public string DeveloperDescription = "";
#endif
        public bool LogListenersOnRegister;
        public bool LogListenersOnRaise;

		private List<ComplexEventListenerBehaviour> listeners = new List<ComplexEventListenerBehaviour>();

        public void Raise()
        {
            if (LogListenersOnRaise == true) {
                Debug.Log(this.name + " raised! Following listeners activated:");
            }
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (LogListenersOnRaise == true) {
                    Debug.Log(listeners[i], listeners[i].gameObject);
                }
                listeners[i].OnEventRaised(EventPayload.CreateInstance());
            }
        }

        public void Raise(string value)
        {
            if (LogListenersOnRaise == true) {
                Debug.Log(this.name + " raised! Following listeners activated:");
            }
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (LogListenersOnRaise == true) {
                    Debug.Log(listeners[i], listeners[i].gameObject);
                }
                listeners[i].OnEventRaised(EventPayload.CreateInstance(value));
            }
        }

        public void Raise(int value)
        {
            if (LogListenersOnRaise == true) {
                Debug.Log(this.name + " raised! Following listeners activated:");
            }
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (LogListenersOnRaise == true) {
                    Debug.Log(listeners[i], listeners[i].gameObject);
                }
                listeners[i].OnEventRaised(EventPayload.CreateInstance(value));
            }
        }

        public void Raise(bool value)
        {
            if (LogListenersOnRaise == true) {
                Debug.Log(this.name + " raised! Following listeners activated:");
            }
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (LogListenersOnRaise == true) {
                    Debug.Log(listeners[i], listeners[i].gameObject);
                }
                listeners[i].OnEventRaised(EventPayload.CreateInstance(value));
            }
        }

        public void Raise(ScriptableObject value)
        {
            if (LogListenersOnRaise == true) {
                Debug.Log(this.name + " raised! Following listeners activated:");
            }
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (LogListenersOnRaise == true) {
                    Debug.Log(listeners[i], listeners[i].gameObject);
                }
                listeners[i].OnEventRaised(EventPayload.CreateInstance(value));
            }
        }

        public void Raise(EventPayload eventPayload)
		{
            if (LogListenersOnRaise == true) {
                Debug.Log(this.name + " raised! Following listeners activated:");
            }
            for (int i = listeners.Count - 1; i >= 0; i--) {
                listeners[i].OnEventRaised(eventPayload);
                if (LogListenersOnRaise == true) {
                    Debug.Log(listeners[i], listeners[i].gameObject);
                }
			}
		}
		
		public void RegisterListener(ComplexEventListenerBehaviour listener)
		{
            if (LogListenersOnRegister == true) {
                Debug.Log(this.name + " - the following listener was registered:");
                Debug.Log(listener.gameObject.name, listener.gameObject);
            }
			listeners.Add(listener);
		}

#if UNITY_EDITOR
        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Display every object currently listening to this event")]
#endif
        public void LogListeners()
        {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                Debug.Log(this.name + " event is registered on " + listeners[i].gameObject.name, listeners[i].gameObject);
            }
        }

        public void UnregisterListener(ComplexEventListenerBehaviour listener)
		{
			listeners.Remove(listener);
		}

	}
}