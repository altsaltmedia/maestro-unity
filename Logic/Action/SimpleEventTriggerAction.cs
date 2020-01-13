using System;
using System.CodeDom;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class SimpleEventTriggerAction : ActionData
    {
        protected override string title => nameof(SimpleEventTriggerAction);

        [SerializeField]
        [HideReferenceObjectPicker]
        [TypeFilter(nameof(GetFilteredTypeList))]
        private SimpleEventTrigger _simpleEventTrigger;

        private SimpleEventTrigger simpleEventTrigger => _simpleEventTrigger;

        public SimpleEventTriggerAction(int priority) : base(priority) { }

        public override void PerformAction(GameObject callingObject)
        {
            simpleEventTrigger.RaiseEvent(callingObject);
        }

        public override void SyncEditorActionHeadings()
        {
            if (string.IsNullOrEmpty(simpleEventTrigger.referenceName) == false) {
                actionDescription = $"Trigger {simpleEventTrigger.referenceName} \n";
            }
            else {
                actionDescription = "Empty simple event trigger";
            }
        }
        
        public IEnumerable<Type> GetFilteredTypeList()
        {
            List<Type> typeList = new List<Type>();
            typeList.Add(typeof(ISimpleEventListener));
            return typeList;
        }
    }
}