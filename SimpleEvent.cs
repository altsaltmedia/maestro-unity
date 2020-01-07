using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Events/Simple Event")]
    public class SimpleEvent : EventBase
    {

#if UNITY_EDITOR
        [Multiline]
        [Header("Simple Event")]
        public string DeveloperDescription = "";
#endif
        [SerializeField]
        List<ScriptableObject> associatedVariables = new List<ScriptableObject>();

        readonly List<ISimpleEventListener> listeners = new List<ISimpleEventListener>();

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void Raise()
        {
            if(CallerRegistered() == true) {
                if (logCallersOnRaise == true || appSettings.logEventCallersAndListeners == true) {
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
                if(logCallersOnRaise == true || logListenersOnRaise == true || appSettings.logEventCallersAndListeners == true) {
                    LogClosingLine();
                }
                ClearCaller();
            }
        }

        protected override void LogCaller()
        {
            Debug.Log(string.Format("[event] [{0}] [{1}] {2} triggered simple event...", callerScene, this.name, callerName), callerObject);
            Debug.Log(string.Format("[event] [{0}] [{1}] {2}", callerScene, this.name, this.name), this);
        }

        void LogListenerOnRaise(ISimpleEventListener simpleEventListener)
        {
            Debug.Log(string.Format("[event] [{0}] [{1}] {2}", simpleEventListener.SceneName, this.name, simpleEventListener.ParentObject.name), simpleEventListener.ParentObject);
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Display every object currently listening to this event")]
        public void LogListeners()
        {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                Debug.Log(this.name + " event is registered on " + listeners[i].ParentObject.name, listeners[i].ParentObject);   
            }
        }

        public void RegisterListener(ISimpleEventListener listener)
        {
            if (logListenersOnRegister == true) {
                Debug.Log("The following listener subscribed to simple event " + this.name, this);
                Debug.Log(listener.ParentObject.name, listener.ParentObject);
            }
            listeners.Add(listener);
        }

        public void UnregisterListener(ISimpleEventListener listener)
        {
            listeners.Remove(listener);
        }

    }
}