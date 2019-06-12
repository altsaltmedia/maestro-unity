using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class ComplexEventTriggerPackagerBehaviour : MonoBehaviour
    {
        [SerializeField]
        List<ComplexEventTriggerPackager> complexEventTriggerPackagers = new List<ComplexEventTriggerPackager>();

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void ActivateTriggers()
        {
            for (int i=0; i<complexEventTriggerPackagers.Count; i++) {
                complexEventTriggerPackagers[i].RaiseEvent(this.gameObject);
            }
        }

    }
}