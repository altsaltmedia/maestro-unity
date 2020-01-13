using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class SimpleEventListenerBehaviour : MonoBehaviour, ISimpleEventListener, ISkipRegistration
    {
        [Required]
        [OnValueChanged(nameof(OnEnable))]
        [SerializeField]
        [FormerlySerializedAs("Event")]
        private SimpleEvent _simpleEvent;

        private SimpleEvent simpleEvent => _simpleEvent;

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        [FormerlySerializedAs("Response")]
        private UnityEvent _response;

        private UnityEvent response => _response;

        [SerializeField]
        [InfoBox("Specifies whether this dependency should be recorded when the RegisterDependencies tool is used.")]
        [FormerlySerializedAs("doNotRecord")]
        private bool _doNotRecord;

        public bool doNotRecord => _doNotRecord;

        public UnityEngine.Object parentObject => gameObject;

        public string sceneName => gameObject.scene.name;

        private void OnEnable()
        {
            if(simpleEvent != null) {
                simpleEvent.RegisterListener(this);
            } else {
                Debug.LogWarning("Please set an event for SimpleEventListenerBehaviour on " + this.name, this.gameObject);
            }
        }

		private void OnDisable()
		{
            if (simpleEvent != null) {
                simpleEvent.UnregisterListener(this);
            }
		}

        public void OnEventRaised()
        {
            response.Invoke();
        }

        public void LogName(string callingInfo)
        {
            Debug.Log(callingInfo + gameObject, gameObject);
        }

        private static bool IsPopulated(UnityEvent attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}