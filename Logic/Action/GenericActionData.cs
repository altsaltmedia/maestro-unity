using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class GenericActionData : ActionData
    {
        protected override string title => nameof(GenericActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private GameObjectGenericAction _action;

        private GameObjectGenericAction action
        {
            get => _action;
            set => _action = value;
        }

        public GenericActionData(int priority) : base(priority) {}

        public override void SyncEditorActionHeadings()
        {
//            string targets = "";
//            
//            for (int i = 0; i < eventAction.GetPersistentEventCount(); i++) {
//                if (eventAction.GetPersistentTarget(i) != null) {
//                    targets += eventAction.GetPersistentTarget(i).name;
//                    if (i < eventAction.GetPersistentEventCount() - 1) {
//                        targets += ", ";
//                    }
//                }
//            }
//
//            actionDescription = targets;
        }

        public void SyncUnityEventHeading(SerializedProperty unityEventSerializedParent)
        {
//            string[] parameterNames = GetParameters(serializedConditionResponse);
//            if (UnityEventValuesChanged(response, parameterNames, cachedEventData, out var eventData)) {
//                eventDescription = GetEventDescription(eventData);
//                cachedEventData = eventData;
//            }
        }

        public override void PerformAction(GameObject callingObject)
        {
            action.Invoke(callingObject);
        }
    }
}