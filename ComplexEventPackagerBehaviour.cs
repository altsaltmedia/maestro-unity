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
    public class ComplexEventPackagerBehaviour : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("complexEventTriggerPackagers")]
        private List<ComplexEventPackager> _complexEventPackagers = new List<ComplexEventPackager>();

        private List<ComplexEventPackager> complexEventPackagers => _complexEventPackagers;

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void ActivateTriggers()
        {
            for (int i=0; i<complexEventPackagers.Count; i++) {
                complexEventPackagers[i].RaiseEvent(this.gameObject);
            }
        }

    }
}