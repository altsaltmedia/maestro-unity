using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class ComplexEventTriggerBehaviour : MonoBehaviour
    {
        [SerializeField]
        List<ComplexEventTrigger> complexEventTriggers = new List<ComplexEventTrigger>();

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void ActivateTriggers()
        {
            for (int i=0; i<complexEventTriggers.Count; i++) {
                complexEventTriggers[i].RaiseEvent(this.gameObject);
            }
        }

    }
}