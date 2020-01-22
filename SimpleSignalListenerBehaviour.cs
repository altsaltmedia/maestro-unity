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
    public class SimpleSignalListenerBehaviour : MonoBehaviour, ISimpleSignalListener, ISkipRegistration, ISerializationCallbackReceiver
    {
        [Required]
        [SerializeField]
        [OnValueChanged(nameof(OnEnable))]
        [FormerlySerializedAs("Event")]
        [FormerlySerializedAs("_simpleEvent")]
        [ReadOnly]
        private SimpleSignal _simpleSignal;

        [SerializeField]
        private SimpleSignalReference _simpleSignalReference = new SimpleSignalReference();

        private SimpleSignal simpleSignal => _simpleSignalReference.GetVariable(this);

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        [FormerlySerializedAs("Response")]
        [ReadOnly]
        private UnityEvent _response;

        private UnityEvent response => _response;
        
        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        protected GameObjectGenericAction _action;

        private GameObjectGenericAction action => _action;

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
            if(simpleSignal != null) {
                simpleSignal.RegisterListener(this);
            } else {
                Debug.LogWarning("Please set an event for SimpleEventListenerBehaviour on " + this.name, this.gameObject);
            }

//            if (migrated == false) {
//                UnityEventUtils.MigrateUnityEventList(nameof(_response), nameof(_action), 
//                    new SerializedObject(this));
//            }
        }

		private void OnDisable()
		{
            if (simpleSignal != null) {
                simpleSignal.UnregisterListener(this);
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

        public void OnAfterDeserialize()
        {
            if (migrated == false) {
                _simpleSignalReference.SetVariable(_simpleSignal);
            }
        }

        public void OnBeforeSerialize()
        {
            
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