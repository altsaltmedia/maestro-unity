using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class EventAction : ActionData
    {
        protected override string title => nameof(EventAction);

        [SerializeField]
        [HideReferenceObjectPicker]
        private UnityEvent _eventAction;

        private UnityEvent eventAction => _eventAction;
        
        public EventAction(int priority) : base(priority) {}

        public override void SyncEditorActionHeadings()
        {
            string targets = "";
            
            for (int i = 0; i < eventAction.GetPersistentEventCount(); i++) {
                if (eventAction.GetPersistentTarget(i) != null) {
                    targets += eventAction.GetPersistentTarget(i).name;
                    if (i < eventAction.GetPersistentEventCount() - 1) {
                        targets += ", ";
                    }
                }
            }

            actionDescription = targets;
        }

        public void SyncUnityEventHeading(SerializedProperty unityEventSerializedParent)
        {
            
        }

        public override void PerformAction(GameObject callingObject)
        {
            eventAction.Invoke();
        }
    }
}