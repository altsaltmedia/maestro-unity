using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

namespace AltSalt
{

    public class UnityEventTriggerBehaviour : MonoBehaviour
    {
        [ValueDropdown("boolValueList")]
        [SerializeField]
        bool triggerOnStart = true;

        private ValueDropdownList<bool> boolValueList = new ValueDropdownList<bool>(){
            {"YES", true },
            {"NO", false }
        };

        [SerializeField]
        List<ConditionEventPair> conditionEventPairs = new List<ConditionEventPair>();

        void Start()
        {
            if(triggerOnStart == true) {
                ExecuteEvents();
            }
        }

        [HorizontalGroup("Split", 1f)]
        [InfoBox("Trigger the list of events")]
        [Button(ButtonSizes.Large), GUIColor(0.4f, 0.8f, 1)]
        public void ExecuteEvents()
        {
            for(int i=0; i<conditionEventPairs.Count; i++) {
                if(conditionEventPairs[i].reference.Value == conditionEventPairs[i].condition) {
                    conditionEventPairs[i].response.Invoke();
                }
            }
        }

        [Serializable]
        class ConditionEventPair
        {
            [SerializeField]
            public BoolReference reference;

            public bool condition; 

            [ValidateInput("IsPopulated")]
            [SerializeField]
            public UnityEvent response;

            private static bool IsPopulated(UnityEvent attribute)
            {
                return Utils.IsPopulated(attribute);
            }
        }
    }

}