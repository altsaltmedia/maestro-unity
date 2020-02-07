using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace AltSalt.Maestro.Logic.Action
{
    [Serializable]
    public class ComplexEventActionData : ActionData
    {
        protected override string title => nameof(ComplexEventActionData);
        
        [FormerlySerializedAs("_complexEventPayloadPackagers")]
        [SerializeField]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(Expanded = true, AlwaysAddDefaultValue = true)]
        private List<ComplexEventConfigurableTrigger> _complexEventConfigurableTriggers = new List<ComplexEventConfigurableTrigger> { new ComplexEventConfigurableTrigger() };

        private List<ComplexEventConfigurableTrigger> complexEventConfigurableTriggers => _complexEventConfigurableTriggers;

        public ComplexEventActionData(int priority) : base(priority) { }
        

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public override void PerformAction(GameObject callingObject)
        {
            for (int i=0; i<complexEventConfigurableTriggers.Count; i++) {
                complexEventConfigurableTriggers[i].RaiseEvent(callingObject);
            }
        }

#if UNITY_EDITOR        
        public override ActionData PopulateReferences(Object parentObject, string serializedPropertyPath)
        {
            string packagersPath = serializedPropertyPath;
            packagersPath += $".{nameof(_complexEventConfigurableTriggers)}";
            
            for (int i = 0; i < complexEventConfigurableTriggers.Count; i++) {
                string referencePath = packagersPath;
                referencePath += $".{i.ToString()}";
                complexEventConfigurableTriggers[i].PopulateVariable(parentObject, referencePath);
                complexEventConfigurableTriggers[i].PopulateReferences(parentObject, referencePath);
            }

            return this;
        }

        public override void SyncEditorActionHeadings()
        {
            string complexEventNames = "";
            
            for (int i = 0; i < complexEventConfigurableTriggers.Count; i++) {
                if (string.IsNullOrEmpty(complexEventConfigurableTriggers[i].referenceName) == false) {
                    complexEventNames += complexEventConfigurableTriggers[i].referenceName;
                    if (i < complexEventConfigurableTriggers.Count - 1) {
                        complexEventNames += ", ";
                    }
                }
            }

            if (string.IsNullOrEmpty(complexEventNames) == false) {
                actionDescription = "Trigger " + complexEventNames;
            }
            else {
                actionDescription = "Please populate your complex event triggers";
            }
        }
#endif
        
    }
}