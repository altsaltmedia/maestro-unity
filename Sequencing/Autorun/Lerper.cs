using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    public class Lerper : Autorun_Module
    {
        private float lerpSpeed => autorunController.appSettings.GetLerpSpeed(this.gameObject, inputGroupKey);
        
        private float frameStepValue => autorunController.appSettings.GetFrameStepValue(this.gameObject, inputGroupKey);

        private delegate Autorun_Data CoroutineCallback(Autorun_Controller autorunController, Autorun_Data autorunData, AutorunExtents extents);

        public void TriggerLerpSequences()
        {
            if (appUtilsRequested == true || moduleActive == false) return;
            
            for (int i = 0; i < autorunController.autorunData.Count; i++) {
                AttemptLerpSequence(autorunController.autorunData[i], this);
            }
        }

        public void TriggerLerpSequence(Sequence targetSequence)
        {
            if (appUtilsRequested == true || moduleActive == false) return;
            
            if (HasTargetLerpData(targetSequence, autorunController, out var autorunData) == false) return;

            AttemptLerpSequence(autorunData, this);
        }
        
        private static bool HasTargetLerpData(Sequence targetSequence, Autorun_Controller autorunController, out Autorun_Data autorunData)
        {
            for (int i = 0; i < autorunController.autorunData.Count; i++)
            {
                if (targetSequence == autorunController.autorunData[i].sequence)
                {
                    autorunData = autorunController.autorunData[i];
                    return true;
                }
            }
            
            throw new Exception("Target sequence not found. Did you forget to assign an InputController or MasterController?");
        }
        
        /// <summary>
        /// Given a target sequence, will search through our
        /// autorun data and either activate or deactivate autoplay.
        /// This should be called every time a sequence-timeline pair
        /// is updated. 
        /// </summary>
        /// <param name="targetSequence"></param>
        public void OnSequenceUpdated(Sequence targetSequence)
        {
            if (moduleActive == false || appUtilsRequested == true) {
                return;
            }
            
            var autorunData = autorunController.autorunData.Find(x => x.sequence == targetSequence);

            if (autorunData == null) return;

            // If autoplay is currently active, deactivate and return if we're
            // beyond the thresholds of the extents where the autoplay originated.
            // (This is how we pause autoplay between intervals).
            // Note that, if looping is activated, we ignore intervals.
            if(HasValidLerpInterval(autorunData)) {
                
                if(autorunController.isReversing == false &&
                   Extents.TimeBeyondEndThresholdExclusive(targetSequence.currentTime + autorunThreshold, autorunData.activeInterval)) {

                    AutorunExtents currentInterval = autorunData.activeInterval;
                    TriggerLerpIntervalComplete(this, autorunData);
                    
                    if (currentInterval.endTime >= targetSequence.duration) {
                        autorunData.sequence.sequenceController.SetEndBoundaryReached(this);
                    }
                    else {
                        autorunData.sequence.sequenceController.SetSequenceTimeWithoutCallbacks(this, (float)currentInterval.endTime);
                    }
                }
                else if (autorunController.isReversing == true &&
                         Extents.TimeBeyondStartThresholdExclusive(targetSequence.currentTime - autorunThreshold, autorunData.activeInterval)) {
                    
                    AutorunExtents currentInterval = autorunData.activeInterval;
                    TriggerLerpIntervalComplete(this, autorunData);
                    
                    if (currentInterval.startTime == 0) {
                        autorunData.sequence.sequenceController.SetStartBoundaryReached(this);
                    }
                    else {
                        autorunData.sequence.sequenceController.SetSequenceTimeWithoutCallbacks(this, (float)currentInterval.startTime);
                    }
                }
            }
        }
        
        private static bool HasValidLerpInterval(Autorun_Data autorunData)
        {
            if(autorunData.isLerping == true && autorunData.loop == false && autorunData.activeInterval != null) {
                return true;
            }

            return false;
        }

        private static Autorun_Data OnSequenceUpdatedGoingForward(Autorun_Data autorunData)
        {
            return autorunData;
        }
        
        private static Autorun_Data OnSequenceUpdatedGoingBackward(Autorun_Data autorunData)
        {
            return autorunData;
        }

        private static Autorun_Data TriggerLerpIntervalComplete(Lerper lerper, Autorun_Data autorunData)
        {
            Sequence targetSequence = autorunData.sequence;
            MasterSequence targetMasterSequence = targetSequence.sequenceController.masterSequence;

            autorunData.eligibleForAutoplay = false;
            autorunData.isLerping = false;
            autorunData.activeInterval = null;
            autorunData.forwardUpdateActive = false;
            autorunData.backwardUpdateActive = false;
            targetMasterSequence.RequestDeactivateForwardAutoplay(targetSequence, lerper.priority, lerper.gameObject.name);
            
            if (autorunData.lerpCoroutine != null) {
                lerper.StopCoroutine(autorunData.lerpCoroutine);
                autorunData.lerpCoroutine = null;
            }
            
            lerper.TriggerInputActionComplete(autorunData.sequence.sequenceController.masterSequence);
            
            return autorunData;
        }

        private static Autorun_Data AttemptLerpSequence(Autorun_Data autorunData, Lerper lerper) {
            
            if (autorunData.sequence.active == true && autorunData.isLerping == false) {

                double modifiedTime = GetLerpTime(autorunData, lerper);

                if (AutorunExtents.TimeWithinThresholdBothBoundsInclusive(modifiedTime,
                        autorunData.autorunIntervals, out var currentInterval) == false) return autorunData;
                
                if(lerper.autorunController.isReversing == false) {
                    AttemptForwardLerp(autorunData, lerper, currentInterval);
                } else {
                    AttemptReverseLerp(autorunData, lerper, currentInterval);
                }
            }

            return autorunData;
        }
        
        private static Autorun_Data AttemptForwardLerp(Autorun_Data autorunData, Lerper lerper, AutorunExtents currentInterval)
        {
            MasterSequence targetMasterSequence = autorunData.sequence.sequenceController.masterSequence;
            Sequence targetSequence = autorunData.sequence;
            
            targetMasterSequence.RequestActivateForwardAutoplay(targetSequence,
                lerper.priority, lerper.gameObject.name, lerper.lerpSpeed, out bool requestSuccessful);
                
            // We should only store the interval and activate autoplay
            // once our request has been accepted by the MasterSequence
            if(requestSuccessful == true) {
                    
                // Once the active interval has been cached, we will use
                // it to determine whether autoplay should halt whenever the
                // sequence gets updated (see RefreshAutoplay() above)
                autorunData.isLerping = true;
                autorunData.activeInterval = currentInterval;
                autorunData.forwardUpdateActive = true;
            }

            return autorunData;
        }
        
        private static Autorun_Data AttemptReverseLerp(Autorun_Data autorunData, Lerper lerper, AutorunExtents currentInterval)
        {
            // Cancel if a coroutine is already created
            if (autorunData.lerpCoroutine != null) {
                return autorunData;
            }
            
            Autorun_Controller autorunController = lerper.autorunController;
            
            autorunData.isLerping = true;
            autorunData.backwardUpdateActive = true;
            
            float lerpModifier = CalculateLerpModifier(lerper);

            autorunData.lerpCoroutine = LerpSequenceManually(autorunController, lerper, autorunData,
                lerpModifier, currentInterval, null);
            autorunData.activeInterval = currentInterval;
            
            lerper.StartCoroutine(autorunData.lerpCoroutine);
            
            return autorunData;
        }

        private static float CalculateLerpModifier(Lerper lerper)
        {
            float lerpModifier;
            
            if (lerper.autorunController.useFrameStepValue == false) {
                lerpModifier = Time.smoothDeltaTime;
            }
            else {
                lerpModifier = lerper.frameStepValue;
            }
            
            if (lerper.autorunController.isReversing == true) {
                lerpModifier *= -1;
            }

            return lerpModifier;
        }

        private static IEnumerator LerpSequenceManually(Autorun_Controller autorunController, Input_Module source,
            Autorun_Data autorunData, float timeModifier, AutorunExtents currentExtents, CoroutineCallback callback)
        {
            MasterSequence masterSequence = autorunController.rootConfig.masterSequences.
                Find(x => x.sequenceControllers.Find(y => y.sequence == autorunData.sequence));
            
            while(true) {
                masterSequence.RequestModifySequenceTime(autorunData.sequence, source.priority, source.gameObject.name, timeModifier);
                yield return new WaitForEndOfFrame();
            }
        }

        private static bool SequenceDeactivated(Autorun_Data autorunData)
        {
            if (autorunData.sequence.active == false && autorunData.isLerping == true) {
                return true;
            }

            return false;
        }

        private static double GetLerpTime(Autorun_Data autorunData, Lerper lerper)
        {
            double modifiedTime = autorunData.sequence.currentTime;
                
            if(lerper.autorunController.isReversing == false) {
                modifiedTime += lerper.autorunThreshold;
            }
            else {
                modifiedTime += lerper.autorunThreshold * -1d;
            }

            return modifiedTime;
        }
        
        public void DeactivateLerp()
        {
            for (int i = 0; i < autorunController.autorunData.Count; i++) {
                if(autorunController.autorunData[i].lerpCoroutine != null) {
                    StopCoroutine(autorunController.autorunData[i].lerpCoroutine);
                }
                autorunController.autorunData[i].isLerping = false;
            }
        }
        
        public void DeactivateLerp(ComplexPayload complexPayload)
        {
            Sequence targetSequence = complexPayload.GetScriptableObjectValue() as Sequence;
            Autorun_Data autorunData = autorunController.autorunData.Find(x => x.sequence == targetSequence);
            if(autorunData.lerpCoroutine != null) {
                StopCoroutine(autorunData.lerpCoroutine);
            }
            autorunData.isLerping = false;
        }

        public void ResetLerpDataDependencies(Autorun_Data autorunData)
        {
            autorunData.isLerping = false;
            autorunData.activeInterval = null;
            autorunData.backwardUpdateActive = false;
            autorunData.forwardUpdateActive = false;
            if(autorunData.lerpCoroutine != null) {
                StopCoroutine(autorunData.lerpCoroutine);
            }
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}