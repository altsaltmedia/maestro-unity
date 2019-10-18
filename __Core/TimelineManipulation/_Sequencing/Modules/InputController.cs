using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing
{
    [ExecuteInEditMode]
    public abstract class InputController : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private AppSettings _appSettings;

        public AppSettings appSettings
        {
            get => _appSettings;
        }

        [ValidateInput("IsPopulated")]
        [SerializeField]
        private List<MasterSequence> _masterSequences = new List<MasterSequence>();

        public List<MasterSequence> masterSequences
        {
            get => _masterSequences;
        }

        [Required]
        [SerializeField]
        private ComplexEventTrigger _applyInputToSequence;

        public ComplexEventTrigger applyInputToSequence
        {
            get => _applyInputToSequence;
        }
        
        protected static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        protected static bool IsPopulated(List<MasterSequence> attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }   
}