using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    [ExecuteInEditMode]
    public class ComplexEventTriggerBehaviour : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("_complexEventPackagers")]
        [FormerlySerializedAs("complexEventTriggerPackagers")]
        private List<ComplexEventConfigurableTrigger> _complexEventConfigurableTriggers = new List<ComplexEventConfigurableTrigger>();

        private List<ComplexEventConfigurableTrigger> complexEventConfigurableTriggers => _complexEventConfigurableTriggers;

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void ActivateTriggers()
        {
            for (int i=0; i<complexEventConfigurableTriggers.Count; i++) {
                complexEventConfigurableTriggers[i].RaiseEvent(this.gameObject);
            }
        }

    }
}