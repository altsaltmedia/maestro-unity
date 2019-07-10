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

            for (int q = 0; q < sequenceLists.Count; q++) {

                for (int i = 0; i < sequenceLists[q].sequences.Count; i++) {

                    if (sequenceLists[q].sequences[i].Active == true && sequenceLists[q].sequences[i].hasAutoplay == true && sequenceLists[q].sequences[i].autoplayActive == true) {

                        for (int z = 0; z < sequenceLists[q].sequences[i].autoplayThresholds.Count; z++) {

                            if (sequenceLists[q].sequences[i].currentTime >= sequenceLists[q].sequences[i].autoplayThresholds[z].startTime &&
                                sequenceLists[q].sequences[i].currentTime <= sequenceLists[q].sequences[i].autoplayThresholds[z].endTime) {

                                if (isReversing.Value == true) {
                                    if (sequenceLists[q].sequences[i].currentTime - sequenceLists[q].sequences[i].autoplayThresholds[z].startTime <= easeThreshold) {
                                        easingModifier = easingFunction(1f, 0f, lerpValue);
                                        lerpValue += lerpModifier;
                                    }
                                } else {
                                    if (sequenceLists[q].sequences[i].autoplayThresholds[z].endTime - sequenceLists[q].sequences[i].currentTime <= easeThreshold) {
                                        easingModifier = easingFunction(1f, 0f, lerpValue);
                                        lerpValue += lerpModifier;
                                    }
                                }
                                AutoplaySequence(sequenceLists[q].sequences[i], sequenceLists[q].sequences[i].autoplayThresholds[z]);
                                sequenceModified.RaiseEvent(this.gameObject);
                            }
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
            for (int q = 0; q < sequenceLists.Count; q++) {

                for (int i = 0; i < sequenceLists[q].sequences.Count; i++) {
                    if (sequenceLists[q].sequences[i].Active == true && sequenceLists[q].sequences[i].hasAutoplay == true && sequenceLists[q].sequences[i].pauseMomentumActive == true) {

                        sequenceLists[q].sequences[i].autoplayActive = true;
                    }
                }
            }
        }

        public void ActivateAutoplay()
        {
            for (int q = 0; q < sequenceLists.Count; q++) {

                for (int i = 0; i < sequenceLists[q].sequences.Count; i++) {

                    if (sequenceLists[q].sequences[i].autoplayActive == true) {
                        continue;
                    }

                    if (sequenceLists[q].sequences[i].Active == true && sequenceLists[q].sequences[i].hasAutoplay == true) {
                        sequenceLists[q].sequences[i].autoplayActive = true;
                    }
                }
            }
        }

        public void DeactivateAutoplay()
        {
            for (int q = 0; q < sequenceLists.Count; q++) {

                for (int i = 0; i < sequenceLists[q].sequences.Count; i++) {
                    if (sequenceLists[q].sequences[i].Active == true) {
                        sequenceLists[q].sequences[i].autoplayActive = false;
                        lerpValue = 0f;
                        easingModifier = 1f;
                    }
                }
            }
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
}