using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    public abstract class Autorun_Module : Input_Module
    {
        [Required]
        [SerializeField]
        private Autorun_Controller _autorunController;

        public Autorun_Controller autorunController => _autorunController;

        protected float autorunThreshold => autorunController.appSettings.GetAutorunThreshold(this.gameObject, autorunController.inputGroupKey);

        protected override Input_Controller inputController => autorunController;

        public abstract void OnSequenceUpdated(Sequence updatedSequence);

        public static Autorun_Data TriggerAutorunIntervalComplete(Autorun_Module autorunModule, Autorun_Data autorunData)
        {
            Sequence targetSequence = autorunData.sequence;
            MasterSequence targetMasterSequence = targetSequence.sequenceController.masterSequence;
            
            autorunData.activeInterval = null;
            autorunData.forwardUpdateActive = false;
            autorunData.backwardUpdateActive = false;
            autorunData.eligibleForAutoplay = false;
            autorunData.activeAutorunModule = null;
            targetMasterSequence.RequestDeactivateForwardAutoplay(targetSequence,
                autorunModule.priority, autorunModule.gameObject.name);
            autorunData.easingUtility.Reset();

            return autorunData;
        }

        public abstract Autorun_Data AttemptRegisterAutorunModule(Autorun_Module autorunModule,
            Autorun_Data autorunData, out bool registrationSuccessful);
    }
}