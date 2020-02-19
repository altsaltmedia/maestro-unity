using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    public class Lerper : Autorun_Module
    {
        
        private float frameStepValue => autorunController.appSettings.GetFrameStepValue(this.gameObject, inputGroupKey);

        [SerializeField]
        private double _lerpThreshold = .03d;

        private double lerpThreshold {
            get => _lerpThreshold;
        }

        private delegate Autorun_Data CoroutineCallback(Autorun_Controller autorunController, Autorun_Data autorunData, AutorunExtents extents);

        public void TriggerLerpSequences()
        {
            if (appUtilsRequested == true || moduleActive == false) return;
            
            for (int i = 0; i < autorunController.autorunData.Count; i++) {
                AttemptLerpSequence(autorunController.autorunData[i]);
            }
        }

        public void TriggerLerpSequence(Sequence targetSequence)
        {
            if (appUtilsRequested == true || moduleActive == false) return;
            
            if (HasLerpData(targetSequence, autorunController, out var autorunData) == false) return;

            AttemptLerpSequence(autorunData);
        }

        private Autorun_Data AttemptLerpSequence(Autorun_Data autorunData) {
            
            if (autorunData.sequence.active == true && autorunData.isLerping == false)
            {
                double thresholdModifier = lerpThreshold;
                if(autorunController.isReversing == true) {
                    thresholdModifier *= -1d;
                }

                if (AutorunExtents.TimeWithinThresholdExclusive(autorunData.sequence.currentTime + thresholdModifier,
                        autorunData.autorunIntervals, out var currentInterval) == false) return autorunData;

                autorunData.isLerping = true;

                float lerpModifier = 0f;

#if UNITY_EDITOR
                lerpModifier = Time.smoothDeltaTime;
#else
                lerpModifier = frameStepValue;
#endif
                lerpModifier *= CalculateLerpModifier(autorunController.isReversing);

                autorunData.lerpCoroutine = LerpSequence(autorunController, this, autorunData,
                    lerpModifier, currentInterval, CheckEndLerp);
                StartCoroutine(autorunData.lerpCoroutine);
            }

            return autorunData;
        }

        private Autorun_Data CheckEndLerp(Autorun_Controller autorunController, Autorun_Data autorunData, AutorunExtents extents)
        {
            if(autorunData.sequence.active == false
             || Mathf.Approximately((float)autorunData.sequence.currentTime, (float)extents.endTime)
             || autorunData.sequence.currentTime > extents.endTime
             || Mathf.Approximately((float)autorunData.sequence.currentTime, (float)extents.startTime)
             || autorunData.sequence.currentTime < extents.startTime) {

                autorunData.isLerping = false;
                StopCoroutine(autorunData.lerpCoroutine);
            }

            return autorunData;
        }
        
        private static bool HasLerpData(Sequence targetSequence, Autorun_Controller autorunController, out Autorun_Data autorunData)
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


        private static float CalculateLerpModifier(bool isReversing)
        {
            if (isReversing == false)
            {
                return 1f;
            }

            return -1f;
        }

        private static IEnumerator LerpSequence(Autorun_Controller autorunController, Input_Module source,
            Autorun_Data autorunData, float timeModifier, AutorunExtents currentExtents, CoroutineCallback callback)
        {
            MasterSequence masterSequence = autorunController.rootConfig.masterSequences.
                Find(x => x.sequenceControllers.Find(y => y.sequence == autorunData.sequence));
            
            while(true) {
                masterSequence.RequestModifySequenceTime(autorunData.sequence, source.priority, source.gameObject.name, timeModifier);
                callback(autorunController, autorunData, currentExtents);
                yield return new WaitForEndOfFrame();
            }
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

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
}