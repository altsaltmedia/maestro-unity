using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEditor;
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
        [ValidateInput(nameof(IsPopulated))]
        protected GameObjectGenericAction _action;

        public GameObjectGenericAction action
        {
            get => _action;
            set => _action = value;
        }

        [SerializeField]
        [InfoBox("Specifies whether this dependency should be recorded when the RegisterDependencies tool is used.")]
        [FormerlySerializedAs("doNotRecord")]
        private bool _doNotRecord;

        public bool doNotRecord => _doNotRecord;

        public UnityEngine.Object parentObject => gameObject;

        public string sceneName => gameObject.scene.name;
        
        [SerializeField]
        private bool _migrated = false;

        private bool migrated
        {
            get => _migrated;
            set => _migrated = value;
        }

        private void OnEnable()
        {
            if(simpleEvent != null) {
                simpleEvent.RegisterListener(this);
            } else {
                Debug.LogWarning("Please set an event for SimpleEventListenerBehaviour on " + this.name, this.gameObject);
            }

            if (migrated == false) {
                MigrationUtils.MigrateUnityEventList(nameof(_response), nameof(_action), 
                    new SerializedObject(this));
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
            action.Invoke(this.gameObject);
        }

        public void LogName(string callingInfo)
        {
            Debug.Log(callingInfo + gameObject, gameObject);
        }

        private static bool IsPopulated(UnityEvent attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        private static bool IsPopulated(GameObjectGenericAction attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}