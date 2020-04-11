using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

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

        private bool bookmarkLoadingCompleted =>
            autorunController.appSettings.GetBookmarkLoadingCompleted(this.gameObject, inputGroupKey);

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
        public void OnSequenceUpdated(Sequence targetSequence)
        {
            if (moduleActive == false || appUtilsRequested == true || bookmarkLoadingCompleted == false) {
                return;
            }
            
            var autorunData = autorunController.autorunData.Find(x => x.sequence == targetSequence);

            if (autorunData == null) return;

            // If autoplay is currently active, deactivate and return if we're
            // beyond the thresholds of the extents where the autoplay originated.
            // (This is how we pause autoplay between intervals).
            // Note that, if looping is activated, we ignore intervals.
            if(HasValidAutoplayInterval(autorunData)) {
                
                if(autorunController.isReversing == false &&
                   Extents.TimeBeyondEndThresholdExclusive(targetSequence.currentTime + autorunThreshold, autorunData.activeInterval)) {
                    
                    AutorunExtents currentInterval = autorunData.activeInterval;
                    TriggerAutoplayIntervalComplete(this, autorunData);
                    
                    if (currentInterval.endTime >= targetSequence.duration) {
                        autorunData.sequence.sequenceController.SetEndBoundaryReached(this);
                    }
                    else {
                        autorunData.sequence.sequenceController.SetSequenceTimeWithoutCallbacks(this, (float)currentInterval.endTime);
                    }
                    
                    return;
                }
                
                if (autorunController.isReversing == true &&
                         Extents.TimeBeyondStartThresholdExclusive(targetSequence.currentTime - autorunThreshold, autorunData.activeInterval)) {
                    
                    AutorunExtents currentInterval = autorunData.activeInterval;
                    TriggerAutoplayIntervalComplete(this, autorunData);
                    
                    if (currentInterval.startTime == 0) {
                        autorunData.sequence.sequenceController.SetStartBoundaryReached(this);
                    }
                    else {
                        autorunData.sequence.sequenceController.SetSequenceTimeWithoutCallbacks(this, (float)currentInterval.startTime);
                    }
                    
                    return;
                }
                
            }
            
            // Note that the conditions for forward vs backward autoplay are different.
            if (autorunController.isReversing == false) {
                
                // For forward autoplay, once it's been activated, we
                // don't want to activate it again until the autoplay is either
                // interrupted or completed.
                if (autorunData.forwardUpdateActive == false) {
                    AttemptForwardAutoplay(this, autorunData);
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

        private static Autorun_Data TriggerAutoplayIntervalComplete(Autoplayer autoplayer, Autorun_Data autorunData)
        {
            Sequence targetSequence = autorunData.sequence;
            MasterSequence targetMasterSequence = targetSequence.sequenceController.masterSequence;
            
            autorunData.activeInterval = null;
            autorunData.forwardUpdateActive = false;
            autorunData.backwardUpdateActive = false;
            autorunData.eligibleForAutoplay = false;
            targetMasterSequence.RequestDeactivateForwardAutoplay(targetSequence,
                autoplayer.priority, autoplayer.gameObject.name);

            return autorunData;
        }
        
        /// <summary>
        /// We need to make a single explicit call to the MasterSequence
        /// in order to set the speed and trigger forward autoplay.
        /// (This is in contrast to backwards autoplay, wherein we send
        /// a modify request every frame). 
        /// </summary>
        /// <param name="autorunData"></param>
        /// <returns></returns>
        private static Autorun_Data AttemptForwardAutoplay(Autoplayer autoplayer, Autorun_Data autorunData)
        {
            MasterSequence targetMasterSequence = autorunData.sequence.sequenceController.masterSequence;
            Sequence targetSequence = autorunData.sequence; 
            
            if (AutorunExtents.TimeWithinThresholdBothBoundsInclusive(targetSequence.currentTime,
                    autorunData.autorunIntervals, out var currentInterval) == true) {
                
                targetMasterSequence.RequestActivateForwardAutoplay(targetSequence,
                    autoplayer.priority, autoplayer.gameObject.name, 1, out bool requestSuccessful);
              
                // We should only store the interval and activate autoplay
                // once our request has been accepted by the MasterSequence
                if(requestSuccessful == true) {
                    
                    // Once the active interval has been cached, we will use
                    // it to determine whether autoplay should halt whenever the
                    // sequence gets updated (see RefreshAutoplay() above)
                    autorunData.activeInterval = currentInterval;
                    autorunData.forwardUpdateActive = true;

                    CheckPauseMomentum(autoplayer.autorunController, autorunData);
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
                    AttemptReverseAutoplay(autorunData, this);
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
        private static Autorun_Data AttemptReverseAutoplay(Autorun_Data autorunData, Autoplayer autoplayer)
        {
            if (SequenceOrAutoplayDeactivated(autorunData)) {
                return autorunData;
            }

            if (AutorunExtents.TimeWithinThresholdBothBoundsInclusive(autorunData.sequence.currentTime,
                    autorunData.autorunIntervals, out var currentInterval) == false) {
                autoplayer.OnSequenceUpdated(autorunData.sequence);
                return autorunData;
            }

            Autorun_Controller autorunController = autoplayer.autorunController;
            CheckPauseMomentum(autorunController, autorunData);

            autorunData.activeInterval = currentInterval;

            float autoplayModifer = GetAutoplayModifier(autoplayer);
            if (SequenceWithinEaseThreshold(autorunData.sequence, currentInterval, true, autoplayer.autoplayEaseThreshold)) {
                autoplayModifer *= CalculateModifierEase(autorunData.loop, autorunData.easingUtility);
            }
            
            MasterSequence targetMasterSequence = autorunData.sequence.sequenceController.masterSequence;
            targetMasterSequence.RequestModifySequenceTime(autorunData.sequence, autoplayer.priority, autoplayer.gameObject.name, autoplayModifer);

            return autorunData;
        }

        private static bool SequenceOrAutoplayDeactivated(Autorun_Data autorunData)
        {
            if (autorunData.sequence.active == false || autorunData.eligibleForAutoplay == false) {
                return true;
            }

            return false;
        }

        private static bool HasValidAutoplayInterval(Autorun_Data autorunData)
        {
            if (autorunData.loop == false && autorunData.activeInterval != null && autorunData.isLerping == false) {
                return true;
            }

            return false;
        }
        
        private static Autorun_Controller CheckPauseMomentum(Autorun_Controller autorunController, Autorun_Data autorunData)
        {
            if (autorunController.pauseMomentumDuringAutorun == true) {
                autorunController.TriggerPauseMomentum(autorunData.sequence);
            }

            return autorunController;
        }
        
        private static float GetAutoplayModifier(Autoplayer autoplayer)
        {
            float autoplayModifer;
            
            if (autoplayer.autorunController.useFrameStepValue == false) {
                autoplayModifer = Time.smoothDeltaTime;
            }
            else {
                autoplayModifer = autoplayer.frameStepValue;
            }

            if (autoplayer.autorunController.isReversing == true) {
                autoplayModifer *= -1f;
            }

            return autoplayModifer;
        }

        /// <summary>
        /// Calculates a modifier based on our current direction.
        /// Importantly, when updating a sequence timeline manually,
        /// the final few frames are abrupt unless we perform an ease,
        /// so this smooths out that motion based a passed-in easing utility.
        /// Note that we only currently use this for reverse autoplay.
        /// </summary>
        /// <param name="targetSequence"></param>
        /// <param name="currentInterval"></param>
        /// <param name="loop"></param>
        /// <param name="isReversing"></param>
        /// <param name="easeThreshold"></param>
        /// <param name="easingUtility"></param>
        /// <returns></returns>
        private static float CalculateModifierEase(bool loop, EasingUtility easingUtility)
        {
            float timeModifier;

            if (loop == false) {
                timeModifier = easingUtility.GetMultiplier();
            } else {
                timeModifier = 1f;
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
            if (isReversing == false)
            {
                if (currentInterval.endTime - targetSequence.currentTime <= easeThreshold)
                {
                    return true;
                }
            }
            else
            {
                if (targetSequence.currentTime - currentInterval.startTime <= easeThreshold)
                {
                    return true;
                }
            }

            return false;
        }
        
        /// <summary>
        /// By default, we enable autoplay when certain conditions are met -
        /// i.e., a swipe a has completed, autoplay has been
        /// explicitly requested, or a sequence has been modified,
        /// either with our without explicit input (an example of the
        /// latter: a joiner activating a preceding or following sequence)
        /// </summary>
        public void ActivateEligibleForAutoplay()
        {
            for (int q = 0; q < autorunController.autorunData.Count; q++) {
                autorunController.autorunData[q].eligibleForAutoplay = true;
            }
        }
        
        public void ActivateEligibleForAutoplayAndRefresh()
        {
            for (int q = 0; q < autorunController.autorunData.Count; q++) {
                autorunController.autorunData[q].eligibleForAutoplay = true;
                Sequence sequence = autorunController.autorunData[q].sequence;
                if (sequence.active == true) {
                    OnSequenceUpdated(sequence);
                }
            }
        }

        /// <summary>
        /// By default, we disable autoplay when app utils get requested,
        /// or a swipe begins.
        /// </summary>
        public void DeactivateEligibleForAutoplay()
        {
            for (int q = 0; q < autorunController.autorunData.Count; q++) {
                Autorun_Data autorunData = autorunController.autorunData[q];
                autorunData.eligibleForAutoplay = false;
                autorunData.backwardUpdateActive = false;
                autorunData.forwardUpdateActive = false;
                autorunData.easingUtility.Reset();
            }
        }
        
        public void DeactivateEligibleForAutoplay(ComplexPayload complexPayload)
        {
            Sequence targetSequence = complexPayload.GetScriptableObjectValue() as Sequence;
            Autorun_Data autorunData = autorunController.autorunData.Find(x => x.sequence == targetSequence);
            autorunData.eligibleForAutoplay = false;
            autorunData.backwardUpdateActive = false;
            autorunData.forwardUpdateActive = false;
            autorunData.easingUtility.Reset();
        }

        public void ActivateLoop(Sequence targetSequence)
        {
            Autorun_Data autorunData = autorunController.autorunData.Find(autorunMatchData => autorunMatchData.sequence == targetSequence);
            if (autorunData != null) {
                autorunData.loop = true;
                autorunData.sequence.sequenceController.playableDirector.extrapolationMode = DirectorWrapMode.Loop;
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
                autorunData.sequence.sequenceController.playableDirector.extrapolationMode = DirectorWrapMode.Hold;
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