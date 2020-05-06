namespace AltSalt.Maestro.Sequencing.Autorun
{
    /// <summary>
    /// The ManualAutoplayer is slightly different from the regular autoplayer.
    /// Rather than attempt to register itself as the active autorun module whenever
    /// the sequence gets modified, it only registers itself when the mantual
    /// autoplay is activated.
    /// </summary>
    public class ManualAutoplayer : Autoplayer
    { 
        public override void OnSequenceUpdated(Sequence targetSequence)
        {
            if (moduleActive == false || appUtilsRequested == true || bookmarkLoadingCompleted == false) {
                return;
            }
            
            var autorunData = autorunController.autorunData.Find(x => x.sequence == targetSequence);

            if (autorunData == null) return;

            if (autorunData.activeAutorunModule != this) {
                return;
            }

            // If autoplay is currently active, deactivate and return if we're
            // beyond the thresholds of the extents where the autoplay originated.
            // (This is how we pause autoplay between intervals).
            // Note that, if looping is activated, we ignore intervals.
            if(HasValidAutoplayInterval(autorunData)) {
                
                if(autorunController.isReversing == false &&
                   (targetSequence.currentTime + autorunThreshold > targetSequence.duration ||
                   Extents.TimeBeyondEndThresholdExclusive(targetSequence.currentTime + autorunThreshold, autorunData.activeInterval))) {
                    FinishForwardAutoplay(this, autorunData);
                    return;
                }
                
                if (autorunController.isReversing == true &&
                    (targetSequence.currentTime - autorunThreshold <= 0 ||
                    Extents.TimeBeyondStartThresholdExclusive(targetSequence.currentTime - autorunThreshold, autorunData.activeInterval))) {
                    FinishBackwardAutoplay(this, autorunData);
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
                autorunData.activeAutorunModule = this;
                autorunData.forwardUpdateActive = false;
                autorunData.backwardUpdateActive = true;
            }
        }
        
        public override Autorun_Data AttemptRegisterAutorunModule(Autorun_Module autorunModule, Autorun_Data autorunData, out bool registrationSuccessful)
        {
            registrationSuccessful = false;

            if (autorunData.activeAutorunModule == null) {
                autorunData.activeAutorunModule = autorunModule;
                registrationSuccessful = true;
            }
            
            else if (autorunData.activeAutorunModule != null &&
                     autorunModule.priority > autorunData.activeAutorunModule.priority) {
                autorunData.activeAutorunModule = autorunModule;
                registrationSuccessful = true;
            }

            else if (autorunData.activeAutorunModule == autorunModule) {
                registrationSuccessful = true;
            }

            return autorunData;
        }
        
        public override void AutoplayAllSequences()
        {
            for (int q = 0; q < autorunController.autorunData.Count; q++) {
                
                Autorun_Data autorunData = autorunController.autorunData[q];
                Sequence sequence = autorunData.sequence;
                
                if (sequence.active == true) {
                    AttemptRegisterAutorunModule(this, autorunData, out bool registrationSuccessful);

                    if (registrationSuccessful == true) {
                        autorunData.eligibleForAutoplay = true;
                        OnSequenceUpdated(sequence);
                    }
                }
            }
        }
        
        public override void AutoplaySequence(Sequence targetSequence)
        {
            if (appUtilsRequested == true || moduleActive == false || targetSequence.active == false) return;
            
            if (HasAutorunData(targetSequence, autorunController, out var autorunData) == false) return;
            
            AttemptRegisterAutorunModule(this, autorunData, out bool registrationSuccessful);

            if (registrationSuccessful == true) {
                autorunData.eligibleForAutoplay = true;
                OnSequenceUpdated(targetSequence);
            }
        }
    }
}