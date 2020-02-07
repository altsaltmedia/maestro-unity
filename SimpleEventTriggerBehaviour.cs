using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    public class SimpleEventTriggerBehaviour : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("simpleEventTriggers")]
        private List<SimpleEventTrigger> _simpleEventTriggers = new List<SimpleEventTrigger>();

        private List<SimpleEventTrigger> simpleEventTriggers
        {
            get => _simpleEventTriggers;
            set => _simpleEventTriggers = value;
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void ActivateTriggers()
        {
            for (int i=0; i<simpleEventTriggers.Count; i++) {
                simpleEventTriggers[i].RaiseEvent(this.gameObject);
            }
        }
    }
}