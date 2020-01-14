using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    [Serializable]
    [ExecuteInEditMode]
    public class ComplexEventPackagerBehaviour : MonoBehaviour
    {
        [SerializeField]
        List<ComplexEventPackager> complexEventTriggerPackagers = new List<ComplexEventPackager>();

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