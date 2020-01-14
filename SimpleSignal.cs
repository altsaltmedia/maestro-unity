using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro
{
    public abstract class SimpleSignal : PersistentDataObject
    {
        readonly List<ISimpleSignalListener> _listeners = new List<ISimpleSignalListener>();

        private List<ISimpleSignalListener> listeners => _listeners;

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void SignalChange()
        {
            SanitizeListenerList();
            
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
            SanitizeListenerList();
            
            Debug.Log(string.Format("[event] [{0}] [{1}] {2} triggered simple event...", callerScene, this.name, callerName), callerObject);
            Debug.Log(string.Format("[event] [{0}] [{1}] {2}", callerScene, this.name, this.name), this);
        }

        private void LogListenerOnRaise(ISimpleSignalListener simpleSignalListener)
        {
            SanitizeListenerList();
            
            Debug.Log(string.Format("[event] [{0}] [{1}] {2}", simpleSignalListener.sceneName, this.name, simpleSignalListener.parentObject.name), simpleSignalListener.parentObject);
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Display every object currently listening to this event")]
        public void LogListeners()
        {
            SanitizeListenerList();
            
            for (int i = listeners.Count - 1; i >= 0; i--) {
                Debug.Log(this.name + " event is registered on " + listeners[i].parentObject.name, listeners[i].parentObject);   
            }
        }

        public void RegisterListener(ISimpleSignalListener listener)
        {
            SanitizeListenerList();

            if (logListenersOnRegister == true) {
                Debug.Log("The following listener subscribed to simple event " + this.name, this);
                Debug.Log(listener.parentObject.name, listener.parentObject);
            }
            listeners.Add(listener);
        }

        public void UnregisterListener(ISimpleSignalListener listener)
        {
            SanitizeListenerList();
            
            listeners.Remove(listener);
        }

        private void SanitizeListenerList()
        {
            _listeners.RemoveAll(x => x == null);
        }
    }
    
}