using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Events/Complex Event")]
    public class ComplexEvent : EventBase
    {

#if UNITY_EDITOR
        [Multiline]
        [Header("Complex Event")]
        public string DeveloperDescription = "";
#endif
		private List<ComplexEventListenerBehaviour> listeners = new List<ComplexEventListenerBehaviour>();

        public void Raise()
        {
            if(CallerRegistered() == true) {
                if(logCallersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogCaller();
                }
                if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogListenersHeading(listeners.Count);
                }
                for (int i = listeners.Count - 1; i >= 0; i--) {
                    if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                        LogListenerOnRaise(listeners[i]);
                    }
                    listeners[i].OnEventRaised(EventPayload.CreateInstance());
                }
                if (logCallersOnRaise == true || logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogClosingLine();
                }
                ClearCaller();
            }
        }

        public void Raise(string value)
        {
            if (CallerRegistered() == true) {
                if (logCallersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogCaller();
                }
                if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogListenersHeading(listeners.Count);
                }
                for (int i = listeners.Count - 1; i >= 0; i--) {
                    if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                        LogListenerOnRaise(listeners[i]);
                    }
                    listeners[i].OnEventRaised(EventPayload.CreateInstance(value));
                }
                if (logCallersOnRaise == true || logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogClosingLine();
                }
                ClearCaller();
            }
        }

        public void Raise(int value)
        {
            if (CallerRegistered() == true) {
                if (logCallersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogCaller();
                }
                if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogListenersHeading(listeners.Count);
                }
                for (int i = listeners.Count - 1; i >= 0; i--) {
                    if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                        LogListenerOnRaise(listeners[i]);
                    }
                    listeners[i].OnEventRaised(EventPayload.CreateInstance(value));
                }
                if (logCallersOnRaise == true || logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogClosingLine();
                }
                ClearCaller();
            }
        }

        public void Raise(bool value)
        {
            if (CallerRegistered() == true) {
                if (logCallersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogCaller();
                }
                if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogListenersHeading(listeners.Count);
                }
                for (int i = listeners.Count - 1; i >= 0; i--) {
                    if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                        LogListenerOnRaise(listeners[i]);
                    }
                    listeners[i].OnEventRaised(EventPayload.CreateInstance(value));
                }
                if (logCallersOnRaise == true || logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogClosingLine();
                }
                ClearCaller();
            }
        }

        public void Raise(ScriptableObject value)
        {
            if (CallerRegistered() == true) {
                if (logCallersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogCaller();
                }
                if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogListenersHeading(listeners.Count);
                }
                for (int i = listeners.Count - 1; i >= 0; i--) {
                    if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                        LogListenerOnRaise(listeners[i]);
                    }
                    listeners[i].OnEventRaised(EventPayload.CreateInstance(value));
                }
                if (logCallersOnRaise == true || logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogClosingLine();
                }
                ClearCaller();
            }
        }

        public void Raise(EventPayload eventPayload)
		{
            if (CallerRegistered() == true) {
                if (logCallersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogCaller();
                }
                if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogListenersHeading(listeners.Count);
                }
                for (int i = listeners.Count - 1; i >= 0; i--) {
                    if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                        LogListenerOnRaise(listeners[i]);
                    }
                    listeners[i].OnEventRaised(eventPayload);
                }
                if (logCallersOnRaise == true || logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogClosingLine();
                }
                ClearCaller();
            }
        }

        protected override void LogCaller()
        {
            if (callerObject != null) {
                Debug.Log("[event] [" + this.name + "] Complex event raised : " + this.name, this);
                Debug.Log("[event] [" + this.name + "] " + callerObject.name + " triggered the event", callerObject);
            } else {
                Debug.Log("[event] [" + this.name + "] Complex event raised : " + this.name, this);
                Debug.Log("[event] [" + this.name + "] " + callerString + " triggered the event");
            }
        }

        void LogListenerOnRaise(ComplexEventListenerBehaviour complexEventListenerBehaviour)
        {
            Debug.Log("[event] [" + this.name + "] " + complexEventListenerBehaviour.name, complexEventListenerBehaviour.gameObject);
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Display every object currently listening to this event")]
        public void LogListeners()
        {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                Debug.Log(this.name + " event is registered on " + listeners[i].gameObject.name, listeners[i].gameObject);
            }
        }

        public void RegisterListener(ComplexEventListenerBehaviour listener)
        {
            if (logListenersOnRegister == true) {
                Debug.Log("The following listener subscribed to complex event " + this.name, this);
                Debug.Log(listener.gameObject.name, listener.gameObject);
            }
            listeners.Add(listener);
        }

        public void UnregisterListener(ComplexEventListenerBehaviour listener)
		{
			listeners.Remove(listener);
		}

	}
}