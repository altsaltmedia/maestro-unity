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

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private List<MasterSequence> _masterSequences = new List<MasterSequence>();

        public List<MasterSequence> masterSequences
        {
            get => _masterSequences;
        }

        [ValidateInput(nameof(IsPopulated))]
        [SerializeField]
        private ComplexEventTrigger _requestModifyToSequence;

        public ComplexEventTrigger requestModifyToSequence
        {
            get => _requestModifyToSequence;
        }

        protected static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

        protected static bool IsPopulated(List<MasterSequence> attribute)
        {
            return Utils.IsPopulated(attribute);
        }
        
        protected static bool IsPopulated(ComplexEventTrigger attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }   
}