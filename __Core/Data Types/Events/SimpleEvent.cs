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

#if UNITY_EDITOR
        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
#endif
        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                if (LogListenersOnRaise == true) {
                    Debug.Log(this.name + " event raised on " + listeners[i].GetGameObject().name, listeners[i].GetGameObject());
                }
                listeners[i].OnEventRaised();
            }
        }

#if UNITY_EDITOR
        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Display every object currently listening to this event")]
#endif
        public void LogListeners()
        {
            for (int i = listeners.Count - 1; i >= 0; i--) {
                Debug.Log(this.name + " event is registered on " + listeners[i].GetGameObject().name, listeners[i].GetGameObject());   
            }
        }

        public void RegisterListener(ISimpleEventListener listener)
        {
            if (LogListenersOnRegister == true) {
                Debug.Log(this.name + " event registered on " + listener.GetGameObject().name, listener.GetGameObject());
            }
            listeners.Add(listener);
        }

        public void UnregisterListener(ISimpleEventListener listener)
        {
            listeners.Remove(listener);
        }

    }
}