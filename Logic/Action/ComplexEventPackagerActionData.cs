using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class ComplexEventPackagerActionData : ActionData
    {
        protected override string title => nameof(ComplexEventPackagerActionData);
        
        [SerializeField]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(Expanded = true)]
        private List<ComplexEventPackager> _complexEventPackagers = new List<ComplexEventPackager> { new ComplexEventPackager() };

        private List<ComplexEventPackager> complexEventPackagers => _complexEventPackagers;

        public ComplexEventPackagerActionData(int priority) : base(priority) { }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public override void PerformAction(GameObject callingObject)
        {
            for (int i=0; i<complexEventPackagers.Count; i++) {
                complexEventPackagers[i].RaiseEvent(callingObject);
            }
        }

        public override void SyncEditorActionHeadings()
        {
            string complexEventNames = "";
            
            for (int i = 0; i < complexEventPackagers.Count; i++) {
                if (complexEventPackagers[i].complexEvent != null) {
                    complexEventNames += complexEventPackagers[i].complexEvent.name;
                    if (i < complexEventPackagers.Count - 1) {
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