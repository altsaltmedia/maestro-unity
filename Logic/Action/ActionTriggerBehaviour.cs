using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt.Maestro.Logic
{
    [ExecuteInEditMode]
    public class ActionTriggerBehaviour : MonoBehaviour, IRegisterActionTriggerBehaviour
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
            if (actionTrigger.active == false) {
                return;
            }
            
            if (actionTrigger.resetGameStateOnStart == true) {
                CallResetGameState(this.gameObject);
            }
            
            if (actionTrigger.triggerOnStart == true) {
                CallPerformActions(this.gameObject);
            }
        }

        public void CallPerformActions(GameObject callingObject)
        {
            if (callingObject == null) {
                throw new UnassignedReferenceException("You must specify a calling game object.");
            }

            if (actionTrigger.active == false) {
                return;
            }

            if (logCallersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                Debug.Log($"Action Trigger on {this.gameObject.name} triggered!", this.gameObject);
                Debug.Log($"Call executed by {callingObject.name}", callingObject);
                Debug.Log("--------------------------");
            }

            if (actionTrigger.hasDelay == false) {
                actionTrigger.PerformActions(this.gameObject);
            }
            else {
                StartCoroutine(actionTrigger.PerformActionsDelayed(this.gameObject));
            }
        }

        private void CallResetGameState(GameObject callingObject)
        {
            actionTrigger.ResetGameState(callingObject);
        }

        public void Activate(GameObject callingObject)
        {
            if (callingObject == null) {
                throw new UnassignedReferenceException("You must specify a calling game object.");
            }
            
            if (logCallersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                Debug.Log($"Action Trigger on {this.gameObject.name} activated!", this.gameObject);
                Debug.Log($"Status changed by {callingObject.name}", callingObject);
                Debug.Log("--------------------------");
            }
            
            actionTrigger.active = true;
        }
        
        public void Deactivate(GameObject callingObject)
        {
            if (callingObject == null) {
                throw new UnassignedReferenceException("You must specify a calling game object.");
            }
            
            if (logCallersOnRaise == true || AppSettings.logEventCallersAndListeners == true) {
                Debug.Log($"Action Trigger on {this.gameObject.name} deactivated!", this.gameObject);
                Debug.Log($"Status changed by {callingObject.name}", callingObject);
                Debug.Log("--------------------------");
            }
            
            actionTrigger.active = false;
        }

#if UNITY_EDITOR
        
        [PropertySpace(20)]
        
        [Button]
        [ShowInInspector]
        private void CallPerformActions()
        {
            // DO NOT USE THIS FOR GAME LOGIC, THIS IS FOR
            // MANUAL TRIGGER IN EDITOR ONLY
            actionTrigger.PerformActions(this.gameObject);
        }
        
        [Button]
        [ShowInInspector]
        private void CallResetGameState()
        {
            // DO NOT USE THIS FOR GAME LOGIC, THIS IS FOR
            // MANUAL TRIGGER IN EDITOR ONLY
            actionTrigger.ResetGameState(this.gameObject);
        }
        
        private void Update()
        {
            if (syncEditorActionHeadings == true) {
                SyncTriggerDescriptions();
            }
        }

        public void SyncTriggerDescriptions()
        {
            actionTrigger.CallSyncEditorActionHeadings();
            actionTrigger.CallSyncComplexSubheadings(this.gameObject,
                new SerializedObject(this).FindProperty(nameof(_actionTrigger)));
            actionTrigger.SyncFullActionDescription();
        }
#endif
    }
}