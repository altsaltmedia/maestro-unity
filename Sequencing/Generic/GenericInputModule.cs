using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Generic
{
    public class GenericInputModule : Input_Module
    {
        [Required]
        [SerializeField]
        private GenericInputController _genericInputController;

        public GenericInputController genericInputController => _genericInputController;

        protected override Input_Controller inputController => _genericInputController;
        
        public void TriggerModifySequence(FloatVariable modifier)
        {
            TriggerModifySequence(modifier.value);
        }
        
        public void TriggerModifySequence(float modifier)
        {
            for (int i = 0; i < genericInputController.inputData.Count; i++) {
                Sequence targetSequence = genericInputController.inputData[i].sequence;
                if (targetSequence.active == true) {
                    MasterSequence targetMasterSequence = targetSequence.sequenceController.masterSequence;
                    targetMasterSequence.RequestModifySequenceTime(targetSequence, this.priority, this.name, modifier);
                }
            }
        }
    }
}