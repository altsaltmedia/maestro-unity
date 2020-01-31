using System;
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
            actionTrigger.CallPopulateReferences();
        }

        private void Start()
        {
            if (actionTrigger.triggerOnStart == true) {
                CallPerformActions(this.gameObject);
            }
        }

        public void CallPerformActions(GameObject callingObject)
        {
            if (callingObject == null) {
                throw new UnassignedReferenceException("You must specify a calling game object.");
            }
            
            actionTrigger.PerformActions(this.gameObject);
        }
        

#if UNITY_EDITOR
        [Button]
        [ShowInInspector]
        public void CallPerformActions()
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