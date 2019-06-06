using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Events/Simple Event")]
    public class SimpleEvent : EventBase
    {

#if UNITY_EDITOR
        [Multiline]
        [Header("Simple Event")]
        public string DeveloperDescription = "";
#endif

        readonly List<ISimpleEventListener> listeners = new List<ISimpleEventListener>();

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void Raise()
        {
            if(CallerRegistered() == true) {
                if (logCallersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogCaller();
                }
                if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners == true) {
                    LogListenersHeading(listeners.Count);
                }
                for (int i = listeners.Count - 1; i >= 0; i--) {
                    if (logListenersOnRaise == true || appSettings.logEventCallersAndListeners == true) {
                        LogListenerOnRaise(listeners[i]);
                    }
                    listeners[i].OnEventRaised();
                }
                if(logCallersOnRaise == true || logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogClosingLine();
                }
                ClearCaller();
            }
        }

        protected override void LogCaller()
        {
            if (callerObject != null) {
                Debug.Log("[event] [" + this.name + "] Simple event raised : " + this.name, this);
                Debug.Log("[event] [" + this.name + "] " + callerObject.name + " triggered the event", callerObject);
            } else {
                Debug.Log("[event] [" + this.name + " Simple event raised : " + this.name, this);
                Debug.Log("[event] [" + this.name + "] " + callerString + " triggered the event");
            }
        }

        void LogListenerOnRaise(ISimpleEventListener simpleEventListener)
        {
            Debug.Log("[event] [" + this.name + "] " + simpleEventListener.GetGameObject().name, simpleEventListener.GetGameObject());
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Display every object currently listening to this event")]
        public void LogListeners()
        {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                Debug.Log(this.name + " event is registered on " + listeners[i].GetGameObject().name, listeners[i].GetGameObject());   
            }
        }

        public void RegisterListener(ISimpleEventListener listener)
        {
            if (logListenersOnRegister == true) {
                Debug.Log("The following listener subscribed to simple event " + this.name, this);
                Debug.Log(listener.GetGameObject().name, listener.GetGameObject());
            }
            listeners.Add(listener);
        }

        public void UnregisterListener(ISimpleEventListener listener)
        {
            listeners.Remove(listener);
        }

    }
}