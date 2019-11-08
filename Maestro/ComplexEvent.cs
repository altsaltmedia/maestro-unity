using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Events/Complex Event")]
    public class ComplexEvent : EventBase
    {

#if UNITY_EDITOR
        [Multiline]
        [Header("Complex Event")]
        public string DeveloperDescription = "";
#endif
        [SerializeField]
        List<ComplexEventParameters> requiredParameters = new List<ComplexEventParameters>();

        [SerializeField]
        List<ComplexEventParameters> optionalParameters = new List<ComplexEventParameters>();

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

        public void Raise(object value)
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

                    if(value is EventPayload) {
                        listeners[i].OnEventRaised(value as EventPayload);
                    } else {
                        listeners[i].OnEventRaised(EventPayload.CreateInstance(value));
                    }
                }
                if (logCallersOnRaise == true || logListenersOnRaise == true || appSettings.logEventCallersAndListeners.Value == true) {
                    LogClosingLine();
                }
                ClearCaller();
            }
        }

        protected override void LogCaller()
        {
            Debug.Log(string.Format("[event] [{0}] [{1}] {2} triggered complex event...", callerScene, this.name, callerName), callerObject);
            Debug.Log(string.Format("[event] [{0}] [{1}] {2}", callerScene, this.name, this.name), this);
        }

        void LogListenerOnRaise(ComplexEventListenerBehaviour complexEventListenerBehaviour)
        {
            Debug.Log(string.Format("[event] [{0}] [{1}] {2}", complexEventListenerBehaviour.gameObject.scene.name, this.name, complexEventListenerBehaviour.name), complexEventListenerBehaviour.gameObject);
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

        [Serializable]
        class ComplexEventParameters
        {
            public string description;
            public DataType dataType;

            public bool customKey;

            [FormerlySerializedAs("eventPayloadKey"),ShowIf(nameof(customKey))]
            public CustomKey _customKey;
        }

	}
}