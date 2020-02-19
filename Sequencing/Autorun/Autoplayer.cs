using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

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
        
        protected virtual void Update()
        {
            if (moduleActive == false || appUtilsRequested == true || autorunController.isReversing == false) {
                return;
            }

            for (int q = 0; q < autorunController.autorunData.Count; q++)
            {
                var autorunData = autorunController.autorunData[q];

                if (autorunData.autoplayActive == false || autorunData.isLerping == true ||
                    autorunData.sequence.active == false || autorunData.backwardUpdateActive == false) {
                    continue;
                }

                if (AutorunExtents.TimeWithinThreshold(autorunData.sequence.currentTime,
                        autorunData.autorunIntervals, out var currentInterval) == false) {
                    continue;
                }
                
                autorunData.activeInterval = currentInterval;

                float autoplayModifer = 0f;

#if UNITY_EDITOR
                autoplayModifer = Time.smoothDeltaTime;
#else
                autoplayModifer = frameStepValue;
#endif
                autoplayModifer *= CalculateAutoplayModifier(autorunData.sequence, currentInterval, autorunData.loop, autorunController.isReversing, autoplayEaseThreshold, autorunData.easingUtility);
                AutoplaySequenceBackward(autorunController.rootConfig.masterSequences, this, autorunData.sequence, autoplayModifer);
                
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

        public virtual void RefreshAutoplay(Sequence updatedSequence)
        {
            if (moduleActive == false || appUtilsRequested == true) {
                return;
            }
            
            var autorunData = autorunController.autorunData.Find(x => x.sequence == updatedSequence);

            if (autorunData == null) return;

            if (autorunData.autoplayActive == false || autorunData.isLerping == true || autorunData.sequence.active == false) {
                autorunData.backwardUpdateActive = false;
                autorunData.forwardUpdateActive = false;
                PauseSequence(autorunController.rootConfig.masterSequences, this, autorunData.sequence, 0);
                return;
            }
            
            if(autorunData.forwardUpdateActive == true && autorunData.activeInterval != null) {
                if (updatedSequence.currentTime > autorunData.activeInterval.endTime
                    || updatedSequence.currentTime < autorunData.activeInterval.startTime) {
                    autorunData.backwardUpdateActive = false;
                    autorunData.forwardUpdateActive = false;
                    PauseSequence(autorunController.rootConfig.masterSequences, this, autorunData.sequence, 0);
                    return;
                }
            }
            
            if (autorunController.isReversing == false) {
                if (autorunData.forwardUpdateActive == false && AutorunExtents.TimeWithinThreshold(updatedSequence.currentTime,
                    autorunData.autorunIntervals, out var currentInterval) == true) {
                    autorunData.backwardUpdateActive = false;
                    if (updatedSequence.sequenceController.masterSequence.TriggerAutoplayRequest(updatedSequence,
                            this.priority, this.gameObject.name, 0) == true) {
                        autorunData.activeInterval = currentInterval;
                        autorunData.forwardUpdateActive = true;
                    }
                    //AutoplaySequenceForward(autorunController.rootConfig.masterSequences, this, updatedSequence, 0);
                }
            }
            else {
                autorunData.forwardUpdateActive = false;
                autorunData.backwardUpdateActive = true;
            }
        }

        // public void CheckDeactivateAutoplay(Sequence updatedSequence)
        // {
        //     var autorunData = autorunController.autorunData.Find(x => x.sequence == updatedSequence);
        //     
        //     if (autorunData != null && updatedSequence.currentTime + frameStepValue > autorunData.activeInterval.endTime
        //         || updatedSequence.currentTime - frameStepValue < autorunData.activeInterval.startTime) {
        //
        //         PauseSequence(autorunController.rootConfig.masterSequences, this, autorunData.sequence, 0);
        //     }
        // }
        //
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

        private static Sequence AutoplaySequenceForward(List<MasterSequence> masterSequences, Input_Module source, Sequence targetSequence, float timeModifier)
        {
            MasterSequence masterSequence = masterSequences.Find(x => x.sequenceControllers.Find(y => y.sequence == targetSequence));
            masterSequence.TriggerAutoplayRequest(targetSequence, source.priority, source.gameObject.name, timeModifier);
            
            return targetSequence;
        }
        
        private static Sequence AutoplaySequenceBackward(List<MasterSequence> masterSequences, Input_Module source, Sequence targetSequence, float timeModifier)
        {
            MasterSequence masterSequence = masterSequences.Find(x => x.sequenceControllers.Find(y => y.sequence == targetSequence));
            masterSequence.TriggerModifyRequest(targetSequence, source.priority, source.gameObject.name, timeModifier);

            return targetSequence;
        }

        private static Sequence PauseSequence(List<MasterSequence> masterSequences, Input_Module source, Sequence targetSequence, float timeModifier)
        {
            MasterSequence masterSequence = masterSequences.Find(x => x.sequenceControllers.Find(y => y.sequence == targetSequence));
            //masterSequence.TriggerModifyRequest(targetSequence, source.priority, source.gameObject.name, timeModifier);
            masterSequence.TriggerAutoplayPause(targetSequence, source.priority, source.gameObject.name, timeModifier);
            
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
                    // autorunController.autorunData[q].forwardUpdateActive = false;
                    // autorunController.autorunData[q].backwardUpdateActive = false;
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