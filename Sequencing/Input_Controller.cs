using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing
{
    [ExecuteInEditMode]
    public abstract class Input_Controller : RootDataCollector
    {
        public Joiner joiner
        {
            get => rootConfig.joiner;
        }
        
        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private ComplexEventTrigger _requestModifyToSequence;

        public ComplexEventTrigger requestModifyToSequence
        {
            get => _requestModifyToSequence;
        }

        protected static bool IsPopulated(ComplexEventTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }   
}