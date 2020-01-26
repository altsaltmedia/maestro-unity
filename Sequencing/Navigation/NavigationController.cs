using System;
using System.Media;
using DG.Tweening;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Navigation
{
    public class NavigationController : Input_Controller
    {
        [SerializeField]
        private Canvas _navigationCanvas;

        private Canvas navigationCanvas => _navigationCanvas;
        
        [SerializeField]
        private CanvasGroup _navigationCanvasGroup;

        private CanvasGroup navigationCanvasGroup => _navigationCanvasGroup;

        [SerializeField]
        private Scrubber _scrubber;

        private Scrubber scrubber => _scrubber;

        private MasterSequence _activeMasterSequence;

        public MasterSequence activeMasterSequence
        {
            get => _activeMasterSequence;
            private set => _activeMasterSequence = value;
        }

        private void Start()
        {
            if (navigationCanvas.worldCamera == null) {
                Debug.LogError("You must specify a render camera for the navigation controller", this);
            }
        }

        public override void ConfigureData()
        {
            if (Application.isPlaying == true) return;
            
        }

        public void ActivateNavigationModules()
        {
            navigationCanvasGroup.alpha = 1;
            navigationCanvasGroup.interactable = true;
            
            activeMasterSequence = 
                masterSequences.Find(x => x.sequenceConfigs.Find(y => y.sequence.active == true));

            scrubber.RefreshScrubber((float)activeMasterSequence.duration, (float)activeMasterSequence.elapsedTime);
        }

        public void DisableNavigationModules()
        {
            navigationCanvasGroup.alpha = 0;
            navigationCanvasGroup.interactable = true;
        }
        
    }
}