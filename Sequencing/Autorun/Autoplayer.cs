using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

namespace AltSalt.Maestro.Sequencing.Autorun
{
    public class Autoplayer : Autorun_Module
    {
        [SerializeField]
        private bool _autoplayEnabled = true;

        public bool autoplayEnabled
        {
            get
            {
                if (_autoplayEnabled == false ||
                    autorunController.appSettings.GetUserAutoplayEnabled(this.gameObject, userKey) == false) {
                    return false;
                }

                return true;
            }
            set => _autoplayEnabled = value;
        }

        private float frameStepValue => autorunController.appSettings.GetFrameStepValue(this.gameObject, inputGroupKey);

        [SerializeField]
        [Range(0f, 1f)]
        private float _autoplayEaseThreshold = 0.25f;

        private float autoplayEaseThreshold => _autoplayEaseThreshold;

        protected virtual void Update()
        {
            if (autoplayEnabled == false || appUtilsRequested == true) {
                return;
            }

            for (int q = 0; q < autorunController.autorunData.Count; q++)
            {
                var autorunData = autorunController.autorunData[q];

                if (autorunData.autoplayActive == false || autorunData.isLerping == true || autorunData.sequence.active == false) {
                    continue;
                }
                
                if (AutorunExtents.TimeWithinThreshold(autorunData.sequence.currentTime,
                        autorunData.autorunIntervals, out var currentInterval) == false)
                    continue;

                
                float autoplayModifer = 0f;

#if UNITY_EDITOR
                autoplayModifer = Time.smoothDeltaTime;
#else
                autoplayModifer = frameStepValue;
#endif
                
                autoplayModifer *= CalculateAutoplayModifier(autorunData.sequence, currentInterval, autorunData.loop, autorunController.isReversing, autoplayEaseThreshold, autorunData.easingUtility);

                AutoplaySequence(autorunController.rootConfig.masterSequences, this, autorunData.sequence, autoplayModifer);
                
                if(autorunData.loop == true) continue;

                if (autorunData.autoplayActive == true &&
                    (Mathf.Approximately((float)autorunData.sequence.currentTime, (float)currentInterval.endTime)
                     || autorunData.sequence.currentTime > currentInterval.endTime
                     || Mathf.Approximately((float)autorunData.sequence.currentTime, (float)currentInterval.startTime)
                     || autorunData.sequence.currentTime < currentInterval.startTime)) {
                    
                    autorunData.autoplayActive = false;
                    autorunData.easingUtility.Reset();
                }
            }
        }
        
        private static float CalculateAutoplayModifier(Sequence targetSequence, Extents currentInterval, bool loop,
            bool isReversing, float easeThreshold, EasingUtility easingUtility)
        {
            float timeModifier = 0f;

            if (loop == false && WithinEaseThreshold(targetSequence, currentInterval, isReversing, easeThreshold)) {
                timeModifier = easingUtility.GetMultiplier();
            } else {
                timeModifier = 1f;
            }

            if (isReversing == true) {
                timeModifier *= -1f;
            }

            return timeModifier;
        }
            
        private static bool WithinEaseThreshold(Sequence targetSequence, Extents currentInterval,
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
                if (currentInterval.endTime - targetSequence.currentTime  <= easeThreshold)
                {
                    return true;
                }
            }

            return false;
        }

        private static Sequence AutoplaySequence(List<MasterSequence> masterSequences, Input_Module source, Sequence targetSequence, float timeModifier)
        {
            MasterSequence masterSequence = masterSequences.Find(x => x.sequenceConfigs.Find(y => y.sequence == targetSequence));
            masterSequence.TriggerModifyRequest(targetSequence, source.priority, source.gameObject.name, timeModifier);
            
            return targetSequence;
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