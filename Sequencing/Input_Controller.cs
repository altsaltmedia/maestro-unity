using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing
{
    [ExecuteInEditMode]
    public abstract class Input_Controller : Input_RootDataCollector
    {
        public JoinTools joinTools
        {
            get => rootConfig.joinTools;
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