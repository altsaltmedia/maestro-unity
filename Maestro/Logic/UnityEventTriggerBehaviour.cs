using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AltSalt.Maestro.Logic
{
    public class UnityEventTriggerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private float _delay;

        private float delay
        {
            get => _delay;
            set => _delay = value;
        }

        [SerializeField]
        private UnityEvent _unityEvent;

        private UnityEvent unityEvent
        {
            get => _unityEvent;
            set => _unityEvent = value;
        }
        
        public void CallEvent()
        {
            StartCoroutine(ExecuteEvent());
        }

        private IEnumerator ExecuteEvent()
        {
            yield return new WaitForSeconds(delay);
            unityEvent.Invoke();
        }
    }
}