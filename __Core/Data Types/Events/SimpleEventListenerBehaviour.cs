using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace AltSalt
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class SimpleEventListenerBehaviour : MonoBehaviour, ISimpleEventListener
    {
        [Required]
        public SimpleEvent Event;

        [ValidateInput("IsPopulated")]
        public UnityEvent Response;


        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

		private void OnDisable()
		{
            Event.UnregisterListener(this);
		}

        public void OnEventRaised()
        {
            Response.Invoke();
        }

        public void LogName(string callingInfo)
        {
            Debug.Log(callingInfo + name, gameObject);
        }

        private static bool IsPopulated(UnityEvent attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}