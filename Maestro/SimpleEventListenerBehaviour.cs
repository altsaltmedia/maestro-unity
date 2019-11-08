using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    #if UNITY_EDITOR
    [ExecuteInEditMode]
    #endif
    public class SimpleEventListenerBehaviour : MonoBehaviour, ISimpleEventListener, ISkipRegistration
    {
        [Required]
        [OnValueChanged("OnEnable")]
        public SimpleEvent Event;

        [ValidateInput("IsPopulated")]
        public UnityEvent Response;

        [SerializeField]
        [InfoBox("Specifies whether this dependency should be recorded when the RegisterDependencies tool is used.")]
        bool doNotRecord;

        public bool DoNotRecord {
            get {
                return doNotRecord;
            }
        }

        public UnityEngine.Object ParentObject {
            get {
                return gameObject;
            }
        }

        public string SceneName {
            get {
                return gameObject.scene.name;
            }
        }

        private void OnEnable()
        {
            if(Event != null) {
                Event.RegisterListener(this);
            } else {
                Debug.LogWarning("Please set an event for SimpleEventListenerBehaviour on " + this.name, this.gameObject);
            }
        }

		private void OnDisable()
		{
            if (Event != null) {
                Event.UnregisterListener(this);
            }
		}

        public void OnEventRaised()
        {
            Response.Invoke();
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