using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class SequenceAutoplayer : SequenceController
    {
        [SerializeField]
        [ValidateInput("IsPopulated")]
        FloatReference frameStepValue;

        [SerializeField]
        EasingFunction.Ease ease = EasingFunction.Ease.EaseOutQuad;

        [HideInInspector]
        public EasingFunction.Function easingFunction;

        [SerializeField]
        [Range(0f, 5f)]
        float easeThreshold;

        [SerializeField]
        [Range(0f, 1f)]
        float lerpModifier;

        float lerpValue = 0f;
        float easingModifier = 1f;

        void Start()
        {
            easingFunction = EasingFunction.GetEasingFunction(ease);
#if UNITY_ANDROID
            internalIsReversingVal = isReversing.Value;
#endif
        }

        protected virtual void Update()
        {
            if (appSettings.autoplayActive.Value == false) {
                return;
            }

            for (int i = 0; i < sequenceList.sequences.Count; i++) {

                if (sequenceList.sequences[i].Active == true && sequenceList.sequences[i].hasAutoplay == true && sequenceList.sequences[i].autoplayActive == true) {

                    for (int q = 0; q < sequenceList.sequences[i].autoplayThresholds.Count; q++) {

                        if (sequenceList.sequences[i].currentTime >= sequenceList.sequences[i].autoplayThresholds[q].startTime &&
                            sequenceList.sequences[i].currentTime <= sequenceList.sequences[i].autoplayThresholds[q].endTime) {

                            if (isReversing.Value == true) {
                                if (sequenceList.sequences[i].currentTime - sequenceList.sequences[i].autoplayThresholds[q].startTime <= easeThreshold) {
                                    easingModifier = easingFunction(1f, 0f, lerpValue);
                                    lerpValue += lerpModifier;
                                }
                            }
                            else {
                                if (sequenceList.sequences[i].autoplayThresholds[q].endTime - sequenceList.sequences[i].currentTime <= easeThreshold) {
                                    easingModifier = easingFunction(1f, 0f, lerpValue);
                                    lerpValue += lerpModifier;
                                }
                            }
                            AutoplaySequence(sequenceList.sequences[i], sequenceList.sequences[i].autoplayThresholds[q]);
                            sequenceModified.RaiseEvent(this.gameObject);
                        }
                    }
                }
            }
        }

        protected void AutoplaySequence(Sequence targetSequence, StartEndThreshold targetAutoplayThreshold)
        {
            if (isReversing.Value == false) {
                // Update sequence moving forward
#if UNITY_EDITOR
                targetSequence.ModifySequenceTime(Time.smoothDeltaTime * easingModifier);
#else

#if UNITY_ANDROID
                targetSequence.ModifySequenceTime(frameStepValue.Value * 3f * easingModifier);
#else
                targetSequence.ModifySequenceTime(frameStepValue.Value * easingModifier);
#endif
#endif
                if (targetSequence.currentTime > targetAutoplayThreshold.endTime) {
                    targetSequence.autoplayActive = false;
                    lerpValue = 0f;
                    easingModifier = 1f;
                }
            }
            else {
                // Update sequence moving backward
#if UNITY_EDITOR
                targetSequence.ModifySequenceTime(Time.smoothDeltaTime * easingModifier * -1f);
#else

#if UNITY_ANDROID
                targetSequence.ModifySequenceTime(frameStepValue.Value * 3f * easingModifier * -1f);
#else
                targetSequence.ModifySequenceTime(frameStepValue.Value * easingModifier * -1f);
#endif
#endif
                if (targetSequence.currentTime < targetAutoplayThreshold.startTime) {
                    targetSequence.autoplayActive = false;
                    lerpValue = 0f;
                    easingModifier = 1f;
                }
            }
        }

        public void ActivateAutoplayIfMomentumPaused()
        {
            for (int i = 0; i < sequenceList.sequences.Count; i++) {
                if (sequenceList.sequences[i].Active == true && sequenceList.sequences[i].hasAutoplay == true && sequenceList.sequences[i].pauseMomentumActive == true) {

                    sequenceList.sequences[i].autoplayActive = true;
                }
            }
        }

        public void ActivateAutoplay()
        {
            for (int i = 0; i < sequenceList.sequences.Count; i++) {
                if (sequenceList.sequences[i].autoplayActive == true) {
                    continue;
                }

                if (sequenceList.sequences[i].Active == true && sequenceList.sequences[i].hasAutoplay == true) {
                    sequenceList.sequences[i].autoplayActive = true;
                }
            }
        }

        public void DeactivateAutoplay()
        {
            for (int i = 0; i < sequenceList.sequences.Count; i++) {
                if (sequenceList.sequences[i].Active == true) {
                    sequenceList.sequences[i].autoplayActive = false;
                    lerpValue = 0f;
                    easingModifier = 1f;
                }
            }
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}