using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{
    public class SimpleEventTriggerBehaviour : MonoBehaviour
    {
        [SerializeField]
        List<SimpleEventTrigger> simpleEventTriggers = new List<SimpleEventTrigger>();

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void ActivateTriggers()
        {
            for (int i=0; i<simpleEventTriggers.Count; i++) {
                simpleEventTriggers[i].RaiseEvent(this.gameObject);
            }
        }

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void ActivateTrigger(SimpleEvent targetEvent)
        {
            for (int i = 0; i < simpleEventTriggers.Count; i++) {
                if(simpleEventTriggers[i].SimpleEventTarget == targetEvent) {
                    simpleEventTriggers[i].RaiseEvent(this.gameObject);
                    break;
                }
            }
        }
    }
}