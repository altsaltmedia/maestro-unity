using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class ComplexEventActionData : ActionData
    {
        protected override string title => nameof(ComplexEventActionData);
        
        [SerializeField]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(Expanded = true)]
        private List<ComplexEventConfigurableTrigger> _complexEventPayloadPackagers = new List<ComplexEventConfigurableTrigger> { new ComplexEventConfigurableTrigger() };

        private List<ComplexEventConfigurableTrigger> complexEventPayloadPackagers => _complexEventPayloadPackagers;

        public ComplexEventActionData(int priority) : base(priority) { }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public override void PerformAction(GameObject callingObject)
        {
            for (int i=0; i<complexEventPayloadPackagers.Count; i++) {
                complexEventPayloadPackagers[i].RaiseEvent(callingObject);
            }
        }

        public override void SyncEditorActionHeadings()
        {
            string complexEventNames = "";
            
            for (int i = 0; i < complexEventPayloadPackagers.Count; i++) {
                if (string.IsNullOrEmpty(complexEventPayloadPackagers[i].referenceName) == false) {
                    complexEventNames += complexEventPayloadPackagers[i].referenceName;
                    if (i < complexEventPayloadPackagers.Count - 1) {
                        complexEventNames += ", ";
                    }
                }
            }

            if (string.IsNullOrEmpty(complexEventNames) == false) {
                actionDescription = "Trigger " + complexEventNames;
            }
            else {
                actionDescription = "Please populate a complex event packager";
            }
        }
    }
}