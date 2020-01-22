using System;
using System.CodeDom;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class SimpleEventActionData : ActionData
    {
        protected override string title => nameof(SimpleEventActionData);

        [SerializeField]
        [HideReferenceObjectPicker]
        private SimpleEventTrigger _simpleEventTrigger = new SimpleEventTrigger();

        private SimpleEventTrigger simpleEventTrigger => _simpleEventTrigger;

        public SimpleEventActionData(int priority) : base(priority) { }

        public override void PerformAction(GameObject callingObject)
        {
            simpleEventTrigger.RaiseEvent(callingObject);
        }

        public override void SyncEditorActionHeadings()
        {
            if (string.IsNullOrEmpty(simpleEventTrigger.referenceName) == false) {
                actionDescription = $"Trigger {simpleEventTrigger.referenceName}";
            }
            else {
                actionDescription = "Empty simple event trigger";
            }
        }
    }
}