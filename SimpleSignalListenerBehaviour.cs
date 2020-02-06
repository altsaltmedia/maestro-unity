using System.Collections.Generic;
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
    public class SimpleSignalListenerBehaviour : MonoBehaviour, ISimpleSignalListener, ISkipRegistration
    {
        [Required]
        [SerializeField]
        [OnValueChanged(nameof(OnEnable))]
        [FormerlySerializedAs("Event")]
        [FormerlySerializedAs("_simpleEvent")]
        //[ReadOnly]
        private SimpleSignal _simpleSignal;

        [SerializeField]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        private List<SimpleSignalReference> _simpleSignalReferences = new List<SimpleSignalReference>();

        private List<SimpleSignalReference> simpleSignalReferences => _simpleSignalReferences;

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
            MigrateSimpleSignal();
            
            string simpleSignalsListPath = nameof(_simpleSignalReferences);
            for (int i = 0; i < simpleSignalReferences.Count; i++) {
#if UNITY_EDITOR                
                simpleSignalReferences[i].PopulateVariable(this, 
                    $"{simpleSignalsListPath}.{i.ToString()}");
#endif                

                var simpleSignal = simpleSignalReferences[i].GetVariable() as SimpleSignal;
                
                if(simpleSignal != null) {
                    simpleSignal.RegisterListener(this);
                } else {
                    Debug.LogWarning("Please set an event for SimpleEventListenerBehaviour on " + this.name, this.gameObject);
                }
            }

#if UNITY_EDITOR            
            if (migrated == false) {
                UnityEventUtils.MigrateUnityEventList(nameof(_response), nameof(_action), 
                    new SerializedObject(this));
            }
#endif            
        }

        private void MigrateSimpleSignal()
        {
#if UNITY_EDITOR            
            if (migrated == false && simpleSignalReferences.Count < 1) {
                var serializedObject = new SerializedObject(this);
                var serializedProperty = serializedObject.FindProperty(nameof(_simpleSignalReferences));
                serializedProperty.arraySize++;
                serializedProperty.GetArrayElementAtIndex(0).FindPropertyRelative("_variable").objectReferenceValue =
                    _simpleSignal;
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();
            }
#endif            
        }

		private void OnDisable()
		{
            for (int i = 0; i < simpleSignalReferences.Count; i++) {
                
                var simpleSignal = simpleSignalReferences[i].GetVariable() as SimpleSignal;
                
                if(simpleSignal != null) {
                    simpleSignal.UnregisterListener(this);
                }
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