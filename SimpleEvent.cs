using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace AltSalt.Maestro
{
    [CreateAssetMenu(menuName = "AltSalt/Events/Simple Event")]
    public class SimpleEvent : PersistentDataObject
    {
        protected override string title => nameof(SimpleEvent);
        
        [SerializeField]
        List<ScriptableObject> associatedVariables = new List<ScriptableObject>();

        readonly List<ISimpleEventListener> listeners = new List<ISimpleEventListener>();

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void SignalChange()
        {
            if (CallerRegistered() == false) return;
            
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

        protected override void LogCaller()
        {
            Debug.Log(string.Format("[event] [{0}] [{1}] {2} triggered simple event...", callerScene, this.name, callerName), callerObject);
            Debug.Log(string.Format("[event] [{0}] [{1}] {2}", callerScene, this.name, this.name), this);
        }

        private void LogListenerOnRaise(ISimpleEventListener simpleEventListener)
        {
            Debug.Log(string.Format("[event] [{0}] [{1}] {2}", simpleEventListener.sceneName, this.name, simpleEventListener.parentObject.name), simpleEventListener.parentObject);
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Display every object currently listening to this event")]
        public void LogListeners()
        {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                Debug.Log(this.name + " event is registered on " + listeners[i].parentObject.name, listeners[i].parentObject);   
            }
        }

        public void RegisterListener(ISimpleEventListener listener)
        {
            if (logListenersOnRegister == true) {
                Debug.Log("The following listener subscribed to simple event " + this.name, this);
                Debug.Log(listener.parentObject.name, listener.parentObject);
            }
            listeners.Add(listener);
        }

        public void UnregisterListener(ISimpleEventListener listener)
        {
            listeners.Remove(listener);
        }

    }
}