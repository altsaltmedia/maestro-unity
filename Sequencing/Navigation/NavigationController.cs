using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Navigation
{
    public class NavigationController : Input_Controller
    {
        [ShowInInspector]
        [ReadOnly]
        private MasterSequence _activeMasterSequence;

        public MasterSequence activeMasterSequence
        {
            get => _activeMasterSequence;
            set => _activeMasterSequence = value;
        }

        private ComplexEventManualTrigger refreshAppUtils =>
            appSettings.GetRefreshAppUtils(this, inputGroupKey);

        public override void ConfigureData()
        {
            if (Application.isPlaying == true) return;

            // Eventually, likely implement user-defined bookmarks here
        }

        public void RefreshActiveMasterSequence() {
            activeMasterSequence =
                masterSequences.Find(x => x.sequenceControllers.Find(
                    y => y.sequence.active && y.sequence.canBeScrubbed == true));
        }

        public void ActivateNavigationModules()
        {
            RefreshActiveMasterSequence();

            if (activeMasterSequence != null) {
                refreshAppUtils.RaiseEvent(this.gameObject, this);
            }
        }

    }
}