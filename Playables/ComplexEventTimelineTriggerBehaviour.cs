using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro
{
    [Serializable]
    public class ComplexEventTimelineTriggerBehaviour : TimelineTriggerBehaviour
    {
        [FormerlySerializedAs("_complexEventTriggerPackagers")]
        [FormerlySerializedAs("complexEventTriggerPackagers")]
        [SerializeField]
        private List<ComplexEventConfigurableTrigger> _complexEventConfigurableTriggers;

        public List<ComplexEventConfigurableTrigger> complexEventConfigurableTriggers
        {
            get => _complexEventConfigurableTriggers;
            set => _complexEventConfigurableTriggers = value;
        }

        [SerializeField]
        private bool _disableOnReverse = true;

        public override bool disableOnReverse => _disableOnReverse;
        
        [SerializeField]
        private bool _executeWhileLoadingBookmarks = false;

        public override bool executeWhileLoadingBookmarks => _executeWhileLoadingBookmarks;

        [SerializeField]
        private bool _forceActivateOnForward = false;

        public override bool forceActivateOnForward => _forceActivateOnForward;
        
        [SerializeField]
        private bool _forceActivateOnReverse = false;
        
        public override bool forceActivateOnReverse => _forceActivateOnReverse;
    }
}