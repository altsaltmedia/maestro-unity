using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class GenericActionData : ActionData, ISyncUnityEventHeadings
    {
        protected override string title => nameof(GenericActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private GameObjectGenericAction _action = new GameObjectGenericAction();

        private GameObjectGenericAction action
        {
            get => _action;
            set => _action = value;
        }

        public GenericActionData(int priority) : base(priority) {}

        private List<UnityEventData> _cachedEventData = new List<UnityEventData>();

        private List<UnityEventData> cachedEventData
        {
            get
            {
                if (_cachedEventData == null) {
                    _cachedEventData = new List<UnityEventData>();
                }

                return _cachedEventData;
            }
            set => _cachedEventData = value;
        }

        public override void PerformAction(GameObject callingObject)
        {
            action.Invoke(callingObject);
        }

#if UNITY_EDITOR        
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

        public void SyncUnityEventHeadings(SerializedProperty unityEventSerializedParent)
        {
            string newDescription = "";
            
            string[] parameterNames = UnityEventUtils.GetUnityEventParameters(unityEventSerializedParent, nameof(_action));
            if (UnityEventUtils.UnityEventValuesChanged(action, parameterNames, cachedEventData, out var eventData)) {
                newDescription = UnityEventUtils.ParseUnityEventDescription(eventData);
                cachedEventData = eventData;
            }

            if (string.IsNullOrEmpty(newDescription) == true) {
                actionDescription = "No generic events populated";
                return;
            }

            actionDescription = newDescription;
        }
#endif

    }
}