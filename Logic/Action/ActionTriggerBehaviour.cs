using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro.Logic.Action
{
    [ExecuteInEditMode]
    public class ActionTriggerBehaviour : MonoBehaviour
    {
        [ShowInInspector]
        private static bool syncEditorActionHeadings = true;
        
        [SerializeField]
        private bool _logCallersOnRaise;

        private bool logCallersOnRaise
        {
            get => _logCallersOnRaise;
            set => _logCallersOnRaise = value;
        }

        [SerializeField]
        [OnValueChanged(nameof(OnEnable))]
        private ActionTrigger _actionTrigger = new ActionTrigger();

        private ActionTrigger actionTrigger
        {
            get => _actionTrigger;
            set => _actionTrigger = value;
        }

        private void OnEnable()
        {
            actionTrigger.Initialize(this, nameof(_actionTrigger));
#if UNITY_EDITOR
            actionTrigger.CallPopulateReferences();
#endif
        }

        private void Start()
        {
            if (actionTrigger.triggerOnStart == true) {
                CallPerformActions(this.gameObject);
            }
            
            if (actionTrigger.resetGameStateOnStart == true) {
                CallResetGameState(this.gameObject);
            }
        }

        public void CallPerformActions(GameObject callingObject)
        {
            if (callingObject == null) {
                throw new UnassignedReferenceException("You must specify a calling game object.");
            }

            if (logCallersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                Debug.Log($"Action Trigger on {this.gameObject.name} triggered!", this.gameObject);
                Debug.Log($"Call executed by {callingObject.name}", callingObject);
                Debug.Log("--------------------------");
            }

            actionTrigger.PerformActions(this.gameObject);
        }

        public void CallResetGameState(GameObject callingObject)
        {
            actionTrigger.ResetGameState(callingObject);
        }

#if UNITY_EDITOR
        [Button]
        [ShowInInspector]
        private void CallPerformActions()
        {
            actionTrigger.PerformActions(this.gameObject);
        }
        
        private void Update()
        {
            if (syncEditorActionHeadings == true) {
                actionTrigger.CallSyncEditorActionHeadings();
                actionTrigger.CallSyncComplexSubheadings(this.gameObject,
                    new SerializedObject(this).FindProperty(nameof(_actionTrigger)));
                actionTrigger.SyncFullActionDescription();
            }
        }
#endif
    }
}