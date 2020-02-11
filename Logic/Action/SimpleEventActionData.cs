using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic
{
    [Serializable]
    public class SimpleEventActionData : ActionData, IRegisterNestedActionData
    {
        protected override string title => nameof(SimpleEventActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(AlwaysAddDefaultValue = true)]
        private List<SimpleEventTrigger> _simpleEventTriggers = new List<SimpleEventTrigger>();

        private List<SimpleEventTrigger> simpleEventTriggers => _simpleEventTriggers;

        public SimpleEventActionData(int priority) : base(priority) { }
        
        public override void PerformAction(GameObject callingObject)
        {
            for (int i = 0; i < simpleEventTriggers.Count; i++) {
                simpleEventTriggers[i].RaiseEvent(callingObject);
            }
        }

#if UNITY_EDITOR
        public override ActionData PopulateReferences(Object parentObject, string serializedPropertyPath)
        {
            string packagersPath = serializedPropertyPath;
            packagersPath += $".{nameof(_simpleEventTriggers)}";
            
            for (int i = 0; i < simpleEventTriggers.Count; i++) {
                string referencePath = packagersPath;
                referencePath += $".{i.ToString()}";
                simpleEventTriggers[i].PopulateVariable(parentObject, referencePath);
            }

            return this;
        }

        public override void SyncEditorActionHeadings()
        {
            string simpleEventNames = "";
            
            for (int i = 0; i < simpleEventTriggers.Count; i++) {
                if (string.IsNullOrEmpty(simpleEventTriggers[i].referenceName) == false) {
                    simpleEventNames += simpleEventTriggers[i].referenceName;
                    if (i < simpleEventTriggers.Count - 1) {
                        simpleEventNames += ", ";
                    }
                }
            }

            if (string.IsNullOrEmpty(simpleEventNames) == false) {
                actionDescription = "Trigger " + simpleEventNames;
            }
            else {
                actionDescription = "Please populate your simple event triggers";
            }
        }
#endif
        
    }
}