using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing
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
        private ComplexEventManualTrigger _requestModifyToSequence;

        public ComplexEventManualTrigger requestModifyToSequence
        {
            get => _requestModifyToSequence;
        }

        protected static bool IsPopulated(ComplexEventManualTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }   
}