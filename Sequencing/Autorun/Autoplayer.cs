using System.Collections.Generic;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    public class Autoplayer : Autorun_Module
    {
        protected override bool moduleActive
        {
            get
            {
                if (_moduleActive == false ||
                    autorunController.appSettings.GetUserAutoplayEnabled(this.gameObject, userKey) == false) {
                    return false;
                }

                return true;
            }
            set => _moduleActive = value;
        }

        private float frameStepValue => autorunController.appSettings.GetFrameStepValue(this.gameObject, inputGroupKey);

        [SerializeField]
        [Range(0f, 1f)]
        private float _autoplayEaseThreshold = 0.25f;

        private float autoplayEaseThreshold => _autoplayEaseThreshold;
        
        /// <summary>
        /// Given a target sequence, will search through our
        /// autorun data and either activate or deactivate autoplay.
        /// This should be called every time a sequence-timeline pair
        /// is updated. 
        /// </summary>
        /// <param name="targetSequence"></param>
        public void RefreshAutoplayStatus(Sequence targetSequence)
        {
            if (moduleActive == false || appUtilsRequested == true) {
                return;
            }
            
            var autorunData = autorunController.autorunData.Find(x => x.sequence == targetSequence);

            if (autorunData == null) return;

            MasterSequence targetMasterSequence = targetSequence.sequenceController.masterSequence;

            // Autoplay may be overriden by game conditions; if so, deactivate
            if (targetSequence.active == false || autorunData.autoplayActive == false || autorunData.isLerping == true) {
                autorunData.backwardUpdateActive = false;
                autorunData.forwardUpdateActive = false;
                targetMasterSequence.RequestDeactivateForwardAutoplay(targetSequence, this.priority, this.gameObject.name);
                return;
            }
            
            // If autoplay is currently active, deactivate and return if we're
            // beyond the thresholds of the extents where the autoplay originated.
            // (This is how we pause autoplay between intervals).
            // Note that, if looping is activated, we ignore intervals.
            if(autorunData.loop == false && autorunData.activeInterval != null) {
                if (Extents.TimeBeyondThresholdInclusive(targetSequence.currentTime, autorunData.activeInterval)) {
                    autorunData.activeInterval = null;
                    autorunData.forwardUpdateActive = false;
                    autorunData.backwardUpdateActive = false;
                    targetMasterSequence.RequestDeactivateForwardAutoplay(targetSequence, this.priority, this.gameObject.name);
                    return;
                }
            }
            
            // Note that the conditions for forward vs backward autoplay are different.
            if (autorunController.isReversing == false) {
                
                // For forward autoplay, once it's been activated, we
                // don't want to activate it again until the autoplay is either
                // interrupted or completed.
                if (autorunData.forwardUpdateActive == false) {
                    AttemptForwardAutoplay(autorunData);
                }
                autorunData.backwardUpdateActive = false;
            }
            else {
                
                // For backwards autoplay, we set the appropriate flags,
                // then the update is handled each frame via the Update() function.
                autorunData.forwardUpdateActive = false;
                autorunData.backwardUpdateActive = true;
            }
        }

        /// <summary>
        /// We need to make a single explicit call to the MasterSequence
        /// in order to set the speed and trigger forward autoplay.
        /// (This is in contrast to backwards autoplay, wherein we send
        /// a modify request every frame). 
        /// </summary>
        /// <param name="autorunData"></param>
        /// <returns></returns>
        private Autorun_Data AttemptForwardAutoplay(Autorun_Data autorunData)
        {
            MasterSequence targetMasterSequence = autorunData.sequence.sequenceController.masterSequence;
            Sequence targetSequence = autorunData.sequence; 
            
            if (AutorunExtents.TimeWithinThresholdExclusive(targetSequence.currentTime,
                    autorunData.autorunIntervals, out var currentInterval) == true) {
                
                targetMasterSequence.RequestActivateForwardAutoplay(targetSequence,
                    this.priority, this.gameObject.name, 1, out bool requestSuccessful);
              
                // We should only store the interval and activate autoplay
                // once our request has been accepted by the MasterSequence
                if(requestSuccessful == true) {
                    
                    // Once the active interval has been cached, we will use
                    // it to determine whether autoplay should halt whenever the
                    // sequence gets updated (see RefreshAutoplay() above)
                    autorunData.activeInterval = currentInterval;
                    autorunData.forwardUpdateActive = true;
                }
            }

            return autorunData;
        }
        
        /// <summary>
        /// This is handling for our reverse autoplay.
        /// </summary>
        protected virtual void Update()
        {
            if (moduleActive == false || appUtilsRequested == true || autorunController.isReversing == false) {
                return;
            }

            for (int q = 0; q < autorunController.autorunData.Count; q++) {
                
                var autorunData = autorunController.autorunData[q];
                
                if (autorunData.backwardUpdateActive == true) {
                    AttemptReverseAutoplay(autorunData);
                }
            }
        }

        /// <summary>
        /// To play a sequence in reverse, we must do so manually every frame. Also,
        /// to be most consistent and smooth, it's best to do this via the Update() loop. 
        /// Given a set of autorun data then, this checks to see if the corresponding
        /// sequence's current time is within a valid set of extents, and if so,
        /// calculates the autoplay modifier and sends out a modify request.
        /// </summary>
        /// <param name="autorunData"></param>
        /// <returns></returns>
        private Autorun_Data AttemptReverseAutoplay(Autorun_Data autorunData)
        {
            if (autorunData.autoplayActive == false || autorunData.isLerping == true || autorunData.sequence.active == false) {
                return autorunData;
            }

            if (AutorunExtents.TimeWithinThresholdExclusive(autorunData.sequence.currentTime,
                    autorunData.autorunIntervals, out var currentInterval) == false) {
                return autorunData;
            }
            
            MasterSequence targetMasterSequence = autorunData.sequence.sequenceController.masterSequence;
            autorunData.activeInterval = currentInterval;

            float autoplayModifer = 0f;

#if UNITY_EDITOR
            autoplayModifer = Time.smoothDeltaTime;
#else
            autoplayModifer = frameStepValue;
#endif
            autoplayModifer *= CalculateAutoplayModifier(autorunData.sequence, currentInterval, autorunData.loop, true, autoplayEaseThreshold, autorunData.easingUtility);
            
            targetMasterSequence.RequestModifySequenceTime(autorunData.sequence, this.priority, this.gameObject.name, autoplayModifer);

            return autorunData;
        }

        /// <summary>
        /// Calculates a modifier based on our current direction.
        /// Importantly, when updating a sequence timeline manually,
        /// the final few frames are abrupt unless we perform an ease,
        /// so this calculates an easy based a passed-in easing utility.
        /// Note that we only currently use this for reverse autoplay.
        /// </summary>
        /// <param name="targetSequence"></param>
        /// <param name="currentInterval"></param>
        /// <param name="loop"></param>
        /// <param name="isReversing"></param>
        /// <param name="easeThreshold"></param>
        /// <param name="easingUtility"></param>
        /// <returns></returns>
        private static float CalculateAutoplayModifier(Sequence targetSequence, Extents currentInterval, bool loop,
            bool isReversing, float easeThreshold, EasingUtility easingUtility)
        {
            float timeModifier = 0f;

            if (loop == false && SequenceWithinEaseThreshold(targetSequence, currentInterval, isReversing, easeThreshold)) {
                timeModifier = easingUtility.GetMultiplier();
            } else {
                timeModifier = 1f;
            }
            
            if (isReversing == true) {
                timeModifier *= -1f;
            }

            return timeModifier;
        }
        
        /// <summary>
        /// Checks to see if we should perform an ease or not.
        /// </summary>
        /// <param name="targetSequence"></param>
        /// <param name="currentInterval"></param>
        /// <param name="isReversing"></param>
        /// <param name="easeThreshold"></param>
        /// <returns></returns>
        private static bool SequenceWithinEaseThreshold(Sequence targetSequence, Extents currentInterval,
            bool isReversing, float easeThreshold)
        {
            if (isReversing == true)
            {
                if (targetSequence.currentTime - currentInterval.startTime <= easeThreshold)
                {
                    return true;
                }
            }
            else
            {
                if (currentInterval.endTime - targetSequence.currentTime <= easeThreshold)
                {
                    return true;
                }
            }

            return false;
        }

        public void ActivateAutoplay()
        {
            for (int q = 0; q < autorunController.autorunData.Count; q++) {
                
                if (autorunController.autorunData[q].sequence.active == true) {
                    autorunController.autorunData[q].autoplayActive = true;
                }
            }
        }

        public void DeactivateAutoplay()
        {
            for (int q = 0; q < autorunController.autorunData.Count; q++) {
                
                if (autorunController.autorunData[q].sequence.active == true) {
                    autorunController.autorunData[q].autoplayActive = false;
                    autorunController.autorunData[q].easingUtility.Reset();
                }
            }
        }

        public void ActivateLoop(Sequence targetSequence)
        {
            Autorun_Data autorunData = autorunController.autorunData.Find(autorunMatchData => autorunMatchData.sequence == targetSequence);
            if (autorunData != null) {
                autorunData.loop = true;
            }
            else {
                Debug.Log("Autoplayer does not contain data for target sequence.");
            }
        }
        
        public void DeactivateLoop(Sequence targetSequence)
        {
            Autorun_Data autorunData = autorunController.autorunData.Find(autorunMatchData => autorunMatchData.sequence == targetSequence);
            if (autorunData != null) {
                autorunData.loop = false;
            }
            else {
                Debug.Log("Autoplayer does not contain data for target sequence.");
            }
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}