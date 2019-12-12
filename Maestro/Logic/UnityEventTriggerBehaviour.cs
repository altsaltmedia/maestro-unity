using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace AltSalt.Maestro.Logic
{
    public class UnityEventTriggerBehaviour : MonoBehaviour
    {
        [SerializeField]
        private bool _hasDelay;
        
        private bool hasDelay => _hasDelay;
    
        [SerializeField]
        [ShowIf(nameof(hasDelay))]
        private float _delay;

        private float delay => _delay;

        [SerializeField]
        private UnityEvent _unityEvent;

        private UnityEvent unityEvent => _unityEvent;

        public void CallEvent()
        {
            if (hasDelay) {
               StartCoroutine(ExecuteEvent());
            }
            else {
                unityEvent.Invoke();
            }
        }

        private IEnumerator ExecuteEvent()
        {
            yield return new WaitForSeconds(delay);
            unityEvent.Invoke();
        }
    }
}