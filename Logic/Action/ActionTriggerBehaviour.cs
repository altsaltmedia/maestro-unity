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
        [SerializeField]
        private ActionTrigger _actionTrigger;

        private ActionTrigger actionTrigger => _actionTrigger;

        public void CallPerformActions()
        {
            actionTrigger.PerformActions(this.gameObject);
        }

#if UNITY_EDITOR
        private void Update()
        {
            actionTrigger.CallSyncEditorActionHeadings();
            actionTrigger.CallSyncComplexSubheadings(this.gameObject,
                new SerializedObject(this).FindProperty(nameof(_actionTrigger)));
        }
#endif
    }
}