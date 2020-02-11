using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    
    [ExecuteInEditMode]
    public class SimpleSignalListenerBehaviour : MonoBehaviour, ISimpleSignalListener, ISkipRegistration
    {
        [SerializeField]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        private List<SimpleSignalReference> _simpleSignalReferences = new List<SimpleSignalReference>();

        private List<SimpleSignalReference> simpleSignalReferences => _simpleSignalReferences;

        [SerializeField]
        [ValidateInput(nameof(IsPopulated))]
        protected GameObjectGenericAction _action;

        private GameObjectGenericAction action => _action;

        [SerializeField]
        [InfoBox("Specifies whether this dependency should be recorded when the RegisterDependencies tool is used.")]
        [FormerlySerializedAs("doNotRecord")]
        private bool _skipRegistration;

        public bool skipRegistration => _skipRegistration;

        public UnityEngine.Object parentObject => gameObject;

        public string sceneName => gameObject.scene.name;

        private void OnEnable()
        {
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