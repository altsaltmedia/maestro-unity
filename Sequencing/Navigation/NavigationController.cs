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

        private ComplexEventManualTrigger refreshScrubber => 
            appSettings.GetRefreshScrubber(this, inputGroupKey);

        public override void ConfigureData()
        {
            if (Application.isPlaying == true) return;
        }

        public void ActivateNavigationModules()
        {
            activeMasterSequence = 
                masterSequences.Find(x => x.sequenceConfigs.Find(
                    y => y.sequence.active && y.sequence.canBeScrubbed == true));

            if (activeMasterSequence != null) {
                refreshScrubber.RaiseEvent(this.gameObject, this);
            }
        }

    }
}